namespace Tachyon.Game.Beatmaps
{
    public interface IBeatmapConverter
    {
        IBeatmap Beatmap { get; }
        
        IBeatmap Convert();
    }
}