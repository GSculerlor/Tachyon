using osu.Framework.Bindables;
using Tachyon.Game.Beatmaps.Timing;

namespace Tachyon.Game.Beatmaps.ControlPoints
{
    public class TimingControlPoint : ControlPoint
    {
        /// <summary>
        /// The time signature at this control point.
        /// </summary>
        public readonly Bindable<TimeSignatures> TimeSignatureBindable = new Bindable<TimeSignatures>(TimeSignatures.SimpleQuadruple) { Default = TimeSignatures.SimpleQuadruple };

        /// <summary>
        /// The time signature at this control point.
        /// </summary>
        public TimeSignatures TimeSignature
        {
            get => TimeSignatureBindable.Value;
            set => TimeSignatureBindable.Value = value;
        }

        public const double DEFAULT_BEAT_LENGTH = 1000;

        /// <summary>
        /// The beat length at this control point.
        /// </summary>
        public readonly BindableDouble BeatLengthBindable = new BindableDouble(DEFAULT_BEAT_LENGTH)
        {
            Default = DEFAULT_BEAT_LENGTH,
            MinValue = 6,
            MaxValue = 60000
        };

        /// <summary>
        /// The beat length at this control point.
        /// </summary>
        public double BeatLength
        {
            get => BeatLengthBindable.Value;
            set => BeatLengthBindable.Value = value;
        }

        /// <summary>
        /// The BPM at this control point.
        /// </summary>
        public double BPM => 60000 / BeatLength;

        public override bool EquivalentTo(ControlPoint other) =>
            other is TimingControlPoint otherTyped
            && TimeSignature == otherTyped.TimeSignature && BeatLength.Equals(otherTyped.BeatLength);
    }
}