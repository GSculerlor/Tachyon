﻿using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;

namespace Tachyon.Game.Overlays.Settings
{
   public abstract class SettingsSection : Container, IHasFilterableChildren
    {
        protected FillFlowContainer FlowContent;
        protected override Container<Drawable> Content => FlowContent;
        public abstract string Header { get; }

        public IEnumerable<IFilterable> FilterableChildren => Children.OfType<IFilterable>();
        public virtual IEnumerable<string> FilterTerms => new[] { Header };

        private const int header_size = 22;
        private const int header_margin = 25;

        public bool MatchingFilter
        {
            set => this.FadeTo(value ? 1 : 0);
        }

        public bool FilteringActive { get; set; }

        protected SettingsSection()
        {
            Margin = new MarginPadding { Top = 20 };
            AutoSizeAxes = Axes.Y;
            RelativeSizeAxes = Axes.X;

            FlowContent = new FillFlowContainer
            {
                Margin = new MarginPadding
                {
                    Top = header_size + header_margin
                },
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, 30),
                AutoSizeAxes = Axes.Y,
                RelativeSizeAxes = Axes.X,
            };
        }

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            AddRangeInternal(new Drawable[]
            {
                new Container
                {
                    Padding = new MarginPadding
                    {
                        Top = 20,
                        Bottom = 10,
                    },
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Children = new Drawable[]
                    {
                        new TachyonSpriteText
                        {
                            Font = TachyonFont.GetFont(size: header_size, weight: FontWeight.SemiBold),
                            Text = Header,
                            Colour = colors.Yellow,
                            Margin = new MarginPadding { Left = SettingsPanel.CONTENT_MARGINS, Right = SettingsPanel.CONTENT_MARGINS }
                        },
                        FlowContent
                    }
                },
            });
        }
    }
}
