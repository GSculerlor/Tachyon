using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Timing;
using Tachyon.Game.Rulesets.Objects;

namespace Tachyon.Game.Screens.Play.HUD
{
    public class BeatmapProgress : OverlayContainer
    {
        private const int bottom_bar_height = 5;

        private readonly BeatmapProgressBar bar;
        
        private IEnumerable<HitObject> objects;
        
        public IEnumerable<HitObject> Objects
        {
            set
            {
                objects = value;
                
                bar.StartTime = firstHitTime;
                bar.EndTime = lastHitTime;
            }
        }
        
        private double firstHitTime => objects.First().StartTime;
        private double lastHitTime => objects.Last().GetEndTime() + 1;

        private IClock gameplayClock;
        
        public override bool HandleNonPositionalInput => false;
        public override bool HandlePositionalInput => false;

        public BeatmapProgress()
        {
            Masking = true;
            Height = bottom_bar_height;
            
            Children = new Drawable[]
            {
                bar = new BeatmapProgressBar
                {
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                },
            };
        }

        [BackgroundDependencyLoader]
        private void load(GameplayClock clock)
        {
            base.LoadComplete();

            if (clock != null)
                gameplayClock = clock;
        }
        
        protected override void PopIn()
        {
            this.FadeIn(500, Easing.OutQuint);
            bar.Show();
        }

        protected override void PopOut()
        {
            this.FadeOut(100);
            bar.Hide();
        }
        
        protected override void Update()
        {
            base.Update();

            if (objects == null)
                return;

            double gameplayTime = gameplayClock?.CurrentTime ?? Time.Current;
            bar.CurrentTime = gameplayTime;
        }
    }
}
