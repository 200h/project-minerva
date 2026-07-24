namespace Minerva.Core
{
    /// <summary>
    /// Applies, rolls back, and releases one owner-local prepared restore.
    /// </summary>
    public interface IRuntimeSnapshotRestoreOperation
    {
        /// <summary>
        /// Attempts the owner-local mutation exactly once.
        /// </summary>
        RuntimeSnapshotStepResult Apply();

        /// <summary>
        /// Restores pre-restore owner state after an apply attempt.
        /// </summary>
        RuntimeSnapshotStepResult Rollback();

        /// <summary>
        /// Idempotently releases operation-held data and references.
        /// </summary>
        RuntimeSnapshotStepResult Release();
    }
}
