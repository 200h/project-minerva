using System;

namespace Minerva.Core
{
    /// <summary>
    /// Describes one immutable actionable snapshot failure without retaining its source.
    /// </summary>
    public sealed class RuntimeSnapshotDiagnostic
    {
        /// <summary>
        /// Creates an expected-failure diagnostic.
        /// </summary>
        public RuntimeSnapshotDiagnostic(
            RuntimeSnapshotOperationPhase phase,
            RuntimeSnapshotContributionIdentity identity,
            string failureReason)
            : this(phase, identity, null, failureReason)
        {
        }

        /// <summary>
        /// Creates an exception diagnostic without retaining the exception object.
        /// </summary>
        public RuntimeSnapshotDiagnostic(
            RuntimeSnapshotOperationPhase phase,
            RuntimeSnapshotContributionIdentity identity,
            Type exceptionType,
            string failureReason)
        {
            if (string.IsNullOrEmpty(failureReason)
                || failureReason.Trim().Length == 0)
            {
                throw new ArgumentException(
                    "An actionable failure reason is required.",
                    "failureReason");
            }

            Phase = phase;
            Identity = identity;
            ExceptionType = exceptionType;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Gets the phase that observed the failure.
        /// </summary>
        public RuntimeSnapshotOperationPhase Phase { get; private set; }

        /// <summary>
        /// Gets the affected contribution identity when known.
        /// </summary>
        public RuntimeSnapshotContributionIdentity Identity
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the thrown exception type when an exception was observed.
        /// </summary>
        public Type ExceptionType { get; private set; }

        /// <summary>
        /// Gets the actionable failure reason.
        /// </summary>
        public string FailureReason { get; private set; }
    }
}
