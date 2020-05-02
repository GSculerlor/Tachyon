using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using Tachyon.Game.Graphics.Sprites;

namespace Tachyon.Game.Graphics.UserInterface
{
    public abstract class StatisticDisplay : CompositeDrawable
    {
        protected SpriteText HeaderText { get; private set; }

        private readonly string header;
        private Drawable content;

        /// <summary>
        /// Creates a new <see cref="StatisticDisplay"/>.
        /// </summary>
        /// <param name="header">The name of the statistic.</param>
        protected StatisticDisplay(string header)
        {
            this.header = header;
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Children = new[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 24,
                        Masking = true,
                        CornerRadius = 5,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = Color4Extensions.FromHex("#222")
                            },
                            HeaderText = new TachyonSpriteText
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Font = TachyonFont.Default.With(size: 20, weight: FontWeight.SemiBold),
                                Text = header.ToUpperInvariant(),
                            }
                        }
                    },
                    new Container
                    {
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        AutoSizeAxes = Axes.Both,
                        Children = new[]
                        {
                            content = CreateContent().With(d =>
                            {
                                d.Anchor = Anchor.TopCentre;
                                d.Origin = Anchor.TopCentre;
                                d.Alpha = 0;
                                d.AlwaysPresent = true;
                            }),
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Shows the statistic value.
        /// </summary>
        public virtual void Appear() => content.FadeIn(100);

        /// <summary>
        /// Creates the content for this <see cref="StatisticDisplay"/>.
        /// </summary>
        protected abstract Drawable CreateContent();
    }
}
