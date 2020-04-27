using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osuTK;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Game.Graphics.UserInterface;
using Tachyon.Game.Scoring;

namespace Tachyon.Game.Screens.Result
{
    public class ResultPanel : CompositeDrawable
    {
        private readonly ScoreInfo score;
        
        private Container middleLayerContainer;
        private Drawable middleLayerBackground;
        private Container middleLayerContentContainer;
        private Drawable middleLayerContent;

        public ResultPanel(ScoreInfo score)
        {
            this.score = score;
            Size = new Vector2(600, 300);
        }

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            InternalChildren = new Drawable[]
            {
                middleLayerContainer = new Container
                {
                    Name = "Middle layer",
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            CornerRadius = 20,
                            CornerExponent = 2.5f,
                            Masking = true,
                            Child = middleLayerBackground = new Box {
                                RelativeSizeAxes = Axes.Both,
                                Colour = ColourInfo.GradientVertical(Color4Extensions.FromHex("#555"), Color4Extensions.FromHex("#333"))
                            }
                        },
                        middleLayerContentContainer = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Child = middleLayerContent = new ResultDetailPanel(score)
                        }
                    }
                }
            };
        }
    }
}
