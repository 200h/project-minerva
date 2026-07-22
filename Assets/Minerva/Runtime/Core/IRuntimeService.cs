namespace Minerva.Core
{
    /// <summary>
    /// Defines the explicit lifecycle used by services owned by a runtime bootstrap.
    /// </summary>
    public interface IRuntimeService
    {
        /// <summary>
        /// Initializes the service and reports an expected failure without throwing.
        /// </summary>
        ServiceInitializationResult Initialize();

        /// <summary>
        /// Stops the service and releases lifecycle-owned resources.
        /// </summary>
        void Shutdown();
    }
}
