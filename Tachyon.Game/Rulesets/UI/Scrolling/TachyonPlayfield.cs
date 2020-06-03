using osu.Framework.Graphics;
using Tachyon.Game.Rulesets.Audio;
using Tachyon.Game.Rulesets.Objects;
using Tachyon.Game.Rulesets.Objects.Drawables;

namespace Tachyon.Game.Rulesets.UI.Scrolling
{
    public class TachyonPlayfield : ScrollingPlayfield
    {
        private readonly DualRow rows;

        public TachyonPlayfield()
        {
            AddRangeInternal(new Drawable[]
            {
                rows = new DualRow(0),
                new HitSound(), 
            });
        }

        public override void Add(DrawableHitObject h) => rows.Add(h);

        public override bool Remove(DrawableHitObject h) => rows.Remove(h);
    }
}
