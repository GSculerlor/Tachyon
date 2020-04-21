using osu.Framework.Allocation;
using osu.Framework.Input;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Rulesets.Objects;
using Tachyon.Game.Rulesets.Objects.Drawables;

namespace Tachyon.Game.Rulesets.UI.Scrolling
{
    public class DrawableTachyonRuleset : DrawableScrollingRuleset<TachyonHitObject>
    {
        protected override bool UserScrollSpeedAdjustment => false;

        public DrawableTachyonRuleset(Ruleset ruleset, IBeatmap beatmap)
            : base(ruleset, beatmap)
        {
            TimeRange.Value = 7000;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            new BarLineGenerator<BarLine>(Beatmap).BarLines.ForEach(bar => Playfield.Add(bar.Major ? new DrawableBarLineMajor(bar) : new DrawableBarLine(bar)));
        }

        public override PlayfieldAdjustmentContainer CreatePlayfieldAdjustmentContainer() => new TachyonPlayfieldAdjustmentContainer();

        protected override PassThroughInputManager CreateInputManager() => new TachyonInputManager(Ruleset.RulesetInfo);

        protected override Playfield CreatePlayfield() => new TachyonPlayfield(Beatmap.ControlPointInfo);

        public override DrawableHitObject<TachyonHitObject> CreateDrawableRepresentation(TachyonHitObject h)
        {
            switch (h)
            {
                case Note note:
                    if (note.Type == NoteType.Upper)
                        return new DrawableUpperNote(note);
                    else
                        return new DrawableLowerNote(note);
                
                case HoldNote hold:
                    return new DrawableHoldNote(hold);
            }

            return null;
        }
    }
}
