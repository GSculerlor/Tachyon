using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ManagedBass;
using osu.Framework.Audio.Callbacks;
using osu.Framework.Audio.Track;
using osu.Framework.Utils;

namespace Tachyon.Game.Generator.Waveforms
{
    public class TachyonWaveform : IDisposable
    {
        /// <summary>
        /// Minimum frequency for low-range (bass) frequencies. 0 seems not really good for lower range.
        /// </summary>
        private const double low_min = 20;

        /// <summary>
        /// Minimum frequency for mid-range frequencies.
        /// </summary>
        private const double mid_min = 250;

        /// <summary>
        /// Minimum frequency for high-range (treble) frequencies.
        /// </summary>
        private const double high_min = 2000;

        /// <summary>
        /// Maximum frequency for high-range (treble) frequencies. 20000 seems not a good idea either.
        /// </summary>
        private const double high_max = 12000;
        
        private List<WaveformSection> sections = new List<WaveformSection>();
        private FileCallbacks fileCallbacks;
        private int channels;
        
        private readonly Task readStreamTask;
        private readonly CancellationTokenSource cancelSource = new CancellationTokenSource();

        public TachyonWaveform(Stream stream)
        {
            if (stream == null) return;

            readStreamTask = Task.Run(() =>
            {
                //Skip if there's no device available.
                if (Bass.CurrentDevice <= 0)
                    return;
                
                fileCallbacks = new FileCallbacks(new DataStreamFileProcedures(stream));
                
                //https://github.com/ManagedBass/ManagedBass.DocFX/blob/master/apidoc/Bass/Bass.CreateStream.md
                int decodeStream = Bass.CreateStream(StreamSystem.NoBuffer, BassFlags.Decode | BassFlags.Float, fileCallbacks.Callbacks, fileCallbacks.Handle);
                // https://github.com/ManagedBass/ManagedBass.DocFX/blob/master/apidoc/Bass/Bass.ChannelGetInfo.md
                Bass.ChannelGetInfo(decodeStream, out ChannelInfo info);

                // https://github.com/ManagedBass/ManagedBass.DocFX/blob/master/apidoc/Bass/Bass.ChannelGetLength.md
                long length = Bass.ChannelGetLength(decodeStream);
                
                // 0.001f means WaveformSection generated every 1ms resolution.
                int samplesPerSection = (int)(info.Frequency * 0.001f * info.Channels);
                int bytesPerSection = samplesPerSection * TrackBass.BYTES_PER_SAMPLE;
                sections.Capacity = (int)(length / bytesPerSection);
                
                // 100000 is considered still managed by BASS's internal buffer size.
                int bytesPerIteration = bytesPerSection * 100000;
                var sampleBuffer = new float[bytesPerIteration / TrackBass.BYTES_PER_SAMPLE];

                while (length > 0)
                {
                    // https://github.com/ManagedBass/ManagedBass.DocFX/blob/master/apidoc/Bass/Bass.ChannelGetData.md
                    length = Bass.ChannelGetData(decodeStream, sampleBuffer, bytesPerIteration);
                    int samplesRead = (int)(length / TrackBass.BYTES_PER_SAMPLE);
                    
                    for (int i = 0; i < samplesRead; i += samplesPerSection)
                    {
                        // Channels are interleaved in the sample data (data[0] -> channel0, data[1] -> channel1, data[2] -> channel0, etc)
                        var section = new WaveformSection(info.Channels);

                        for (int j = i; j < i + samplesPerSection; j += info.Channels)
                        {
                            // Find the maximum amplitude for each channel in the section
                            for (int c = 0; c < info.Channels; c++)
                                section.Amplitude[c] = Math.Max(section.Amplitude[c], Math.Abs(sampleBuffer[j + c]));
                        }

                        // BASS may provide un-clipped samples, so clip them ourselves
                        for (int c = 0; c < info.Channels; c++)
                            section.Amplitude[c] = Math.Min(1, section.Amplitude[c]);

                        sections.Add(section);
                    }
                }
                
                Bass.ChannelSetPosition(decodeStream, 0);
                //https://github.com/ManagedBass/ManagedBass.DocFX/blob/master/apidoc/Bass/Bass.ChannelSetPosition.md
                length = Bass.ChannelGetLength(decodeStream);
                
                float[] bins = new float[512];
                int currentPoint = 0;
                long currentByte = 0;

                while (length > 0)
                {
                    length = Bass.ChannelGetData(decodeStream, bins, 512);
                    currentByte += length;
                    
                    double lowIntensity = computeIntensity(info, bins, low_min, mid_min);
                    double midIntensity = computeIntensity(info, bins, mid_min, high_min);
                    double highIntensity = computeIntensity(info, bins, high_min, high_max);

                    for (; currentPoint < sections.Count && currentPoint * bytesPerSection < currentByte; currentPoint++)
                    {
                        sections[currentPoint].LowIntensity = lowIntensity;
                        sections[currentPoint].MidIntensity = midIntensity;
                        sections[currentPoint].HighIntensity = highIntensity;
                    }
                }
                
                channels = info.Channels;
            }, cancelSource.Token);
        }
        
        private double computeIntensity(ChannelInfo info, float[] bins, double startFrequency, double endFrequency)
        {
            // 512 fft_bins
            int startBin = (int)(512 * 2 * startFrequency / info.Frequency);
            int endBin = (int)(512 * 2 * endFrequency / info.Frequency);

            startBin = Math.Clamp(startBin, 0, bins.Length);
            endBin = Math.Clamp(endBin, 0, bins.Length);

            double value = 0;
            for (int i = startBin; i < endBin; i++)
                value += bins[i];
            
            return value;
        }
        
        public List<WaveformSection> GetWaveformSections() => GetWaveformSectionAsync().Result;

        public async Task<List<WaveformSection>> GetWaveformSectionAsync()
        {
            if (readStreamTask == null)
                return sections;

            await readStreamTask;
            return sections;
        }
        
        public int GetChannels() => GetChannelsAsync().Result;
        
        public async Task<int> GetChannelsAsync()
        {
            if (readStreamTask == null)
                return channels;

            await readStreamTask;
            return channels;
        }
        
        public async Task<TachyonWaveform> GenerateResampledAsync(int pointCount, CancellationToken cancellationToken = default)
        {
            if (pointCount < 0) throw new ArgumentOutOfRangeException(nameof(pointCount));

            if (pointCount == 0 || readStreamTask == null)
                return new TachyonWaveform(null);

            await readStreamTask;

            return await Task.Run(() =>
            {
                var generatedSection = new List<WaveformSection>();
                float sectionsPerGeneratedSection = (float)sections.Count / pointCount;

                const int kernel_width_factor = 3;
                int kernelWidth = (int)(sectionsPerGeneratedSection * kernel_width_factor) + 1;
                float[] filter = new float[kernelWidth + 1];

                for (int i = 0; i < filter.Length; ++i)
                {
                    filter[i] = (float)Blur.EvalGaussian(i, sectionsPerGeneratedSection);
                }

                for (float i = 0; i < sections.Count; i += sectionsPerGeneratedSection)
                {
                    if (cancellationToken.IsCancellationRequested) break;

                    int startIndex = (int)i - kernelWidth;
                    int endIndex = (int)i + kernelWidth;

                    var section = new WaveformSection(channels);
                    float totalWeight = 0;

                    for (int j = startIndex; j < endIndex; j++)
                    {
                        if (j < 0 || j >= sections.Count) continue;

                        float weight = filter[Math.Abs(j - startIndex - kernelWidth)];
                        totalWeight += weight;

                        for (int c = 0; c < channels; c++)
                            section.Amplitude[c] += weight * sections[j].Amplitude[c];
                        section.LowIntensity += weight * sections[j].LowIntensity;
                        section.MidIntensity += weight * sections[j].MidIntensity;
                        section.HighIntensity += weight * sections[j].HighIntensity;
                    }

                    for (int c = 0; c < channels; c++)
                        section.Amplitude[c] /= totalWeight;
                    
                    section.LowIntensity /= totalWeight;
                    section.MidIntensity /= totalWeight;
                    section.HighIntensity /= totalWeight;

                    generatedSection.Add(section);
                }

                return new TachyonWaveform(null)
                {
                    sections = generatedSection,
                    channels = channels
                };
            }, cancellationToken);
        }
        
        ~TachyonWaveform()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            isDisposed = true;

            cancelSource?.Cancel();
            cancelSource?.Dispose();
            sections = null;

            fileCallbacks?.Dispose();
            fileCallbacks = null;
        }
        
        public class WaveformSection
        {
            public readonly float[] Amplitude;
            
            public double LowIntensity;

            public double MidIntensity;

            public double HighIntensity;

            public WaveformSection(int channels)
            {
                Amplitude = new float[channels];
            }
        }
    }
}
