using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using osuTK;
using Tachyon.Game.Audio;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.Beatmaps.Objects;
using Tachyon.Game.Rulesets.Objects;
using Tachyon.Game.Rulesets.Objects.Types;

namespace Tachyon.Game.Beatmaps.Formats
{
    public class BeatmapEncoder
    {
        public const int LATEST_VERSION = 128;

        private readonly IBeatmap beatmap;
        
        public BeatmapEncoder(IBeatmap beatmap)
        {
            this.beatmap = beatmap;
        }
        
        public void Encode(TextWriter writer)
        {
            writer.WriteLine($"osu file format v{LATEST_VERSION}");

            writer.WriteLine();
            handleGeneral(writer);

            writer.WriteLine();
            handleMetadata(writer);

            writer.WriteLine();
            handleDifficulty(writer);

            writer.WriteLine();
            handleEvents(writer);

            writer.WriteLine();
            handleControlPoints(writer);

            writer.WriteLine();
            handleHitObjects(writer);
        }
        
         private void handleGeneral(TextWriter writer)
        {
            writer.WriteLine("[General]");

            if (beatmap.Metadata.AudioFile != null) writer.WriteLine(FormattableString.Invariant($"AudioFilename: {Path.GetFileName(beatmap.Metadata.AudioFile)}"));
            writer.WriteLine(FormattableString.Invariant($"AudioLeadIn: {beatmap.BeatmapInfo.AudioLeadIn}"));
            writer.WriteLine(FormattableString.Invariant($"PreviewTime: {beatmap.Metadata.PreviewTime}"));
            writer.WriteLine(FormattableString.Invariant($"Countdown: {(beatmap.BeatmapInfo.Countdown ? '1' : '0')}"));
            writer.WriteLine(FormattableString.Invariant($"SampleSet: {toSampleBank(beatmap.ControlPointInfo.SamplePointAt(double.MinValue).SampleBank)}"));
            writer.WriteLine(FormattableString.Invariant($"StackLeniency: {beatmap.BeatmapInfo.StackLeniency}")); 
        }
        
        private void handleMetadata(TextWriter writer)
        {
            writer.WriteLine("[Metadata]");

            writer.WriteLine(FormattableString.Invariant($"Title: {beatmap.Metadata.Title}"));
            if (beatmap.Metadata.TitleUnicode != null) writer.WriteLine(FormattableString.Invariant($"TitleUnicode: {beatmap.Metadata.TitleUnicode}"));
            writer.WriteLine(FormattableString.Invariant($"Artist: {beatmap.Metadata.Artist}"));
            if (beatmap.Metadata.ArtistUnicode != null) writer.WriteLine(FormattableString.Invariant($"ArtistUnicode: {beatmap.Metadata.ArtistUnicode}"));
            writer.WriteLine(FormattableString.Invariant($"Version: {beatmap.BeatmapInfo.Version}"));
            if (beatmap.Metadata.Source != null) writer.WriteLine(FormattableString.Invariant($"Source: {beatmap.Metadata.Source}"));
            if (beatmap.Metadata.Tags != null) writer.WriteLine(FormattableString.Invariant($"Tags: {beatmap.Metadata.Tags}"));
            if (beatmap.BeatmapInfo.OnlineBeatmapID != null) writer.WriteLine(FormattableString.Invariant($"BeatmapID: {beatmap.BeatmapInfo.OnlineBeatmapID}"));
            if (beatmap.BeatmapInfo.BeatmapSet?.OnlineBeatmapSetID != null) writer.WriteLine(FormattableString.Invariant($"BeatmapSetID: {beatmap.BeatmapInfo.BeatmapSet.OnlineBeatmapSetID}"));
        }
        
        private void handleDifficulty(TextWriter writer)
        {
            writer.WriteLine("[Difficulty]");

            writer.WriteLine(FormattableString.Invariant($"HPDrainRate: {beatmap.BeatmapInfo.BaseDifficulty.DrainRate}"));
            writer.WriteLine(FormattableString.Invariant($"CircleSize: {beatmap.BeatmapInfo.BaseDifficulty.CircleSize}"));
            writer.WriteLine(FormattableString.Invariant($"OverallDifficulty: {beatmap.BeatmapInfo.BaseDifficulty.OverallDifficulty}"));
            writer.WriteLine(FormattableString.Invariant($"ApproachRate: {beatmap.BeatmapInfo.BaseDifficulty.ApproachRate}"));
            writer.WriteLine(FormattableString.Invariant($"SliderMultiplier: {beatmap.BeatmapInfo.BaseDifficulty.SliderMultiplier / 1.4f}"));
            writer.WriteLine(FormattableString.Invariant($"SliderTickRate: {beatmap.BeatmapInfo.BaseDifficulty.SliderTickRate}"));
        }
        
        private void handleEvents(TextWriter writer)
        {
            writer.WriteLine("[Events]");

            if (!string.IsNullOrEmpty(beatmap.BeatmapInfo.Metadata.BackgroundFile))
                writer.WriteLine(FormattableString.Invariant($"{(int)EventType.Background},0,\"{beatmap.BeatmapInfo.Metadata.BackgroundFile}\",0,0"));
        }
        
        private void handleControlPoints(TextWriter writer)
        {
            if (beatmap.ControlPointInfo.Groups.Count == 0)
                return;

            writer.WriteLine("[TimingPoints]");

            foreach (var group in beatmap.ControlPointInfo.Groups)
            {
                var groupTimingPoint = group.ControlPoints.OfType<TimingControlPoint>().FirstOrDefault();

                // If the group contains a timing control point, it needs to be output separately.
                if (groupTimingPoint != null)
                {
                    writer.Write(FormattableString.Invariant($"{groupTimingPoint.Time},"));
                    writer.Write(FormattableString.Invariant($"{groupTimingPoint.BeatLength},"));
                    outputControlPointEffectsAt(groupTimingPoint.Time, true);
                }

                // Output any remaining effects as secondary non-timing control point.
                var difficultyPoint = beatmap.ControlPointInfo.DifficultyPointAt(group.Time);
                writer.Write(FormattableString.Invariant($"{group.Time},"));
                writer.Write(FormattableString.Invariant($"{-100 / difficultyPoint.SpeedMultiplier},"));
                outputControlPointEffectsAt(group.Time, false);
            }

            void outputControlPointEffectsAt(double time, bool isTimingPoint)
            {
                var samplePoint = beatmap.ControlPointInfo.SamplePointAt(time);
                var effectPoint = beatmap.ControlPointInfo.EffectPointAt(time);

                // Apply the control point to a hit sample to uncover  properties (e.g. suffix)
                HitSampleInfo tempHitSample = samplePoint.ApplyTo(new HitSampleInfo());

                // Convert effect flags to the  format
                EffectFlags effectFlags = EffectFlags.None;
                if (effectPoint.KiaiMode)
                    effectFlags |= EffectFlags.Kiai;
                if (effectPoint.OmitFirstBarLine)
                    effectFlags |= EffectFlags.OmitFirstBarLine;

                writer.Write(FormattableString.Invariant($"{(int)beatmap.ControlPointInfo.TimingPointAt(time).TimeSignature},"));
                writer.Write(FormattableString.Invariant($"{(int)toSampleBank(tempHitSample.Bank)},"));
                writer.Write(FormattableString.Invariant($"{toCustomSampleBank(tempHitSample)},"));
                writer.Write(FormattableString.Invariant($"{tempHitSample.Volume},"));
                writer.Write(FormattableString.Invariant($"{(isTimingPoint ? '1' : '0')},"));
                writer.Write(FormattableString.Invariant($"{(int)effectFlags}"));
                writer.WriteLine();
            }
        }

        private void handleHitObjects(TextWriter writer)
        {
            if (beatmap.HitObjects.Count == 0)
                return;

            writer.WriteLine("[HitObjects]");

            foreach (var h in beatmap.HitObjects)
                handleHitObject(writer, h);
        }

        private void handleHitObject(TextWriter writer, HitObject hitObject)
        {
            Vector2 position = new Vector2(256, 192);

            writer.Write(FormattableString.Invariant($"{position.X},"));
            writer.Write(FormattableString.Invariant($"{position.Y},"));
            writer.Write(FormattableString.Invariant($"{hitObject.StartTime},"));
            writer.Write(FormattableString.Invariant($"{(int)getObjectType(hitObject)},"));
            writer.Write(FormattableString.Invariant($"{(int)toHitSoundType(hitObject.Samples)},"));

            if (hitObject is IHasCurve curveData)
            {
                addCurveData(writer, curveData, position);
                writer.Write(getSampleBank(hitObject.Samples, zeroBanks: true));
            }
            else
            {
                if (hitObject is IHasEndTime)
                    addEndTimeData(writer, hitObject);

                writer.Write(getSampleBank(hitObject.Samples));
            }

            writer.WriteLine();
        }
        
        private HitObjectType getObjectType(HitObject hitObject)
        {
            HitObjectType type = 0;

            switch (hitObject)
            {
                case IHasCurve _:
                    type |= HitObjectType.Slider;
                    break;

                case IHasEndTime _:
                    type |= HitObjectType.Spinner;
                    break;

                default:
                    type |= HitObjectType.Circle;
                    break;
            }

            return type;
        }

        private void addCurveData(TextWriter writer, IHasCurve curveData, Vector2 position)
        {
            PathType? lastType = null;

            for (int i = 0; i < curveData.Path.ControlPoints.Count; i++)
            {
                PathControlPoint point = curveData.Path.ControlPoints[i];

                if (point.Type.Value != null)
                {
                    if (point.Type.Value != lastType)
                    {
                        switch (point.Type.Value)
                        {
                            case PathType.Bezier:
                                writer.Write("B|");
                                break;

                            case PathType.Catmull:
                                writer.Write("C|");
                                break;

                            case PathType.PerfectCurve:
                                writer.Write("P|");
                                break;

                            case PathType.Linear:
                                writer.Write("L|");
                                break;
                        }

                        lastType = point.Type.Value;
                    }
                    else
                    {
                        // New segment with the same type - duplicate the control point
                        writer.Write(FormattableString.Invariant($"{position.X + point.Position.Value.X}:{position.Y + point.Position.Value.Y}|"));
                    }
                }

                if (i != 0)
                {
                    writer.Write(FormattableString.Invariant($"{position.X + point.Position.Value.X}:{position.Y + point.Position.Value.Y}"));
                    writer.Write(i != curveData.Path.ControlPoints.Count - 1 ? "|" : ",");
                }
            }

            writer.Write(FormattableString.Invariant($"{curveData.RepeatCount + 1},"));
            writer.Write(FormattableString.Invariant($"{curveData.Path.Distance},"));

            for (int i = 0; i < curveData.NodeSamples.Count; i++)
            {
                writer.Write(FormattableString.Invariant($"{(int)toHitSoundType(curveData.NodeSamples[i])}"));
                writer.Write(i != curveData.NodeSamples.Count - 1 ? "|" : ",");
            }

            for (int i = 0; i < curveData.NodeSamples.Count; i++)
            {
                writer.Write(getSampleBank(curveData.NodeSamples[i], true));
                writer.Write(i != curveData.NodeSamples.Count - 1 ? "|" : ",");
            }
        }

        private void addEndTimeData(TextWriter writer, HitObject hitObject)
        {
            var endTimeData = (IHasEndTime)hitObject;
            getObjectType(hitObject);

            char suffix = ',';

            writer.Write(FormattableString.Invariant($"{endTimeData.EndTime}{suffix}"));
        }

        private string getSampleBank(IList<HitSampleInfo> samples, bool banksOnly = false, bool zeroBanks = false)
        {
            SampleBank normalBank = toSampleBank(samples.SingleOrDefault(s => s.Name == HitSampleInfo.HIT_NORMAL)?.Bank);
            SampleBank addBank = toSampleBank(samples.FirstOrDefault(s => !string.IsNullOrEmpty(s.Name) && s.Name != HitSampleInfo.HIT_NORMAL)?.Bank);

            StringBuilder sb = new StringBuilder();

            sb.Append(FormattableString.Invariant($"{(zeroBanks ? 0 : (int)normalBank)}:"));
            sb.Append(FormattableString.Invariant($"{(zeroBanks ? 0 : (int)addBank)}"));

            if (!banksOnly)
            {
                string customSampleBank = toCustomSampleBank(samples.FirstOrDefault(s => !string.IsNullOrEmpty(s.Name)));
                string sampleFilename = samples.FirstOrDefault(s => string.IsNullOrEmpty(s.Name))?.LookupNames.First() ?? string.Empty;
                int volume = samples.FirstOrDefault()?.Volume ?? 100;

                sb.Append(":");
                sb.Append(FormattableString.Invariant($"{customSampleBank}:"));
                sb.Append(FormattableString.Invariant($"{volume}:"));
                sb.Append(FormattableString.Invariant($"{sampleFilename}"));
            }

            return sb.ToString();
        }

        private HitSoundType toHitSoundType(IList<HitSampleInfo> samples)
        {
            HitSoundType type = HitSoundType.None;

            foreach (var sample in samples)
            {
                switch (sample.Name)
                {
                    case HitSampleInfo.HIT_WHISTLE:
                        type |= HitSoundType.Whistle;
                        break;

                    case HitSampleInfo.HIT_FINISH:
                        type |= HitSoundType.Finish;
                        break;

                    case HitSampleInfo.HIT_CLAP:
                        type |= HitSoundType.Clap;
                        break;
                }
            }

            return type;
        }

        private SampleBank toSampleBank(string sampleBank)
        {
            switch (sampleBank?.ToLowerInvariant())
            {
                case "normal":
                    return SampleBank.Normal;

                case "soft":
                    return SampleBank.Soft;

                case "drum":
                    return SampleBank.Drum;

                default:
                    return SampleBank.None;
            }
        }

        private string toCustomSampleBank(HitSampleInfo hitSampleInfo)
        {
            if (hitSampleInfo == null)
                return "0";

            if (hitSampleInfo is { } legacy)
                return legacy.CustomSampleBank.ToString(CultureInfo.InvariantCulture);

            return "0";
        }
    }
}
