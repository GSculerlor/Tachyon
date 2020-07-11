using System.Collections.Generic;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Presentation.Utils;

namespace Tachyon.Presentation.Graphics
{
    public class ItemDrawable : CompositeDrawable
    {
        private const int cover_width = 100;
        private const int corner_radius = 6;

        private readonly KeyValuePair<string, string> itemPair;
        private readonly IconUsage? icon;

        public ItemDrawable(KeyValuePair<string, string> itemPair, IconUsage? icon)
        {
            this.itemPair = itemPair;
            this.icon = icon;
            
            RelativeSizeAxes = Axes.X;
            Height = 80;
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;

            Masking = true;
            CornerRadius = corner_radius;
        }

        [BackgroundDependencyLoader]
        private void load(ColorUtils colorUtils)
        {
            AddRangeInternal(new Drawable[]
            {
                new Container
                {
                    RelativeSizeAxes = Axes.Y,
                    Width = cover_width,
                    Child = new SpriteIcon
                    {
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        Size = new Vector2(24),
                        Icon = icon ?? FontAwesome.Solid.DotCircle,
                        FillMode = FillMode.Fill,
                    },
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Left = cover_width - corner_radius },
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Masking = true,
                            CornerRadius = corner_radius,
                            Children = new Drawable[]
                            {
                                new ItemContentContainer
                                {
                                    Child = new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Padding = new MarginPadding(10),
                                        Children = new Drawable[]
                                        {
                                            new FillFlowContainer
                                            {
                                                Anchor = Anchor.CentreLeft,
                                                Origin = Anchor.CentreLeft,
                                                RelativeSizeAxes = Axes.Both,
                                                Direction = FillDirection.Vertical,
                                                Children = new Drawable[]
                                                {
                                                    new ItemTextContainer(itemPair.Key),
                                                    new TachyonTextFlowContainer
                                                    {
                                                        RelativeSizeAxes = Axes.X,
                                                        AutoSizeAxes = Axes.Y,
                                                    }.With(d =>
                                                    {
                                                        d.AddText(itemPair.Value, t => { t.Font = TachyonFont.Default.With(size: 20, weight: FontWeight.SemiBold); });
                                                    }),
                                                }
                                            },
                                        }
                                    },
                                }
                            }
                        }
                    }
                }
            });
        }

        private class ItemContentContainer : Container
        {
            protected override Container<Drawable> Content => content;

            private readonly Box background;
            private readonly Container content;

            private Color4 idleColor;

            private Color4 IdleColor
            {
                set
                {
                    idleColor = value;
                    background.Colour = idleColor;
                }
            }

            public ItemContentContainer()
            {
                RelativeSizeAxes = Axes.Both;
                Masking = true;
                CornerRadius = 6;

                AddRangeInternal(new Drawable[]
                {
                    background = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                    },
                    content = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                    }
                });
            }
            
            [BackgroundDependencyLoader]
            private void load(ColorUtils colorUtils)
            {
                IdleColor = colorUtils.Background4;
            }
        }

        private class ItemTextContainer : Container
        {
            private readonly string content;
            
            public ItemTextContainer(string content)
            {
                this.content = content;
                
                AutoSizeAxes = Axes.Both;
            }
            
            [BackgroundDependencyLoader(true)]
            private void load()
            {
                Child = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new TachyonSpriteText
                        {
                            Text = content,
                            Font = TachyonFont.Default.With(weight: FontWeight.Bold)
                        },
                    }
                };
            }
        }
    }
}
