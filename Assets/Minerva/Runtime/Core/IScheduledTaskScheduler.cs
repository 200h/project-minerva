using System;

namespace Minerva.Core
{
    /// <summary>
    /// Exposes the narrow capability to schedule parameterless callbacks at elapsed
    /// runtime timestamps.
    /// </summary>
    public interface IScheduledTaskScheduler
    {
        /// <summary>
        /// Schedules a callback without invoking or normalizing it.
        /// </summary>
        /// <param name="dueTime">The exact elapsed runtime timestamp at which work is due.</param>
        /// <param name="callback">The callback to invoke during a later qualifying drain.</param>
        /// <returns>A readable handle that can cancel the task while it is pending.</returns>
        IScheduledTaskHandle Schedule(RuntimeInstant dueTime, Action callback);
    }
}
