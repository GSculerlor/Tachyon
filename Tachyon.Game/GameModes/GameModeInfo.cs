using System;
using System.Diagnostics.CodeAnalysis;

namespace Tachyon.Game.GameModes
{
    public class GameModeInfo : IEquatable<GameModeInfo>
    {
        public int? ID { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string InstantiationInfo { get; set; }
        
        public virtual GameMode CreateInstance()
        {
            var gameMode = (GameMode)Activator.CreateInstance(Type.GetType(InstantiationInfo));

            gameMode.GameModeInfo = this;

            return gameMode;
        }
        
        public bool Equals(GameModeInfo other) => other != null && ID == other.ID && Name == other.Name && InstantiationInfo == other.InstantiationInfo;

        public override bool Equals(object obj) => obj is GameModeInfo gameModeInfo && Equals(gameModeInfo);

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ID.HasValue ? ID.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (InstantiationInfo != null ? InstantiationInfo.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString() => Name ?? $"{Name} ({ShortName}) ID: {ID}";
    }
}