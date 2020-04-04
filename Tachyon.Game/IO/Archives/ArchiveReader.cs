using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using osu.Framework.IO.Stores;

namespace Tachyon.Game.IO.Archives
{
    public abstract class ArchiveReader : IResourceStore<byte[]>
    {
        public abstract Stream GetStream(string name);

        public IEnumerable<string> GetAvailableResources() => Filenames;

        public abstract void Dispose();

        public readonly string Name;

        protected ArchiveReader(string name)
        {
            Name = name;
        }

        public abstract IEnumerable<string> Filenames { get; }

        public virtual byte[] Get(string name) => GetAsync(name).Result;

        public async Task<byte[]> GetAsync(string name)
        {
            using (Stream input = GetStream(name))
            {
                if (input == null)
                    return null;

                byte[] buffer = new byte[input.Length];
                await input.ReadAsync(buffer, 0, buffer.Length);
                return buffer;
            }
        }
    }
}
