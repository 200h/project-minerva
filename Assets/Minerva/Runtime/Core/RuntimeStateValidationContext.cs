namespace Minerva.Core
{
    /// <summary>
    /// Describes the immutable inputs for initial-value or mutation validation.
    /// </summary>
    public sealed class RuntimeStateValidationContext<T>
    {
        internal RuntimeStateValidationContext(
            RuntimeStateIdentity identity,
            bool hasCurrentValue,
            T currentValue,
            T proposedValue)
        {
            Identity = identity;
            HasCurrentValue = hasCurrentValue;
            CurrentValue = currentValue;
            ProposedValue = proposedValue;
        }

        /// <summary>
        /// Gets the stable identity of the state being validated.
        /// </summary>
        public RuntimeStateIdentity Identity { get; private set; }

        /// <summary>
        /// Gets whether validation describes a transition from an existing value.
        /// </summary>
        public bool HasCurrentValue { get; private set; }

        /// <summary>
        /// Gets the authoritative current value, or default(T) during initial validation.
        /// </summary>
        public T CurrentValue { get; private set; }

        /// <summary>
        /// Gets the proposed value being validated.
        /// </summary>
        public T ProposedValue { get; private set; }
    }
}
