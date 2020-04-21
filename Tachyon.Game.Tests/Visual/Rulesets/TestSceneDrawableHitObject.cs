using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.Graphics.Sprites;
using Tachyon.Game.Rulesets.Objects;
using Tachyon.Game.Rulesets.Objects.Drawables;

namespace Tachyon.Game.Tests.Visual.Rulesets
{
    [TestFixture]
    public class TestSceneDrawableHitObject : TachyonGridTestScene
    {
        public override IReadOnlyList<Type> RequiredTypes => base.RequiredTypes.Concat(new[]
        {
            typeof(DrawableNote),
            typeof(DrawableHoldNote)
        }).ToList();
        
        public TestSceneDrawableHitObject() : base(1, 3)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            AddStep("Create note", () =>
            {
                Cell(0).Add(new HitObjectContainer("Upper Note", new DrawableUpperNote(createNoteAtCurrentTime())
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                })
                {
                    RelativeSizeAxes = Axes.Both,
                });
                
                Cell(1).Add(new HitObjectContainer("Lower Note", new DrawableLowerNote(createNoteAtCurrentTime())
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                })
                {
                    RelativeSizeAxes = Axes.Both,
                });
                
                Cell(2).Add(new HitObjectContainer("Long Note/Slider", new DrawableHoldNote(createHoldNoteAtCurrentTime()){
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                })
                {
                    RelativeSizeAxes = Axes.Both,
                });
            });
        }

        public class HitObjectContainer : Container
        {
            public HitObjectContainer(string title, Drawable hitObject)
            {
                Add(new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    BorderColour = Color4.White,
                    BorderThickness = 5,
                    Masking = true,

                    Children = new Drawable[]
                    {
                        new Box
                        {
                            AlwaysPresent = true,
                            Alpha = 0,
                            RelativeSizeAxes = Axes.Both,
                        },
                        new TachyonSpriteText
                        {
                            Text = title,
                            Scale = new Vector2(1.5f),
                            Padding = new MarginPadding(5),
                        },
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Child = hitObject
                        },
                    }
                });
            }
        }
        
        private Note createNoteAtCurrentTime()
        {
            var note = new Note
            {
                StartTime = Time.Current + 3000,
            };

            note.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty());

            return note;
        }

        private HoldNote createHoldNoteAtCurrentTime()
        {
            var holdNote = new HoldNote
            {
                StartTime = Time.Current + 3000, 
                Duration = 5000
            };

            holdNote.ApplyDefaults(new ControlPointInfo(), new BeatmapDifficulty());

            return holdNote;
        }
    }
}