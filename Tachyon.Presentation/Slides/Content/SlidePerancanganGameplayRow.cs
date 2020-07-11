using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.Rulesets;
using Tachyon.Game.Rulesets.Objects;
using Tachyon.Game.Rulesets.Objects.Drawables;
using Tachyon.Game.Rulesets.UI.Scrolling;
using Tachyon.Game.Tests.Visual;
using Tachyon.Presentation.Graphics;

namespace Tachyon.Presentation.Slides.Content
{
    public class SlidePerancanganGameplayRow : SlideWithTitle
    {
        public SlidePerancanganGameplayRow()
            : base("Perancangan Gameplay")
        {
        }

        private const double default_duration = 1000;
        private const float scroll_time = 1000;
        
        [Resolved]
        protected AudioManager Audio { get; private set; }
        
        private DrawableTachyonRuleset drawableRuleset;

        [BackgroundDependencyLoader]
        private void load(LargeTextureStore textures, AudioManager audio)
        {
            var controlPointInfo = new ControlPointInfo();
            controlPointInfo.Add(0, new TimingControlPoint());
            
            WorkingBeatmap beatmap = CreateWorkingBeatmap(new Beatmap
            {
                HitObjects = new List<HitObject> { new Note { Type = NoteType.Upper }},
                BeatmapInfo = new BeatmapInfo
                {
                    BaseDifficulty = new BeatmapDifficulty(),
                    Metadata = new BeatmapMetadata
                    {
                        Artist = @"Unknown",
                        Title = @"Sample Beatmap",
                    }
                },
                ControlPointInfo = controlPointInfo
            });
            
            Content.Add(new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(0, -80),
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 5,
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre
                    },
                    drawableRuleset = new DrawableTachyonRuleset(new TachyonRuleset(), beatmap.GetPlayableBeatmap(new TachyonRuleset().RulesetInfo))
                    {
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre
                    },
                    new ItemDrawable(new KeyValuePair<string, string>("Playfield dan Hit Objects", "Gameplay pada umumnya sama seperti rhythm game pada umumnya. Terdapat 3 jenis hit object, playfield berupa dual lane dan judgement area di representasikan dengan lingkaran"), FontAwesome.Solid.EllipsisH)
                }
            });

            Scheduler.AddDelayed(addUpperNote, 2000);
            Scheduler.AddDelayed(addLowerNote, 4000);
            Scheduler.AddDelayed(addHoldNote, 6000);
        }
        
        protected virtual WorkingBeatmap CreateWorkingBeatmap(IBeatmap beatmap) =>
            new TachyonTestScene.ClockBackedTestWorkingBeatmap(beatmap, Clock, Audio);
        
        private void addUpperNote()
        {
            Note note = new Note
            {
                StartTime = drawableRuleset.Playfield.Time.Current + scroll_time,
                Row = 0,
            };

            note.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty());

            drawableRuleset.Playfield.Add(new DrawableUpperNote(note));
        }

        private void addLowerNote()
        {
            Note note = new Note
            {
                StartTime = drawableRuleset.Playfield.Time.Current + scroll_time,
                Row = 1,
            };

            note.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty());

            drawableRuleset.Playfield.Add(new DrawableLowerNote(note));
        }
        
        private void addHoldNote()
        {
            var holdNote = new HoldNote
            {
                StartTime = drawableRuleset.Playfield.Time.Current + scroll_time,
                Duration = default_duration,
            };

            holdNote.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty());

            drawableRuleset.Playfield.Add(new DrawableHoldNote(holdNote));
        }
    }
}
