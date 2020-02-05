using System;
using osuTK.Graphics;

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

        public readonly Color4 YellowLighter = FromHex(@"ffffdd");
        public readonly Color4 YellowLight = FromHex(@"ffdd55");
        public readonly Color4 Yellow = FromHex(@"ffcc22");
        public readonly Color4 YellowDark = FromHex(@"eeaa00");
        public readonly Color4 YellowDarker = FromHex(@"cc6600");

        public readonly Color4 BackButtonGray = FromHex(@"313131");
    }
}