﻿using System.ComponentModel;

namespace Tachyon.Game.Rulesets.Scoring
{
    public enum HitResult
    {
        [Description(@"")]
        None,

        [Description(@"Miss")]
        Miss,

        [Description(@"Good")]
        Good,

        [Description(@"Perfect")]
        Perfect,
    }
}