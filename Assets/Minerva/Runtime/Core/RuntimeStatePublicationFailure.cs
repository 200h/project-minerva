using System;

namespace Minerva.Core
{
    /// <summary>
    /// Describes a publisher exception without retaining the exception object.
    /// </summary>
    public sealed class RuntimeStatePublicationFailure
    {
        internal RuntimeStatePublicationFailure(
            Type exceptionType,
            string failureReason)
        {
            ExceptionType = exceptionType;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Gets the concrete type of the publisher exception.
        /// </summary>
        public Type ExceptionType { get; private set; }

        /// <summary>
        /// Gets the exception-derived failure reason.
        /// </summary>
        public string FailureReason { get; private set; }
    }
}
