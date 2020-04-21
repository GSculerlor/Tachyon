using osu.Framework.Allocation;

namespace Tachyon.Game.Rulesets.UI.Scrolling
{
    public class ScrollingPlayfield : Playfield
    {
        protected override HitObjectContainer CreateHitObjectContainer() => new ScrollingHitObjectContainer();
    }
}
