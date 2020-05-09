using NUnit.Framework;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using Tachyon.Game.Configuration;

namespace Tachyon.Game.Tests.Visual.Settings
{
    [TestFixture]
    public class TestSceneSettingsSource : TachyonTestScene
    {
        public TestSceneSettingsSource()
        {
            Children = new Drawable[]
            {
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(20),
                    Width = 0.5f,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Padding = new MarginPadding(50),
                    ChildrenEnumerable = new TestTargetClass().CreateSettingsControls()
                },
            };
        }

        private class TestTargetClass
        {
            [SettingSource("Sample bool", "Clicking this changes a setting")]
            public BindableBool TickBindable { get; } = new BindableBool();

            [SettingSource("Sample enum", "Change something for a mod")]
            public Bindable<TestEnum> EnumBindable { get; } = new Bindable<TestEnum>
            {
                Default = TestEnum.Value1,
                Value = TestEnum.Value2
            };
            
            [SettingSource("Sample float", "Change something for a mod")]
            public BindableFloat SliderBindable { get; } = new BindableFloat
            {
                MinValue = 0,
                MaxValue = 10,
                Default = 5,
                Value = 7
            };
        }

        private enum TestEnum
        {
            Value1,
            Value2
        }
    }
}
