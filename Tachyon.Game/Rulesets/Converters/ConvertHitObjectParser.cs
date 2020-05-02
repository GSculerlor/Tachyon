using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using osu.Framework.Utils;
using osuTK;
using Tachyon.Game.Audio;
using Tachyon.Game.Beatmaps.Formats;
using Tachyon.Game.Beatmaps.Objects;
using Tachyon.Game.Rulesets.Objects;
using Tachyon.Game.Rulesets.Objects.Types;

namespace Tachyon.Game.Rulesets.Converters
{
    public class ConvertHitObjectParser : HitObjectParser
    {
        protected bool FirstObject { get; private set; } = true;

        public ConvertHitObjectParser()
        {
        }

        [CanBeNull]
        public override HitObject Parse(string text)
        {
            string[] split = text.Split(',');

            Vector2 pos = new Vector2((int)Parsing.ParseFloat(split[0], Parsing.MAX_COORDINATE_VALUE), (int)Parsing.ParseFloat(split[1], Parsing.MAX_COORDINATE_VALUE));

            //Handle time
            double startTime = Parsing.ParseDouble(split[2]);

            HitObjectType type = (HitObjectType)Parsing.ParseInt(split[3]);

            var soundType = (HitSoundType)Parsing.ParseInt(split[4]);
            var bankInfo = new SampleBankInfo();

            HitObject result = null;

            if (type.HasFlag(HitObjectType.Circle))
            {
                result = createHit();
                
                if (split.Length > 5)
                    readCustomSampleBanks(split[5], bankInfo);
            }
            else if (type.HasFlag(HitObjectType.Slider))
            {
                PathType pathType = PathType.Catmull;
                double? length = null;

                string[] pointSplit = split[5].Split('|');

                int pointCount = 1;

                foreach (var t in pointSplit)
                {
                    if (t.Length > 1)
                        pointCount++;
                }

                var points = new Vector2[pointCount];

                int pointIndex = 1;

                foreach (string t in pointSplit)
                {
                    if (t.Length == 1)
                    {
                        switch (t)
                        {
                            case @"C":
                                pathType = PathType.Catmull;
                                break;

                            case @"B":
                                pathType = PathType.Bezier;
                                break;

                            case @"L":
                                pathType = PathType.Linear;
                                break;

                            case @"P":
                                pathType = PathType.PerfectCurve;
                                break;
                        }

                        continue;
                    }

                    string[] temp = t.Split(':');
                    points[pointIndex++] = new Vector2((int)Parsing.ParseDouble(temp[0], Parsing.MAX_COORDINATE_VALUE), (int)Parsing.ParseDouble(temp[1], Parsing.MAX_COORDINATE_VALUE)) - pos;
                }

                int repeatCount = Parsing.ParseInt(split[6]);

                if (repeatCount > 9000)
                    throw new FormatException(@"Repeat count is way too high");

                // osu-stable treated the first span of the slider as a repeat, but no repeats are happening
                repeatCount = Math.Max(0, repeatCount - 1);

                if (split.Length > 7)
                {
                    length = Math.Max(0, Parsing.ParseDouble(split[7], Parsing.MAX_COORDINATE_VALUE));
                    if (length == 0)
                        length = null;
                }

                if (split.Length > 10)
                    readCustomSampleBanks(split[10], bankInfo);

                // One node for each repeat + the start and end nodes
                int nodes = repeatCount + 2;

                // Populate node sample bank infos with the default hit object sample bank
                var nodeBankInfos = new List<SampleBankInfo>();
                for (int i = 0; i < nodes; i++)
                    nodeBankInfos.Add(bankInfo.Clone());

                // Read any per-node sample banks
                if (split.Length > 9 && split[9].Length > 0)
                {
                    string[] sets = split[9].Split('|');

                    for (int i = 0; i < nodes; i++)
                    {
                        if (i >= sets.Length)
                            break;

                        SampleBankInfo info = nodeBankInfos[i];
                        readCustomSampleBanks(sets[i], info);
                    }
                }

                // Populate node sound types with the default hit object sound type
                var nodeSoundTypes = new List<HitSoundType>();
                for (int i = 0; i < nodes; i++)
                    nodeSoundTypes.Add(soundType);

                // Read any per-node sound types
                if (split.Length > 8 && split[8].Length > 0)
                {
                    string[] adds = split[8].Split('|');

                    for (int i = 0; i < nodes; i++)
                    {
                        if (i >= adds.Length)
                            break;

                        int.TryParse(adds[i], out var sound);
                        nodeSoundTypes[i] = (HitSoundType)sound;
                    }
                }

                // Generate the final per-node samples
                var nodeSamples = new List<IList<HitSampleInfo>>(nodes);
                for (int i = 0; i < nodes; i++)
                    nodeSamples.Add(convertSoundType(nodeSoundTypes[i], nodeBankInfos[i]));

                result = createSlider(convertControlPoints(points, pathType), length, repeatCount);

                // The samples are played when the slider ends, which is the last node
                result.Samples = nodeSamples[^1];
            }
            else if (type.HasFlag(HitObjectType.Spinner))
            {
                double endTime = Math.Max(startTime, Parsing.ParseDouble(split[5]));

                result = createSpinner(endTime);
                
                if (split.Length > 6)
                    readCustomSampleBanks(split[6], bankInfo);
            }
            //TODO: Implement hold
            /*else if (type.HasFlag(HitObjectType.Hold))
            {
                // Note: Hold is generated by BMS converts

                double endTime = Math.Max(startTime, Parsing.ParseDouble(split[2]));

                if (split.Length > 5 && !string.IsNullOrEmpty(split[5]))
                {
                    string[] ss = split[5].Split(':');
                    endTime = Math.Max(startTime, Parsing.ParseDouble(ss[0]));
                    readCustomSampleBanks(string.Join(":", ss.Skip(1)), bankInfo);
                }

                result = CreateHold(pos, combo, comboOffset, endTime + Offset);
            }*/

            if (result == null)
                throw new InvalidDataException($"Unknown hit object type: {split[3]}");

            result.StartTime = startTime;

            if (result.Samples.Count == 0)
                result.Samples = convertSoundType(soundType, bankInfo);

            FirstObject = false;

            return result;
        }

        private void readCustomSampleBanks(string str, SampleBankInfo bankInfo)
        {
            if (string.IsNullOrEmpty(str))
                return;

            string[] split = str.Split(':');

            var bank = (SampleBank)Parsing.ParseInt(split[0]);
            var addbank = (SampleBank)Parsing.ParseInt(split[1]);

            string stringBank = bank.ToString().ToLowerInvariant();
            if (stringBank == @"none")
                stringBank = null;
            string stringAddBank = addbank.ToString().ToLowerInvariant();
            if (stringAddBank == @"none")
                stringAddBank = null;

            bankInfo.Normal = stringBank;
            bankInfo.Add = string.IsNullOrEmpty(stringAddBank) ? stringBank : stringAddBank;

            if (split.Length > 2)
                bankInfo.CustomSampleBank = Parsing.ParseInt(split[2]);

            if (split.Length > 3)
                bankInfo.Volume = Math.Max(0, Parsing.ParseInt(split[3]));

            bankInfo.Filename = split.Length > 4 ? split[4] : null;
        }

        private PathControlPoint[] convertControlPoints(Vector2[] vertices, PathType type)
        {
            if (type == PathType.PerfectCurve)
            {
                if (vertices.Length != 3)
                    type = PathType.Bezier;
                else if (isLinear(vertices))
                {
                    // osu-stable special-cased colinear perfect curves to a linear path
                    type = PathType.Linear;
                }
            }

            var points = new List<PathControlPoint>(vertices.Length)
            {
                new PathControlPoint
                {
                    Position = { Value = vertices[0] },
                    Type = { Value = type }
                }
            };

            for (int i = 1; i < vertices.Length; i++)
            {
                if (vertices[i] == vertices[i - 1])
                {
                    points[^1].Type.Value = type;
                    continue;
                }

                points.Add(new PathControlPoint { Position = { Value = vertices[i] } });
            }

            return points.ToArray();

            static bool isLinear(Vector2[] p) => Precision.AlmostEquals(0, (p[1].Y - p[0].Y) * (p[2].X - p[0].X) - (p[1].X - p[0].X) * (p[2].Y - p[0].Y));
        }

        private HitObject createHit()
        {
            return new ConvertHit();
        }

        private HitObject createSlider(PathControlPoint[] controlPoints, double? length, int repeatCount)
        {
            return new ConvertSlider
            {
                Path = new SliderPath(controlPoints, length),
                RepeatCount = repeatCount
            };
        }

        private HitObject createSpinner(double endTime)
        {
            return new ConvertSpinner
            {
                EndTime = endTime
            };
        }
        
        private List<HitSampleInfo> convertSoundType(HitSoundType type, SampleBankInfo bankInfo)
        {
            if (!string.IsNullOrEmpty(bankInfo.Filename))
            {
                return new List<HitSampleInfo>
                {
                    new FileHitSampleInfo
                    {
                        Filename = bankInfo.Filename,
                        Volume = bankInfo.Volume
                    }
                };
            }

            var soundTypes = new List<HitSampleInfo>
            {
                new HitSampleInfo
                {
                    Bank = bankInfo.Normal,
                    Name = HitSampleInfo.HIT_NORMAL,
                    Volume = bankInfo.Volume,
                    CustomSampleBank = bankInfo.CustomSampleBank
                }
            };

            if (type.HasFlag(HitSoundType.Finish))
            {
                soundTypes.Add(new HitSampleInfo
                {
                    Bank = bankInfo.Add,
                    Name = HitSampleInfo.HIT_FINISH,
                    Volume = bankInfo.Volume,
                    CustomSampleBank = bankInfo.CustomSampleBank
                });
            }

            if (type.HasFlag(HitSoundType.Whistle))
            {
                soundTypes.Add(new HitSampleInfo
                {
                    Bank = bankInfo.Add,
                    Name = HitSampleInfo.HIT_WHISTLE,
                    Volume = bankInfo.Volume,
                    CustomSampleBank = bankInfo.CustomSampleBank
                });
            }

            if (type.HasFlag(HitSoundType.Clap))
            {
                soundTypes.Add(new HitSampleInfo
                {
                    Bank = bankInfo.Add,
                    Name = HitSampleInfo.HIT_CLAP,
                    Volume = bankInfo.Volume,
                    CustomSampleBank = bankInfo.CustomSampleBank
                });
            }

            return soundTypes;
        }

        private class SampleBankInfo
        {
            public string Filename;

            public string Normal;
            public string Add;
            public int Volume;

            public int CustomSampleBank;

            public SampleBankInfo Clone() => (SampleBankInfo)MemberwiseClone();
        }

        private class FileHitSampleInfo : HitSampleInfo
        {
            public string Filename;

            public FileHitSampleInfo()
            {
                // Make sure that the BeatmapSkin does not fall back to the user skin.
                // Note that this does not change the lookup names, as they are overridden locally.
                CustomSampleBank = 1;
            }

            public override IEnumerable<string> LookupNames => new[]
            {
                Filename,
                Path.ChangeExtension(Filename, null)
            };
        }
    }
}