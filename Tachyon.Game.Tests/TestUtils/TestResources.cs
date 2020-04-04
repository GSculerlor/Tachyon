using System.IO;
using NUnit.Framework;
using osu.Framework.IO.Stores;

namespace Tachyon.Game.Tests.TestUtils
{
    public static class TestResources
    {
        public static DllResourceStore GetStore() => new DllResourceStore(@"Tachyon.Resources.dll");
        
        public static Stream OpenResource(string name) => GetStore().GetStream($"{name}");

        public static Stream GetBeatmapsetForTest(string filename = "1125727 O2i3 - Ooi.osz") =>
            OpenResource($"Tracks/Tests/{filename}");

        public static Stream GetBeatmapForTest(string filename = "O2i3 - Ooi (Capu) [Inner Ooni].osu") =>
            OpenResource($"Tracks/Tests/Osu/{filename}");
        
        public static string GetTestBeatmapForImport()
        {
            var temp = Path.GetTempFileName() + ".osz";

            using (var stream = GetBeatmapsetForTest("1016229 Chroma - Made In Love_test.osz"))
            using (var newFile = File.Create(temp))
                stream.CopyTo(newFile);

            Assert.IsTrue(File.Exists(temp));
            return temp;
        }
    }
}