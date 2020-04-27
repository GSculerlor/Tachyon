using System;
using Microsoft.EntityFrameworkCore.Internal;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using Tachyon.Game.Beatmaps;

namespace Tachyon.Game.Screens
{
    public abstract class TachyonScreen : Screen, ITachyonScreen
    {
        /// <summary>
        /// A user-facing title for this screen.
        /// </summary>
        public virtual string Title => GetType().ShortDisplayName();

        public string Description => Title;

        public virtual bool AllowBackButton => true;

        public virtual bool AllowExternalScreenChange => false;
        
        public virtual bool DisallowExternalBeatmapChanges => false;
        
        public Bindable<WorkingBeatmap> Beatmap { get; private set; }

        protected BackgroundScreen Background => backgroundStack?.CurrentScreen as BackgroundScreen;

        private BackgroundScreen localBackground;

        [Resolved(canBeNull: true)]
        private BackgroundScreenStack backgroundStack { get; set; }

        
        protected TachyonScreen()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        private TachyonScreenDependencies screenDependencies;

        internal void CreateLeasedDependencies(IReadOnlyDependencyContainer dependencies) => createDependencies(dependencies);

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            if (screenDependencies == null)
            {
                if (DisallowExternalBeatmapChanges)
                    throw new InvalidOperationException($"Screens that specify {nameof(DisallowExternalBeatmapChanges)} must be pushed immediately.");

                createDependencies(parent);
            }

            return base.CreateChildDependencies(screenDependencies);
        }

        private void createDependencies(IReadOnlyDependencyContainer dependencies)
        {
            screenDependencies = new TachyonScreenDependencies(DisallowExternalBeatmapChanges, dependencies);

            Beatmap = screenDependencies.Beatmap;
        }
        
        public override void OnEntering(IScreen last)
        {
            backgroundStack?.Push(localBackground = CreateBackground());
            base.OnEntering(last);
        }

        public override bool OnExiting(IScreen next)
        {
            if (base.OnExiting(next))
                return true;

            if (localBackground != null && backgroundStack?.CurrentScreen == localBackground)
                backgroundStack?.Exit();

            return false;
        }
        
        protected virtual BackgroundScreen CreateBackground() => null;
    }
}