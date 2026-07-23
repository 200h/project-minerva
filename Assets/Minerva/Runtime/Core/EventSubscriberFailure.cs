using System;

namespace Minerva.Core
{
    /// <summary>
    /// Describes one subscriber exception isolated during event publication.
    /// </summary>
    public sealed class EventSubscriberFailure
    {
        internal EventSubscriberFailure(
            Type eventType,
            Type subscriberType,
            string subscriberMethodName,
            Type exceptionType,
            string failureReason)
        {
            EventType = eventType;
            SubscriberType = subscriberType;
            SubscriberMethodName = subscriberMethodName;
            ExceptionType = exceptionType;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Gets the exact event type being delivered when the subscriber failed.
        /// </summary>
        public Type EventType { get; private set; }

        /// <summary>
        /// Gets the subscriber target type, or the declaring type for a static handler.
        /// </summary>
        public Type SubscriberType { get; private set; }

        /// <summary>
        /// Gets the name of the failed subscriber method.
        /// </summary>
        public string SubscriberMethodName { get; private set; }

        /// <summary>
        /// Gets the concrete type of the subscriber exception.
        /// </summary>
        public Type ExceptionType { get; private set; }

        /// <summary>
        /// Gets the exception-derived failure reason.
        /// </summary>
        public string FailureReason { get; private set; }
    }
}
