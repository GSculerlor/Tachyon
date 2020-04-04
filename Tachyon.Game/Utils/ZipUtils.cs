using System;
using System.IO;
using SharpCompress.Archives.Zip;

namespace Tachyon.Game.Utils
{
    public static class ZipUtils
    {
        public static bool IsZipArchive(string path)
        {
            if (!File.Exists(path))
                return false;

            try
            {
                using (var arc = ZipArchive.Open(path))
                {
                    foreach (var entry in arc.Entries)
                    {
                        using (entry.OpenEntryStream())
                        {
                        }
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
