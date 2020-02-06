using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using Tachyon.Game.Graphics.Sprites;

namespace Tachyon.Game.Graphics.Containers
{
    public class TachyonTextFlowContainer : TextFlowContainer
    {
        public TachyonTextFlowContainer(Action<SpriteText> defaultCreationParameters = null)
            : base(defaultCreationParameters)
        {
        }

        protected override SpriteText CreateSpriteText() => new TachyonSpriteText();
        
        public void AddArbitraryDrawable(Drawable drawable) => AddInternal(drawable);

        public IEnumerable<Drawable> AddIcon(IconUsage icon, Action<SpriteText> creationParameters = null) => AddText(icon.Icon.ToString(), creationParameters);
    }
}