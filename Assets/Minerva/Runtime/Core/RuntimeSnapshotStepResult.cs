using System;

namespace Minerva.Core
{
    /// <summary>
    /// Represents an expected success or actionable participant-step failure.
    /// </summary>
    public sealed class RuntimeSnapshotStepResult
    {
        private RuntimeSnapshotStepResult(
            bool isSuccessful,
            string failureReason)
        {
            IsSuccessful = isSuccessful;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Gets whether the step succeeded.
        /// </summary>
        public bool IsSuccessful { get; private set; }

        /// <summary>
        /// Gets the actionable failure reason, or null after success.
        /// </summary>
        public string FailureReason { get; private set; }

        /// <summary>
        /// Creates a successful step result.
        /// </summary>
        public static RuntimeSnapshotStepResult Success()
        {
            return new RuntimeSnapshotStepResult(true, null);
        }

        /// <summary>
        /// Creates a failed step result.
        /// </summary>
        public static RuntimeSnapshotStepResult Failure(string reason)
        {
            ValidateFailureReason(reason);
            return new RuntimeSnapshotStepResult(false, reason);
        }

        internal static void ValidateFailureReason(string reason)
        {
            if (reason == null)
            {
                throw new ArgumentNullException("reason");
            }

            if (reason.Trim().Length == 0)
            {
                throw new ArgumentException(
                    "An actionable failure reason is required.",
                    "reason");
            }
        }
    }
}
