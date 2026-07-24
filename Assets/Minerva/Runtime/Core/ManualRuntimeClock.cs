using System;

namespace Minerva.Core
{
    /// <summary>
    /// Provides an instance-owned elapsed runtime clock that changes only through
    /// explicit successful advancement and participates in runtime service ownership.
    /// </summary>
    public sealed class ManualRuntimeClock :
        IRuntimeClock,
        IRuntimeClockControl,
        IRuntimeService
    {
        private RuntimeInstant _currentTime;
        private bool _isPaused;
        private bool _isShutDown;

        /// <summary>
        /// Creates a running manual clock at the runtime session's zero point.
        /// </summary>
        public ManualRuntimeClock()
            : this(RuntimeInstant.Zero)
        {
        }

        /// <summary>
        /// Creates a running manual clock at an exact initial elapsed timestamp.
        /// </summary>
        /// <param name="initialTime">The initial elapsed runtime timestamp.</param>
        public ManualRuntimeClock(RuntimeInstant initialTime)
        {
            _currentTime = initialTime;
        }

        /// <summary>
        /// Gets the clock's exact elapsed runtime timestamp.
        /// This remains readable after shutdown.
        /// </summary>
        public RuntimeInstant CurrentTime
        {
            get { return _currentTime; }
        }

        /// <summary>
        /// Gets a value indicating whether explicit advancement is paused.
        /// This remains readable after shutdown.
        /// </summary>
        public bool IsPaused
        {
            get { return _isPaused; }
        }

        /// <summary>
        /// Reports lifecycle readiness without changing time or pause state.
        /// Initialization is repeatable before shutdown and cannot revive a shut-down clock.
        /// </summary>
        public ServiceInitializationResult Initialize()
        {
            if (_isShutDown)
            {
                return ServiceInitializationResult.Failure(
                    "The manual runtime clock has been shut down and cannot be initialized again.");
            }

            return ServiceInitializationResult.Success();
        }

        /// <summary>
        /// Permanently ends mutation through this clock without changing readable state.
        /// </summary>
        public void Shutdown()
        {
            _isShutDown = true;
        }

        /// <summary>
        /// Pauses explicit advancement. Repeated calls are idempotent.
        /// </summary>
        public void Pause()
        {
            ThrowIfShutDown();
            _isPaused = true;
        }

        /// <summary>
        /// Resumes explicit advancement. Repeated calls are idempotent.
        /// </summary>
        public void Resume()
        {
            ThrowIfShutDown();
            _isPaused = false;
        }

        /// <summary>
        /// Advances time exactly when running. Advancement while paused is ignored.
        /// Overflow fails atomically without changing current time.
        /// </summary>
        /// <param name="duration">The exact nonnegative duration to advance.</param>
        public void Advance(RuntimeDuration duration)
        {
            ThrowIfShutDown();

            if (_isPaused || duration == RuntimeDuration.Zero)
            {
                return;
            }

            long milliseconds = duration.Milliseconds;
            if (milliseconds >
                long.MaxValue - _currentTime.Milliseconds)
            {
                throw new OverflowException(
                    "Advancing the manual runtime clock would exceed Int64.MaxValue.");
            }

            _currentTime = new RuntimeInstant(
                _currentTime.Milliseconds + milliseconds);
        }

        private void ThrowIfShutDown()
        {
            if (_isShutDown)
            {
                throw new ObjectDisposedException(
                    "ManualRuntimeClock",
                    "The manual runtime clock has been shut down.");
            }
        }
    }
}
