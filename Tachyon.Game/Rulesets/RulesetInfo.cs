﻿using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Tachyon.Game.Rulesets
{
    public class RulesetInfo : IEquatable<RulesetInfo>
    {
        public int? ID { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string InstantiationInfo { get; set; }

        [JsonIgnore]
        public bool Available { get; set; }
        
        public virtual Ruleset CreateInstance()
        {
            if (!Available) return null;

            var ruleset = (Ruleset)Activator.CreateInstance(Type.GetType(InstantiationInfo));

            ruleset.RulesetInfo = this;

            return ruleset;
        }
        
        public bool Equals(RulesetInfo other) => other != null && ID == other.ID && Available == other.Available && Name == other.Name && InstantiationInfo == other.InstantiationInfo;

        public override bool Equals(object obj) => obj is RulesetInfo rulesetInfo && Equals(rulesetInfo);

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ID.HasValue ? ID.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (InstantiationInfo != null ? InstantiationInfo.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Available.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString() => Name ?? $"{Name} ({ShortName}) ID: {ID}";
    }
}
