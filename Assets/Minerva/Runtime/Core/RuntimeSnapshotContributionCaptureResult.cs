using System;

namespace Minerva.Core
{
    /// <summary>
    /// Represents one contributor's immutable capture outcome.
    /// </summary>
    public sealed class RuntimeSnapshotContributionCaptureResult
    {
        private RuntimeSnapshotContributionCaptureResult(
            RuntimeSnapshotContribution contribution,
            string failureReason)
        {
            Contribution = contribution;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Gets whether capture produced a contribution.
        /// </summary>
        public bool IsSuccessful
        {
            get { return Contribution != null; }
        }

        /// <summary>
        /// Gets the captured contribution after success.
        /// </summary>
        public RuntimeSnapshotContribution Contribution
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the actionable failure reason after failure.
        /// </summary>
        public string FailureReason { get; private set; }

        /// <summary>
        /// Creates a successful contribution capture result.
        /// </summary>
        public static RuntimeSnapshotContributionCaptureResult Success(
            RuntimeSnapshotContribution contribution)
        {
            if (contribution == null)
            {
                throw new ArgumentNullException("contribution");
            }

            return new RuntimeSnapshotContributionCaptureResult(
                contribution,
                null);
        }

        /// <summary>
        /// Creates a failed contribution capture result.
        /// </summary>
        public static RuntimeSnapshotContributionCaptureResult Failure(
            string reason)
        {
            RuntimeSnapshotStepResult.ValidateFailureReason(reason);
            return new RuntimeSnapshotContributionCaptureResult(null, reason);
        }
    }
}
