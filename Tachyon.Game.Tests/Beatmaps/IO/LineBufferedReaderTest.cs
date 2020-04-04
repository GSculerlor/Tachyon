using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Tachyon.Game.IO;

namespace Tachyon.Game.Tests.Beatmaps.IO
{
    [TestFixture]
    public class LineBufferedReaderTest
    {
        [Test]
        public void TestReadLineByLine()
        {
            const string contents = "Exusiai\rTexas\nLappland";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
            using (var bufferedReader = new LineBufferedReader(stream))
            {    
                Assert.AreEqual("Exusiai", bufferedReader.ReadLine());
                Assert.AreEqual("Texas", bufferedReader.ReadLine());
                Assert.AreEqual("Lappland", bufferedReader.ReadLine());
                Assert.IsNull(bufferedReader.ReadLine());
            }
        }
        
        [Test]
        public void TestPeekLineOnce()
        {
            const string contents = "Exusiai\r\nTexas\nLappland";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
            using (var bufferedReader = new LineBufferedReader(stream))
            {
                Assert.AreEqual("Exusiai", bufferedReader.ReadLine());
                Assert.AreEqual("Texas", bufferedReader.PeekLine());
                Assert.AreEqual("Texas", bufferedReader.ReadLine());
                Assert.AreEqual("Lappland", bufferedReader.ReadLine());
                Assert.IsNull(bufferedReader.ReadLine());
            }
        }
        
        [Test]
        public void TestPeekLineMultipleTimes()
        {
            const string contents = "Peeking Exusiai\nTexas\rPeeking Lappland multiple times";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
            using (var bufferedReader = new LineBufferedReader(stream))
            {
                Assert.AreEqual("Peeking Exusiai", bufferedReader.PeekLine());
                Assert.AreEqual("Peeking Exusiai", bufferedReader.ReadLine());
                Assert.AreEqual("Texas", bufferedReader.ReadLine());
                Assert.AreEqual("Peeking Lappland multiple times", bufferedReader.PeekLine());
                Assert.AreEqual("Peeking Lappland multiple times", bufferedReader.PeekLine());
                Assert.AreEqual("Peeking Lappland multiple times", bufferedReader.PeekLine());
                Assert.AreEqual("Peeking Lappland multiple times", bufferedReader.ReadLine());
                Assert.IsNull(bufferedReader.ReadLine());
            }
        }
        
        [Test]
        public void TestPeekLineAtEndOfStream()
        {
            const string contents = "Exusiai\r\nTexas";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
            using (var bufferedReader = new LineBufferedReader(stream))
            {
                Assert.AreEqual("Exusiai", bufferedReader.ReadLine());
                Assert.AreEqual("Texas", bufferedReader.ReadLine());
                Assert.IsNull(bufferedReader.PeekLine());
                Assert.IsNull(bufferedReader.ReadLine());
                Assert.IsNull(bufferedReader.PeekLine());
            }
        }
        
        [Test]
        public void TestPeekReadLineOnEmptyStream()
        {
            using (var stream = new MemoryStream())
            using (var bufferedReader = new LineBufferedReader(stream))
            {
                Assert.IsNull(bufferedReader.PeekLine());
                Assert.IsNull(bufferedReader.ReadLine());
                Assert.IsNull(bufferedReader.ReadLine());
                Assert.IsNull(bufferedReader.PeekLine());
            }
        }
        
        [Test]
        public void TestReadToEndNoPeeks()
        {
            const string contents = "Exusiai\r\nTexas";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
            using (var bufferedReader = new LineBufferedReader(stream))
            {
                Assert.AreEqual(contents, bufferedReader.ReadToEnd());
            }
        }
        
        [Test]
        public void TestReadToEndAfterReadsAndPeeks()
        {
            const string contents = "Exusiai\rTexas\r\nLappland\nWot";

            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
            using (var bufferedReader = new LineBufferedReader(stream))
            {
                Assert.AreEqual("Exusiai", bufferedReader.ReadLine());
                Assert.AreEqual("Texas", bufferedReader.PeekLine());

                var endingLines = bufferedReader.ReadToEnd().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                Assert.AreEqual(3, endingLines.Length);
                Assert.AreEqual("Texas", endingLines[0]);
                Assert.AreEqual("Lappland", endingLines[1]);
                Assert.AreEqual("Wot", endingLines[2]);
            }
        }
    }
}