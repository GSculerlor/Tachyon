using System;
using System.Collections.Generic;
using System.IO;
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
        /// <summary>
        /// Handle [HitObject] parsing.
        /// Hit object syntax: x,y,time,type,hitSound,objectParams,hitSample
        /// - x (Integer) and y (Integer): Position in osu! pixels of the object.
        /// - time (Integer): Time when the object is to be hit, in milliseconds from the beginning of the beatmap's audio.
        /// - type (Integer): Bit flags indicating the type of the object. See the type section.
        /// - hitSound (Integer): Bit flags indicating the hitsound applied to the object. See the hitsounds section.
        /// - objectParams (Comma-separated list): Extra parameters specific to the object's type.
        /// - hitSample (Colon-separated list): Information about which samples are played when the object is hit. It is closely related to hitSound; see the hitsounds section. If it is not written, it defaults to 0:0:0:0:.
        /// </summary>
        /// <param name="text">text that will be parsed to hitobject.</param>
        /// <returns>HitObject result from parsing.</returns>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="InvalidDataException"></exception>
        public override HitObject Parse(string text)
        {
            string[] split = text.Split(',');
            
            // Handle x,y. But not really useful for Tachyon at this point. Will be considered to use for mapping.
            Vector2 pos = new Vector2((int)Parsing.ParseFloat(split[0], Parsing.MAX_COORDINATE_VALUE), (int)Parsing.ParseFloat(split[1], Parsing.MAX_COORDINATE_VALUE));

            //Handle time
            double startTime = Parsing.ParseDouble(split[2]);

            // Handle type that defined in HitObjectType. Not really used at this moment.
            HitObjectType type = (HitObjectType)Parsing.ParseInt(split[3]);

            int comboOffset = (int)(type & HitObjectType.ComboOffset) >> 4;
            type &= ~HitObjectType.ComboOffset;

            bool combo = type.HasFlag(HitObjectType.NewCombo);
            type &= ~HitObjectType.NewCombo;

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

                repeatCount = Math.Max(0, repeatCount - 1);

                if (split.Length > 7)
                {
                    length = Math.Max(0, Parsing.ParseDouble(split[7], Parsing.MAX_COORDINATE_VALUE));
                    // ReSharper disable once CompareOfFloatsByEqualityOperator
                    if (length == 0)
                        length = null;
                }
                
                if (split.Length > 10)
                    readCustomSampleBanks(split[10], bankInfo);

                int nodes = repeatCount + 2;

                var nodeBankInfos = new List<SampleBankInfo>();
                for (int i = 0; i < nodes; i++)
                    nodeBankInfos.Add(bankInfo.Clone());

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

                var nodeSoundTypes = new List<HitSoundType>();
                for (int i = 0; i < nodes; i++)
                    nodeSoundTypes.Add(soundType);

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

                var nodeSamples = new List<IList<HitSampleInfo>>(nodes);
                for (int i = 0; i < nodes; i++)
                    nodeSamples.Add(convertSoundType(nodeSoundTypes[i], nodeBankInfos[i]));

                result = createSlider(convertControlPoints(points, pathType), length, repeatCount);

                result.Samples = nodeSamples[^1];

            }
            else if (type.HasFlag(HitObjectType.Spinner))
            {
                double endTime = Math.Max(startTime, Parsing.ParseDouble(split[5]));

                result = createSpinner(endTime);
                
                if (split.Length > 6)
                    readCustomSampleBanks(split[6], bankInfo);
            }

            if (result == null)
                throw new InvalidDataException($"Unknown hit object type: {split[3]}");

            result.StartTime = startTime;
            
            if (result.Samples.Count == 0)
                result.Samples = convertSoundType(soundType, bankInfo);

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

            public override IEnumerable<string> LookupNames => new[]
            {
                Filename,
                Path.ChangeExtension(Filename, null)
            };
        }
    }
}