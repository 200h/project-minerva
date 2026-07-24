namespace Minerva.Core
{
    /// <summary>
    /// Captures and prepares restoration of one explicitly owned snapshot contribution.
    /// </summary>
    public interface IRuntimeSnapshotContributor
    {
        /// <summary>
        /// Gets the immutable owner-qualified contribution identity.
        /// </summary>
        RuntimeSnapshotContributionIdentity Identity { get; }

        /// <summary>
        /// Gets the positive schema version used for new captures.
        /// </summary>
        int CurrentSchemaVersion { get; }

        /// <summary>
        /// Captures detached immutable data without mutating authoritative state.
        /// </summary>
        RuntimeSnapshotContributionCaptureResult Capture();

        /// <summary>
        /// Validates and prepares a non-mutating rollback-capable restore operation.
        /// </summary>
        RuntimeSnapshotPreparationResult PrepareRestore(
            RuntimeSnapshotContribution contribution);
    }
}
