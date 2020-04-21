using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using osu.Framework.Extensions;
using osu.Framework.Logging;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.Beatmaps.Objects;
using Tachyon.Game.Beatmaps.Timing;
using Tachyon.Game.IO;
using Tachyon.Game.Rulesets.Converters;

namespace Tachyon.Game.Beatmaps.Formats
{
    public class BeatmapDecoder : Decoder<Beatmap>
    {
        private readonly List<ControlPoint> pendingControlPoints = new List<ControlPoint>();
        
        private Beatmap outputBeatmap;
        private SampleBank defaultSampleBank;
        private ConvertHitObjectParser parser;
        
        private double pendingControlPointsTime;
        private int defaultSampleVolume = 100;
        
        public static void Register()
        {
            AddDecoder<Beatmap>(@"osu file format v", m => new BeatmapDecoder());
        }
        
        protected override void ParseStreamInto(LineBufferedReader stream, Beatmap beatmap)
        {
            outputBeatmap = beatmap;
            
            var section = Section.None;

            string line;

            while ((line = stream.ReadLine()) != null)
            {
                if (shouldSkipLine(line))
                    continue;

                if (line.StartsWith('[') && line.EndsWith(']'))
                {
                    if (!Enum.TryParse(line[1..^1], out section))
                    {
                        Logger.Log($"Unknown section \"{line}\" in \"{beatmap}\"");
                        section = Section.None;
                    }

                    continue;
                }

                try
                {
                    ParseLine(section, line);
                }
                catch (Exception e)
                {
                    Logger.Log($"Failed to process line \"{line}\" into \"{beatmap}\": {e.Message}", LoggingTarget.Runtime, LogLevel.Important);
                }
            }
            
            // Objects may be out of order *only* if a user has manually edited an .osu file.
            // Unfortunately there are ranked maps in this state (example: https://osu.ppy.sh/s/594828).
            // OrderBy is used to guarantee that the parsing order of hitobjects with equal start times is maintained (stably-sorted)
            // The parsing order of hitobjects matters in mania difficulty calculation
            outputBeatmap.HitObjects = outputBeatmap.HitObjects.OrderBy(h => h.StartTime).ToList();

            foreach (var hitObject in outputBeatmap.HitObjects)
                hitObject.ApplyDefaults(outputBeatmap.ControlPointInfo, outputBeatmap.BeatmapInfo.BaseDifficulty);
        }
        
        private bool shouldSkipLine(string line) => string.IsNullOrWhiteSpace(line) || line.AsSpan().TrimStart().StartsWith("//".AsSpan(), StringComparison.Ordinal) || line.StartsWith(' ') || line.StartsWith('_');

        private string cleanFilename(string path) => path.Trim('"').ToStandardisedPath();
        
        protected void ParseLine(Section section, string line)
        {
            var strippedLine = stripComments(line);

            switch (section)
            {
                case Section.General:
                    handleGeneral(strippedLine);
                    return;

                case Section.Metadata:
                    handleMetadata(line);
                    return;

                case Section.Difficulty:
                    handleDifficulty(strippedLine);
                    return;
                
                case Section.Events:
                    handleEvent(strippedLine);
                    return;

                case Section.TimingPoints:
                    handleTimingPoint(strippedLine);
                    return;

                case Section.HitObjects:
                    handleHitObject(strippedLine);
                    return;
                
                case Section.Editor:
                    return;
            }
        }

        private string stripComments(string line)
        {
            var index = line.AsSpan().IndexOf("//".AsSpan());
            if (index > 0)
                return line.Substring(0, index);

            return line;
        }

        private KeyValuePair<string, string> splitKeyVal(string line, char separator = ':')
        {
            var split = line.Split(separator, 2);

            return new KeyValuePair<string, string>
            (
                split[0].Trim(),
                split.Length > 1 ? split[1].Trim() : string.Empty
            );
        }

        private TimingControlPoint createTimingControlPoint() => new TimingControlPoint();
        
        private void addControlPoint(double time, ControlPoint point, bool timingChange)
        {
            if (Math.Abs(time - pendingControlPointsTime) > 0.01)
                flushPendingPoints();

            if (timingChange)
            {
                outputBeatmap.ControlPointInfo.Add(time, point);
                return;
            }

            pendingControlPoints.Add(point);
            pendingControlPointsTime = time;
        }
        
        private void flushPendingPoints()
        {
            foreach (var p in pendingControlPoints)
                outputBeatmap.ControlPointInfo.Add(pendingControlPointsTime, p);

            pendingControlPoints.Clear();
        }
        
        private void handleGeneral(string line)
        {
            var pair = splitKeyVal(line);

            var metadata = outputBeatmap.BeatmapInfo.Metadata;

            switch (pair.Key)
            {
                case @"AudioFilename":
                    metadata.AudioFile = pair.Value.ToStandardisedPath();
                    break;

                case @"AudioLeadIn":
                    outputBeatmap.BeatmapInfo.AudioLeadIn = Parsing.ParseInt(pair.Value);
                    break;

                case @"PreviewTime":
                    metadata.PreviewTime = Parsing.ParseInt(pair.Value);
                    break;

                case @"Countdown":
                    outputBeatmap.BeatmapInfo.Countdown = Parsing.ParseInt(pair.Value) == 1;
                    break;

                case @"SampleSet":
                    defaultSampleBank = (SampleBank)Enum.Parse(typeof(SampleBank), pair.Value);
                    break;

                case @"SampleVolume":
                    defaultSampleVolume = Parsing.ParseInt(pair.Value);
                    break;

                case @"StackLeniency":
                    outputBeatmap.BeatmapInfo.StackLeniency = Parsing.ParseFloat(pair.Value);
                    break;

                case @"Mode":
                    parser = new ConvertHitObjectParser();
                    break;
            }
        }

        private void handleMetadata(string line)
        {
            var pair = splitKeyVal(line);

            var metadata = outputBeatmap.BeatmapInfo.Metadata;

            switch (pair.Key)
            {
                case @"Title":
                    metadata.Title = pair.Value;
                    break;

                case @"TitleUnicode":
                    metadata.TitleUnicode = pair.Value;
                    break;

                case @"Artist":
                    metadata.Artist = pair.Value;
                    break;

                case @"ArtistUnicode":
                    metadata.ArtistUnicode = pair.Value;
                    break;

                case @"Version":
                    outputBeatmap.BeatmapInfo.Version = pair.Value;
                    break;

                case @"Source":
                    metadata.Source = pair.Value;
                    break;

                case @"Tags":
                    metadata.Tags = pair.Value;
                    break;
                
                case @"BeatmapID":
                    outputBeatmap.BeatmapInfo.OnlineBeatmapID = Parsing.ParseInt(pair.Value);
                    break;

                case @"BeatmapSetID":
                    outputBeatmap.BeatmapInfo.BeatmapSet = new BeatmapSetInfo { OnlineBeatmapSetID = Parsing.ParseInt(pair.Value) };
                    break;
            }
        }

        private void handleDifficulty(string line)
        {
            var pair = splitKeyVal(line);

            var difficulty = outputBeatmap.BeatmapInfo.BaseDifficulty;

            switch (pair.Key)
            {
                case @"HPDrainRate":
                    difficulty.DrainRate = Parsing.ParseFloat(pair.Value);
                    break;

                case @"CircleSize":
                    difficulty.CircleSize = Parsing.ParseFloat(pair.Value);
                    break;

                case @"OverallDifficulty":
                    difficulty.OverallDifficulty = Parsing.ParseFloat(pair.Value);
                    break;

                case @"ApproachRate":
                    difficulty.ApproachRate = Parsing.ParseFloat(pair.Value);
                    break;

                case @"SliderMultiplier":
                    difficulty.SliderMultiplier = Parsing.ParseDouble(pair.Value);
                    break;

                case @"SliderTickRate":
                    difficulty.SliderTickRate = Parsing.ParseDouble(pair.Value);
                    break;
            }
        }
        
        private void handleEvent(string line)
        {
            string[] split = line.Split(',');

            if (!Enum.TryParse(split[0], out EventType type))
                throw new InvalidDataException($@"Unknown event type: {split[0]}");

            switch (type)
            {
                case EventType.Background:
                    outputBeatmap.BeatmapInfo.Metadata.BackgroundFile = cleanFilename(split[2]);
                    break;
            }
        }

        // 374, 681.818181818182, 4, 1, 0, 20, 1, 0
        // time, beatLength, meter, sampleSet, sampleIndex, volume, uninherited, effects
        private void handleTimingPoint(string line)
        {
            var split = line.Split(',');

            var time = Parsing.ParseDouble(split[0].Trim());
            var beatLength = Parsing.ParseDouble(split[1].Trim());
            var speedMultiplier = beatLength < 0 ? 100.0 / -beatLength : 1;

            var timeSignature = TimeSignatures.SimpleQuadruple;
            if (split.Length >= 3)
                timeSignature = split[2][0] == '0' ? TimeSignatures.SimpleQuadruple : (TimeSignatures)Parsing.ParseInt(split[2]);

            var sampleSet = defaultSampleBank;
            if (split.Length >= 4)
                sampleSet = (SampleBank)Parsing.ParseInt(split[3]);

            var sampleVolume = defaultSampleVolume;
            if (split.Length >= 6)
                sampleVolume = Parsing.ParseInt(split[5]);

            var timingChange = true;
            if (split.Length >= 7)
                timingChange = split[6][0] == '1';

            var kiaiMode = false;
            var omitFirstBarSignature = false;

            if (split.Length >= 8)
            {
                var effectFlags = (EffectFlags)Parsing.ParseInt(split[7]);
                kiaiMode = effectFlags.HasFlag(EffectFlags.Kiai);
                omitFirstBarSignature = effectFlags.HasFlag(EffectFlags.OmitFirstBarLine);
            }

            var stringSampleSet = sampleSet.ToString().ToLowerInvariant();
            if (stringSampleSet == @"none")
                stringSampleSet = @"normal";

            if (timingChange)
            {
                var controlPoint = createTimingControlPoint();

                controlPoint.BeatLength = beatLength;
                controlPoint.TimeSignature = timeSignature;

                addControlPoint(time, controlPoint, true);
            }

            addControlPoint(time, new DifficultyControlPoint
            {
                SpeedMultiplier = speedMultiplier,
            }, timingChange);

            addControlPoint(time, new EffectControlPoint
            {
                KiaiMode = kiaiMode,
                OmitFirstBarLine = omitFirstBarSignature,
            }, timingChange);

            addControlPoint(time, new SampleControlPoint
            {
                SampleBank = stringSampleSet,
                SampleVolume = sampleVolume,
            }, timingChange);

            if (timingChange)
                flushPendingPoints();
        }
        
        private void handleHitObject(string line)
        {
            if (parser == null)
                parser = new ConvertHitObjectParser();

            var obj = parser.Parse(line);
            if (obj != null)
                outputBeatmap.HitObjects.Add(obj);
        }

        protected enum Section
        {
            None,
            General,
            Metadata,
            Difficulty,
            Events,
            TimingPoints,
            HitObjects,
            //Not used for now
            Editor
        }
    }
}