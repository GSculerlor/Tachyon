using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;
using Tachyon.Game.Graphics.Containers;

namespace Tachyon.Game.Overlays.Settings
{
    public abstract class SettingsPanel : TachyonFocusedOverlayContainer
    {
        public const float CONTENT_MARGINS = 15;

        public const float TRANSITION_LENGTH = 600;

        protected const float WIDTH = 400;

        protected Container<Drawable> ContentContainer;

        protected override Container<Drawable> Content => ContentContainer;

        protected SettingsSectionsContainer SectionsContainer;

        protected Box Background;

        protected SettingsPanel()
        {
            RelativeSizeAxes = Axes.Y;
            AutoSizeAxes = Axes.X;
        }

        protected virtual IEnumerable<SettingsSection> CreateSections() => null;

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = ContentContainer = new Container
            {
                Width = WIDTH,
                RelativeSizeAxes = Axes.Y,
                Children = new Drawable[]
                {
                    Background = new Box
                    {
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4.Black,
                        Alpha = 0.6f,
                    },
                    SectionsContainer = new SettingsSectionsContainer
                    {
                        Masking = true,
                        RelativeSizeAxes = Axes.Both,
                        ExpandableHeader = CreateHeader(),
                    },
                }
            };
            
            CreateSections()?.ForEach(AddSection);
        }

        protected void AddSection(SettingsSection section)
        {
            SectionsContainer.Add(section);
        }

        protected virtual Drawable CreateHeader() => new Container();

        protected override void PopIn()
        {
            base.PopIn();

            ContentContainer.MoveToX(0, TRANSITION_LENGTH, Easing.OutQuint);

            this.FadeTo(1, TRANSITION_LENGTH, Easing.OutQuint);
        }

        protected override void PopOut()
        {
            base.PopOut();

            ContentContainer.MoveToX(-WIDTH, TRANSITION_LENGTH, Easing.OutQuint);

            this.FadeTo(0, TRANSITION_LENGTH, Easing.OutQuint);
        }

        public override bool AcceptsFocus => true;

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();
            Padding = new MarginPadding { Top = 50 };
        }

        protected class SettingsSectionsContainer : SectionsContainer<SettingsSection>
        {
            public SearchContainer<SettingsSection> SearchContainer;

            protected override FlowContainer<SettingsSection> CreateScrollContentContainer()
                => SearchContainer = new SearchContainer<SettingsSection>
                {
                    AutoSizeAxes = Axes.Y,
                    RelativeSizeAxes = Axes.X,
                };

            public SettingsSectionsContainer()
            {
                HeaderBackground = new Box
                {
                    Colour = Color4.Black,
                    RelativeSizeAxes = Axes.Both
                };
            }

            protected override void UpdateAfterChildren()
            {
                base.UpdateAfterChildren();

                // no null check because the usage of this class is strict
                HeaderBackground.Alpha = -ExpandableHeader.Y / ExpandableHeader.LayoutSize.Y * 0.5f;
            }
        }
    }
}
