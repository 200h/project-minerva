using System;

namespace Minerva.Core
{
    /// <summary>
    /// Represents the outcome of initializing one runtime service.
    /// </summary>
    public sealed class ServiceInitializationResult
    {
        private ServiceInitializationResult(bool isSuccessful, string failureReason)
        {
            IsSuccessful = isSuccessful;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Gets a value indicating whether initialization succeeded.
        /// </summary>
        public bool IsSuccessful { get; private set; }

        /// <summary>
        /// Gets the reason initialization failed, or null following success.
        /// </summary>
        public string FailureReason { get; private set; }

        /// <summary>
        /// Creates a successful result.
        /// </summary>
        public static ServiceInitializationResult Success()
        {
            return new ServiceInitializationResult(true, null);
        }

        /// <summary>
        /// Creates a failed result with an actionable reason.
        /// </summary>
        public static ServiceInitializationResult Failure(string reason)
        {
            if (string.IsNullOrEmpty(reason))
            {
                throw new ArgumentException("A failure reason is required.", "reason");
            }

            return new ServiceInitializationResult(false, reason);
        }
    }
}
