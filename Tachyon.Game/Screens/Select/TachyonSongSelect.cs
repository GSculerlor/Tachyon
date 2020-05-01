using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK.Input;
using Tachyon.Game.Screens.Play;

namespace Tachyon.Game.Screens.Select
{
    public class TachyonSongSelect : SongSelect
    {
        private TachyonScreen player;
        
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
