using System.Threading;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osuTK;
using Tachyon.Game.Graphics;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Game.Scoring;

namespace Tachyon.Game.Screens.Select.Detail
{
    public class HighscoreDetail : VisibilityContainer
    {
        private const int duration = 500;

        private readonly Container scoreContainer;

        public Bindable<ScoreInfo> Score = new Bindable<ScoreInfo>();

        protected override bool StartHidden => true;

        public HighscoreDetail()
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            Margin = new MarginPadding { Vertical = 5 };

            Children = new Drawable[]
            {
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(5),
                    Children = new Drawable[]
                    {
                        new TachyonSpriteText
                        {
                            Padding = new MarginPadding { Left = 50 },
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Text = @"Highscore".ToUpper(),
                            Font = TachyonFont.GetFont(size: 22, weight: FontWeight.Bold),
                        },
                        scoreContainer = new Container
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                        }
                    }
                }
            };

            Score.BindValueChanged(onScoreChanged);
        }

        private CancellationTokenSource loadScoreCancellation;

        private void onScoreChanged(ValueChangedEvent<ScoreInfo> score)
        {
            var newScore = score.NewValue;

            scoreContainer.Clear();
            loadScoreCancellation?.Cancel();

            if (newScore == null)
            {
                State.Value = Visibility.Hidden;
                return;
            }

            State.Value = Visibility.Visible;
            
            LoadComponentAsync(new DrawableScore(newScore), drawableScore =>
            {
                scoreContainer.Child = drawableScore;
                drawableScore.FadeInFromZero(duration, Easing.OutQuint);
            }, (loadScoreCancellation = new CancellationTokenSource()).Token);
        }

        protected override void PopIn() => this.FadeIn(duration, Easing.OutQuint);

        protected override void PopOut() => this.FadeOut(duration, Easing.OutQuint);
    }
}
