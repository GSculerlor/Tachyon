using System.Collections.Generic;

namespace Tachyon.Game.Audio
{
    /// <summary>
    /// Describes a gameplay sample.
    /// </summary>
    public class SampleInfo : ISampleInfo
    {
        private readonly string sampleName;

        public SampleInfo(string sampleName)
        {
            this.sampleName = sampleName;
        }

        public IEnumerable<string> LookupNames => new[] { sampleName };

        public int Volume { get; } = 100;
    }
}
