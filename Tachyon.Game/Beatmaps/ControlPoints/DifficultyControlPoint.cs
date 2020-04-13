using osu.Framework.Bindables;

namespace Tachyon.Game.Beatmaps.ControlPoints
{
    public class DifficultyControlPoint : ControlPoint
    {
        /// <summary>
        /// The speed multiplier at this control point.
        /// </summary>
        public readonly BindableDouble SpeedMultiplierBindable = new BindableDouble(1)
        {
            Precision = 0.1,
            Default = 1,
            MinValue = 0.1,
            MaxValue = 10
        };

        /// <summary>
        /// The speed multiplier at this control point.
        /// </summary>
        public double SpeedMultiplier
        {
            get => SpeedMultiplierBindable.Value;
            set => SpeedMultiplierBindable.Value = value;
        }

        public override bool EquivalentTo(ControlPoint other) =>
            other is DifficultyControlPoint otherTyped && otherTyped.SpeedMultiplier.Equals(SpeedMultiplier);
    }
}