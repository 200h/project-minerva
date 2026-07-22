namespace Minerva.Core
{
    /// <summary>
    /// Identifies the current lifecycle state of a runtime bootstrap.
    /// </summary>
    public enum RuntimeLifecycleState
    {
        Created,
        Initializing,
        Running,
        InitializationFailed,
        ShutDown,
        ShutdownFailed,
        Disposed
    }
}
