using osu.Framework.Graphics;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK.Input;
using Tachyon.Game.Screens.Play;

namespace Tachyon.Game.Screens.Select
{
    public class TachyonSongSelect : SongSelect
    {
        private TachyonScreen player;
        private PlayButton playButton;

        protected override void LoadComplete()
        {
            base.LoadComplete();

            AddRangeInternal(new Drawable[]
            {
                playButton = new PlayButton
                {
                    Anchor = Anchor.BottomRight,
                    Origin = Anchor.BottomRight,
                    Action = () => { OnStart(); }
                }
            });
            
            playButton.Show();
        }

        public override void OnResuming(IScreen last)
        {
            base.OnResuming(last);

            player = null;
        }
        
        protected override bool OnKeyDown(KeyDownEvent e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                case Key.KeypadEnter:
                    FinaliseSelection();
                    return true;
            }

            return base.OnKeyDown(e);
        }
        
        protected override bool OnStart()
        {
            if (player != null) return false;
            
            this.Push(player = new PlayerLoader(() => new Player()));

            return true;
        }
    }
}
