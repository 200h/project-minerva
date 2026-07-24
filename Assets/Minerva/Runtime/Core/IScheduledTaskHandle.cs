namespace Minerva.Core
{
    /// <summary>
    /// Exposes readable scheduled-task state and idempotent pending cancellation.
    /// </summary>
    public interface IScheduledTaskHandle
    {
        /// <summary>
        /// Gets the exact authored elapsed runtime timestamp.
        /// </summary>
        RuntimeInstant DueTime { get; }

        /// <summary>
        /// Gets the task's current lifecycle state.
        /// </summary>
        ScheduledTaskState State { get; }

        /// <summary>
        /// Cancels the task only when it is still pending.
        /// </summary>
        /// <returns>True when this call changed the task to cancelled; otherwise false.</returns>
        bool Cancel();
    }
}
