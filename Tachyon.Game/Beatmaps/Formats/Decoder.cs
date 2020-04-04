using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tachyon.Game.IO;

namespace Tachyon.Game.Beatmaps.Formats
{
    public abstract class Decoder<TOutput> : Decoder
        where TOutput : new()
    {
        protected virtual TOutput CreateTemplateObject() => new TOutput();

        public TOutput Decode(LineBufferedReader primaryStream, params LineBufferedReader[] otherStreams)
        {
            var output = CreateTemplateObject();
            foreach (LineBufferedReader stream in otherStreams.Prepend(primaryStream))
                ParseStreamInto(stream, output);
            return output;
        }

        protected abstract void ParseStreamInto(LineBufferedReader stream, TOutput output);
    }
    
    public abstract class Decoder
    {
        private static readonly Dictionary<Type, Dictionary<string, Func<string, Decoder>>> decoders = new Dictionary<Type, Dictionary<string, Func<string, Decoder>>>();

        static Decoder()
        {
            BeatmapDecoder.Register();
        }

        public static Decoder<T> GetDecoder<T>(LineBufferedReader stream)
            where T : new()
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!decoders.TryGetValue(typeof(T), out var typedDecoders))
                throw new IOException(@"Unknown decoder type");

            string line = stream.PeekLine()?.Trim();

            while (line != null && line.Length == 0)
            {
                stream.ReadLine();
                line = stream.PeekLine()?.Trim();
            }

            if (line == null)
                throw new IOException("Unknown file format (null)");

            var decoder = typedDecoders.Select(d => line.StartsWith(d.Key, StringComparison.InvariantCulture) ? d.Value : null).FirstOrDefault();

            return (Decoder<T>)decoder?.Invoke(line);
        }
        
        protected static void AddDecoder<T>(string magic, Func<string, Decoder> constructor)
        {
            if (!decoders.TryGetValue(typeof(T), out var typedDecoders))
                decoders.Add(typeof(T), typedDecoders = new Dictionary<string, Func<string, Decoder>>());

            typedDecoders[magic] = constructor;
        }
    }
}