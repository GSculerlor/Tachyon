﻿using osu.Framework.Screens;
using Tachyon.Game.Graphics.UserInterface;

namespace Tachyon.Game.Screens
{
    public interface ITachyonScreen : IScreen
    {
        /// <summary>
        /// Whether the user can exit this this <see cref="ITachyonScreen"/> by pressing the back button.
        /// </summary>
        bool AllowBackButton { get; }
        
        /// <summary>
        /// Whether this <see cref="ITachyonScreen"/> allows the toolbar to be displayed.
        /// </summary>
        bool ToolbarVisible { get; }
        
        /// <summary>
        /// Whether a top-level component should be allowed to exit the current screen to, for example,
        /// complete an import. Note that this can be overridden by a user if they specifically request.
        /// </summary>
        bool AllowExternalScreenChange { get; }

        /// <summary>
        /// Whether this <see cref="TachyonScreen"/> allows the cursor to be displayed.
        /// </summary>
        bool CursorVisible { get; }
    }
}