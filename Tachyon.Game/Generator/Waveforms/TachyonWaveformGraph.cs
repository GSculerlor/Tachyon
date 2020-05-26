using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Batches;
using osu.Framework.Graphics.OpenGL;
using osu.Framework.Graphics.OpenGL.Vertices;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shaders;
using osu.Framework.Graphics.Textures;
using osu.Framework.Layout;
using osu.Framework.Threading;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;

namespace Tachyon.Game.Generator.Waveforms
{
    public class TachyonWaveformGraph : Drawable
    {
        private IShader shader;
        private readonly Texture texture;

        public TachyonWaveformGraph()
        {
            texture = Texture.WhitePixel;
        }

        [BackgroundDependencyLoader]
        private void load(ShaderManager shaders)
        {
            shader = shaders.Load(VertexShaderDescriptor.TEXTURE_2, FragmentShaderDescriptor.TEXTURE_ROUNDED);
        }

        private float resolution = 1;

        public float Resolution
        {
            get => resolution;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                if (resolution == value)
                    return;

                resolution = value;
                generate();
            }
        }

        private TachyonWaveform waveform;

        public TachyonWaveform Waveform
        {
            get => waveform;
            set
            {
                if (waveform == value)
                    return;

                waveform = value;
                generate();
            }
        }

        private Color4? lowColor;
        public Color4? LowColor
        {
            get => lowColor;
            set
            {
                if (lowColor == value)
                    return;

                lowColor = value;

                Invalidate(Invalidation.DrawNode);
            }
        }

        private Color4? midColor;
        public Color4? MidColor
        {
            get => midColor;
            set
            {
                if (midColor == value)
                    return;

                midColor = value;

                Invalidate(Invalidation.DrawNode);
            }
        }

        private Color4? highColor;
        public Color4? HighColor
        {
            get => highColor;
            set
            {
                if (highColor == value)
                    return;

                highColor = value;

                Invalidate(Invalidation.DrawNode);
            }
        }

        protected override bool OnInvalidate(Invalidation invalidation, InvalidationSource source)
        {
            var result = base.OnInvalidate(invalidation, source);

            if ((invalidation & Invalidation.RequiredParentSizeToFit) > 0)
            {
                generate();
                result = true;
            }

            return result;
        }

        private CancellationTokenSource cancelSource = new CancellationTokenSource();
        private ScheduledDelegate scheduledGenerate;

        protected TachyonWaveform ResampledWaveform { get; private set; }

        private void generate()
        {
            scheduledGenerate?.Cancel();
            cancelGeneration();

            if (Waveform == null)
                return;

            scheduledGenerate = Schedule(() =>
            {
                cancelSource = new CancellationTokenSource();
                var token = cancelSource.Token;

                Waveform.GenerateResampledAsync((int)Math.Max(0, Math.Ceiling(DrawWidth * Scale.X) * Resolution), token).ContinueWith(w =>
                {
                    ResampledWaveform = w.Result;
                    Schedule(() => Invalidate(Invalidation.DrawNode));
                }, token);
            });
        }

        private void cancelGeneration()
        {
            cancelSource?.Cancel();
            cancelSource?.Dispose();
            cancelSource = null;
        }

        protected override DrawNode CreateDrawNode() => new TachyonWaveformDrawNode(this);

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            cancelGeneration();
        }

        private class TachyonWaveformDrawNode : DrawNode
        {
            private IShader shader;
            private Texture texture;

            private IReadOnlyList<TachyonWaveform.WaveformSection> sections;

            private Vector2 drawSize;
            private int channels;

            private Color4 lowColor;
            private Color4 midColor;
            private Color4 highColor;

            private double highMax;
            private double midMax;
            private double lowMax;

            protected new TachyonWaveformGraph Source => (TachyonWaveformGraph)base.Source;

            public TachyonWaveformDrawNode(TachyonWaveformGraph source)
                : base(source)
            {
            }

            public override void ApplyState()
            {
                base.ApplyState();

                shader = Source.shader;
                texture = Source.texture;
                drawSize = Source.DrawSize;
                sections = Source.ResampledWaveform?.GetWaveformSections();
                channels = Source.ResampledWaveform?.GetChannels() ?? 0;
                lowColor = Source.lowColor ?? DrawColourInfo.Colour;
                midColor = Source.midColor ?? DrawColourInfo.Colour;
                highColor = Source.highColor ?? DrawColourInfo.Colour;

                if (sections?.Any() == true)
                {
                    highMax = sections.Max(p => p.HighIntensity);
                    midMax = sections.Max(p => p.MidIntensity);
                    lowMax = sections.Max(p => p.LowIntensity);
                }
            }

            private readonly QuadBatch<TexturedVertex2D> vertexBatch = new QuadBatch<TexturedVertex2D>(1000, 10);

            public override void Draw(Action<TexturedVertex2D> vertexAction)
            {
                base.Draw(vertexAction);

                if (texture?.Available != true || sections == null || sections.Count == 0)
                    return;

                shader.Bind();
                texture.TextureGL.Bind();

                Vector2 localInflationAmount = new Vector2(0, 1) * DrawInfo.MatrixInverse.ExtractScale().Xy;

                RectangleF localMaskingRectangle = (Quad.FromRectangle(GLWrapper.CurrentMaskingInfo.ScreenSpaceAABB) * DrawInfo.MatrixInverse).AABBFloat;

                float separation = drawSize.X / (sections.Count - 1);

                for (int i = 0; i < sections.Count - 1; i++)
                {
                    float leftX = i * separation;
                    float rightX = (i + 1) * separation;

                    if (rightX < localMaskingRectangle.Left)
                        continue;

                    if (leftX > localMaskingRectangle.Right)
                        break; // X is always increasing

                    Color4 color = DrawColourInfo.Colour;

                    // coloring is applied in the order of interest to a viewer.
                    color = Interpolation.ValueAt(sections[i].MidIntensity / midMax, color, midColor, 0, 1);
                    // high end (cymbal) can help find beat, so give it priority over mids.
                    color = Interpolation.ValueAt(sections[i].HighIntensity / highMax, color, highColor, 0, 1);
                    // low end (bass drum) is generally the best visual aid for beat matching, so give it priority over high/mid.
                    color = Interpolation.ValueAt(sections[i].LowIntensity / lowMax, color, lowColor, 0, 1);

                    Quad quadToDraw;

                    switch (channels)
                    {
                        default:
                        case 2:
                        {
                            float height = drawSize.Y / 2;
                            quadToDraw = new Quad(
                                new Vector2(leftX, height - sections[i].Amplitude[0] * height),
                                new Vector2(rightX, height - sections[i + 1].Amplitude[0] * height),
                                new Vector2(leftX, height + sections[i].Amplitude[1] * height),
                                new Vector2(rightX, height + sections[i + 1].Amplitude[1] * height)
                            );
                            break;
                        }

                        case 1:
                        {
                            quadToDraw = new Quad(
                                new Vector2(leftX, drawSize.Y - sections[i].Amplitude[0] * drawSize.Y),
                                new Vector2(rightX, drawSize.Y - sections[i + 1].Amplitude[0] * drawSize.Y),
                                new Vector2(leftX, drawSize.Y),
                                new Vector2(rightX, drawSize.Y)
                            );
                            break;
                        }
                    }

                    quadToDraw *= DrawInfo.Matrix;
                    DrawQuad(texture, quadToDraw, color, null, vertexBatch.AddAction, Vector2.Divide(localInflationAmount, quadToDraw.Size));
                }

                shader.Unbind();
            }

            protected override void Dispose(bool isDisposing)
            {
                base.Dispose(isDisposing);

                vertexBatch.Dispose();
            }
        }
    }
}
