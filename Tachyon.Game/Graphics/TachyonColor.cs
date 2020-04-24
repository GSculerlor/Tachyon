using System;
using osuTK.Graphics;
using Tachyon.Game.Rulesets.Scoring;

namespace Tachyon.Game.Graphics
{
    public class TachyonColor
    {
        public static Color4 Gray(float amt) => new Color4(amt, amt, amt, 1f);
        public static Color4 Gray(byte amt) => new Color4(amt, amt, amt, 255);

        public static Color4 FromHex(string hex)
        {
            if (hex[0] == '#')
                hex = hex.Substring(1);

            switch (hex.Length)
            {
                default:
                    throw new ArgumentException(@"Invalid hex string length!");

                case 3:
                    return new Color4(
                        (byte) (Convert.ToByte(hex.Substring(0, 1), 16) * 17),
                        (byte) (Convert.ToByte(hex.Substring(1, 1), 16) * 17),
                        (byte) (Convert.ToByte(hex.Substring(2, 1), 16) * 17),
                        255);

                case 6:
                    return new Color4(
                        Convert.ToByte(hex.Substring(0, 2), 16),
                        Convert.ToByte(hex.Substring(2, 2), 16),
                        Convert.ToByte(hex.Substring(4, 2), 16),
                        255);

                case 4:
                    return new Color4(
                        (byte) (Convert.ToByte(hex.Substring(0, 1), 16) * 17),
                        (byte) (Convert.ToByte(hex.Substring(1, 1), 16) * 17),
                        (byte) (Convert.ToByte(hex.Substring(2, 1), 16) * 17),
                        (byte) (Convert.ToByte(hex.Substring(3, 1), 16) * 17));

                case 8:
                    return new Color4(
                        Convert.ToByte(hex.Substring(0, 2), 16),
                        Convert.ToByte(hex.Substring(2, 2), 16),
                        Convert.ToByte(hex.Substring(4, 2), 16),
                        Convert.ToByte(hex.Substring(6, 2), 16));
            }
        }

        public readonly Color4 Gray0 = FromHex(@"000");
        public readonly Color4 Gray1 = FromHex(@"111");
        public readonly Color4 Gray2 = FromHex(@"222");
        public readonly Color4 Gray3 = FromHex(@"333");
        public readonly Color4 Gray4 = FromHex(@"444");
        public readonly Color4 Gray5 = FromHex(@"555");
        public readonly Color4 Gray6 = FromHex(@"666");
        public readonly Color4 Gray7 = FromHex(@"777");
        public readonly Color4 Gray8 = FromHex(@"888");
        public readonly Color4 Gray9 = FromHex(@"999");
        public readonly Color4 GrayA = FromHex(@"aaa");
        public readonly Color4 GrayB = FromHex(@"bbb");
        public readonly Color4 GrayC = FromHex(@"ccc");
        public readonly Color4 GrayD = FromHex(@"ddd");
        public readonly Color4 GrayE = FromHex(@"eee");
        public readonly Color4 GrayF = FromHex(@"fff");
        
        public readonly Color4 YellowLighter = FromHex(@"ffffdd");
        public readonly Color4 YellowLight = FromHex(@"ffdd55");
        public readonly Color4 Yellow = FromHex(@"ffcc22");
        public readonly Color4 YellowDark = FromHex(@"eeaa00");
        public readonly Color4 YellowDarker = FromHex(@"cc6600");
        
        public readonly Color4 PinkLighter = FromHex(@"ffddee");
        public readonly Color4 PinkLight = FromHex(@"ff99cc");
        public readonly Color4 Pink = FromHex(@"ff66aa");
        public readonly Color4 PinkDark = FromHex(@"cc5288");
        public readonly Color4 PinkDarker = FromHex(@"bb1177");
        
        public readonly Color4 BlueLighter = FromHex(@"ddffff");
        public readonly Color4 BlueLight = FromHex(@"99eeff");
        public readonly Color4 Blue = FromHex(@"66ccff");
        public readonly Color4 BlueDark = FromHex(@"44aadd");
        public readonly Color4 BlueDarker = FromHex(@"2299bb");
        
        public readonly Color4 RedLighter = FromHex(@"ffeded");
        public readonly Color4 RedLight = FromHex(@"ed7787");
        public readonly Color4 Red = FromHex(@"ed1121");
        public readonly Color4 RedDark = FromHex(@"ba0011");
        public readonly Color4 RedDarker = FromHex(@"870000");
        
        public readonly Color4 RedSomething = FromHex(@"E9291A");

        public readonly Color4 BackButtonGray = FromHex(@"313131");
        
        /// <summary>
        /// Retrieves the colour for a <see cref="HitResult"/>.
        /// </summary>
        public Color4 ForHitResult(HitResult judgement)
        {
            switch (judgement)
            {
                case HitResult.Perfect:
                    return Blue;
                
                case HitResult.Good:
                    return Yellow;

                case HitResult.Miss:
                    return Red;

                default:
                    return Color4.White;
            }
        }
    }
}