using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Rulesets.Objects.Drawables;
using Tachyon.Game.Rulesets.UI;

namespace Tachyon.Game.Screens.Play
{
    /// <summary>
    /// Manage the animation to be applied when a player fails.
    /// Single file; automatically disposed after use.
    /// </summary>
    public class FailAnimation : Component
    {
        public Action OnComplete;

        private readonly DrawableRuleset drawableRuleset;

        private readonly BindableDouble trackFreq = new BindableDouble(1);

        private Track track;

        private const float duration = 2500;

        public FailAnimation(DrawableRuleset drawableRuleset)
        {
            this.drawableRuleset = drawableRuleset;
        }

        [BackgroundDependencyLoader]
        private void load(AudioManager audio, IBindable<WorkingBeatmap> beatmap)
        {
            track = beatmap.Value.Track;
        }

        private bool started;

        /// <summary>
        /// Start the fail animation playing.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if started more than once.</exception>
        public void Start()
        {
            if (started) throw new InvalidOperationException("Animation cannot be started more than once.");

            started = true;

            this.TransformBindableTo(trackFreq, 0, duration).OnComplete(_ =>
            {
                OnComplete?.Invoke();
                Expire();
            });

            track.AddAdjustment(AdjustableProperty.Frequency, trackFreq);

            applyToPlayfield(drawableRuleset.Playfield);
            drawableRuleset.Playfield.HitObjectContainer.FlashColour(Color4.Red, 500);
            drawableRuleset.Playfield.HitObjectContainer.FadeOut(duration / 2);
        }

        protected override void Update()
        {
            base.Update();

            if (!started)
                return;

            applyToPlayfield(drawableRuleset.Playfield);
        }

        private readonly List<DrawableHitObject> appliedObjects = new List<DrawableHitObject>();

        private void applyToPlayfield(Playfield playfield)
        {
            foreach (var nested in playfield.NestedPlayfields)
                applyToPlayfield(nested);

            foreach (DrawableHitObject obj in playfield.HitObjectContainer.AliveObjects)
            {
                if (appliedObjects.Contains(obj))
                    continue;

                obj.RotateTo(RNG.NextSingle(-90, 90), duration);
                obj.ScaleTo(obj.Scale * 0.5f, duration);
                obj.MoveToOffset(new Vector2(0, 400), duration);
                appliedObjects.Add(obj);
            }
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            track?.RemoveAdjustment(AdjustableProperty.Frequency, trackFreq);
        }
    }
}
