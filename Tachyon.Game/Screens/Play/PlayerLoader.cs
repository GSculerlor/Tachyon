using System;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.Screens;
using osu.Framework.Threading;
using osuTK.Graphics;

namespace Tachyon.Game.Screens.Play
{
    public class PlayerLoader : DimmedBackgroundScreen
    {
        public override bool DisallowExternalBeatmapChanges => true;
        protected Task LoadTask { get; private set; }

        protected Task DisposalTask { get; private set; }
        
        private readonly Func<Player> createPlayer;

        private bool readyForPush => player.LoadState == LoadState.Ready;
        private Player player;
        private InputManager inputManager;
        private ScheduledDelegate scheduledPushPlayer;

        private Container content;
        
        [Resolved]
        private AudioManager audioManager { get; set; }
        
        public PlayerLoader(Func<Player> createPlayer)
        {
            this.createPlayer = createPlayer;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = content = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
            };
        }
        
        protected override void LoadComplete()
        {
            base.LoadComplete();

            inputManager = GetContainingInputManager();
        }

        public override void OnEntering(IScreen last)
        {
            base.OnEntering(last);
            
            prepareNewPlayer();

            this.Delay(1800).Schedule(pushWhenLoaded);
        }

        public override void OnResuming(IScreen last)
        {
            base.OnResuming(last);
            
            prepareNewPlayer();

            this.Delay(400).Schedule(pushWhenLoaded);
        }

        public override void OnSuspending(IScreen next)
        {
            base.OnSuspending(next);
            cancelLoad();
        }

        public override bool OnExiting(IScreen next)
        {
            cancelLoad();
            return base.OnExiting(next);
        }

        protected override void Update()
        {
            base.Update();
            
            if (!this.IsCurrentScreen())
                return;
        }

        private void prepareNewPlayer()
        {
            player = createPlayer();

            LoadTask = LoadComponentAsync(player);
        }
        
        private void pushWhenLoaded()
        {
            if (!this.IsCurrentScreen()) return;

            try
            {
                if (!readyForPush)
                {
                    // as the pushDebounce below has a delay, we need to keep checking and cancel a future debounce
                    // if we become unready for push during the delay.
                    cancelLoad();
                    return;
                }

                if (scheduledPushPlayer != null)
                    return;

                scheduledPushPlayer = Scheduler.AddDelayed(() =>
                {
                    this.Delay(250).Schedule(() =>
                    {
                        if (!this.IsCurrentScreen()) return;

                        LoadTask = null;

                        //By default, we want to load the player and never be returned to.
                        //Note that this may change if the player we load requested a re-run.
                        ValidForResume = false;

                        if (player.LoadedBeatmapSuccessfully)
                            this.Push(player);
                        else
                            this.Exit();
                    });
                }, 500);
            }
            finally
            {
                Schedule(pushWhenLoaded);
            }
        }
        
        private void cancelLoad()
        {
            scheduledPushPlayer?.Cancel();
            scheduledPushPlayer = null;
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (isDisposing)
            {
                // if the player never got pushed, we should explicitly dispose it.
                DisposalTask = LoadTask?.ContinueWith(_ => player.Dispose());
            }
        }
    }
}
