using System;

namespace Minerva.Core
{
    /// <summary>
    /// Returns separate read and mutation capabilities for one created state cell.
    /// </summary>
    public sealed class RuntimeStateCapabilities<T>
    {
        internal RuntimeStateCapabilities(
            IRuntimeState<T> state,
            IRuntimeStateMutator<T> mutator)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            if (mutator == null)
            {
                throw new ArgumentNullException("mutator");
            }

            State = state;
            Mutator = mutator;
        }

        /// <summary>
        /// Gets the read-only state capability.
        /// </summary>
        public IRuntimeState<T> State { get; private set; }

        /// <summary>
        /// Gets the explicit mutation capability.
        /// </summary>
        public IRuntimeStateMutator<T> Mutator { get; private set; }
    }
}
