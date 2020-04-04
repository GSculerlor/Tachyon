using System.Collections.Generic;
using Tachyon.Game.GameModes.Objects;

namespace Tachyon.Game.Beatmaps
{
    public class BeatmapConverter : IBeatmapConverter
    {
        public IBeatmap Beatmap { get; }
        
        public BeatmapConverter(IBeatmap beatmap)
        {
            Beatmap = beatmap;
        }
        
        public IBeatmap Convert()
        {
            return convertBeatmap(Beatmap.Clone());
        }
        
        private Beatmap convertBeatmap(IBeatmap original)
        {
            var beatmap = new Beatmap
            {
                BeatmapInfo = original.BeatmapInfo,
                ControlPointInfo = original.ControlPointInfo,
                HitObjects = convertHitObjects(original.HitObjects, original)
            };

            return beatmap;
        }
        
        private List<HitObject> convertHitObjects(IReadOnlyList<HitObject> hitObjects, IBeatmap beatmap)
        {
            var result = new List<HitObject>(hitObjects.Count);
            result.AddRange(hitObjects);

            return result;
        }
    }
}