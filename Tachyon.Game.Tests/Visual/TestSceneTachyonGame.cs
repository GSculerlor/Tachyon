using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Platform;

namespace Tachyon.Game.Tests.Visual
{
    public class TestSceneTachyonGame : TachyonTestScene
    {
        private TachyonGame game;

        public override IReadOnlyList<Type> RequiredTypes => new[]
        {
            typeof(TachyonGame),
        };

        [BackgroundDependencyLoader]
        private void load(GameHost host)
        {
            game = new TachyonGame();
            game.SetHost(host);

            Add(game);
        }

        // Add visual tests to ensure correct behaviour of your game: https://github.com/ppy/osu-framework/wiki/Development-and-Testing
    }
}
