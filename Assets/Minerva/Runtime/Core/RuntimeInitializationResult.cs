using System;

namespace Minerva.Core
{
    /// <summary>
    /// Describes the complete runtime initialization outcome.
    /// </summary>
    public sealed class RuntimeInitializationResult
    {
        private RuntimeInitializationResult(
            bool isSuccessful,
            Type failedServiceType,
            string failureReason)
        {
            IsSuccessful = isSuccessful;
            FailedServiceType = failedServiceType;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Gets a value indicating whether every registered service initialized.
        /// </summary>
        public bool IsSuccessful { get; private set; }

        /// <summary>
        /// Gets the concrete type of the failed service, or null after success.
        /// </summary>
        public Type FailedServiceType { get; private set; }

        /// <summary>
        /// Gets the reported or exception-derived failure reason.
        /// </summary>
        public string FailureReason { get; private set; }

        internal static RuntimeInitializationResult Success()
        {
            return new RuntimeInitializationResult(true, null, null);
        }

        internal static RuntimeInitializationResult Failure(
            Type failedServiceType,
            string failureReason)
        {
            return new RuntimeInitializationResult(false, failedServiceType, failureReason);
        }
    }
}
