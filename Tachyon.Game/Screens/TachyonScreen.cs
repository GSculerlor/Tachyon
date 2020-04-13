using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using Tachyon.Game.Beatmaps;

 namespace Tachyon.Game.Screens
{
    public abstract class TachyonScreen : Screen, ITachyonScreen
    {
        public virtual bool AllowBackButton => true;
        
        public virtual bool ToolbarVisible => true;
        
        public bool AllowExternalScreenChange => false;

        public virtual bool CursorVisible => true;

        protected BackgroundScreen Background => backgroundStack?.CurrentScreen as BackgroundScreen;

        private BackgroundScreen localBackground;
        private TachyonScreenDependencies screenDependencies;

        [Resolved(canBeNull: true)]
        private BackgroundScreenStack backgroundStack { get; set; }
        
        public Bindable<WorkingBeatmap> Beatmap { get; private set; }

        
        protected TachyonScreen()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
        }

        internal void CreateLeasedDependencies(IReadOnlyDependencyContainer dependencies) => createDependencies(dependencies);

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            if (screenDependencies == null)
            {
                createDependencies(parent);
            }

            return base.CreateChildDependencies(screenDependencies);
        }

        private void createDependencies(IReadOnlyDependencyContainer dependencies)
        {
            screenDependencies = new TachyonScreenDependencies(false, dependencies);

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