using System;
using Tachyon.Game.Beatmaps.ControlPoints;

namespace Tachyon.Game.Beatmaps.ControlPoints
{
    public abstract class ControlPoint : IComparable<ControlPoint>, IEquatable<ControlPoint>
    {
        /// <summary>
        /// The time at which the control point takes effect.
        /// </summary>
        public double Time => controlPointGroup?.Time ?? 0;

        private ControlPointGroup controlPointGroup;

        public void AttachGroup(ControlPointGroup pointGroup) => controlPointGroup = pointGroup;

        public int CompareTo(ControlPoint other) => Time.CompareTo(other.Time);

        /// <summary>
        /// Whether this control point is equivalent to another, ignoring time.
        /// </summary>
        /// <param name="other">Another control point to compare with.</param>
        /// <returns>Whether equivalent.</returns>
        public abstract bool EquivalentTo(ControlPoint other);

        public bool Equals(ControlPoint other) => Time == other?.Time && EquivalentTo(other);
    }
}
