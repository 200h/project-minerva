using System;

namespace Minerva.Core
{
    /// <summary>
    /// Exposes an immutable registration-independent collection of contributions.
    /// </summary>
    public sealed class RuntimeSnapshot
    {
        private readonly RuntimeSnapshotContribution[] _contributions;

        /// <summary>
        /// Creates a detached aggregate while preserving input order.
        /// </summary>
        public RuntimeSnapshot(
            RuntimeSnapshotContribution[] contributions)
        {
            if (contributions == null)
            {
                throw new ArgumentNullException("contributions");
            }

            _contributions =
                new RuntimeSnapshotContribution[contributions.Length];
            Array.Copy(
                contributions,
                _contributions,
                contributions.Length);
        }

        /// <summary>
        /// Gets the number of contribution records.
        /// </summary>
        public int ContributionCount
        {
            get { return _contributions.Length; }
        }

        /// <summary>
        /// Gets one contribution without exposing the aggregate's backing collection.
        /// </summary>
        public RuntimeSnapshotContribution GetContribution(int index)
        {
            if (index < 0 || index >= _contributions.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return _contributions[index];
        }
    }
}
