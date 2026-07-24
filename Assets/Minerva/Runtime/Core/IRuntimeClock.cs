namespace Minerva.Core
{
    /// <summary>
    /// Exposes read-only state for an authoritative elapsed runtime clock.
    /// </summary>
    public interface IRuntimeClock
    {
        /// <summary>
        /// Gets the clock's current elapsed runtime timestamp.
        /// </summary>
        RuntimeInstant CurrentTime { get; }

        /// <summary>
        /// Gets a value indicating whether explicit advancement is paused.
        /// </summary>
        bool IsPaused { get; }
    }
}
