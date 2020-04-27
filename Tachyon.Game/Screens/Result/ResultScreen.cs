using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using Tachyon.Game.Scoring;
using Tachyon.Game.Screens.Backgrounds;

namespace Tachyon.Game.Screens.Result
{
    public class ResultScreen : TachyonScreen
    {
        public override bool DisallowExternalBeatmapChanges => true;
        
        public readonly ScoreInfo Score;

        public ResultScreen(ScoreInfo score)
        {
            Score = score;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChildren = new Drawable[]
            {
                new Container
                { 
                    RelativeSizeAxes = Axes.Both,
                    Child = new ResultPanel(Score)
                    {
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                    }
                }
            };
        }
        
        protected override BackgroundScreen CreateBackground() => new BeatmapBackgroundScreen(Beatmap.Value);
        
        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);

            Background.FadeTo(0.5f, 250);
        }

        public override bool OnExiting(IScreen next)
        {
            Background.FadeTo(1, 250);

            return base.OnExiting(next);
        }
    }
}
