﻿using System;
using osu.Framework.Extensions.TypeExtensions;
using osu.Framework.Graphics;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Rulesets.Judgements;
using Tachyon.Game.Rulesets.Objects;

namespace Tachyon.Game.Rulesets.Scoring
{
    public abstract class JudgementProcessor : Component
    {
        /// <summary>
        /// Invoked when all <see cref="HitObject"/>s have been judged by this <see cref="JudgementProcessor"/>.
        /// </summary>
        public event Action AllJudged;

        /// <summary>
        /// Invoked when a new judgement has occurred. This occurs after the judgement has been processed by this <see cref="JudgementProcessor"/>.
        /// </summary>
        public event Action<JudgementResult> NewJudgement;

        /// <summary>
        /// The maximum number of hits that can be judged.
        /// </summary>
        protected int MaxHits { get; private set; }

        /// <summary>
        /// The total number of judged <see cref="HitObject"/>s at the current point in time.
        /// </summary>
        public int JudgedHits { get; private set; }

        /// <summary>
        /// Whether all <see cref="Judgement"/>s have been processed.
        /// </summary>
        public bool HasCompleted => JudgedHits == MaxHits;

        /// <summary>
        /// Applies a <see cref="IBeatmap"/> to this <see cref="ScoreProcessor"/>.
        /// </summary>
        /// <param name="beatmap">The <see cref="IBeatmap"/> to read properties from.</param>
        public virtual void ApplyBeatmap(IBeatmap beatmap)
        {
            Reset(false);
            SimulateAutoplay(beatmap);
            Reset(true);
        }

        /// <summary>
        /// Applies the score change of a <see cref="JudgementResult"/> to this <see cref="ScoreProcessor"/>.
        /// </summary>
        /// <param name="result">The <see cref="JudgementResult"/> to apply.</param>
        public void ApplyResult(JudgementResult result)
        {
            JudgedHits++;

            ApplyResultInternal(result);

            NewJudgement?.Invoke(result);

            if (HasCompleted)
                AllJudged?.Invoke();
        }

        /// <summary>
        /// Reverts the score change of a <see cref="JudgementResult"/> that was applied to this <see cref="ScoreProcessor"/>.
        /// </summary>
        /// <param name="result">The judgement scoring result.</param>
        public void RevertResult(JudgementResult result)
        {
            JudgedHits--;

            RevertResultInternal(result);
        }

        /// <summary>
        /// Applies the score change of a <see cref="JudgementResult"/> to this <see cref="ScoreProcessor"/>.
        /// </summary>
        /// <remarks>
        /// Any changes applied via this method can be reverted via <see cref="RevertResultInternal"/>.
        /// </remarks>
        /// <param name="result">The <see cref="JudgementResult"/> to apply.</param>
        protected abstract void ApplyResultInternal(JudgementResult result);

        /// <summary>
        /// Reverts the score change of a <see cref="JudgementResult"/> that was applied to this <see cref="ScoreProcessor"/> via <see cref="ApplyResultInternal"/>.
        /// </summary>
        /// <param name="result">The judgement scoring result.</param>
        protected abstract void RevertResultInternal(JudgementResult result);

        /// <summary>
        /// Resets this <see cref="JudgementProcessor"/> to a default state.
        /// </summary>
        /// <param name="storeResults">Whether to store the current state of the <see cref="JudgementProcessor"/> for future use.</param>
        protected virtual void Reset(bool storeResults)
        {
            if (storeResults)
                MaxHits = JudgedHits;

            JudgedHits = 0;
        }

        /// <summary>
        /// Creates the <see cref="JudgementResult"/> that represents the scoring result for a <see cref="HitObject"/>.
        /// </summary>
        /// <param name="hitObject">The <see cref="HitObject"/> which was judged.</param>
        /// <param name="judgement">The <see cref="Judgement"/> that provides the scoring information.</param>
        protected virtual JudgementResult CreateResult(HitObject hitObject, Judgement judgement) => new JudgementResult(hitObject, judgement);

        /// <summary>
        /// Simulates an autoplay of the <see cref="IBeatmap"/> to determine scoring values.
        /// </summary>
        /// <remarks>This provided temporarily. DO NOT USE.</remarks>
        /// <param name="beatmap">The <see cref="IBeatmap"/> to simulate.</param>
        protected virtual void SimulateAutoplay(IBeatmap beatmap)
        {
            foreach (var obj in beatmap.HitObjects)
                simulate(obj);

            void simulate(HitObject obj)
            {
                foreach (var nested in obj.NestedHitObjects)
                    simulate(nested);

                var judgement = obj.CreateJudgement();

                var result = CreateResult(obj, judgement);
                if (result == null)
                    throw new InvalidOperationException($"{GetType().ReadableName()} must provide a {nameof(JudgementResult)} through {nameof(CreateResult)}.");

                result.Type = judgement.MaxResult;
                ApplyResult(result);
            }
        }
    }
}