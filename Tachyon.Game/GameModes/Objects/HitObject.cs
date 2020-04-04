using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using osu.Framework.Bindables;
using Tachyon.Game.Audio;
using Tachyon.Game.Beatmaps;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.GameModes.Judgements;
using Tachyon.Game.GameModes.Objects.Types;
using Tachyon.Game.GameModes.Scoring;

namespace Tachyon.Game.GameModes.Objects
{
    public class HitObject
    {
        private const double control_point_leniency = 1;

        public event Action DefaultsApplied;

        public readonly Bindable<double> StartTimeBindable = new BindableDouble();

        public virtual double StartTime
        {
            get => StartTimeBindable.Value;
            set => StartTimeBindable.Value = value;
        }

        public readonly BindableList<HitSampleInfo> SamplesBindable = new BindableList<HitSampleInfo>();

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

        [JsonIgnore]
        public bool Kiai { get; private set; }

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

        public void ApplyDefaults(ControlPointInfo controlPointInfo, BeatmapDifficulty difficulty)
        {
            ApplyDefaultsToSelf(controlPointInfo, difficulty);

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

        public virtual Judgement CreateJudgement() => new Judgement();

        protected virtual HitWindows CreateHitWindows() => new HitWindows();
    }

    public static class HitObjectExtensions
    {
        public static double GetEndTime(this HitObject hitObject) => (hitObject as IHasEndTime)?.EndTime ?? hitObject.StartTime;
    }
}
