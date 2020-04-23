using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Tachyon.Game.Graphics.Containers
{
    public abstract class DimmableContainer : Container
    {
        protected override Container<Drawable> Content => dimContent;

        private Container dimContent { get; }

        protected DimmableContainer()
        {
            AddInternal(dimContent = new Container { RelativeSizeAxes = Axes.Both });
        }
        
        protected override void LoadComplete()
        {
            base.LoadComplete();
            UpdateVisuals();
        }
        
        protected virtual void UpdateVisuals()
        {
            dimContent.FadeColour(TachyonColor.Gray(0.8f), 800, Easing.OutQuint);
        }
    }
}
