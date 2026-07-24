namespace Minerva.Core
{
    /// <summary>
    /// Exposes the narrow capability to synchronously process bounded due work.
    /// </summary>
    public interface IScheduledTaskProcessor
    {
        /// <summary>
        /// Drains callbacks eligible at one captured clock and insertion boundary.
        /// </summary>
        /// <param name="maxCallbacks">The positive maximum number of callback attempts.</param>
        /// <returns>An immutable summary of the completed drain.</returns>
        ScheduledTaskDrainResult DrainDue(int maxCallbacks);
    }
}
