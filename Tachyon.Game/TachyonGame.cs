﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Development;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Framework.Threading;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Components;
using Tachyon.Game.Graphics.Containers;
using Tachyon.Game.Overlays;
using Tachyon.Game.Overlays.Music;
using Tachyon.Game.Overlays.Toolbar;
using Tachyon.Game.Screens;
using Tachyon.Game.Screens.Menu;

namespace Tachyon.Game
{
    public class TachyonGame : TachyonGameBase
    {
        public Toolbar Toolbar;

        private TachyonScreenStack screenStack;
        private IntroScreen introScreen;
        private DependencyContainer dependencies;
        private Container rightFloatingOverlayContent;
        private Container leftFloatingOverlayContent;
        private Container bottomMostOverlayContent;
        private Container overlayContainer;
        private MusicController musicController;
        private ScalingContainer screenContainer;
        
        public float ToolbarOffset => Toolbar.DrawHeight + 10;
        
        private readonly List<OverlayContainer> overlays = new List<OverlayContainer>();
        
        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
        
        [BackgroundDependencyLoader]
        private void load(FrameworkConfigManager frameworkConfig)
        {
            if (!Host.IsPrimaryInstance && !DebugUtils.IsDebugBuild)
            {
                Logger.Log(@"Can't run multiple instances.", LoggingTarget.Runtime, LogLevel.Error);
                Environment.Exit(0);
            }

            dependencies.CacheAs(this);
            Beatmap.BindValueChanged(beatmapChanged, true);
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            AddRange(new Drawable[]
            {
                screenContainer = new ScalingContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        screenStack = new TachyonScreenStack { RelativeSizeAxes = Axes.Both },
                    }
                },
                overlayContainer = new Container { RelativeSizeAxes = Axes.Both },
                bottomMostOverlayContent = new Container { RelativeSizeAxes = Axes.Both },
                rightFloatingOverlayContent = new Container { RelativeSizeAxes = Axes.Both },
                leftFloatingOverlayContent = new Container { RelativeSizeAxes = Axes.Both },
            });

            screenStack.ScreenPushed += screenPushed;
            screenStack.ScreenExited += screenExited;
            
            screenStack.Push(createLoader().With(l => l.RelativeSizeAxes = Axes.Both));
            
            loadComponentSingleFile(Toolbar = new Toolbar(), bottomMostOverlayContent.Add);

            loadComponentSingleFile(musicController = new MusicController(), Add, true);
            
            loadComponentSingleFile(new MusicPlayerOverlay
            {
                Margin = new MarginPadding
                {
                    Top = ToolbarOffset,
                    Right = 10
                },
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
            }, rightFloatingOverlayContent.Add, true);
            
            Screen.BindTo(Toolbar.Screen);
        }
        
        [Cached]
        [Cached(typeof(IBindable<Toolbar.MenuScreen>))]
        protected readonly Bindable<Toolbar.MenuScreen> Screen = new Bindable<Toolbar.MenuScreen>();
        
        protected override Container CreateScalingContainer() => new ScalingContainer();

        private LoaderScreen createLoader() => new LoaderScreen();
        
        private Task asyncLoadStream;
        
        private void loadComponentSingleFile<T>(T d, Action<T> add, bool cache = false)
            where T : Drawable
        {
            if (cache)
                dependencies.Cache(d);

            if (d is OverlayContainer overlay)
                overlays.Add(overlay);

            Schedule(() =>
            {
                var previousLoadStream = asyncLoadStream;

                asyncLoadStream = Task.Run(async () =>
                {
                    if (previousLoadStream != null)
                        await previousLoadStream;

                    try
                    {
                        Logger.Log($"Loading {d}...", level: LogLevel.Debug);

                        Task task = null;
                        var del = new ScheduledDelegate(() => task = LoadComponentAsync(d, add));
                        Scheduler.Add(del);

                        while (!IsDisposed && !del.Completed)
                            await Task.Delay(10);

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
        }
        
        private void beatmapChanged(ValueChangedEvent<WorkingBeatmap> beatmap)
        {
            beatmap.OldValue?.CancelAsyncLoad();

            var newBeatmap = beatmap.NewValue;

            if (newBeatmap != null)
            {
                newBeatmap.Track.Completed += () => Scheduler.AddOnce(() => trackCompleted(newBeatmap));
                newBeatmap.BeginAsyncLoad();
            }

            void trackCompleted(WorkingBeatmap b)
            {
                if (Beatmap.Value != b)
                    return;

                if (!Beatmap.Value.Track.Looping && !Beatmap.Disabled)
                    musicController.NextTrack();
            }
        }
        
        protected override bool OnExiting()
        {
            if (screenStack.CurrentScreen is LoaderScreen)
                return false;

            if (introScreen == null)
                return true;

            if (!(screenStack.CurrentScreen is IntroScreen))
            {
                Scheduler.Add(introScreen.MakeCurrent);
                return true;
            }

            return base.OnExiting();
        }
        
        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            overlayContainer.Padding = new MarginPadding { Top = ToolbarOffset };
        }

        // ReSharper disable once UnusedParameter.Local
        private void screenChanged(IScreen current, IScreen newScreen)
        {
            switch (newScreen)
            {
                case IntroScreen intro:
                    introScreen = intro;
                    break;
            }

            if (newScreen is ITachyonScreen newTachyonScreen)
            {
                if (newTachyonScreen.ToolbarVisible)
                    Toolbar?.Show();
                else
                    Toolbar?.Hide();
                
                /*if (newTachyonScreen.AllowBackButton)
                    Toolbar?.ToolbarBackButton?.Show();
                else
                    Toolbar?.ToolbarBackButton?.Hide();*/
            }
        }

        private void screenPushed(IScreen lastScreen, IScreen newScreen)
        {
            screenChanged(lastScreen, newScreen);
            Logger.Log($"Screen changed → {newScreen}");
        }

        private void screenExited(IScreen lastScreen, IScreen newScreen)
        {
            screenChanged(lastScreen, newScreen);
            Logger.Log($"Screen changed ← {newScreen}");

            if (newScreen == null)
                Exit();
        }
    }
}
