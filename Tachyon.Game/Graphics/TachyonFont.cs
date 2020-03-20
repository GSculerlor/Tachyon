﻿using osu.Framework.Graphics.Sprites;

namespace Tachyon.Game.Graphics
{
    public static class TachyonFont
    {
        /// <summary>
        /// The default font size.
        /// </summary>
        public const float DEFAULT_FONT_SIZE = 24;

        /// <summary>
        /// The default font.
        /// </summary>
        public static FontUsage Default => GetFont();

        public static FontUsage Numeric => GetFont(Typeface.Venera);

        /// <summary>
        /// Retrieves a <see cref="FontUsage"/>.
        /// </summary>
        /// <param name="typeface">The font typeface.</param>
        /// <param name="size">The size of the text in local space. For a value of 16, a single line will have a height of 16px.</param>
        /// <param name="weight">The font weight.</param>
        /// <param name="italics">Whether the font is italic.</param>
        /// <param name="fixedWidth">Whether all characters should be spaced the same distance apart.</param>
        /// <returns>The <see cref="FontUsage"/>.</returns>
        public static FontUsage GetFont(Typeface typeface = Typeface.Quicksand, float size = DEFAULT_FONT_SIZE, FontWeight weight = FontWeight.Regular, bool italics = false, bool fixedWidth = false)
            => new FontUsage(GetFamilyString(typeface), size, GetWeightString(weight), italics, fixedWidth);

        /// <summary>
        /// Retrieves the string representation of a <see cref="Typeface"/>.
        /// </summary>
        /// <param name="typeface">The <see cref="Typeface"/>.</param>
        /// <returns>The string representation.</returns>
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

        /// <summary>
        /// Retrieves the string representation of a <see cref="FontWeight"/>.
        /// </summary>
        /// <param name="weight">The <see cref="FontWeight"/>.</param>
        /// <returns>The string representation of <paramref name="weight"/>.</returns>
        public static string GetWeightString(FontWeight weight)
            => weight.ToString();
    }

    public static class TachyonFontExtensions
    {
        /// <summary>
        /// Creates a new <see cref="FontUsage"/> by applying adjustments to this <see cref="FontUsage"/>.
        /// </summary>
        /// <param name="usage">The base <see cref="FontUsage"/>.</param>
        /// <param name="typeface">The font typeface. If null, the value is copied from this <see cref="FontUsage"/>.</param>
        /// <param name="size">The text size. If null, the value is copied from this <see cref="FontUsage"/>.</param>
        /// <param name="weight">The font weight. If null, the value is copied from this <see cref="FontUsage"/>.</param>
        /// <param name="italics">Whether the font is italic. If null, the value is copied from this <see cref="FontUsage"/>.</param>
        /// <param name="fixedWidth">Whether all characters should be spaced apart the same distance. If null, the value is copied from this <see cref="FontUsage"/>.</param>
        /// <returns>The resulting <see cref="FontUsage"/>.</returns>
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