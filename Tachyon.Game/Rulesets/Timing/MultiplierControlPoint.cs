﻿using System;
using Tachyon.Game.Beatmaps.ControlPoints;
using Tachyon.Game.IO.Serialization;

namespace Tachyon.Game.Rulesets.Timing
{
    public class MultiplierControlPoint : IJsonSerializable, IComparable<MultiplierControlPoint>
    {
        /// <summary>
        /// The time in milliseconds at which this <see cref="MultiplierControlPoint"/> starts.
        /// </summary>
        public double StartTime;

        /// <summary>
        /// The aggregate multiplier which this <see cref="MultiplierControlPoint"/> provides.
        /// </summary>
        public double Multiplier => Velocity * DifficultyPoint.SpeedMultiplier * BaseBeatLength / TimingPoint.BeatLength;

        /// <summary>
        /// The base beat length to scale the <see cref="TimingPoint"/> provided multiplier relative to.
        /// </summary>
        /// <example>For a <see cref="BaseBeatLength"/> of 1000, a <see cref="TimingPoint"/> with a beat length of 500 will increase the multiplier by 2.</example>
        public double BaseBeatLength = TimingControlPoint.DEFAULT_BEAT_LENGTH;

        /// <summary>
        /// The velocity multiplier.
        /// </summary>
        public double Velocity = 1;

        /// <summary>
        /// The <see cref="TimingControlPoint"/> that provides the timing information for this <see cref="MultiplierControlPoint"/>.
        /// </summary>
        public TimingControlPoint TimingPoint = new TimingControlPoint();

        /// <summary>
        /// The <see cref="DifficultyControlPoint"/> that provides additional difficulty information for this <see cref="MultiplierControlPoint"/>.
        /// </summary>
        public DifficultyControlPoint DifficultyPoint = new DifficultyControlPoint();

        /// <summary>
        /// Creates a <see cref="MultiplierControlPoint"/>. This is required for JSON serialization
        /// </summary>
        public MultiplierControlPoint()
        {
        }

        /// <summary>
        /// Creates a <see cref="MultiplierControlPoint"/>.
        /// </summary>
        /// <param name="startTime">The start time of this <see cref="MultiplierControlPoint"/>.</param>
        public MultiplierControlPoint(double startTime)
        {
            StartTime = startTime;
        }

        // ReSharper disable once ImpureMethodCallOnReadonlyValueField
        public int CompareTo(MultiplierControlPoint other) => StartTime.CompareTo(other?.StartTime);
    }
}