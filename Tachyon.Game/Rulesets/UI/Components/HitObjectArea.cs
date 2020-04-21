using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using Tachyon.Game.Rulesets.UI.Scrolling;

namespace Tachyon.Game.Rulesets.UI.Components
{
    public class HitObjectArea  : CompositeDrawable
    {
        public HitObjectArea(HitObjectContainer hitObjectContainer)
        {
            Padding = new MarginPadding { Left = 100 };
            
            InternalChildren = new[]
            {
                hitObjectContainer,
            };
        }
    }
}
