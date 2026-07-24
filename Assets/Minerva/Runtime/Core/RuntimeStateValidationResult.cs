using System;

namespace Minerva.Core
{
    /// <summary>
    /// Represents an accepted validation or an actionable rejection.
    /// </summary>
    public sealed class RuntimeStateValidationResult
    {
        private RuntimeStateValidationResult(
            bool isAccepted,
            string rejectionReason)
        {
            IsAccepted = isAccepted;
            RejectionReason = rejectionReason;
        }

        /// <summary>
        /// Gets whether the proposed value is accepted.
        /// </summary>
        public bool IsAccepted { get; private set; }

        /// <summary>
        /// Gets the actionable rejection reason, or null after acceptance.
        /// </summary>
        public string RejectionReason { get; private set; }

        /// <summary>
        /// Creates an accepted validation result.
        /// </summary>
        public static RuntimeStateValidationResult Accepted()
        {
            return new RuntimeStateValidationResult(true, null);
        }

        /// <summary>
        /// Creates a rejected validation result with an actionable reason.
        /// </summary>
        public static RuntimeStateValidationResult Rejected(
            string rejectionReason)
        {
            if (rejectionReason == null)
            {
                throw new ArgumentNullException("rejectionReason");
            }

            if (rejectionReason.Trim().Length == 0)
            {
                throw new ArgumentException(
                    "A rejected validation requires an actionable reason.",
                    "rejectionReason");
            }

            return new RuntimeStateValidationResult(
                false,
                rejectionReason);
        }
    }
}
