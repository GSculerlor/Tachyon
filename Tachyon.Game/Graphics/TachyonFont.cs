using osu.Framework.Graphics.Sprites;

namespace Tachyon.Game.Graphics
{
    public static class TachyonFont
    {
        public const float DEFAULT_FONT_SIZE = 24;

        public static FontUsage Default => GetFont();

        public static FontUsage Numeric => GetFont(Typeface.Venera);

        public static FontUsage GetFont(Typeface typeface = Typeface.Quicksand, float size = DEFAULT_FONT_SIZE, FontWeight weight = FontWeight.Regular, bool italics = false, bool fixedWidth = false)
            => new FontUsage(GetFamilyString(typeface), size, GetWeightString(weight), italics, fixedWidth);

        public static string GetFamilyString(Typeface typeface)
        {
            switch (typeface)
            {
                case Typeface.Quicksand:
                    return "Quicksand";

                case Typeface.Venera:
                    return "Venera";
            }

            return null;
        }

        public static string GetWeightString(FontWeight weight)
        {
            var weightString = weight.ToString();
            if (weight == FontWeight.Regular)
                weightString = string.Empty;

            return weightString;
        }
    }

    public static class TachyonFontExtensions
    {
        public static FontUsage With(this FontUsage usage, Typeface? typeface = null, float? size = null, FontWeight? weight = null, bool? italics = null, bool? fixedWidth = null)
        {
            string familyString = typeface != null ? TachyonFont.GetFamilyString(typeface.Value) : usage.Family;
            string weightString = weight != null ? TachyonFont.GetWeightString(weight.Value) : usage.Weight;

            return usage.With(familyString, size, weightString, italics, fixedWidth);
        }
    }

    public enum Typeface
    {
        Venera,
        Quicksand
    }

    public enum FontWeight
    {
        Light,
        Regular,
        SemiBold,
        Bold
    }
}