using System;
using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using Tachyon.Game.Overlays.Settings;

namespace Tachyon.Game.Tests.Visual.Settings
{
    [TestFixture]
    public class TestSceneSettingsPanel : TachyonTestScene
    {
        private readonly SettingsPanel settings;

        public override IReadOnlyList<Type> RequiredTypes => new[]
        {
            typeof(SettingsOverlay),
            typeof(SettingsSection),
            typeof(SettingsSubsection)
        };

        public TestSceneSettingsPanel()
        {
            settings = new SettingsOverlay
            {
                State = { Value = Visibility.Visible },
            };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Add(settings);
        }
    }
}
