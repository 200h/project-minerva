using System;

namespace Minerva.Core
{
    /// <summary>
    /// Represents one contributor's immutable restore-preparation outcome.
    /// </summary>
    public sealed class RuntimeSnapshotPreparationResult
    {
        private RuntimeSnapshotPreparationResult(
            IRuntimeSnapshotRestoreOperation operation,
            string failureReason)
        {
            Operation = operation;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Gets whether preparation produced a restore operation.
        /// </summary>
        public bool IsSuccessful
        {
            get { return Operation != null; }
        }

        /// <summary>
        /// Gets the prepared rollback-capable operation after success.
        /// </summary>
        public IRuntimeSnapshotRestoreOperation Operation
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the actionable failure reason after failure.
        /// </summary>
        public string FailureReason { get; private set; }

        /// <summary>
        /// Creates a successful preparation result.
        /// </summary>
        public static RuntimeSnapshotPreparationResult Success(
            IRuntimeSnapshotRestoreOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            return new RuntimeSnapshotPreparationResult(operation, null);
        }

        /// <summary>
        /// Creates a failed preparation result.
        /// </summary>
        public static RuntimeSnapshotPreparationResult Failure(string reason)
        {
            RuntimeSnapshotStepResult.ValidateFailureReason(reason);
            return new RuntimeSnapshotPreparationResult(null, reason);
        }
    }
}
