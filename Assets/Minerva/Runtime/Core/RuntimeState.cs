using System.Collections.Generic;

namespace Minerva.Core
{
    /// <summary>
    /// Creates isolated typed runtime-state capabilities with fixed policies.
    /// </summary>
    public static class RuntimeState
    {
        /// <summary>
        /// Creates state using default equality without validation or publication.
        /// </summary>
        public static RuntimeStateCapabilities<T> Create<T>(
            RuntimeStateIdentity identity,
            T initialValue)
        {
            return Create<T>(
                identity,
                initialValue,
                null,
                null,
                null);
        }

        /// <summary>
        /// Creates state using the supplied fixed comparer.
        /// </summary>
        public static RuntimeStateCapabilities<T> Create<T>(
            RuntimeStateIdentity identity,
            T initialValue,
            IEqualityComparer<T> comparer)
        {
            return Create<T>(
                identity,
                initialValue,
                comparer,
                null,
                null);
        }

        /// <summary>
        /// Creates state using fixed comparer, validator, and optional publisher policies.
        /// </summary>
        public static RuntimeStateCapabilities<T> Create<T>(
            RuntimeStateIdentity identity,
            T initialValue,
            IEqualityComparer<T> comparer,
            IRuntimeStateValidator<T> validator,
            IEventPublisher publisher)
        {
            RuntimeStateCell<T> cell = new RuntimeStateCell<T>(
                identity,
                initialValue,
                comparer,
                validator,
                publisher);

            return cell.CreateCapabilities();
        }
    }
}
