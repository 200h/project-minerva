using System;
using System.Collections.Generic;

namespace Minerva.Core
{
    /// <summary>
    /// Describes completion and isolated subscriber failures for one publication.
    /// </summary>
    public sealed class EventPublicationResult
    {
        private readonly List<EventSubscriberFailure> _failures;

        internal EventPublicationResult(Type eventType)
        {
            EventType = eventType;
            _failures = new List<EventSubscriberFailure>();
        }

        /// <summary>
        /// Gets the exact event type associated with this result.
        /// </summary>
        public Type EventType { get; private set; }

        /// <summary>
        /// Gets whether dispatch has finished.
        /// </summary>
        /// <remarks>
        /// A result returned by a nested publication becomes complete when the outer
        /// publication finishes draining the queue.
        /// </remarks>
        public bool IsComplete { get; internal set; }

        /// <summary>
        /// Gets whether dispatch completed without subscriber failures.
        /// </summary>
        public bool IsSuccessful
        {
            get { return IsComplete && _failures.Count == 0; }
        }

        /// <summary>
        /// Gets the number of isolated subscriber failures.
        /// </summary>
        public int FailureCount
        {
            get { return _failures.Count; }
        }

        /// <summary>
        /// Gets a subscriber failure in deterministic invocation order.
        /// </summary>
        public EventSubscriberFailure GetFailure(int index)
        {
            if (index < 0 || index >= _failures.Count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return _failures[index];
        }

        internal void AddFailure(EventSubscriberFailure failure)
        {
            _failures.Add(failure);
        }
    }
}
