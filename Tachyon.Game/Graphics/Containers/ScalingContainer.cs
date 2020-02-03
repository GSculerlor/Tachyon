using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using Tachyon.Game.Configuration;

namespace Tachyon.Game.Graphics.Containers
{
    public class ScalingContainer : Container
    {
        private readonly Container content;
        
        protected override Container<Drawable> Content => content;

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;

        public ScalingContainer()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChild = new AlwaysInputContainer
            {
                RelativeSizeAxes = Axes.Both,
                RelativePositionAxes = Axes.Both,
                CornerRadius = 10,
                Child = content = new ScalingDrawSizePreservingFillContainer(false)
            };
        }

        private class ScalingDrawSizePreservingFillContainer : DrawSizePreservingFillContainer
        {
            private readonly bool applyUIScale;
            private Bindable<float> uiScale;

            public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;

            public ScalingDrawSizePreservingFillContainer(bool applyUIScale)
            {
                this.applyUIScale = applyUIScale;
            }

            [BackgroundDependencyLoader]
            private void load(TachyonConfigManager tachyonConfig)
            {
                if (applyUIScale)
                {
                    uiScale = tachyonConfig.GetBindable<float>(TachyonSetting.UIScale);
                    uiScale.BindValueChanged(scaleChanged, true);
                }
            }

            private void scaleChanged(ValueChangedEvent<float> args)
            {
                this.ScaleTo(new Vector2(args.NewValue), 500, Easing.Out);
                this.ResizeTo(new Vector2(1 / args.NewValue), 500, Easing.Out);
            }
        }

        private class AlwaysInputContainer : Container
        {
            public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => true;

            public AlwaysInputContainer()
            {
                RelativeSizeAxes = Axes.Both;
            }
        }
    }
}