namespace Minerva.Core
{
    /// <summary>
    /// Represents an aggregate snapshot capture outcome.
    /// </summary>
    public sealed class RuntimeSnapshotCaptureResult
    {
        internal RuntimeSnapshotCaptureResult(
            RuntimeSnapshot snapshot,
            RuntimeSnapshotDiagnostic failure)
        {
            Snapshot = snapshot;
            Failure = failure;
        }

        /// <summary>
        /// Gets whether a complete usable snapshot was captured.
        /// </summary>
        public bool IsSuccessful
        {
            get { return Snapshot != null; }
        }

        /// <summary>
        /// Gets the complete snapshot after success.
        /// </summary>
        public RuntimeSnapshot Snapshot { get; private set; }

        /// <summary>
        /// Gets the primary capture failure after failure.
        /// </summary>
        public RuntimeSnapshotDiagnostic Failure { get; private set; }
    }
}
