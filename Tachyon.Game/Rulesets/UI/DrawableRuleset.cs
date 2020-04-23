using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input;
using osu.Framework.IO.Stores;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Rulesets.Judgements;
using Tachyon.Game.Rulesets.Objects;
using Tachyon.Game.Rulesets.Objects.Drawables;
using Tachyon.Game.Rulesets.Scoring;
using Tachyon.Game.Screens.Play;

namespace Tachyon.Game.Rulesets.UI
{
    public abstract class DrawableRuleset<TObject> : DrawableRuleset
        where TObject : HitObject
    {
        public override event Action<JudgementResult> OnNewResult;

        public override event Action<JudgementResult> OnRevertResult;

        /// <summary>
        /// The selected variant.
        /// </summary>
        public virtual int Variant => 0;

        /// <summary>
        /// The key conversion input manager for this DrawableRuleset.
        /// </summary>
        public PassThroughInputManager KeyBindingInputManager;

        public override double GameplayStartTime => Objects.First().StartTime - 2000;

        private readonly Lazy<Playfield> playfield;

        private TextureStore textureStore;

        private ISampleStore localSampleStore;

        /// <summary>
        /// The playfield.
        /// </summary>
        public override Playfield Playfield => playfield.Value;

        public override Container Overlays { get; } = new Container { RelativeSizeAxes = Axes.Both };

        public override Container FrameStableComponents { get; } = new Container { RelativeSizeAxes = Axes.Both };

        public override GameplayClock FrameStableClock => frameStabilityContainer.GameplayClock;

        private bool frameStablePlayback = true;

        /// <summary>
        /// Whether to enable frame-stable playback.
        /// </summary>
        internal bool FrameStablePlayback
        {
            get => frameStablePlayback;
            set
            {
                frameStablePlayback = false;
                if (frameStabilityContainer != null)
                    frameStabilityContainer.FrameStablePlayback = value;
            }
        }

        /// <summary>
        /// The beatmap.
        /// </summary>
        public readonly Beatmap<TObject> Beatmap;

        public override IEnumerable<HitObject> Objects => Beatmap.HitObjects;

        private FrameStabilityContainer frameStabilityContainer;

        /// <summary>
        /// Creates a ruleset visualisation for the provided ruleset and beatmap.
        /// </summary>
        /// <param name="ruleset">The ruleset being represented.</param>
        /// <param name="beatmap">The beatmap to create the hit renderer for.</param>
        protected DrawableRuleset(Ruleset ruleset, IBeatmap beatmap)
            : base(ruleset)
        {
            if (beatmap == null)
                throw new ArgumentNullException(nameof(beatmap), "Beatmap cannot be null.");

            if (!(beatmap is Beatmap<TObject> tBeatmap))
                throw new ArgumentException($"{GetType()} expected the beatmap to contain hitobjects of type {typeof(TObject)}.", nameof(beatmap));

            Beatmap = tBeatmap;

            RelativeSizeAxes = Axes.Both;

            KeyBindingInputManager = CreateInputManager();
            playfield = new Lazy<Playfield>(CreatePlayfield);

            IsPaused.ValueChanged += paused =>
            {
                KeyBindingInputManager.UseParentInput = !paused.NewValue;
            };
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

            var resources = Ruleset.CreateResourceStore();

            if (resources != null)
            {
                textureStore = new TextureStore(new TextureLoaderStore(new NamespacedResourceStore<byte[]>(resources, "Textures")));
                textureStore.AddStore(dependencies.Get<TextureStore>());
                dependencies.Cache(textureStore);

                localSampleStore = dependencies.Get<AudioManager>().GetSampleStore(new NamespacedResourceStore<byte[]>(resources, "Samples"));
                localSampleStore.PlaybackConcurrency = TachyonGameBase.SAMPLE_CONCURRENCY;
                dependencies.CacheAs<ISampleStore>(new FallbackSampleStore(localSampleStore, dependencies.Get<ISampleStore>()));
            }

            return dependencies;
        }

        public virtual PlayfieldAdjustmentContainer CreatePlayfieldAdjustmentContainer() => new PlayfieldAdjustmentContainer();

        [BackgroundDependencyLoader]
        private void load(CancellationToken? cancellationToken)
        {
            InternalChildren = new Drawable[]
            {
                frameStabilityContainer = new FrameStabilityContainer(GameplayStartTime)
                {
                    FrameStablePlayback = FrameStablePlayback,
                    Children = new Drawable[]
                    {
                        FrameStableComponents,
                        KeyBindingInputManager
                            .WithChild(CreatePlayfieldAdjustmentContainer()
                                .WithChild(Playfield)
                            ),
                        Overlays,
                    }
                },
            };

            loadObjects(cancellationToken);
        }

        /// <summary>
        /// Creates and adds drawable representations of hit objects to the play field.
        /// </summary>
        private void loadObjects(CancellationToken? cancellationToken)
        {
            foreach (TObject h in Beatmap.HitObjects)
            {
                cancellationToken?.ThrowIfCancellationRequested();
                addHitObject(h);
            }

            cancellationToken?.ThrowIfCancellationRequested();

            Playfield.PostProcess();
        }

        /// <summary>
        /// Creates and adds the visual representation of a <typeparamref name="TObject"/> to this <see cref="DrawableRuleset{TObject}"/>.
        /// </summary>
        /// <param name="hitObject">The <typeparamref name="TObject"/> to add the visual representation for.</param>
        private void addHitObject(TObject hitObject)
        {
            var drawableObject = CreateDrawableRepresentation(hitObject);

            if (drawableObject == null)
                return;

            drawableObject.OnNewResult += (_, r) => OnNewResult?.Invoke(r);
            drawableObject.OnRevertResult += (_, r) => OnRevertResult?.Invoke(r);

            Playfield.Add(drawableObject);
        }

        /// <summary>
        /// Creates a DrawableHitObject from a HitObject.
        /// </summary>
        /// <param name="h">The HitObject to make drawable.</param>
        /// <returns>The DrawableHitObject.</returns>
        public abstract DrawableHitObject<TObject> CreateDrawableRepresentation(TObject h);

        /// <summary>
        /// Creates a key conversion input manager. An exception will be thrown if a valid <see cref="RulesetInputManager{T}"/> is not returned.
        /// </summary>
        /// <returns>The input manager.</returns>
        protected abstract PassThroughInputManager CreateInputManager();

        /// <summary>
        /// Creates a Playfield.
        /// </summary>
        /// <returns>The Playfield.</returns>
        protected abstract Playfield CreatePlayfield();

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            localSampleStore?.Dispose();
        }
    }
    
    public abstract class DrawableRuleset : CompositeDrawable
    {
        /// <summary>
        /// Invoked when a <see cref="JudgementResult"/> has been applied by a <see cref="DrawableHitObject"/>.
        /// </summary>
        public abstract event Action<JudgementResult> OnNewResult;

        /// <summary>
        /// Invoked when a <see cref="JudgementResult"/> is being reverted by a <see cref="DrawableHitObject"/>.
        /// </summary>
        public abstract event Action<JudgementResult> OnRevertResult;

        /// <summary>
        /// Whether the game is paused. Used to block user input.
        /// </summary>
        public readonly BindableBool IsPaused = new BindableBool();

        /// <summary>
        /// The playfield.
        /// </summary>
        public abstract Playfield Playfield { get; }

        /// <summary>
        /// Content to be placed above hitobjects. Will be affected by frame stability.
        /// </summary>
        public abstract Container Overlays { get; }

        /// <summary>
        /// Components to be run potentially multiple times in line with frame-stable gameplay.
        /// </summary>
        public abstract Container FrameStableComponents { get; }

        /// <summary>
        /// The frame-stable clock which is being used for playfield display.
        /// </summary>
        public abstract GameplayClock FrameStableClock { get; }

        /// <summary>~
        /// The associated ruleset.
        /// </summary>
        public readonly Ruleset Ruleset;

        /// <summary>
        /// Creates a ruleset visualisation for the provided ruleset.
        /// </summary>
        /// <param name="ruleset">The ruleset.</param>
        internal DrawableRuleset(Ruleset ruleset)
        {
            Ruleset = ruleset;
        }

        /// <summary>
        /// All the converted hit objects contained by this hit renderer.
        /// </summary>
        public abstract IEnumerable<HitObject> Objects { get; }

        /// <summary>
        /// The point in time at which gameplay starts, including any required lead-in for display purposes.
        /// Defaults to two seconds before the first <see cref="HitObject"/>. Override as necessary.
        /// </summary>
        public abstract double GameplayStartTime { get; }

        /// <summary>
        /// Returns first available <see cref="HitWindows"/> provided by a <see cref="HitObject"/>.
        /// </summary>
        [CanBeNull]
        public HitWindows FirstAvailableHitWindows
        {
            get
            {
                foreach (var h in Objects)
                {
                    if (h.HitWindows.WindowFor(HitResult.Miss) > 0)
                        return h.HitWindows;

                    foreach (var n in h.NestedHitObjects)
                    {
                        if (h.HitWindows.WindowFor(HitResult.Miss) > 0)
                            return n.HitWindows;
                    }
                }

                return null;
            }
        }
    }
    
    public class BeatmapInvalidForRulesetException : ArgumentException
    {
        public BeatmapInvalidForRulesetException(string text)
            : base(text)
        {
        }
    }
    
    public class FallbackSampleStore : ISampleStore
    {
        private readonly ISampleStore primary;
        private readonly ISampleStore secondary;

        public FallbackSampleStore(ISampleStore primary, ISampleStore secondary)
        {
            this.primary = primary;
            this.secondary = secondary;
        }

        public SampleChannel Get(string name) => primary.Get(name) ?? secondary.Get(name);

        public Task<SampleChannel> GetAsync(string name) => primary.GetAsync(name) ?? secondary.GetAsync(name);

        public Stream GetStream(string name) => primary.GetStream(name) ?? secondary.GetStream(name);

        public IEnumerable<string> GetAvailableResources() => throw new NotSupportedException();

        public void AddAdjustment(AdjustableProperty type, BindableNumber<double> adjustBindable) => throw new NotSupportedException();

        public void RemoveAdjustment(AdjustableProperty type, BindableNumber<double> adjustBindable) => throw new NotSupportedException();

        public BindableNumber<double> Volume => throw new NotSupportedException();

        public BindableNumber<double> Balance => throw new NotSupportedException();

        public BindableNumber<double> Frequency => throw new NotSupportedException();

        public BindableNumber<double> Tempo => throw new NotSupportedException();

        public IBindable<double> GetAggregate(AdjustableProperty type) => throw new NotSupportedException();

        public IBindable<double> AggregateVolume => throw new NotSupportedException();

        public IBindable<double> AggregateBalance => throw new NotSupportedException();

        public IBindable<double> AggregateFrequency => throw new NotSupportedException();

        public IBindable<double> AggregateTempo => throw new NotSupportedException();

        public int PlaybackConcurrency
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public void Dispose()
        {
        }
    }
}
