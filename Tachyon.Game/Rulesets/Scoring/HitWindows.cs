using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Tachyon.Game.Beatmaps;

namespace Tachyon.Game.Rulesets.Scoring
{
    public class HitWindows
    {
        private static readonly DifficultyRange[] tachyon_ranges =
        {
            new DifficultyRange(HitResult.Perfect, 50, 35, 20),
            new DifficultyRange(HitResult.Late, 120, 80, 50),
            new DifficultyRange(HitResult.Miss, 135, 95, 70),
        };
        
        public static HitWindows Empty => new EmptyHitWindows();
        
        private double perfect;
        private double late;
        private double miss;
        
        public HitWindows()
        {
            Debug.Assert(GetRanges().Any(r => r.Result == HitResult.Miss), $"{nameof(GetRanges)} should always contain {nameof(HitResult.Miss)}");
            Debug.Assert(GetRanges().Any(r => r.Result != HitResult.Miss), $"{nameof(GetRanges)} should always contain at least one result type other than {nameof(HitResult.Miss)}.");
        }
        
        protected HitResult LowestSuccessfulHitResult()
        {
            for (var result = HitResult.Late; result <= HitResult.Perfect; ++result)
            {
                if (IsHitResultAllowed(result))
                    return result;
            }

            return HitResult.None;
        }
        
        public IEnumerable<(HitResult result, double length)> GetAllAvailableWindows()
        {
            for (var result = HitResult.Late; result <= HitResult.Perfect; ++result)
            {
                if (IsHitResultAllowed(result))
                    yield return (result, WindowFor(result));
            }
        }
        
        public virtual bool IsHitResultAllowed(HitResult result)
        {
            switch (result)
            {
                case HitResult.Perfect:
                case HitResult.Late:
                case HitResult.Miss:
                    return true;
            }

            return false;
        }
        
        public void SetDifficulty(double difficulty)
        {
            foreach (var range in GetRanges())
            {
                var value = BeatmapDifficulty.DifficultyRange(difficulty, (range.Min, range.Average, range.Max));

                switch (range.Result)
                {
                    case HitResult.Miss:
                        miss = value;
                        break;
                    
                    case HitResult.Late:
                        late = value;
                        break;

                    case HitResult.Perfect:
                        perfect = value;
                        break;
                }
            }
        }
        
        public HitResult ResultFor(double timeOffset)
        {
            timeOffset = Math.Abs(timeOffset);

            for (var result = HitResult.Perfect; result >= HitResult.Miss; --result)
            {
                if (IsHitResultAllowed(result) && timeOffset <= WindowFor(result))
                    return result;
            }

            return HitResult.None;
        }
        
        public double WindowFor(HitResult result)
        {
            switch (result)
            {
                case HitResult.Perfect:
                    return perfect;

                case HitResult.Late:
                    return late;

                case HitResult.Miss:
                    return miss;

                default:
                    throw new ArgumentException("Unknown enum member", nameof(result));
            }
        }
        
        public bool CanBeHit(double timeOffset) => timeOffset <= WindowFor(LowestSuccessfulHitResult());
        
        protected virtual DifficultyRange[] GetRanges() => tachyon_ranges;
        
        public class EmptyHitWindows : HitWindows
        {
            private static readonly DifficultyRange[] ranges =
            {
                new DifficultyRange(HitResult.Perfect, 0, 0, 0),
                new DifficultyRange(HitResult.Miss, 0, 0, 0),
            };

            public override bool IsHitResultAllowed(HitResult result)
            {
                switch (result)
                {
                    case HitResult.Perfect:
                    case HitResult.Miss:
                        return true;
                }

                return false;
            }

            protected override DifficultyRange[] GetRanges() => ranges;
        }
    }
    
    public struct DifficultyRange
    {
        public readonly HitResult Result;

        public double Min;
        public double Average;
        public double Max;

        public DifficultyRange(HitResult result, double min, double average, double max)
        {
            Result = result;

            Min = min;
            Average = average;
            Max = max;
        }
    }
}