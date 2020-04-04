using System.Collections.Generic;
using System.IO;

namespace Tachyon.Game.IO.Archives
{
    /// <summary>
    /// Allows reading a single file from the provided stream.
    /// </summary>
    public class LegacyByteArrayReader : ArchiveReader
    {
        private readonly byte[] content;

        public LegacyByteArrayReader(byte[] content, string filename)
            : base(filename)
        {
            this.content = content;
        }

        public override Stream GetStream(string name) => new MemoryStream(content);

        public override void Dispose()
        {
        }

        public override IEnumerable<string> Filenames => new[] { Name };
    }
}
