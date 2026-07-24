using System;
using System.Collections.Generic;

namespace Minerva.Core
{
    /// <summary>
    /// Provides deterministic, bounded, single-threaded scheduled callback processing
    /// against a retained read-only runtime clock.
    /// </summary>
    public sealed class ScheduledTaskQueue :
        IScheduledTaskScheduler,
        IScheduledTaskProcessor,
        IRuntimeService
    {
        private readonly IRuntimeClock _clock;
        private readonly List<ScheduledTaskEntry> _entries;
        private long _lastInsertionSequence;
        private bool _isDraining;
        private bool _isShutDown;

        /// <summary>
        /// Creates an immediately operational queue using a non-null read-only clock.
        /// The queue never advances, controls, replaces, or shuts down the clock.
        /// </summary>
        /// <param name="clock">The authoritative read-only elapsed runtime clock.</param>
        public ScheduledTaskQueue(IRuntimeClock clock)
        {
            if (clock == null)
            {
                throw new ArgumentNullException("clock");
            }

            _clock = clock;
            _entries = new List<ScheduledTaskEntry>();
        }

        /// <summary>
        /// Reports lifecycle readiness without altering task, sequence, or drain state.
        /// </summary>
        public ServiceInitializationResult Initialize()
        {
            if (_isShutDown)
            {
                return ServiceInitializationResult.Failure(
                    "The scheduled task queue has been shut down and cannot be initialized again.");
            }

            return ServiceInitializationResult.Success();
        }

        /// <summary>
        /// Terminally cancels all pending work without invoking callbacks.
        /// An executing callback is allowed to finish.
        /// </summary>
        public void Shutdown()
        {
            if (_isShutDown)
            {
                return;
            }

            _isShutDown = true;

            for (int index = 0; index < _entries.Count; index++)
            {
                _entries[index].Cancel();
            }

            _entries.Clear();
        }

        /// <summary>
        /// Schedules a callback at its exact supplied timestamp for a later drain.
        /// </summary>
        /// <param name="dueTime">The exact due timestamp.</param>
        /// <param name="callback">The non-null callback.</param>
        /// <returns>A readable, idempotently cancellable handle.</returns>
        public IScheduledTaskHandle Schedule(
            RuntimeInstant dueTime,
            Action callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            ThrowIfShutDown();

            if (_lastInsertionSequence == long.MaxValue)
            {
                throw new OverflowException(
                    "The scheduled task insertion sequence cannot be incremented.");
            }

            long insertionSequence = _lastInsertionSequence + 1;
            ScheduledTaskEntry entry = new ScheduledTaskEntry(
                dueTime,
                insertionSequence,
                callback);
            int insertionIndex = FindInsertionIndex(entry);

            _entries.Insert(insertionIndex, entry);
            _lastInsertionSequence = insertionSequence;
            return entry;
        }

        /// <summary>
        /// Processes due work at one captured clock and insertion boundary.
        /// Callback exceptions are returned as diagnostics and do not escape.
        /// </summary>
        /// <param name="maxCallbacks">The positive maximum callback-attempt count.</param>
        /// <returns>An immutable drain result.</returns>
        public ScheduledTaskDrainResult DrainDue(int maxCallbacks)
        {
            if (maxCallbacks <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "maxCallbacks",
                    "The callback limit must be greater than zero.");
            }

            if (_isDraining)
            {
                throw new InvalidOperationException(
                    "The scheduled task queue cannot be drained reentrantly.");
            }

            ThrowIfShutDown();
            _isDraining = true;

            try
            {
                RuntimeInstant cutoff = _clock.CurrentTime;
                long eligibilityCeiling = _lastInsertionSequence;
                List<ScheduledTaskEntry> eligible =
                    CaptureEligibleEntries(cutoff, eligibilityCeiling);
                List<ScheduledTaskCallbackFailure> failures =
                    new List<ScheduledTaskCallbackFailure>();
                int attemptedCount = 0;
                int successfulCompletionCount = 0;
                int cancellationSkipCount = 0;
                bool hasRemainingDueWork = false;

                for (int index = 0; index < eligible.Count; index++)
                {
                    ScheduledTaskEntry entry = eligible[index];

                    if (_isShutDown)
                    {
                        cancellationSkipCount +=
                            RemoveCancelledEntries(eligible, index);
                        break;
                    }

                    if (entry.State == ScheduledTaskState.Cancelled)
                    {
                        _entries.Remove(entry);
                        cancellationSkipCount++;
                        continue;
                    }

                    if (attemptedCount == maxCallbacks)
                    {
                        hasRemainingDueWork =
                            ResolveRemainingEntries(
                                eligible,
                                index,
                                ref cancellationSkipCount);
                        break;
                    }

                    Action callback = entry.BeginExecution();
                    Type callbackTargetType = callback.Target == null
                        ? null
                        : callback.Target.GetType();
                    string callbackMethodName = callback.Method == null
                        ? null
                        : callback.Method.Name;
                    attemptedCount++;

                    try
                    {
                        callback();
                        successfulCompletionCount++;
                    }
                    catch (Exception exception)
                    {
                        failures.Add(
                            new ScheduledTaskCallbackFailure(
                                entry.DueTime,
                                entry.InsertionSequence,
                                callbackTargetType,
                                callbackMethodName,
                                exception.GetType(),
                                exception.Message));
                    }
                    finally
                    {
                        entry.CompleteExecution();
                        _entries.Remove(entry);
                        callback = null;
                    }
                }

                bool limitReached = hasRemainingDueWork;
                return new ScheduledTaskDrainResult(
                    attemptedCount,
                    successfulCompletionCount,
                    cancellationSkipCount,
                    failures.ToArray(),
                    limitReached,
                    hasRemainingDueWork);
            }
            finally
            {
                _isDraining = false;
            }
        }

        private List<ScheduledTaskEntry> CaptureEligibleEntries(
            RuntimeInstant cutoff,
            long eligibilityCeiling)
        {
            List<ScheduledTaskEntry> eligible =
                new List<ScheduledTaskEntry>();

            for (int index = 0; index < _entries.Count; index++)
            {
                ScheduledTaskEntry entry = _entries[index];
                if (entry.DueTime > cutoff)
                {
                    break;
                }

                if (entry.InsertionSequence <= eligibilityCeiling)
                {
                    eligible.Add(entry);
                }
            }

            return eligible;
        }

        private int FindInsertionIndex(ScheduledTaskEntry entry)
        {
            int index = 0;
            while (index < _entries.Count &&
                CompareEntries(_entries[index], entry) <= 0)
            {
                index++;
            }

            return index;
        }

        private bool ResolveRemainingEntries(
            List<ScheduledTaskEntry> eligible,
            int startIndex,
            ref int cancellationSkipCount)
        {
            bool hasPendingEntry = false;

            for (int index = startIndex; index < eligible.Count; index++)
            {
                ScheduledTaskEntry entry = eligible[index];
                if (entry.State == ScheduledTaskState.Cancelled)
                {
                    _entries.Remove(entry);
                    cancellationSkipCount++;
                }
                else if (entry.State == ScheduledTaskState.Pending)
                {
                    hasPendingEntry = true;
                }
            }

            return hasPendingEntry;
        }

        private int RemoveCancelledEntries(
            List<ScheduledTaskEntry> eligible,
            int startIndex)
        {
            int removedCount = 0;

            for (int index = startIndex; index < eligible.Count; index++)
            {
                ScheduledTaskEntry entry = eligible[index];
                if (entry.State == ScheduledTaskState.Cancelled)
                {
                    _entries.Remove(entry);
                    removedCount++;
                }
            }

            return removedCount;
        }

        private static int CompareEntries(
            ScheduledTaskEntry left,
            ScheduledTaskEntry right)
        {
            int dueTimeComparison = left.DueTime.CompareTo(right.DueTime);
            if (dueTimeComparison != 0)
            {
                return dueTimeComparison;
            }

            return left.InsertionSequence.CompareTo(
                right.InsertionSequence);
        }

        private void ThrowIfShutDown()
        {
            if (_isShutDown)
            {
                throw new ObjectDisposedException(
                    "ScheduledTaskQueue",
                    "The scheduled task queue has been shut down.");
            }
        }

        private sealed class ScheduledTaskEntry : IScheduledTaskHandle
        {
            private Action _callback;
            private ScheduledTaskState _state;

            public ScheduledTaskEntry(
                RuntimeInstant dueTime,
                long insertionSequence,
                Action callback)
            {
                DueTime = dueTime;
                InsertionSequence = insertionSequence;
                _callback = callback;
                _state = ScheduledTaskState.Pending;
            }

            public RuntimeInstant DueTime { get; private set; }

            public long InsertionSequence { get; private set; }

            public ScheduledTaskState State
            {
                get { return _state; }
            }

            public bool Cancel()
            {
                if (_state != ScheduledTaskState.Pending)
                {
                    return false;
                }

                _state = ScheduledTaskState.Cancelled;
                _callback = null;
                return true;
            }

            public Action BeginExecution()
            {
                _state = ScheduledTaskState.Executing;
                return _callback;
            }

            public void CompleteExecution()
            {
                _callback = null;
                _state = ScheduledTaskState.Completed;
            }
        }
    }
}
