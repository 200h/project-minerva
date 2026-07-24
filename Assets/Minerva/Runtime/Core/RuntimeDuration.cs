using System;

namespace Minerva.Core
{
    /// <summary>
    /// Represents an immutable nonnegative elapsed interval in exact milliseconds.
    /// It has no wall-clock, calendar, or time-zone meaning.
    /// </summary>
    public struct RuntimeDuration :
        IEquatable<RuntimeDuration>,
        IComparable<RuntimeDuration>
    {
        /// <summary>
        /// Represents an interval of zero milliseconds.
        /// </summary>
        public static readonly RuntimeDuration Zero =
            new RuntimeDuration(0);

        private readonly long _milliseconds;

        /// <summary>
        /// Creates an elapsed interval from a nonnegative millisecond value.
        /// </summary>
        /// <param name="milliseconds">The exact interval in milliseconds.</param>
        public RuntimeDuration(long milliseconds)
        {
            if (milliseconds < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "milliseconds",
                    "A runtime duration cannot be negative.");
            }

            _milliseconds = milliseconds;
        }

        /// <summary>
        /// Gets the exact number of milliseconds in this interval.
        /// </summary>
        public long Milliseconds
        {
            get { return _milliseconds; }
        }

        /// <summary>
        /// Compares this duration with another elapsed runtime duration.
        /// </summary>
        public int CompareTo(RuntimeDuration other)
        {
            return _milliseconds.CompareTo(other._milliseconds);
        }

        /// <summary>
        /// Determines whether this duration has the same millisecond value as another.
        /// </summary>
        public bool Equals(RuntimeDuration other)
        {
            return _milliseconds == other._milliseconds;
        }

        /// <summary>
        /// Determines whether this duration has the same millisecond value as an object.
        /// </summary>
        public override bool Equals(object value)
        {
            return value is RuntimeDuration &&
                Equals((RuntimeDuration)value);
        }

        /// <summary>
        /// Returns a hash code derived from the exact millisecond value.
        /// </summary>
        public override int GetHashCode()
        {
            return _milliseconds.GetHashCode();
        }

        /// <summary>
        /// Determines whether two durations have equal millisecond values.
        /// </summary>
        public static bool operator ==(
            RuntimeDuration left,
            RuntimeDuration right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two durations have different millisecond values.
        /// </summary>
        public static bool operator !=(
            RuntimeDuration left,
            RuntimeDuration right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether one duration is shorter than another.
        /// </summary>
        public static bool operator <(
            RuntimeDuration left,
            RuntimeDuration right)
        {
            return left._milliseconds < right._milliseconds;
        }

        /// <summary>
        /// Determines whether one duration is shorter than or equal to another.
        /// </summary>
        public static bool operator <=(
            RuntimeDuration left,
            RuntimeDuration right)
        {
            return left._milliseconds <= right._milliseconds;
        }

        /// <summary>
        /// Determines whether one duration is longer than another.
        /// </summary>
        public static bool operator >(
            RuntimeDuration left,
            RuntimeDuration right)
        {
            return left._milliseconds > right._milliseconds;
        }

        /// <summary>
        /// Determines whether one duration is longer than or equal to another.
        /// </summary>
        public static bool operator >=(
            RuntimeDuration left,
            RuntimeDuration right)
        {
            return left._milliseconds >= right._milliseconds;
        }
    }
}
