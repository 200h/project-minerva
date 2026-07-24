namespace Minerva.Core
{
    /// <summary>
    /// Exposes explicit mutation controls for an elapsed runtime clock.
    /// </summary>
    public interface IRuntimeClockControl
    {
        /// <summary>
        /// Pauses explicit time advancement without changing current time.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes explicit time advancement without changing current time.
        /// </summary>
        void Resume();

        /// <summary>
        /// Advances current time by an exact nonnegative duration when not paused.
        /// </summary>
        /// <param name="duration">The exact elapsed duration to advance.</param>
        void Advance(RuntimeDuration duration);
    }
}
