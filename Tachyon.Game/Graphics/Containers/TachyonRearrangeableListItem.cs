using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Tachyon.Game.Graphics.Containers
{
    public abstract class TachyonRearrangeableListItem<TModel> : RearrangeableListItem<TModel>
    {
        public TachyonRearrangeableListItem(TModel item)
            : base(item)
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;
        }
        
        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Child = CreateContent()
            };
        }
        
        protected abstract Drawable CreateContent();
    }
}