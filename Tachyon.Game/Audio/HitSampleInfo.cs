﻿using System;
using System.Collections.Generic;

namespace Tachyon.Game.Audio
{
    /// <summary>
    /// Describes a gameplay hit sample.
    /// </summary>
    [Serializable]
    public class HitSampleInfo : ISampleInfo
    {
        public const string HIT_WHISTLE = @"hitwhistle";
        public const string HIT_FINISH = @"hitfinish";
        public const string HIT_NORMAL = @"hitnormal";
        public const string HIT_CLAP = @"hitclap";

        /// <summary>
        /// The bank to load the sample from.
        /// </summary>
        public string Bank;

        /// <summary>
        /// The name of the sample to load.
        /// </summary>
        public string Name;

        /// <summary>
        /// An optional suffix to provide priority lookup. Falls back to non-suffixed <see cref="Name"/>.
        /// </summary>
        public string Suffix;

        /// <summary>
        /// The sample volume.
        /// </summary>
        public int Volume { get; set; }
        
        public int CustomSampleBank
        {
            set
            {
                if (value > 1)
                    Suffix = value.ToString();
            }
        }

        /// <summary>
        /// Retrieve all possible filenames that can be used as a source, returned in order of preference (highest first).
        /// </summary>
        public virtual IEnumerable<string> LookupNames
        {
            get
            {
                if (!string.IsNullOrEmpty(Suffix))
                    yield return $"{Bank}-{Name}{Suffix}";

                yield return $"{Bank}-{Name}";
            }
        }

        public HitSampleInfo Clone() => (HitSampleInfo)MemberwiseClone();
    }
}