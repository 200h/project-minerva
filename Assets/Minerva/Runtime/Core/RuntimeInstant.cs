using System;

namespace Minerva.Core
{
    /// <summary>
    /// Represents an immutable elapsed runtime timestamp in nonnegative milliseconds.
    /// It has no wall-clock, calendar, or time-zone meaning.
    /// </summary>
    public struct RuntimeInstant :
        IEquatable<RuntimeInstant>,
        IComparable<RuntimeInstant>
    {
        /// <summary>
        /// Represents the zero point of an owning runtime session.
        /// </summary>
        public static readonly RuntimeInstant Zero =
            new RuntimeInstant(0);

        private readonly long _milliseconds;

        /// <summary>
        /// Creates an elapsed runtime timestamp from a nonnegative millisecond value.
        /// </summary>
        /// <param name="milliseconds">
        /// Milliseconds elapsed from the owning runtime session's zero point.
        /// </param>
        public RuntimeInstant(long milliseconds)
        {
            if (milliseconds < 0)
            {
                throw new ArgumentOutOfRangeException(
                    "milliseconds",
                    "A runtime instant cannot be negative.");
            }

            _milliseconds = milliseconds;
        }

        /// <summary>
        /// Gets the exact number of elapsed runtime milliseconds.
        /// </summary>
        public long Milliseconds
        {
            get { return _milliseconds; }
        }

        /// <summary>
        /// Compares this instant with another elapsed runtime instant.
        /// </summary>
        public int CompareTo(RuntimeInstant other)
        {
            return _milliseconds.CompareTo(other._milliseconds);
        }

        /// <summary>
        /// Determines whether this instant has the same millisecond value as another.
        /// </summary>
        public bool Equals(RuntimeInstant other)
        {
            return _milliseconds == other._milliseconds;
        }

        /// <summary>
        /// Determines whether this instant has the same millisecond value as an object.
        /// </summary>
        public override bool Equals(object value)
        {
            return value is RuntimeInstant &&
                Equals((RuntimeInstant)value);
        }

        /// <summary>
        /// Returns a hash code derived from the exact millisecond value.
        /// </summary>
        public override int GetHashCode()
        {
            return _milliseconds.GetHashCode();
        }

        /// <summary>
        /// Determines whether two instants have equal millisecond values.
        /// </summary>
        public static bool operator ==(
            RuntimeInstant left,
            RuntimeInstant right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two instants have different millisecond values.
        /// </summary>
        public static bool operator !=(
            RuntimeInstant left,
            RuntimeInstant right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether one instant occurs before another.
        /// </summary>
        public static bool operator <(
            RuntimeInstant left,
            RuntimeInstant right)
        {
            return left._milliseconds < right._milliseconds;
        }

        /// <summary>
        /// Determines whether one instant occurs at or before another.
        /// </summary>
        public static bool operator <=(
            RuntimeInstant left,
            RuntimeInstant right)
        {
            return left._milliseconds <= right._milliseconds;
        }

        /// <summary>
        /// Determines whether one instant occurs after another.
        /// </summary>
        public static bool operator >(
            RuntimeInstant left,
            RuntimeInstant right)
        {
            return left._milliseconds > right._milliseconds;
        }

        /// <summary>
        /// Determines whether one instant occurs at or after another.
        /// </summary>
        public static bool operator >=(
            RuntimeInstant left,
            RuntimeInstant right)
        {
            return left._milliseconds >= right._milliseconds;
        }
    }
}
