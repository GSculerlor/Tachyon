// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Platform;
using osu.Framework;
using Tachyon.Game;

namespace Tachyon.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (GameHost host = Host.GetSuitableHost(@"Tachyon"))
            using (osu.Framework.Game game = new TachyonGame())
                host.Run(game);
        }
    }
}
