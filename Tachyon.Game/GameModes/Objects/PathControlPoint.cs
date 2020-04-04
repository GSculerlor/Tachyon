using System;
using osu.Framework.Bindables;
using osuTK;
using Tachyon.Game.GameModes.Objects.Types;

namespace Tachyon.Game.GameModes.Objects
{
    public class PathControlPoint : IEquatable<PathControlPoint>
    {
        public readonly Bindable<Vector2> Position = new Bindable<Vector2>();

        public readonly Bindable<PathType?> Type = new Bindable<PathType?>();

        internal event Action Changed;

        public PathControlPoint()
        {
            Position.ValueChanged += _ => Changed?.Invoke();
            Type.ValueChanged += _ => Changed?.Invoke();
        }

        public PathControlPoint(Vector2 position, PathType? type = null)
            : this()
        {
            Position.Value = position;
            Type.Value = type;
        }

        public bool Equals(PathControlPoint other) => Position.Value == other?.Position.Value && Type.Value == other.Type.Value;
    }
}