﻿using Tachyon.Game.Graphics.Backgrounds;
using Tachyon.Game.Screens.Backgrounds;

namespace Tachyon.Game.Screens
{
    public abstract class DimmedBackgroundScreen : TachyonScreen
    {
        public override bool AllowBackButton => false;

        protected override BackgroundScreen CreateBackground() => new BeatmapBackgroundScreen(Beatmap.Value);

        public new BeatmapBackgroundScreen Background => (BeatmapBackgroundScreen)base.Background;
    }
}    
