using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.Graphics;

namespace Tachyon.Game.Screens.Generate.Components.Parts
{
    /// <summary>
    /// The part of the timeline that displays the control points.
    /// </summary>
    public class ControlPointPart : TimelinePart
    {
        protected override void LoadBeatmap(WorkingBeatmap beatmap)
        {
            base.LoadBeatmap(beatmap);

            ControlPointInfo cpi = beatmap.Beatmap.ControlPointInfo;

            cpi.TimingPoints.ForEach(addTimingPoint);

            // Consider all non-timing points as the same type
            cpi.SamplePoints.Select(c => (ControlPoint)c)
               .Concat(cpi.EffectPoints)
               .Concat(cpi.DifficultyPoints)
               .Distinct()
               // Non-timing points should not be added where there are timing points
               .Where(c => cpi.TimingPointAt(c.Time).Time != c.Time)
               .ForEach(addNonTimingPoint);
        }

        private void addTimingPoint(ControlPoint controlPoint) => Add(new TimingPointVisualisation(controlPoint));
        private void addNonTimingPoint(ControlPoint controlPoint) => Add(new NonTimingPointVisualisation(controlPoint));

        private class TimingPointVisualisation : ControlPointVisualisation
        {
            public TimingPointVisualisation(ControlPoint controlPoint)
                : base(controlPoint)
            {
            }

            [BackgroundDependencyLoader]
            private void load(TachyonColor colors) => Colour = colors.YellowDark;
        }

        private class NonTimingPointVisualisation : ControlPointVisualisation
        {
            public NonTimingPointVisualisation(ControlPoint controlPoint)
                : base(controlPoint)
            {
            }

            [BackgroundDependencyLoader]
            private void load(TachyonColor colors) => Colour = colors.Green;
        }

        private abstract class ControlPointVisualisation : PointVisualisation
        {
            protected ControlPointVisualisation(ControlPoint controlPoint)
                : base(controlPoint.Time)
            {
            }
        }
    }
}
