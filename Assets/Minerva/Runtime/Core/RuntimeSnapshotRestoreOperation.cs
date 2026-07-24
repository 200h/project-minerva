namespace Minerva.Core
{
    /// <summary>
    /// Enforces the externally observable lifecycle of a prepared restore operation.
    /// </summary>
    public abstract class RuntimeSnapshotRestoreOperation :
        IRuntimeSnapshotRestoreOperation
    {
        private bool _wasApplyAttempted;
        private bool _isRollbackComplete;
        private bool _isReleased;

        /// <summary>
        /// Attempts application once and rejects invalid lifecycle use.
        /// </summary>
        public RuntimeSnapshotStepResult Apply()
        {
            if (_isReleased)
            {
                return RuntimeSnapshotStepResult.Failure(
                    "A released restore operation cannot be applied.");
            }

            if (_isRollbackComplete)
            {
                return RuntimeSnapshotStepResult.Failure(
                    "A rolled-back restore operation cannot be applied.");
            }

            if (_wasApplyAttempted)
            {
                return RuntimeSnapshotStepResult.Failure(
                    "A restore operation cannot be applied more than once.");
            }

            _wasApplyAttempted = true;
            return ApplyCore();
        }

        /// <summary>
        /// Restores pre-restore state after an apply attempt.
        /// </summary>
        public RuntimeSnapshotStepResult Rollback()
        {
            if (_isReleased)
            {
                return RuntimeSnapshotStepResult.Failure(
                    "A released restore operation cannot be rolled back.");
            }

            if (!_wasApplyAttempted)
            {
                return RuntimeSnapshotStepResult.Failure(
                    "A restore operation cannot be rolled back before apply is attempted.");
            }

            if (_isRollbackComplete)
            {
                return RuntimeSnapshotStepResult.Success();
            }

            RuntimeSnapshotStepResult result = RollbackCore();
            if (result != null && result.IsSuccessful)
            {
                _isRollbackComplete = true;
            }

            return result;
        }

        /// <summary>
        /// Idempotently releases operation-held state.
        /// </summary>
        public RuntimeSnapshotStepResult Release()
        {
            if (_isReleased)
            {
                return RuntimeSnapshotStepResult.Success();
            }

            RuntimeSnapshotStepResult result = ReleaseCore();
            if (result != null && result.IsSuccessful)
            {
                _isReleased = true;
            }

            return result;
        }

        /// <summary>
        /// Implements the owner-local apply behavior.
        /// </summary>
        protected abstract RuntimeSnapshotStepResult ApplyCore();

        /// <summary>
        /// Implements the owner-local rollback behavior.
        /// </summary>
        protected abstract RuntimeSnapshotStepResult RollbackCore();

        /// <summary>
        /// Implements release of operation-held data.
        /// </summary>
        protected abstract RuntimeSnapshotStepResult ReleaseCore();
    }
}
