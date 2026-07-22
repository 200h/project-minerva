using System;

namespace Minerva.Core
{
    /// <summary>
    /// Identifies a runtime service that threw while shutting down.
    /// </summary>
    public sealed class RuntimeShutdownFailure
    {
        internal RuntimeShutdownFailure(
            Type serviceType,
            Type exceptionType,
            string failureReason)
        {
            ServiceType = serviceType;
            ExceptionType = exceptionType;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Gets the concrete type of the service that failed to shut down.
        /// </summary>
        public Type ServiceType { get; private set; }

        /// <summary>
        /// Gets the concrete type of exception thrown by the service.
        /// </summary>
        public Type ExceptionType { get; private set; }

        /// <summary>
        /// Gets the exception-derived failure reason.
        /// </summary>
        public string FailureReason { get; private set; }
    }
}
