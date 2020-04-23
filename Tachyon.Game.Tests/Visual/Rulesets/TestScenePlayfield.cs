using System;
using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.Rulesets;
using Tachyon.Game.Rulesets.Objects;
using Tachyon.Game.Rulesets.Objects.Drawables;
using Tachyon.Game.Rulesets.UI.Scrolling;

namespace Tachyon.Game.Tests.Visual.Rulesets
{
    [TestFixture]
    public class TestScenePlayfield : TachyonTestScene
    {
        private const double default_duration = 1000;
        private const float scroll_time = 1000;

        protected override double TimePerAction => default_duration * 2;

        private readonly Random rng = new Random(1337);
        private DrawableTachyonRuleset drawableRuleset;
        private Container playfieldContainer;
        
        [BackgroundDependencyLoader]
        private void load()
        {
            AddStep("Upper", addUpperNote);
            AddStep("Lower", addLowerNote);
            AddStep("Hold Note", () => addHoldNote());
            AddStep("Hold Note (short)", () => addHoldNote(100));
            AddStep("BarLine", () => addBarLine(false));
            AddStep("BarLine (major)", () => addBarLine(true));
            AddStep("Add centre w/ bar line", () =>
            {
                addUpperNote();
                addBarLine(true);
            });
            AddStep("Height test 1", () => changePlayfieldSize(1));
            AddStep("Height test 2", () => changePlayfieldSize(2));
            AddStep("Height test 3", () => changePlayfieldSize(3));
            AddStep("Height test 4", () => changePlayfieldSize(4));
            AddStep("Height test 5", () => changePlayfieldSize(5));
            AddStep("Reset height", () => changePlayfieldSize(6));
            
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
            
            Add(playfieldContainer = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativeSizeAxes = Axes.X,
                Height = 1000,
                Children = new[] { drawableRuleset = new DrawableTachyonRuleset(new TachyonRuleset(), beatmap.GetPlayableBeatmap(new TachyonRuleset().RulesetInfo)) }
            });
        }
        
        private void changePlayfieldSize(int step)
        {
            double delay = 0;

            // Add new hits
            switch (step)
            {
                case 1:
                    addUpperNote();
                    break;

                case 2:
                    addUpperNote();
                    break;

                case 3:
                    addHoldNote();
                    break;

                case 4:
                    addHoldNote();
                    break;

                case 5:
                    addLowerNote();
                    delay = scroll_time - 100;
                    break;
            }

            switch (step)
            {
                default:
                    playfieldContainer.Delay(delay).ResizeTo(new Vector2(1, rng.Next(25, 400)), 500);
                    break;

                case 6:
                    playfieldContainer.Delay(delay).ResizeTo(new Vector2(1, Row.DEFAULT_HEIGHT), 500);
                    break;
            }
        }
        
        private void addUpperNote()
        {
            Note note = new Note
            {
                StartTime = drawableRuleset.Playfield.Time.Current + scroll_time,
            };

            note.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty());

            drawableRuleset.Playfield.Add(new DrawableUpperNote(note));
        }

        private void addLowerNote()
        {
            Note note = new Note
            {
                StartTime = drawableRuleset.Playfield.Time.Current + scroll_time,
            };

            note.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty());

            drawableRuleset.Playfield.Add(new DrawableLowerNote(note));
        }
        
        private void addHoldNote(double duration = default_duration)
        {
            addBarLine(true);
            addBarLine(true, scroll_time + duration);

            var holdNote = new HoldNote
            {
                StartTime = drawableRuleset.Playfield.Time.Current + scroll_time,
                Duration = duration,
            };

            holdNote.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty());

            drawableRuleset.Playfield.Add(new DrawableHoldNote(holdNote));
        }
        
        private void addBarLine(bool major, double delay = scroll_time)
        {
            BarLine bl = new BarLine { StartTime = drawableRuleset.Playfield.Time.Current + delay };

            drawableRuleset.Playfield.Add(major ? new DrawableBarLineMajor(bl) : new DrawableBarLine(bl));
        }
    }
}
