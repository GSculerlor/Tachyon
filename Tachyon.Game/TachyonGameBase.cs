using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Performance;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;
using osu.Framework.Platform;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Configuration;
using Tachyon.Game.Database;
using Tachyon.Game.Graphics;
using Tachyon.Game.Input;
using Tachyon.Game.IO;

namespace Tachyon.Game
{
    public class TachyonGameBase : osu.Framework.Game, ICanAcceptFiles
    {
        public const int SAMPLE_CONCURRENCY = 6;
        
        private DependencyContainer dependencies;
        private TachyonConfigManager LocalConfig;
        
        protected KeyBindingStore KeyBindingStore;
        
        protected FileStore FileStore;
        protected BeatmapManager BeatmapManager;

        private Bindable<bool> fpsDisplayVisible;

        private Storage Storage { get; set; }
        
        protected Bindable<WorkingBeatmap> Beatmap { get; private set; }
        
        public TachyonGameBase()
        {
            Name = @"Tachyon";
        }
        
        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        private DatabaseContextFactory contextFactory;
        
        [BackgroundDependencyLoader]
        private void load()
        {
            Resources.AddStore(new DllResourceStore(@"Tachyon.Resources.dll"));
            
            dependencies.Cache(contextFactory = new DatabaseContextFactory(Storage));
            
            var largeStore = new LargeTextureStore(Host.CreateTextureLoaderStore(new NamespacedResourceStore<byte[]>(Resources, @"Textures")));
            largeStore.AddStore(Host.CreateTextureLoaderStore(new OnlineStore()));
            dependencies.Cache(largeStore);
            
            dependencies.CacheAs(this);
            dependencies.Cache(LocalConfig);
            
            AddFont(Resources, @"Fonts/Quicksand");
            AddFont(Resources, @"Fonts/Quicksand-Italic");
            AddFont(Resources, @"Fonts/Quicksand-SemiBold");
            AddFont(Resources, @"Fonts/Quicksand-SemiBoldItalic");
            AddFont(Resources, @"Fonts/Quicksand-Bold");
            AddFont(Resources, @"Fonts/Quicksand-BoldItalic");
            AddFont(Resources, @"Fonts/Quicksand-Light");
            AddFont(Resources, @"Fonts/Quicksand-LightItalic");

            AddFont(Resources, @"Fonts/Noto-Basic");
            AddFont(Resources, @"Fonts/Noto-Hangul");
            AddFont(Resources, @"Fonts/Noto-CJK-Basic");
            AddFont(Resources, @"Fonts/Noto-CJK-Compatibility");

            AddFont(Resources, @"Fonts/Venera");
            AddFont(Resources, @"Fonts/Venera-Light");
            AddFont(Resources, @"Fonts/Venera-Medium");
            
            Audio.Samples.PlaybackConcurrency = SAMPLE_CONCURRENCY;
            
            runMigrations();
            
            dependencies.Cache(FileStore = new FileStore(contextFactory, Storage));
            
            var defaultBeatmap = new PlaceholderWorkingBeatmap(Audio, Textures);
            
            dependencies.Cache(KeyBindingStore = new KeyBindingStore(contextFactory));
            dependencies.Cache(BeatmapManager = new BeatmapManager(Storage, contextFactory, Audio, Host, defaultBeatmap));
            dependencies.Cache(new TachyonColor());
            
            fileImporters.Add(BeatmapManager);
            
            Beatmap = new NonNullableBindable<WorkingBeatmap>(defaultBeatmap);
            
            Beatmap.BindValueChanged(b => ScheduleAfterChildren(() =>
            {
                if (b.OldValue?.TrackLoaded == true && b.OldValue?.Track != b.NewValue?.Track)
                    b.OldValue.RecycleTrack();
            }));
            
            dependencies.CacheAs<IBindable<WorkingBeatmap>>(Beatmap);
            dependencies.CacheAs(Beatmap);
            
            FileStore.Cleanup();

            base.Content.Add(CreateScalingContainer());
        }
        
        protected virtual Container CreateScalingContainer() => new DrawSizePreservingFillContainer();
        
        protected override void LoadComplete()
        {
            base.LoadComplete();
            
            //TODO: Change this hardcoded config when setting is implemented
            fpsDisplayVisible = new Bindable<bool>();
            fpsDisplayVisible.ValueChanged += visible => { FrameStatistics.Value = visible.NewValue ? FrameStatisticsMode.Minimal : FrameStatisticsMode.None; };
            fpsDisplayVisible.TriggerChange();

            FrameStatistics.ValueChanged += e => fpsDisplayVisible.Value = e.NewValue != FrameStatisticsMode.None;
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            
            if (Storage == null)
                Storage = host.Storage;

            if (LocalConfig == null)
                LocalConfig = new TachyonConfigManager(Storage);
        }

        private readonly List<ICanAcceptFiles> fileImporters = new List<ICanAcceptFiles>();
        
        public async Task Import(params string[] paths)
        {
            var extension = Path.GetExtension(paths.First())?.ToLowerInvariant();

            foreach (var importer in fileImporters)
            {
                if (importer.HandledExtensions.Contains(extension))
                    await importer.Import(paths);
            }
        }
        
        public string[] HandledExtensions => fileImporters.SelectMany(i => i.HandledExtensions).ToArray();
        
        private void runMigrations()
        {
            try
            {
                using (var db = contextFactory.GetForWrite(false))
                    db.Context.Migrate();
            }
            catch (Exception e)
            {
                Logger.Error(e.InnerException ?? e, "Migration failed! We'll be starting with a fresh database.", LoggingTarget.Database);

                contextFactory.ResetDatabase();

                Logger.Log("Database purged successfully.", LoggingTarget.Database);

                using (var db = contextFactory.GetForWrite(false))
                    db.Context.Migrate();
            }
        }
    }
}