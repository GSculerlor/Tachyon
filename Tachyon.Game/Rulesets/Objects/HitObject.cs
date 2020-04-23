﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Framework.Bindables;
using Tachyon.Game.Audio;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.Rulesets.Judgements;
using Tachyon.Game.Rulesets.Objects.Types;
using Tachyon.Game.Rulesets.Scoring;

namespace Tachyon.Game.Rulesets.Objects
{
    /// <summary>
    /// A HitObject describes an object in a Beatmap.
    /// <para>
    /// HitObjects may contain more properties for which you should be checking through the IHas* types.
    /// </para>
    /// </summary>
    public class HitObject
    {
        /// <summary>
        /// A small adjustment to the start time of control points to account for rounding/precision errors.
        /// </summary>
        private const double control_point_leniency = 1;

        /// <summary>
        /// Invoked after <see cref="ApplyDefaults"/> has completed on this <see cref="HitObject"/>.
        /// </summary>
        public event Action DefaultsApplied;

        public readonly Bindable<double> StartTimeBindable = new BindableDouble();

        /// <summary>
        /// The time at which the HitObject starts.
        /// </summary>
        public virtual double StartTime
        {
            get => StartTimeBindable.Value;
            set => StartTimeBindable.Value = value;
        }

        public readonly BindableList<HitSampleInfo> SamplesBindable = new BindableList<HitSampleInfo>();

        /// <summary>
        /// The samples to be played when this hit object is hit.
        /// </summary>
        public IList<HitSampleInfo> Samples
        {
            get => SamplesBindable;
            set
            {
                SamplesBindable.Clear();
                SamplesBindable.AddRange(value);
            }
        }

        [JsonIgnore]
        public SampleControlPoint SampleControlPoint;

        /// <summary>
        /// Whether this <see cref="HitObject"/> is in Kiai time.
        /// </summary>
        [JsonIgnore]
        public bool Kiai { get; private set; }

        /// <summary>
        /// The hit windows for this <see cref="HitObject"/>.
        /// </summary>
        public HitWindows HitWindows { get; set; }

        private readonly List<HitObject> nestedHitObjects = new List<HitObject>();

        [JsonIgnore]
        public IReadOnlyList<HitObject> NestedHitObjects => nestedHitObjects;

        public HitObject()
        {
            StartTimeBindable.ValueChanged += time =>
            {
                double offset = time.NewValue - time.OldValue;

                foreach (var nested in NestedHitObjects)
                    nested.StartTime += offset;
            };
        }

        /// <summary>
        /// Applies default values to this HitObject.
        /// </summary>
        /// <param name="controlPointInfo">The control points.</param>
        /// <param name="difficulty">The difficulty settings to use.</param>
        public void ApplyDefaults(ControlPointInfo controlPointInfo, BeatmapDifficulty difficulty)
        {
            ApplyDefaultsToSelf(controlPointInfo, difficulty);

            // This is done here since ApplyDefaultsToSelf may be used to determine the end time
            SampleControlPoint = controlPointInfo.SamplePointAt(this.GetEndTime() + control_point_leniency);

            nestedHitObjects.Clear();

            CreateNestedHitObjects();

            nestedHitObjects.Sort((h1, h2) => h1.StartTime.CompareTo(h2.StartTime));

            foreach (var h in nestedHitObjects)
                h.ApplyDefaults(controlPointInfo, difficulty);

            DefaultsApplied?.Invoke();
        }

        protected virtual void ApplyDefaultsToSelf(ControlPointInfo controlPointInfo, BeatmapDifficulty difficulty)
        {
            Kiai = controlPointInfo.EffectPointAt(StartTime + control_point_leniency).KiaiMode;

            if (HitWindows == null)
                HitWindows = CreateHitWindows();
            HitWindows?.SetDifficulty(difficulty.OverallDifficulty);
        }

        protected virtual void CreateNestedHitObjects()
        {
        }

        protected void AddNested(HitObject hitObject) => nestedHitObjects.Add(hitObject);

        /// <summary>
        /// Creates the <see cref="Judgement"/> that represents the scoring information for this <see cref="HitObject"/>.
        /// </summary>
        [NotNull]
        public virtual Judgement CreateJudgement() => new Judgement();

        /// <summary>
        /// Creates the <see cref="HitWindows"/> for this <see cref="HitObject"/>.
        /// This can be null to indicate that the <see cref="HitObject"/> has no <see cref="HitWindows"/> and timing errors should not be displayed to the user.
        /// <para>
        /// This will only be invoked if <see cref="HitWindows"/> hasn't been set externally (e.g. from a <see cref="BeatmapConverter"/>.
        /// </para>
        /// </summary>
        [NotNull]
        protected virtual HitWindows CreateHitWindows() => new HitWindows();
    }

    public static class HitObjectExtensions
    {
        /// <summary>
        /// Returns the end time of this object.
        /// </summary>
        /// <remarks>
        /// This returns the <see cref="IHasEndTime.EndTime"/> where available, falling back to <see cref="HitObject.StartTime"/> otherwise.
        /// </remarks>
        /// <param name="hitObject">The object.</param>
        /// <returns>The end time of this object.</returns>
        public static double GetEndTime(this HitObject hitObject) => (hitObject as IHasEndTime)?.EndTime ?? hitObject.StartTime;
    }
}