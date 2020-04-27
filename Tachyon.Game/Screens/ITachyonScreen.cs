﻿using osu.Framework.Bindables;
 using osu.Framework.Screens;
 using Tachyon.Game.Beatmaps;
 using Tachyon.Game.Graphics.UserInterface;
 using Tachyon.Game.Rulesets;

 namespace Tachyon.Game.Screens
{
    public interface ITachyonScreen : IScreen
    {
        /// <summary>
        /// Whether the beatmap or ruleset should be allowed to be changed by the user or game.
        /// Used to mark exclusive areas where this is strongly prohibited, like gameplay.
        /// </summary>
        bool DisallowExternalBeatmapChanges { get; }

        /// <summary>
        /// Whether the user can exit this this <see cref="ITachyonScreen"/> by pressing the back button.
        /// </summary>
        bool AllowBackButton { get; }

        /// <summary>
        /// Whether a top-level component should be allowed to exit the current screen to, for example,
        /// complete an import. Note that this can be overridden by a user if they specifically request.
        /// </summary>
        bool AllowExternalScreenChange { get; }

        Bindable<WorkingBeatmap> Beatmap { get; }
    }
}