using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Graphics;

namespace Tachyon.Game.Screens.Generate.Components
{
    public class TimelineTickDisplay : TimelinePart
    {
        [Resolved]
        private EditorBeatmap beatmap { get; set; }

        [Resolved]
        private Bindable<WorkingBeatmap> working { get; set; }

        [Resolved]
        private BindableBeatDivisor beatDivisor { get; set; }

        [Resolved]
        private TachyonColor colors { get; set; }

        public TimelineTickDisplay()
        {
            RelativeSizeAxes = Axes.Both;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            beatDivisor.BindValueChanged(_ => createLines(), true);
        }

        private void createLines()
        {
            Clear();

            for (var i = 0; i < beatmap.ControlPointInfo.TimingPoints.Count; i++)
            {
                var point = beatmap.ControlPointInfo.TimingPoints[i];
                var until = i + 1 < beatmap.ControlPointInfo.TimingPoints.Count ? beatmap.ControlPointInfo.TimingPoints[i + 1].Time : working.Value.Track.Length;

                int beat = 0;

                for (double t = point.Time; t < until; t += point.BeatLength / beatDivisor.Value)
                {
                    var indexInBeat = beat % beatDivisor.Value;

                    if (indexInBeat == 0)
                    {
                        Add(new PointVisualisation(t)
                        {
                            Colour = BindableBeatDivisor.GetColourFor(1, colors),
                            Origin = Anchor.TopCentre,
                        });
                    }
                    else
                    {
                        var divisor = BindableBeatDivisor.GetDivisorForBeatIndex(beat, beatDivisor.Value);
                        var colour = BindableBeatDivisor.GetColourFor(divisor, colors);
                        var height = 0.1f - (float)divisor / BindableBeatDivisor.VALID_DIVISORS.Last() * 0.08f;

                        Add(new PointVisualisation(t)
                        {
                            Colour = colour,
                            Height = height,
                            Origin = Anchor.TopCentre,
                        });

                        Add(new PointVisualisation(t)
                        {
                            Colour = colour,
                            Anchor = Anchor.BottomLeft,
                            Origin = Anchor.BottomCentre,
                            Height = height,
                        });
                    }

                    beat++;
                }
            }
        }
    }
}