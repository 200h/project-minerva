namespace Minerva.Core
{
    /// <summary>
    /// Describes one immutable typed mutation outcome and optional publication evidence.
    /// </summary>
    public sealed class RuntimeStateMutationResult<T>
    {
        private RuntimeStateMutationResult(
            RuntimeStateMutationStatus status,
            RuntimeStateChange<T> change,
            string rejectionReason,
            bool isPublicationConfigured,
            bool wasPublicationAttempted,
            EventPublicationResult publicationResult,
            RuntimeStatePublicationFailure publicationFailure)
        {
            Status = status;
            Change = change;
            RejectionReason = rejectionReason;
            IsPublicationConfigured = isPublicationConfigured;
            WasPublicationAttempted = wasPublicationAttempted;
            PublicationResult = publicationResult;
            PublicationFailure = publicationFailure;
        }

        /// <summary>
        /// Gets the semantic mutation outcome.
        /// </summary>
        public RuntimeStateMutationStatus Status { get; private set; }

        /// <summary>
        /// Gets the completed change after a changed outcome, or null otherwise.
        /// </summary>
        public RuntimeStateChange<T> Change { get; private set; }

        /// <summary>
        /// Gets the actionable validation rejection reason, or null otherwise.
        /// </summary>
        public string RejectionReason { get; private set; }

        /// <summary>
        /// Gets whether this state cell has a fixed event publisher.
        /// </summary>
        public bool IsPublicationConfigured { get; private set; }

        /// <summary>
        /// Gets whether completed-change publication was attempted.
        /// </summary>
        public bool WasPublicationAttempted { get; private set; }

        /// <summary>
        /// Gets the normally returned publication result, or null.
        /// </summary>
        public EventPublicationResult PublicationResult { get; private set; }

        /// <summary>
        /// Gets publisher-exception diagnostics, or null.
        /// </summary>
        public RuntimeStatePublicationFailure PublicationFailure
        {
            get;
            private set;
        }

        internal static RuntimeStateMutationResult<T> ChangedWithoutPublication(
            RuntimeStateChange<T> change)
        {
            return new RuntimeStateMutationResult<T>(
                RuntimeStateMutationStatus.Changed,
                change,
                null,
                false,
                false,
                null,
                null);
        }

        internal static RuntimeStateMutationResult<T> ChangedWithPublication(
            RuntimeStateChange<T> change,
            EventPublicationResult publicationResult)
        {
            return new RuntimeStateMutationResult<T>(
                RuntimeStateMutationStatus.Changed,
                change,
                null,
                true,
                true,
                publicationResult,
                null);
        }

        internal static RuntimeStateMutationResult<T> ChangedWithFailure(
            RuntimeStateChange<T> change,
            RuntimeStatePublicationFailure publicationFailure)
        {
            return new RuntimeStateMutationResult<T>(
                RuntimeStateMutationStatus.Changed,
                change,
                null,
                true,
                true,
                null,
                publicationFailure);
        }

        internal static RuntimeStateMutationResult<T> Unchanged(
            bool isPublicationConfigured)
        {
            return new RuntimeStateMutationResult<T>(
                RuntimeStateMutationStatus.Unchanged,
                null,
                null,
                isPublicationConfigured,
                false,
                null,
                null);
        }

        internal static RuntimeStateMutationResult<T> Rejected(
            string rejectionReason,
            bool isPublicationConfigured)
        {
            return new RuntimeStateMutationResult<T>(
                RuntimeStateMutationStatus.Rejected,
                null,
                rejectionReason,
                isPublicationConfigured,
                false,
                null,
                null);
        }
    }
}
