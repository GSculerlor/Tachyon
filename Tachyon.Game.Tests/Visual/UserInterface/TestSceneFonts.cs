using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Containers;

namespace Tachyon.Game.Tests.Visual.UserInterface
{
    [TestFixture]
    public class TestSceneFonts : TachyonGridTestScene
    {
        public TestSceneFonts() : base(4, 2)
        {
            Cell(0).Add(new TextTestContainer(FontWeight.Light, false));
            Cell(1).Add(new TextTestContainer(FontWeight.Light, true));
            
            Cell(2).Add(new TextTestContainer(FontWeight.Regular, false));
            Cell(3).Add(new TextTestContainer(FontWeight.Regular, true));
            
            Cell(4).Add(new TextTestContainer(FontWeight.SemiBold, false));
            Cell(5).Add(new TextTestContainer(FontWeight.SemiBold, true));
            
            Cell(6).Add(new TextTestContainer(FontWeight.Bold, false));
            Cell(7).Add(new TextTestContainer(FontWeight.Bold, true));
        }

        private class TextTestContainer : FillFlowContainer
        {
            public TextTestContainer(FontWeight fontWeight, bool shouldItalic, Typeface typeface = Typeface.Quicksand)
            {
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;
                RelativeSizeAxes = Axes.X;
                AutoSizeAxes = Axes.Y;

                Add(new TachyonTextFlowContainer(text =>
                {
                    text.Font = TachyonFont.GetFont(typeface, weight: fontWeight, italics: shouldItalic);
                })
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = "The quick brown fox jumps over the lazy dog. 1234567890.\n" +
                           "GSculerlor/Tachyon\n" +
                           "ここにいつまでもいるつもり"
                });
            }
        }
    }
}