﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Framework.Threading;
using Tachyon.Game.Components;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Game.Overlays;
using Tachyon.Game.Screens;
using Tachyon.Game.Screens.Menu;

namespace Tachyon.Game
{
    public class TachyonGame : TachyonGameBase
    {
        private Toolbar toolbar;

        private TachyonScreenStack screenStack;
        
        private IntroScreen introScreen;

        private DependencyContainer dependencies;
        
        private FrameworkConfigManager config;
        
        private Container bottomMostOverlayContent;

        private MusicController musicController;
        
        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
        
        [BackgroundDependencyLoader]
        private void load(FrameworkConfigManager frameworkConfig)
        {
            config = frameworkConfig;
            
            dependencies.CacheAs(this);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            AddRange(new Drawable[]
            {
                new ScalingContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        screenStack = new TachyonScreenStack { RelativeSizeAxes = Axes.Both },
                    }
                },
                bottomMostOverlayContent = new Container { RelativeSizeAxes = Axes.Both },
            });

            screenStack.ScreenPushed += screenPushed;
            screenStack.ScreenExited += screenExited;
            
            loadComponentSingleFile(toolbar = new Toolbar
            {
                Back = () =>
                {
                    if ((screenStack.CurrentScreen as ITachyonScreen)?.AllowBackButton == true)
                        screenStack.Exit();
                }
            }, bottomMostOverlayContent.Add);

            loadComponentSingleFile(musicController = new MusicController(), Add, true);
            
            screenStack.Push(introScreen = new IntroScreen());
        }
        
        protected override Container CreateScalingContainer() => new ScalingContainer();
        
        private Task asyncLoadStream;
        
        private T loadComponentSingleFile<T>(T d, Action<T> add, bool cache = false)
            where T : Drawable
        {
            if (cache)
                dependencies.Cache(d);

            /*if (d is OverlayContainer overlay)
                overlays.Add(overlay);*/

            // schedule is here to ensure that all component loads are done after LoadComplete is run (and thus all dependencies are cached).
            // with some better organisation of LoadComplete to do construction and dependency caching in one step, followed by calls to loadComponentSingleFile,
            // we could avoid the need for scheduling altogether.
            Schedule(() =>
            {
                var previousLoadStream = asyncLoadStream;

                //chain with existing load stream
                asyncLoadStream = Task.Run(async () =>
                {
                    if (previousLoadStream != null)
                        await previousLoadStream;

                    try
                    {
                        Logger.Log($"Loading {d}...", level: LogLevel.Debug);

                        // Since this is running in a separate thread, it is possible for OsuGame to be disposed after LoadComponentAsync has been called
                        // throwing an exception. To avoid this, the call is scheduled on the update thread, which does not run if IsDisposed = true
                        Task task = null;
                        var del = new ScheduledDelegate(() => task = LoadComponentAsync(d, add));
                        Scheduler.Add(del);

                        // The delegate won't complete if OsuGame has been disposed in the meantime
                        while (!IsDisposed && !del.Completed)
                            await Task.Delay(10);

                        // Either we're disposed or the load process has started successfully
                        if (IsDisposed)
                            return;

                        Debug.Assert(task != null);

                        await task;

                        Logger.Log($"Loaded {d}!", level: LogLevel.Debug);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                });
            });

            return d;
        }
        
        protected virtual void ScreenChanged(IScreen current, IScreen newScreen)
        {
            introScreen = newScreen switch
            {
                IntroScreen intro => intro,
                _ => introScreen
            };

            if (newScreen is ITachyonScreen newTachyonScreen)
            {
                if (newTachyonScreen.ToolbarVisible)
                    toolbar.Show();
                else
                    toolbar.Hide();
                
                if (newTachyonScreen.AllowBackButton)
                    toolbar.ToolbarBackButton?.Show();
                else
                    toolbar.ToolbarBackButton?.Hide();
            }
        }

        private void screenPushed(IScreen lastScreen, IScreen newScreen)
        {
            ScreenChanged(lastScreen, newScreen);
            Logger.Log($"Screen changed → {newScreen}");
        }

        private void screenExited(IScreen lastScreen, IScreen newScreen)
        {
            ScreenChanged(lastScreen, newScreen);
            Logger.Log($"Screen changed ← {newScreen}");

            if (newScreen == null)
                Exit();
        }
    }
}
