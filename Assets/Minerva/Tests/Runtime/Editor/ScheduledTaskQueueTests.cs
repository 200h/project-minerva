using System;
using System.Collections.Generic;
using System.Reflection;
using Minerva.Core;
using NUnit.Framework;

namespace Minerva.Tests.Runtime
{
    public sealed class ScheduledTaskQueueTests
    {
        [Test]
        public void ConstructionAndInitialization_RequireClockAndRemainStateNeutral()
        {
            AssertArgumentNull(
                delegate { new ScheduledTaskQueue(null); },
                "clock");

            ManualRuntimeClock clock = new ManualRuntimeClock();
            ScheduledTaskQueue queue = new ScheduledTaskQueue(clock);
            IScheduledTaskHandle handle = queue.Schedule(
                new RuntimeInstant(5),
                delegate { });

            Assert.IsTrue(queue.Initialize().IsSuccessful);
            Assert.IsTrue(queue.Initialize().IsSuccessful);
            Assert.AreEqual(ScheduledTaskState.Pending, handle.State);
            Assert.AreEqual(0, queue.DrainDue(1).AttemptedCount);
            Assert.AreEqual(ScheduledTaskState.Pending, handle.State);
        }

        [Test]
        public void Schedule_AcceptsPastCurrentAndFutureWithoutImmediateInvocation()
        {
            ManualRuntimeClock clock =
                new ManualRuntimeClock(new RuntimeInstant(10));
            ScheduledTaskQueue queue = new ScheduledTaskQueue(clock);
            List<string> calls = new List<string>();

            IScheduledTaskHandle future = queue.Schedule(
                new RuntimeInstant(11),
                delegate { calls.Add("future"); });
            IScheduledTaskHandle current = queue.Schedule(
                new RuntimeInstant(10),
                delegate { calls.Add("current"); });
            IScheduledTaskHandle past = queue.Schedule(
                new RuntimeInstant(9),
                delegate { calls.Add("past"); });

            Assert.AreEqual(0, calls.Count);
            Assert.AreEqual(11, future.DueTime.Milliseconds);
            Assert.AreEqual(10, current.DueTime.Milliseconds);
            Assert.AreEqual(9, past.DueTime.Milliseconds);

            ScheduledTaskDrainResult first = queue.DrainDue(10);

            CollectionAssert.AreEqual(
                new string[] { "past", "current" },
                calls);
            Assert.AreEqual(2, first.AttemptedCount);
            Assert.AreEqual(ScheduledTaskState.Pending, future.State);

            clock.Advance(new RuntimeDuration(1));
            queue.DrainDue(10);
            CollectionAssert.AreEqual(
                new string[] { "past", "current", "future" },
                calls);
        }

        [Test]
        public void DrainDue_OrdersByTimestampThenExactInsertionSequence()
        {
            ManualRuntimeClock clock =
                new ManualRuntimeClock(new RuntimeInstant(20));
            ScheduledTaskQueue queue = new ScheduledTaskQueue(clock);
            List<string> calls = new List<string>();

            queue.Schedule(
                new RuntimeInstant(20),
                delegate { calls.Add("equal-first"); });
            queue.Schedule(
                new RuntimeInstant(10),
                delegate { calls.Add("earlier"); });
            queue.Schedule(
                new RuntimeInstant(20),
                delegate { calls.Add("equal-second"); });
            queue.Schedule(
                new RuntimeInstant(20),
                delegate { calls.Add("equal-third"); });

            ScheduledTaskDrainResult result = queue.DrainDue(10);

            CollectionAssert.AreEqual(
                new string[]
                {
                    "earlier",
                    "equal-first",
                    "equal-second",
                    "equal-third"
                },
                calls);
            Assert.AreEqual(4, result.AttemptedCount);
            Assert.AreEqual(4, result.SuccessfulCompletionCount);
            Assert.AreEqual(0, result.CallbackFailureCount);
        }

        [Test]
        public void Schedule_RejectsNullAndSequenceOverflowAtomically()
        {
            ManualRuntimeClock clock = new ManualRuntimeClock();
            ScheduledTaskQueue queue = new ScheduledTaskQueue(clock);
            int calls = 0;
            IScheduledTaskHandle existing = queue.Schedule(
                RuntimeInstant.Zero,
                delegate { calls++; });

            AssertArgumentNull(
                delegate
                {
                    queue.Schedule(RuntimeInstant.Zero, null);
                },
                "callback");

            SetLastInsertionSequence(queue, long.MaxValue);
            AssertOverflow(
                delegate
                {
                    queue.Schedule(
                        RuntimeInstant.Zero,
                        delegate { calls += 100; });
                });

            ScheduledTaskDrainResult result = queue.DrainDue(10);
            Assert.AreEqual(1, calls);
            Assert.AreEqual(1, result.AttemptedCount);
            Assert.AreEqual(ScheduledTaskState.Completed, existing.State);
        }

        [Test]
        public void Cancel_IsIdempotentAcrossPendingExecutingAndCompletedStates()
        {
            ManualRuntimeClock clock = new ManualRuntimeClock();
            ScheduledTaskQueue queue = new ScheduledTaskQueue(clock);
            int cancelledCalls = 0;
            IScheduledTaskHandle cancelled = queue.Schedule(
                RuntimeInstant.Zero,
                delegate { cancelledCalls++; });

            Assert.IsTrue(cancelled.Cancel());
            Assert.IsFalse(cancelled.Cancel());
            Assert.AreEqual(ScheduledTaskState.Cancelled, cancelled.State);

            IScheduledTaskHandle executing = null;
            bool executingCancelResult = true;
            ScheduledTaskState stateDuringCallback =
                ScheduledTaskState.Pending;
            executing = queue.Schedule(
                RuntimeInstant.Zero,
                delegate
                {
                    stateDuringCallback = executing.State;
                    executingCancelResult = executing.Cancel();
                });

            ScheduledTaskDrainResult result = queue.DrainDue(10);

            Assert.AreEqual(0, cancelledCalls);
            Assert.AreEqual(1, result.CancellationSkipCount);
            Assert.AreEqual(
                ScheduledTaskState.Executing,
                stateDuringCallback);
            Assert.IsFalse(executingCancelResult);
            Assert.AreEqual(ScheduledTaskState.Completed, executing.State);
            Assert.IsFalse(executing.Cancel());
        }

        [Test]
        public void CallbackCanCancelLaterWorkWithoutConsumingAttemptBudget()
        {
            ManualRuntimeClock clock = new ManualRuntimeClock();
            ScheduledTaskQueue queue = new ScheduledTaskQueue(clock);
            List<string> calls = new List<string>();
            IScheduledTaskHandle later = null;

            queue.Schedule(
                RuntimeInstant.Zero,
                delegate
                {
                    calls.Add("first");
                    Assert.IsTrue(later.Cancel());
                });
            later = queue.Schedule(
                RuntimeInstant.Zero,
                delegate { calls.Add("cancelled"); });
            queue.Schedule(
                RuntimeInstant.Zero,
                delegate { calls.Add("third"); });

            ScheduledTaskDrainResult result = queue.DrainDue(2);

            CollectionAssert.AreEqual(
                new string[] { "first", "third" },
                calls);
            Assert.AreEqual(2, result.AttemptedCount);
            Assert.AreEqual(1, result.CancellationSkipCount);
            Assert.IsFalse(result.LimitReached);
            Assert.IsFalse(result.HasRemainingDueWork);
        }

        [Test]
        public void SchedulingDuringDrain_IsDeferredRegardlessOfDueTimestamp()
        {
            ManualRuntimeClock clock =
                new ManualRuntimeClock(new RuntimeInstant(10));
            ScheduledTaskQueue queue = new ScheduledTaskQueue(clock);
            List<string> calls = new List<string>();

            queue.Schedule(
                new RuntimeInstant(10),
                delegate
                {
                    calls.Add("original");
                    queue.Schedule(
                        new RuntimeInstant(9),
                        delegate { calls.Add("past"); });
                    queue.Schedule(
                        new RuntimeInstant(10),
                        delegate { calls.Add("equal"); });
                    queue.Schedule(
                        RuntimeInstant.Zero,
                        delegate { calls.Add("zero"); });
                });

            ScheduledTaskDrainResult first = queue.DrainDue(10);

            CollectionAssert.AreEqual(new string[] { "original" }, calls);
            Assert.AreEqual(1, first.AttemptedCount);
            Assert.IsFalse(first.HasRemainingDueWork);
            Assert.IsFalse(first.LimitReached);

            queue.DrainDue(10);
            CollectionAssert.AreEqual(
                new string[] { "original", "zero", "past", "equal" },
                calls);
        }

        [Test]
        public void DrainDue_CapturesClockCutoffOnce()
        {
            CountingClock clock =
                new CountingClock(new RuntimeInstant(10));
            ScheduledTaskQueue queue = new ScheduledTaskQueue(clock);
            List<string> calls = new List<string>();

            queue.Schedule(
                new RuntimeInstant(10),
                delegate
                {
                    calls.Add("due");
                    clock.SetCurrentTime(new RuntimeInstant(20));
                });
            queue.Schedule(
                new RuntimeInstant(20),
                delegate { calls.Add("future"); });

            ScheduledTaskDrainResult first = queue.DrainDue(10);

            CollectionAssert.AreEqual(new string[] { "due" }, calls);
            Assert.AreEqual(1, clock.CurrentTimeReadCount);
            Assert.AreEqual(1, first.AttemptedCount);
            Assert.IsFalse(first.HasRemainingDueWork);

            queue.DrainDue(10);
            CollectionAssert.AreEqual(
                new string[] { "due", "future" },
                calls);
            Assert.AreEqual(2, clock.CurrentTimeReadCount);
        }

        [Test]
        public void BoundedDrain_ReportsRemainingWorkAndExactExhaustion()
        {
            ManualRuntimeClock clock = new ManualRuntimeClock();
            ScheduledTaskQueue queue = new ScheduledTaskQueue(clock);
            List<string> calls = new List<string>();

            queue.Schedule(
                RuntimeInstant.Zero,
                delegate { calls.Add("one"); });
            queue.Schedule(
                RuntimeInstant.Zero,
                delegate { calls.Add("two"); });
            queue.Schedule(
                RuntimeInstant.Zero,
                delegate { calls.Add("three"); });

            ScheduledTaskDrainResult first = queue.DrainDue(2);

            CollectionAssert.AreEqual(new string[] { "one", "two" }, calls);
            Assert.AreEqual(2, first.AttemptedCount);
            Assert.IsTrue(first.LimitReached);
            Assert.IsTrue(first.HasRemainingDueWork);

            ScheduledTaskDrainResult second = queue.DrainDue(1);

            CollectionAssert.AreEqual(
                new string[] { "one", "two", "three" },
                calls);
            Assert.AreEqual(1, second.AttemptedCount);
            Assert.IsFalse(second.LimitReached);
            Assert.IsFalse(second.HasRemainingDueWork);
        }

        [Test]
        public void BoundedDrain_SkipsCancelledRemainderBeforeReportingLimit()
        {
            ManualRuntimeClock clock = new ManualRuntimeClock();
            ScheduledTaskQueue queue = new ScheduledTaskQueue(clock);
            IScheduledTaskHandle cancelled = null;

            queue.Schedule(RuntimeInstant.Zero, delegate { });
            cancelled = queue.Schedule(RuntimeInstant.Zero, delegate { });
            Assert.IsTrue(cancelled.Cancel());

            ScheduledTaskDrainResult result = queue.DrainDue(1);

            Assert.AreEqual(1, result.AttemptedCount);
            Assert.AreEqual(1, result.CancellationSkipCount);
            Assert.IsFalse(result.LimitReached);
            Assert.IsFalse(result.HasRemainingDueWork);
        }

        [Test]
        public void DrainDue_RejectsNonpositiveLimitsWithoutTaskChanges()
        {
            ScheduledTaskQueue queue =
                new ScheduledTaskQueue(new ManualRuntimeClock());
            IScheduledTaskHandle handle = queue.Schedule(
                RuntimeInstant.Zero,
                delegate { });

            AssertArgumentOutOfRange(
                delegate { queue.DrainDue(0); },
                "maxCallbacks");
            AssertArgumentOutOfRange(
                delegate { queue.DrainDue(-1); },
                "maxCallbacks");

            Assert.AreEqual(ScheduledTaskState.Pending, handle.State);
            Assert.AreEqual(1, queue.DrainDue(1).AttemptedCount);
        }

        [Test]
        public void CallbackFailures_AreIsolatedOrderedAndImmutableDiagnostics()
        {
            ManualRuntimeClock clock =
                new ManualRuntimeClock(new RuntimeInstant(5));
            ScheduledTaskQueue queue = new ScheduledTaskQueue(clock);
            ThrowingCallbackTarget target =
                new ThrowingCallbackTarget();
            int successfulCalls = 0;

            IScheduledTaskHandle first = queue.Schedule(
                new RuntimeInstant(4),
                target.ThrowFirst);
            queue.Schedule(
                new RuntimeInstant(5),
                delegate { successfulCalls++; });
            IScheduledTaskHandle third = queue.Schedule(
                new RuntimeInstant(5),
                target.ThrowThird);

            ScheduledTaskDrainResult result = queue.DrainDue(3);

            Assert.AreEqual(3, result.AttemptedCount);
            Assert.AreEqual(1, result.SuccessfulCompletionCount);
            Assert.AreEqual(2, result.CallbackFailureCount);
            Assert.AreEqual(
                result.SuccessfulCompletionCount +
                    result.CallbackFailureCount,
                result.AttemptedCount);
            Assert.AreEqual(1, successfulCalls);
            Assert.AreEqual(ScheduledTaskState.Completed, first.State);
            Assert.AreEqual(ScheduledTaskState.Completed, third.State);

            ScheduledTaskCallbackFailure firstFailure =
                result.GetCallbackFailure(0);
            ScheduledTaskCallbackFailure secondFailure =
                result.GetCallbackFailure(1);
            Assert.AreEqual(4, firstFailure.DueTime.Milliseconds);
            Assert.AreEqual(1, firstFailure.InsertionSequence);
            Assert.AreEqual(
                typeof(ThrowingCallbackTarget),
                firstFailure.CallbackTargetType);
            Assert.AreEqual("ThrowFirst", firstFailure.CallbackMethodName);
            Assert.AreEqual(
                typeof(InvalidOperationException),
                firstFailure.ExceptionType);
            Assert.AreEqual("first failure", firstFailure.ExceptionMessage);
            Assert.AreEqual(3, secondFailure.InsertionSequence);
            Assert.AreEqual("ThrowThird", secondFailure.CallbackMethodName);
            Assert.AreEqual(
                typeof(ArgumentException),
                secondFailure.ExceptionType);
            Assert.AreEqual("third failure", secondFailure.ExceptionMessage);
        }

        [Test]
        public void ReentrantDrain_IsCapturedAndOuterDrainContinues()
        {
            ScheduledTaskQueue queue =
                new ScheduledTaskQueue(new ManualRuntimeClock());
            int laterCalls = 0;

            queue.Schedule(
                RuntimeInstant.Zero,
                delegate { queue.DrainDue(1); });
            queue.Schedule(
                RuntimeInstant.Zero,
                delegate { laterCalls++; });

            ScheduledTaskDrainResult result = queue.DrainDue(2);

            Assert.AreEqual(2, result.AttemptedCount);
            Assert.AreEqual(1, result.SuccessfulCompletionCount);
            Assert.AreEqual(1, result.CallbackFailureCount);
            Assert.AreEqual(
                typeof(InvalidOperationException),
                result.GetCallbackFailure(0).ExceptionType);
            Assert.AreEqual(1, laterCalls);

            ScheduledTaskDrainResult empty = queue.DrainDue(1);
            Assert.AreEqual(0, empty.AttemptedCount);
        }

        [Test]
        public void CallbackMayDrainAnotherQueueInstance()
        {
            ManualRuntimeClock clock = new ManualRuntimeClock();
            ScheduledTaskQueue first = new ScheduledTaskQueue(clock);
            ScheduledTaskQueue second = new ScheduledTaskQueue(clock);
            int secondCalls = 0;

            second.Schedule(
                RuntimeInstant.Zero,
                delegate { secondCalls++; });
            first.Schedule(
                RuntimeInstant.Zero,
                delegate { second.DrainDue(1); });

            ScheduledTaskDrainResult result = first.DrainDue(1);

            Assert.AreEqual(1, result.SuccessfulCompletionCount);
            Assert.AreEqual(0, result.CallbackFailureCount);
            Assert.AreEqual(1, secondCalls);
        }

        [Test]
        public void Shutdown_IsIdempotentTerminalAndLeavesHandlesReadable()
        {
            ScheduledTaskQueue queue =
                new ScheduledTaskQueue(new ManualRuntimeClock());
            int calls = 0;
            IScheduledTaskHandle handle = queue.Schedule(
                RuntimeInstant.Zero,
                delegate { calls++; });

            queue.Shutdown();
            queue.Shutdown();

            Assert.AreEqual(0, calls);
            Assert.AreEqual(ScheduledTaskState.Cancelled, handle.State);
            Assert.IsFalse(handle.Cancel());
            Assert.IsFalse(queue.Initialize().IsSuccessful);
            AssertObjectDisposed(
                delegate
                {
                    queue.Schedule(RuntimeInstant.Zero, delegate { });
                });
            AssertObjectDisposed(delegate { queue.DrainDue(1); });
        }

        [Test]
        public void ShutdownDuringCallback_CompletesCurrentAndStopsRemainingWork()
        {
            ScheduledTaskQueue queue =
                new ScheduledTaskQueue(new ManualRuntimeClock());
            List<string> calls = new List<string>();
            IScheduledTaskHandle current = null;
            IScheduledTaskHandle later = null;

            current = queue.Schedule(
                RuntimeInstant.Zero,
                delegate
                {
                    calls.Add("current");
                    queue.Shutdown();
                    Assert.AreEqual(
                        ScheduledTaskState.Executing,
                        current.State);
                });
            later = queue.Schedule(
                RuntimeInstant.Zero,
                delegate { calls.Add("later"); });

            ScheduledTaskDrainResult result = queue.DrainDue(10);

            CollectionAssert.AreEqual(new string[] { "current" }, calls);
            Assert.AreEqual(ScheduledTaskState.Completed, current.State);
            Assert.AreEqual(ScheduledTaskState.Cancelled, later.State);
            Assert.AreEqual(1, result.AttemptedCount);
            Assert.AreEqual(1, result.CancellationSkipCount);
            Assert.IsFalse(result.HasRemainingDueWork);
        }

        [Test]
        public void Composition_ShutsQueueDownBeforeItsClock()
        {
            LifecycleClock clock = new LifecycleClock();
            ScheduledTaskQueue queue = new ScheduledTaskQueue(clock);
            IScheduledTaskHandle handle = queue.Schedule(
                RuntimeInstant.Zero,
                delegate { });
            clock.HandleToObserve = handle;

            RuntimeCompositionResult result =
                RuntimeCompositionRoot.Compose(clock, queue);
            result.Runtime.Dispose();

            Assert.IsTrue(result.Runtime.ShutdownResult.IsSuccessful);
            Assert.IsTrue(clock.SawCancelledHandleDuringShutdown);
            Assert.AreEqual(ScheduledTaskState.Cancelled, handle.State);
        }

        [Test]
        public void SeparateQueues_HaveIndependentStateSequenceAndDiagnostics()
        {
            ManualRuntimeClock firstClock = new ManualRuntimeClock();
            ManualRuntimeClock secondClock = new ManualRuntimeClock();
            ScheduledTaskQueue first =
                new ScheduledTaskQueue(firstClock);
            ScheduledTaskQueue second =
                new ScheduledTaskQueue(secondClock);

            first.Schedule(
                RuntimeInstant.Zero,
                delegate { throw new InvalidOperationException("first"); });
            second.Schedule(
                RuntimeInstant.Zero,
                delegate { });

            ScheduledTaskDrainResult firstResult = first.DrainDue(1);
            ScheduledTaskDrainResult secondResult = second.DrainDue(1);

            Assert.AreEqual(
                1,
                firstResult.GetCallbackFailure(0).InsertionSequence);
            Assert.AreEqual(1, secondResult.SuccessfulCompletionCount);
            Assert.AreEqual(0, secondResult.CallbackFailureCount);

            IScheduledTaskHandle secondPending = second.Schedule(
                new RuntimeInstant(1),
                delegate { });
            first.Shutdown();
            Assert.AreEqual(
                ScheduledTaskState.Pending,
                secondPending.State);
        }

        [Test]
        public void EmptyDrain_ReturnsSuccessfulZeroResult()
        {
            ScheduledTaskQueue queue =
                new ScheduledTaskQueue(new ManualRuntimeClock());

            ScheduledTaskDrainResult result = queue.DrainDue(3);

            Assert.AreEqual(0, result.AttemptedCount);
            Assert.AreEqual(0, result.SuccessfulCompletionCount);
            Assert.AreEqual(0, result.CancellationSkipCount);
            Assert.AreEqual(0, result.CallbackFailureCount);
            Assert.IsFalse(result.LimitReached);
            Assert.IsFalse(result.HasRemainingDueWork);
        }

        private static void SetLastInsertionSequence(
            ScheduledTaskQueue queue,
            long value)
        {
            FieldInfo field = typeof(ScheduledTaskQueue).GetField(
                "_lastInsertionSequence",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field);
            field.SetValue(queue, value);
        }

        private static void AssertArgumentNull(
            TestDelegate action,
            string parameterName)
        {
            try
            {
                action();
                Assert.Fail("Expected ArgumentNullException.");
            }
            catch (ArgumentNullException exception)
            {
                Assert.AreEqual(parameterName, exception.ParamName);
            }
        }

        private static void AssertArgumentOutOfRange(
            TestDelegate action,
            string parameterName)
        {
            try
            {
                action();
                Assert.Fail("Expected ArgumentOutOfRangeException.");
            }
            catch (ArgumentOutOfRangeException exception)
            {
                Assert.AreEqual(parameterName, exception.ParamName);
            }
        }

        private static void AssertOverflow(TestDelegate action)
        {
            try
            {
                action();
                Assert.Fail("Expected OverflowException.");
            }
            catch (OverflowException)
            {
            }
        }

        private static void AssertObjectDisposed(TestDelegate action)
        {
            try
            {
                action();
                Assert.Fail("Expected ObjectDisposedException.");
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private sealed class ThrowingCallbackTarget
        {
            public void ThrowFirst()
            {
                throw new InvalidOperationException("first failure");
            }

            public void ThrowThird()
            {
                throw new ArgumentException("third failure");
            }
        }

        private sealed class CountingClock : IRuntimeClock
        {
            private RuntimeInstant _currentTime;

            public CountingClock(RuntimeInstant currentTime)
            {
                _currentTime = currentTime;
            }

            public int CurrentTimeReadCount { get; private set; }

            public RuntimeInstant CurrentTime
            {
                get
                {
                    CurrentTimeReadCount++;
                    return _currentTime;
                }
            }

            public bool IsPaused
            {
                get { return false; }
            }

            public void SetCurrentTime(RuntimeInstant currentTime)
            {
                _currentTime = currentTime;
            }
        }

        private sealed class LifecycleClock :
            IRuntimeClock,
            IRuntimeService
        {
            public IScheduledTaskHandle HandleToObserve { private get; set; }

            public bool SawCancelledHandleDuringShutdown
            {
                get;
                private set;
            }

            public RuntimeInstant CurrentTime
            {
                get { return RuntimeInstant.Zero; }
            }

            public bool IsPaused
            {
                get { return false; }
            }

            public ServiceInitializationResult Initialize()
            {
                return ServiceInitializationResult.Success();
            }

            public void Shutdown()
            {
                SawCancelledHandleDuringShutdown =
                    HandleToObserve.State ==
                    ScheduledTaskState.Cancelled;
            }
        }
    }
}
