namespace Minerva.Core
{
    /// <summary>
    /// Announces one completed strongly typed runtime-state change.
    /// </summary>
    public sealed class RuntimeStateChange<T> : IEvent
    {
        internal RuntimeStateChange(
            RuntimeStateIdentity identity,
            T previousValue,
            T currentValue)
        {
            Identity = identity;
            PreviousValue = previousValue;
            CurrentValue = currentValue;
        }

        /// <summary>
        /// Gets the stable identity of the changed state.
        /// </summary>
        public RuntimeStateIdentity Identity { get; private set; }

        /// <summary>
        /// Gets the authoritative value before the completed change.
        /// </summary>
        public T PreviousValue { get; private set; }

        /// <summary>
        /// Gets the authoritative value after the completed change.
        /// </summary>
        public T CurrentValue { get; private set; }
    }
}
