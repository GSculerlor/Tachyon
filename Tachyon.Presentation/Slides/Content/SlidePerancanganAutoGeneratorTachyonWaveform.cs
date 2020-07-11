using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Timing;
using osuTK;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.Rulesets;
using Tachyon.Game.Screens.Generate;
using Tachyon.Game.Screens.Generate.Components;
using Tachyon.Game.Tests;
using Tachyon.Presentation.Graphics;

namespace Tachyon.Presentation.Slides.Content
{
    public class SlidePerancanganAutoGeneratorTachyonWaveform : SlideWithTitle
    {
        public SlidePerancanganAutoGeneratorTachyonWaveform()
            : base("TachyonWaveform dan Clock Seeking")
        {
            Clock = new EditorClock(new ControlPointInfo(), 5000, BeatDivisor) { IsCoupled = false };
            BeatDivisor.Value = 4;
        }
        
        private DependencyContainer dependencies;
        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
            => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
        
        protected readonly BindableBeatDivisor BeatDivisor = new BindableBeatDivisor();
        protected new readonly EditorClock Clock;

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures, AudioManager audio)
        {
            dependencies.Cache(BeatDivisor);
            
            dependencies.CacheAs<IFrameBasedClock>(Clock);
            dependencies.CacheAs<IAdjustableClock>(Clock);
            
            Beatmap.Value = new WaveformTestBeatmap(audio);
            var playable = Beatmap.Value.GetPlayableBeatmap(new TachyonRuleset().RulesetInfo);
            var editorBeatmap = new EditorBeatmap(playable);
            
            dependencies.Cache(editorBeatmap);
            dependencies.CacheAs<IBeatSnapProvider>(editorBeatmap);

            Content.AddRange(new Drawable[]
            {
                editorBeatmap,
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = FillDirection.Vertical,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Spacing = new Vector2(0, 20),
                    Children = new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = 200,
                            Direction = FillDirection.Horizontal,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Children = new Drawable[]
                            {
                                new TimelineArea
                                {
                                    Child = new TimelineTickDisplay(),
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre,
                                    RelativeSizeAxes = Axes.X,
                                    Size = new Vector2(0.8f, 100),
                                },
                                
                                new BeatDivisorControl(BeatDivisor)
                                {
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre,
                                    Margin = new MarginPadding { Left = 30 },
                                    Size = new Vector2(100)
                                },
                            }
                        },
                        new ItemDrawable(new KeyValuePair<string, string>("TachyonWaveform", "Class yang dibaca pada metode Ganen dalam menentukan hit object. Membawa komponen amplitude, intensity dan channel info"), FontAwesome.Solid.WaveSquare),
                        new ItemDrawable(new KeyValuePair<string, string>("Clock Seeking", "Merupakan proses yang dilakukan clock untuk berpindah ke titik tertentu berdasarkan value dari beat divisor dan BPM dari audio."), FontAwesome.Solid.History)
                    }
                }
            });
        }
    }
}
