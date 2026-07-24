using System;

namespace Minerva.Core
{
    /// <summary>
    /// Represents an immutable aggregate restore outcome and ordered cleanup diagnostics.
    /// </summary>
    public sealed class RuntimeSnapshotRestoreResult
    {
        private readonly RuntimeSnapshotDiagnostic[] _rollbackFailures;
        private readonly RuntimeSnapshotDiagnostic[] _releaseFailures;

        internal RuntimeSnapshotRestoreResult(
            bool isSuccessful,
            RuntimeSnapshotDiagnostic primaryFailure,
            RuntimeSnapshotDiagnostic[] rollbackFailures,
            RuntimeSnapshotDiagnostic[] releaseFailures)
        {
            IsSuccessful = isSuccessful;
            PrimaryFailure = primaryFailure;
            _rollbackFailures = Copy(rollbackFailures);
            _releaseFailures = Copy(releaseFailures);
        }

        /// <summary>
        /// Gets whether restore and required release completed successfully.
        /// </summary>
        public bool IsSuccessful { get; private set; }

        /// <summary>
        /// Gets the primary validation, preparation, apply, or release failure.
        /// </summary>
        public RuntimeSnapshotDiagnostic PrimaryFailure
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of ordered rollback failures.
        /// </summary>
        public int RollbackFailureCount
        {
            get { return _rollbackFailures.Length; }
        }

        /// <summary>
        /// Gets the number of ordered release failures.
        /// </summary>
        public int ReleaseFailureCount
        {
            get { return _releaseFailures.Length; }
        }

        /// <summary>
        /// Gets one rollback failure in observation order.
        /// </summary>
        public RuntimeSnapshotDiagnostic GetRollbackFailure(int index)
        {
            return Get(_rollbackFailures, index);
        }

        /// <summary>
        /// Gets one release failure in observation order.
        /// </summary>
        public RuntimeSnapshotDiagnostic GetReleaseFailure(int index)
        {
            return Get(_releaseFailures, index);
        }

        private static RuntimeSnapshotDiagnostic[] Copy(
            RuntimeSnapshotDiagnostic[] diagnostics)
        {
            if (diagnostics == null || diagnostics.Length == 0)
            {
                return new RuntimeSnapshotDiagnostic[0];
            }

            RuntimeSnapshotDiagnostic[] copy =
                new RuntimeSnapshotDiagnostic[diagnostics.Length];
            Array.Copy(diagnostics, copy, diagnostics.Length);
            return copy;
        }

        private static RuntimeSnapshotDiagnostic Get(
            RuntimeSnapshotDiagnostic[] diagnostics,
            int index)
        {
            if (index < 0 || index >= diagnostics.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return diagnostics[index];
        }
    }
}
