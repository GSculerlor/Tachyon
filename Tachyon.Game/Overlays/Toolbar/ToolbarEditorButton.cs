using System;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;

namespace Tachyon.Game.Overlays.Toolbar
{
    public class ToolbarEditorButton : ToolbarButton
    {
        public Action EditorAction;
        
        public ToolbarEditorButton()
        {
            Icon = FontAwesome.Solid.PenSquare;
        }

        protected override bool OnClick(ClickEvent e)
        {
            EditorAction?.Invoke();
            
            return base.OnClick(e);
        }
    }
}
