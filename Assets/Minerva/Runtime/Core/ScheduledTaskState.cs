namespace Minerva.Core
{
    /// <summary>
    /// Identifies the observable lifecycle state of one scheduled task.
    /// </summary>
    public enum ScheduledTaskState
    {
        /// <summary>The task is waiting for a qualifying drain.</summary>
        Pending,

        /// <summary>The task's callback is currently being invoked.</summary>
        Executing,

        /// <summary>The callback returned or threw and will not run again.</summary>
        Completed,

        /// <summary>The pending task was cancelled and will not run.</summary>
        Cancelled
    }
}
