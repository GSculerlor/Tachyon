using System;
using System.Collections.Generic;
using System.Linq;
using Tachyon.Game.Audio;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.Rulesets.Objects;
using Tachyon.Game.Rulesets.Objects.Types;

namespace Tachyon.Game.Rulesets.Beatmaps
{
    internal class TachyonBeatmapConverter : BeatmapConverter<TachyonHitObject>
    {
        /// <summary>
        /// osu! is generally slower than tachyon, so a factor is added to increase
        /// speed. This must be used everywhere slider length or beat length is used.
        /// </summary>
        private const float legacy_velocity_multiplier = 1.4f;

        /// <summary>
        /// Because swells are easier in tachyon than spinners are in osu!,
        /// legacy tachyon multiplies a factor when converting the number of required hits.
        /// </summary>
        private const float swell_hit_multiplier = 1.65f;

        /// <summary>
        /// Base osu! slider scoring distance.
        /// </summary>
        private const float osu_base_scoring_distance = 100;

        /// <summary>
        /// Drum roll distance that results in a duration of 1 speed-adjusted beat length.
        /// </summary>
        private const float tachyon_base_distance = 100;

        private readonly bool isForCurrentRuleset;

        public TachyonBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
            isForCurrentRuleset = true;
        }

        public override bool CanConvert() => true;

        protected override Beatmap<TachyonHitObject> ConvertBeatmap(IBeatmap original)
        {
            // Rewrite the beatmap info to add the slider velocity multiplier
            original.BeatmapInfo = original.BeatmapInfo.Clone();
            original.BeatmapInfo.BaseDifficulty = original.BeatmapInfo.BaseDifficulty.Clone();
            original.BeatmapInfo.BaseDifficulty.SliderMultiplier *= legacy_velocity_multiplier;

            Beatmap<TachyonHitObject> converted = base.ConvertBeatmap(original);

            return converted;
        }

        protected override IEnumerable<TachyonHitObject> ConvertHitObject(HitObject obj, IBeatmap beatmap)
        {
            // Old osu! used hit sounding to determine various hit type information
            IList<HitSampleInfo> samples = obj.Samples;

            bool strong = samples.Any(s => s.Name == HitSampleInfo.HIT_FINISH);

            switch (obj)
            {
                case IHasDistance distanceData:
                {
                    // Number of spans of the object - one for the initial length and for each repeat
                    int spans = (obj as IHasRepeats)?.SpanCount() ?? 1;

                    TimingControlPoint timingPoint = beatmap.ControlPointInfo.TimingPointAt(obj.StartTime);
                    DifficultyControlPoint difficultyPoint = beatmap.ControlPointInfo.DifficultyPointAt(obj.StartTime);

                    double speedAdjustment = difficultyPoint.SpeedMultiplier;
                    double speedAdjustedBeatLength = timingPoint.BeatLength / speedAdjustment;

                    // The true distance, accounting for any repeats. This ends up being the drum roll distance later
                    double distance = distanceData.Distance * spans * legacy_velocity_multiplier;

                    // The velocity of the tachyon hit object - calculated as the velocity of a drum roll
                    double tachyonVelocity = tachyon_base_distance * beatmap.BeatmapInfo.BaseDifficulty.SliderMultiplier / speedAdjustedBeatLength;
                    // The duration of the tachyon hit object
                    double tachyonDuration = distance / tachyonVelocity;

                    // The velocity of the osu! hit object - calculated as the velocity of a slider
                    double osuVelocity = osu_base_scoring_distance * beatmap.BeatmapInfo.BaseDifficulty.SliderMultiplier / speedAdjustedBeatLength;
                    // The duration of the osu! hit object
                    double osuDuration = distance / osuVelocity;
                    
                    speedAdjustedBeatLength *= speedAdjustment;

                    // If the drum roll is to be split into hit circles, assume the ticks are 1/8 spaced within the duration of one beat
                    double tickSpacing = Math.Min(speedAdjustedBeatLength / beatmap.BeatmapInfo.BaseDifficulty.SliderTickRate, tachyonDuration / spans);

                    if (!isForCurrentRuleset && tickSpacing > 0 && osuDuration < 2 * speedAdjustedBeatLength)
                    {
                        List<IList<HitSampleInfo>> allSamples = obj is IHasCurve curveData ? curveData.NodeSamples : new List<IList<HitSampleInfo>>(new[] { samples });

                        int i = 0;

                        for (double j = obj.StartTime; j <= obj.StartTime + tachyonDuration + tickSpacing / 8; j += tickSpacing)
                        {
                            IList<HitSampleInfo> currentSamples = allSamples[i];
                            bool isUpper = currentSamples.Any(s => s.Name == HitSampleInfo.HIT_CLAP || s.Name == HitSampleInfo.HIT_WHISTLE);

                            yield return new Note
                            {
                                StartTime = j,
                                Type = isUpper ? NoteType.Upper : NoteType.Lower,
                                Row = isUpper ? 0 : 1,
                                Samples = currentSamples,
                            };

                            i = (i + 1) % allSamples.Count;
                        }
                    }
                    else
                    {
                        yield return new HoldNote
                        {
                            StartTime = obj.StartTime,
                            Samples = obj.Samples,
                            Duration = tachyonDuration,
                            TickRate = beatmap.BeatmapInfo.BaseDifficulty.SliderTickRate == 3 ? 3 : 4,
                            Row = 0
                        };
                    }

                    break;
                }

                case IHasEndTime endTimeData:
                {
                    double hitMultiplier = BeatmapDifficulty.DifficultyRange(beatmap.BeatmapInfo.BaseDifficulty.OverallDifficulty, 3, 5, 7.5) * swell_hit_multiplier;

                    yield return new HoldNote
                    {
                        StartTime = obj.StartTime,
                        Samples = obj.Samples,
                        Duration = endTimeData.Duration,
                        TickRate = beatmap.BeatmapInfo.BaseDifficulty.SliderTickRate == 3 ? 3 : 4,
                        Row = 1
                    };

                    break;
                }

                default:
                {
                    bool isUpper = samples.Any(s => s.Name == HitSampleInfo.HIT_CLAP || s.Name == HitSampleInfo.HIT_WHISTLE);

                    yield return new Note
                    {
                        StartTime = obj.StartTime,
                        Type = isUpper ? NoteType.Upper : NoteType.Lower,
                        Row = isUpper ? 0 : 1,
                        Samples = obj.Samples,
                    };

                    break;
                }
            }
        }

        protected override Beatmap<TachyonHitObject> CreateBeatmap() => new TachyonBeatmap();
    }
}
