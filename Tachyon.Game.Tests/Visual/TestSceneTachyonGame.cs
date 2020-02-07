// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Platform;
using Tachyon.Game.Testing.Visual;

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
