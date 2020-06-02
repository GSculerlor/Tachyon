// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.ComponentModel;

namespace Tachyon.Game.Generator
{
    public enum DivisorValue
    {
        [Description("1/1")]
        One = 1,
        
        [Description("1/2")]
        Two = 2,
        
        [Description("1/3")]
        Three = 3,
        
        [Description("1/4")]
        Four = 4,
        
        [Description("1/6")]
        Six = 6,
        
        [Description("1/8")]
        Eight = 8
    }
}
