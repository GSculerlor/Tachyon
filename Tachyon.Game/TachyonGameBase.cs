using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Performance;
using osu.Framework.Graphics.Textures;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using Tachyon.Game.Configuration;
using Tachyon.Game.Graphics;

namespace Tachyon.Game
{
    public class TachyonGameBase : osu.Framework.Game
    {
        private DependencyContainer dependencies;

        private TachyonConfigManager LocalConfig;

        private Bindable<bool> fpsDisplayVisible;

        private Storage Storage { get; set; }
        
        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) =>
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

        [BackgroundDependencyLoader]
        private void load()
        {
            Resources.AddStore(new DllResourceStore(@"Tachyon.Resources.dll"));
            
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
            
            dependencies.Cache(new TachyonColor());
            
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
    }
}