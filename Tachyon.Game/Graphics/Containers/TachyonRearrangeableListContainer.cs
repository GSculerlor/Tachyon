using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Tachyon.Game.Graphics.Containers
{
    public abstract class TachyonRearrangeableListContainer<TModel> : RearrangeableListContainer<TModel>
    {
        protected override ScrollContainer<Drawable> CreateScrollContainer() => new TachyonScrollContainer();

        protected override RearrangeableListItem<TModel> CreateDrawable(TModel item) => CreateListItemDrawable(item);
        
        protected abstract TachyonRearrangeableListItem<TModel> CreateListItemDrawable(TModel item);
    }
}