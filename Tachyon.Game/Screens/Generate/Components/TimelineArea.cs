using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using Tachyon.Game.Graphics;

namespace Tachyon.Game.Screens.Generate.Components
{
    public class TimelineArea : Container
    {
        private readonly Timeline timeline = new Timeline { RelativeSizeAxes = Axes.Both };

        protected override Container<Drawable> Content => timeline;

        [BackgroundDependencyLoader]
        private void load(TachyonColor colors)
        {
            Masking = true;
            CornerRadius = 5;
            
            InternalChildren = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = colors.Gray1
                }, 
                new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    ColumnDimensions = new []
                    {
                        new Dimension(GridSizeMode.AutoSize),
                        new Dimension(), 
                    },
                    Content = new []
                    {
                        new Drawable[]
                        {
                            new Container
                            {
                                RelativeSizeAxes = Axes.Y,
                                AutoSizeAxes = Axes.X,
                                Children = new Drawable[]
                                {
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Colour = colors.Gray3
                                    }, 
                                    new Container<TimelineButton>
                                    {
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        RelativeSizeAxes = Axes.Y,
                                        AutoSizeAxes = Axes.X,
                                        Masking = true,
                                        Children = new[]
                                        {
                                            new TimelineButton
                                            {
                                                RelativeSizeAxes = Axes.Y,
                                                Height = 0.5f,
                                                Icon = FontAwesome.Solid.SearchPlus,
                                                Action = () => changeZoom(1)
                                            },
                                            new TimelineButton
                                            {
                                                Anchor = Anchor.BottomLeft,
                                                Origin = Anchor.BottomLeft,
                                                RelativeSizeAxes = Axes.Y,
                                                Height = 0.5f,
                                                Icon = FontAwesome.Solid.SearchMinus,
                                                Action = () => changeZoom(-1)
                                            },
                                        }
                                    } 
                                }
                            }, 
                            timeline
                        }, 
                    }
                }, 
            };
        }
        
        private void changeZoom(float change) => timeline.Zoom += change;
    }
}