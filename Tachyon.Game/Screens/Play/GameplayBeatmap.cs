using System.Collections.Generic;
using osu.Framework.Graphics;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.Rulesets.Objects;

namespace Tachyon.Game.Screens.Play
{
    public class GameplayBeatmap : Component, IBeatmap
    {
        public readonly IBeatmap PlayableBeatmap;

        public GameplayBeatmap(IBeatmap playableBeatmap)
        {
            PlayableBeatmap = playableBeatmap;
        }

        public BeatmapInfo BeatmapInfo
        {
            get => PlayableBeatmap.BeatmapInfo;
            set => PlayableBeatmap.BeatmapInfo = value;
        }

        public BeatmapMetadata Metadata => PlayableBeatmap.Metadata;

        public ControlPointInfo ControlPointInfo => PlayableBeatmap.ControlPointInfo;

        public IReadOnlyList<HitObject> HitObjects => PlayableBeatmap.HitObjects;

        public IBeatmap Clone() => PlayableBeatmap.Clone();
    }
}
