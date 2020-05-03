using System;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Colour;
using osuTK.Graphics;
using Tachyon.Game.Graphics;

namespace Tachyon.Game.Screens.Generate
{
    public class BindableBeatDivisor : BindableInt
    {
        public static readonly int[] VALID_DIVISORS = { 1, 2, 3, 4, 6, 8, 12, 16 };

        public BindableBeatDivisor(int value = 1)
            : base(value)
        {
        }

        public void Next() => Value = VALID_DIVISORS[Math.Min(VALID_DIVISORS.Length - 1, Array.IndexOf(VALID_DIVISORS, Value) + 1)];

        public void Previous() => Value = VALID_DIVISORS[Math.Max(0, Array.IndexOf(VALID_DIVISORS, Value) - 1)];

        public override int Value
        {
            get => base.Value;
            set
            {
                if (!VALID_DIVISORS.Contains(value))
                {
                    // If it doesn't match, value will be 0, but will be clamped to the valid range via DefaultMinValue
                    value = Array.FindLast(VALID_DIVISORS, d => d < value);
                }

                base.Value = value;
            }
        }

        protected override int DefaultMinValue => VALID_DIVISORS.First();
        protected override int DefaultMaxValue => VALID_DIVISORS.Last();
        protected override int DefaultPrecision => 1;

        /// <summary>
        /// Retrieves the appropriate colour for a beat divisor.
        /// </summary>
        /// <param name="beatDivisor">The beat divisor.</param>
        /// <param name="colors">The set of colours.</param>
        /// <returns>The applicable colour from <paramref name="colors"/> for <paramref name="beatDivisor"/>.</returns>
        public static ColourInfo GetColourFor(int beatDivisor, TachyonColor colors)
        {
            switch (beatDivisor)
            {
                case 1:
                    return Color4.White;

                case 2:
                    return colors.Red;

                case 4:
                    return colors.Blue;

                case 8:
                    return colors.Yellow;

                case 16:
                    return colors.PurpleDark;

                case 3:
                    return colors.Purple;

                case 6:
                    return colors.YellowDark;

                case 12:
                    return colors.YellowDarker;

                default:
                    return Color4.Red;
            }
        }

        /// <summary>
        /// Retrieves the applicable divisor for a specific beat index.
        /// </summary>
        /// <param name="index">The 0-based beat index.</param>
        /// <param name="beatDivisor">The beat divisor.</param>
        /// <returns>The applicable divisor.</returns>
        public static int GetDivisorForBeatIndex(int index, int beatDivisor)
        {
            int beat = index % beatDivisor;

            foreach (var divisor in BindableBeatDivisor.VALID_DIVISORS)
            {
                if ((beat * divisor) % beatDivisor == 0)
                    return divisor;
            }

            return 0;
        }
    }
}