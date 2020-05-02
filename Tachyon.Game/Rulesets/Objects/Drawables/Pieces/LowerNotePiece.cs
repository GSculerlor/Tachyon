using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using Tachyon.Game.Graphics;

namespace Tachyon.Game.Rulesets.Objects.Drawables.Pieces
{
    public class LowerNotePiece : CirclePiece
    {
        public LowerNotePiece()
        {
            Add(new CentreHitSymbolPiece());
        }
        
        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            AccentColor = colors.LowerHitObject;
        }
        
        /// <summary>
        /// The symbol used for centre hit pieces.
        /// </summary>
        public class CentreHitSymbolPiece : Container
        {
            public CentreHitSymbolPiece()
            {
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;

                RelativeSizeAxes = Axes.Both;
                Size = new Vector2(SYMBOL_SIZE);
                Padding = new MarginPadding(SYMBOL_BORDER);
            }
        }
    }
}
