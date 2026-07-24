namespace Minerva.Core
{
    /// <summary>
    /// Identifies the coordinator phase that observed a snapshot failure.
    /// </summary>
    public enum RuntimeSnapshotOperationPhase
    {
        StructuralValidation,
        Capture,
        Preparation,
        Apply,
        Rollback,
        Release
    }
}
