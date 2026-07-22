using System;
using System.Collections.Generic;

namespace Minerva.Core
{
    /// <summary>
    /// Describes the outcome of shutting down all initialized runtime services.
    /// </summary>
    public sealed class RuntimeShutdownResult
    {
        private readonly RuntimeShutdownFailure[] _failures;

        internal RuntimeShutdownResult(List<RuntimeShutdownFailure> failures)
        {
            _failures = failures.ToArray();
        }

        /// <summary>
        /// Gets a value indicating whether every initialized service shut down cleanly.
        /// </summary>
        public bool IsSuccessful
        {
            get { return _failures.Length == 0; }
        }

        /// <summary>
        /// Gets the number of services that threw while shutting down.
        /// </summary>
        public int FailureCount
        {
            get { return _failures.Length; }
        }

        /// <summary>
        /// Gets a shutdown failure in reverse lifecycle execution order.
        /// </summary>
        public RuntimeShutdownFailure GetFailure(int index)
        {
            if (index < 0 || index >= _failures.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return _failures[index];
        }
    }
}
