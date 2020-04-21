﻿using Newtonsoft.Json;

 namespace Tachyon.Game.Rulesets.Objects.Types
{
    public interface IHasEndTime
    {
        [JsonIgnore]
        double EndTime { get; set; }

        double Duration { get; }
    }
}