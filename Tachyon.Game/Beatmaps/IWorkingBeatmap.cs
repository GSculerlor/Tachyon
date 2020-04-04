using System;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics.Textures;

namespace Tachyon.Game.Beatmaps
{
    public interface IWorkingBeatmap
    {
        IBeatmap Beatmap { get; }
        
        IBeatmap GetPlayableBeatmap(TimeSpan? timeout = null);
        
        Texture Background { get; }
        
        Track Track { get; }
        
        Waveform Waveform { get; }
    }
}