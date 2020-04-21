using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Rulesets.Objects;

namespace Tachyon.Game.Rulesets.UI.Components
{
    /// <summary>
    /// A component that is displayed at the hit position in the playfield.
    /// </summary>
    public class HitTarget : Container
    {
        /// <summary>
        /// Thickness of all drawn line pieces.
        /// </summary>
        private const float border_thickness = 2.5f;

        public HitTarget()
        {
            Children = new Drawable[]
            {
                new CircularContainer
                {
                    Name = "Normal Hit Ring",
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    FillMode = FillMode.Fit,
                    Scale = new Vector2(TachyonHitObject.DEFAULT_SIZE),
                    Masking = true,
                    BorderColour = Color4.White,
                    BorderThickness = border_thickness,
                    Alpha = 0.5f,
                    Children = new[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Alpha = 0,
                            AlwaysPresent = true
                        }
                    }
                }
            };
        }
    }
}
