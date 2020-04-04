using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tachyon.Game.IO
{
    public class LineBufferedReader : IDisposable
    {
        private readonly StreamReader streamReader;
        private readonly Queue<string> lineBuffer;

        public LineBufferedReader(Stream stream)
        {
            streamReader = new StreamReader(stream);
            lineBuffer = new Queue<string>();
        }

        public string PeekLine()
        {
            if (lineBuffer.Count > 0)
                return lineBuffer.Peek();

            var line = streamReader.ReadLine();
            if (line != null)
                lineBuffer.Enqueue(line);
            return line;
        }

        public string ReadLine() => lineBuffer.Count > 0 ? lineBuffer.Dequeue() : streamReader.ReadLine();

        public string ReadToEnd()
        {
            var remainingText = streamReader.ReadToEnd();
            if (lineBuffer.Count == 0)
                return remainingText;

            var builder = new StringBuilder();

            while (lineBuffer.Count > 0)
                builder.AppendLine(lineBuffer.Dequeue());
            builder.Append(remainingText);

            return builder.ToString();
        }

        public void Dispose()
        {
            streamReader?.Dispose();
        }
    }
}
