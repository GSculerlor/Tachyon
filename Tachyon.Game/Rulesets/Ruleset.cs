using System;
using System.Collections.Generic;
using osu.Framework.Input.Bindings;
using osu.Framework.IO.Stores;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Rulesets.Scoring;
using Tachyon.Game.Rulesets.UI;

namespace Tachyon.Game.Rulesets
{
    public abstract class Ruleset
    {
        public RulesetInfo RulesetInfo { get; internal set; }

        protected Ruleset()
        {
            RulesetInfo = new RulesetInfo
            {
                Name = Description,
                ShortName = ShortName,
                ID = RulesetID,
                InstantiationInfo = GetType().AssemblyQualifiedName,
                Available = true,
            };
        }

        /// <summary>
        /// Attempt to create a hit renderer for a beatmap
        /// </summary>
        /// <param name="beatmap">The beatmap to create the hit renderer for.</param>
        /// <exception cref="BeatmapInvalidForRulesetException">Unable to successfully load the beatmap to be usable with this ruleset.</exception>
        /// <returns></returns>
        public abstract DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap);

        /// <summary>
        /// Creates a <see cref="ScoreProcessor"/> for this <see cref="Ruleset"/>.
        /// </summary>
        /// <returns>The score processor.</returns>
        public virtual ScoreProcessor CreateScoreProcessor() => new ScoreProcessor();

        /// <summary>
        /// Creates a <see cref="HealthProcessor"/> for this <see cref="Ruleset"/>.
        /// </summary>
        /// <returns>The health processor.</returns>
        public HealthProcessor CreateHealthProcessor(double drainStartTime) => new DrainingHealthProcessor(drainStartTime);
        
        //TODO: Implement all of this components
        /* public virtual IBeatmapProcessor CreateBeatmapProcessor(IBeatmap beatmap) => null;

        public abstract DifficultyCalculator CreateDifficultyCalculator(WorkingBeatmap beatmap);
        */
        
        /// <summary>
        /// Creates a <see cref="IBeatmapConverter"/> to convert a <see cref="IBeatmap"/> to one that is applicable for this <see cref="Ruleset"/>.
        /// </summary>
        /// <param name="beatmap">The <see cref="IBeatmap"/> to be converted.</param>
        /// <returns>The <see cref="IBeatmapConverter"/>.</returns>
        public abstract IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap);

        public virtual IResourceStore<byte[]> CreateResourceStore() => new NamespacedResourceStore<byte[]>(new DllResourceStore(GetType().Assembly), @"Resources");

        public abstract int RulesetID { get; }
        
        public abstract string Description { get; }

        /// <summary>
        /// A unique short name to reference this ruleset in online requests.
        /// </summary>
        public abstract string ShortName { get; }

        /// <summary>
        /// Get a list of default keys for the specified variant.
        /// </summary>
        /// <param name="variant">A variant.</param>
        /// <returns>A list of valid <see cref="KeyBinding"/>s.</returns>
        public virtual IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => Array.Empty<KeyBinding>();
    }
}
