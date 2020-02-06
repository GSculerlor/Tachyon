using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Transforms;

namespace Tachyon.Game.Graphics.Sprites
{
    public class TachyonSpriteText : SpriteText
    {
        public TachyonSpriteText()
        {
            Shadow = false;
            Font = TachyonFont.Default;
        }
    }

    public static class TachyonSpriteTextExtensions
    {
        public static TransformSequence<T> TransformTextTo<T>(this T spriteText, string newText, double duration = 0,
            Easing easing = Easing.None)
            where T : TachyonSpriteText => spriteText.TransformTo(nameof(TachyonSpriteText.Text), newText, duration, easing);
    }
}