using System;

namespace Minerva.Core
{
    /// <summary>
    /// Summarizes one immutable bounded scheduled-task drain.
    /// </summary>
    public sealed class ScheduledTaskDrainResult
    {
        private readonly ScheduledTaskCallbackFailure[] _callbackFailures;

        internal ScheduledTaskDrainResult(
            int attemptedCount,
            int successfulCompletionCount,
            int cancellationSkipCount,
            ScheduledTaskCallbackFailure[] callbackFailures,
            bool limitReached,
            bool hasRemainingDueWork)
        {
            AttemptedCount = attemptedCount;
            SuccessfulCompletionCount = successfulCompletionCount;
            CancellationSkipCount = cancellationSkipCount;
            _callbackFailures =
                (ScheduledTaskCallbackFailure[])callbackFailures.Clone();
            LimitReached = limitReached;
            HasRemainingDueWork = hasRemainingDueWork;
        }

        /// <summary>Gets the number of callbacks invoked, including failures.</summary>
        public int AttemptedCount { get; private set; }

        /// <summary>Gets the number of callbacks that returned normally.</summary>
        public int SuccessfulCompletionCount { get; private set; }

        /// <summary>Gets the number of captured cancelled entries skipped.</summary>
        public int CancellationSkipCount { get; private set; }

        /// <summary>Gets the number of isolated callback failures.</summary>
        public int CallbackFailureCount
        {
            get { return _callbackFailures.Length; }
        }

        /// <summary>
        /// Gets a value indicating that the callback limit prevented eligible captured work.
        /// </summary>
        public bool LimitReached { get; private set; }

        /// <summary>
        /// Gets a value indicating that eligible captured work remains pending.
        /// </summary>
        public bool HasRemainingDueWork { get; private set; }

        /// <summary>
        /// Gets one immutable failure diagnostic in callback-attempt order.
        /// </summary>
        /// <param name="index">The zero-based diagnostic index.</param>
        public ScheduledTaskCallbackFailure GetCallbackFailure(int index)
        {
            if (index < 0 || index >= _callbackFailures.Length)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return _callbackFailures[index];
        }
    }
}
