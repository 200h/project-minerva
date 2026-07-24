using System;
using Minerva.Core;
using NUnit.Framework;

namespace Minerva.Tests.Runtime
{
    public sealed class ManualRuntimeClockTests
    {
        [Test]
        public void RuntimeInstant_UsesNonnegativeExactMillisecondsAndOrdering()
        {
            RuntimeInstant earlier = new RuntimeInstant(1);
            RuntimeInstant equal = new RuntimeInstant(1);
            RuntimeInstant later = new RuntimeInstant(long.MaxValue);

            Assert.AreEqual(0, RuntimeInstant.Zero.Milliseconds);
            Assert.AreEqual(1, earlier.Milliseconds);
            Assert.AreEqual(long.MaxValue, later.Milliseconds);
            Assert.IsTrue(earlier == equal);
            Assert.IsFalse(earlier != equal);
            Assert.IsTrue(earlier < later);
            Assert.IsTrue(earlier <= equal);
            Assert.IsTrue(later > earlier);
            Assert.IsTrue(later >= earlier);
            Assert.AreEqual(0, earlier.CompareTo(equal));
            Assert.AreEqual(earlier.GetHashCode(), equal.GetHashCode());
        }

        [Test]
        public void RuntimeDuration_UsesNonnegativeExactMillisecondsAndOrdering()
        {
            RuntimeDuration shorter = new RuntimeDuration(1);
            RuntimeDuration equal = new RuntimeDuration(1);
            RuntimeDuration longer = new RuntimeDuration(long.MaxValue);

            Assert.AreEqual(0, RuntimeDuration.Zero.Milliseconds);
            Assert.AreEqual(1, shorter.Milliseconds);
            Assert.AreEqual(long.MaxValue, longer.Milliseconds);
            Assert.IsTrue(shorter == equal);
            Assert.IsFalse(shorter != equal);
            Assert.IsTrue(shorter < longer);
            Assert.IsTrue(shorter <= equal);
            Assert.IsTrue(longer > shorter);
            Assert.IsTrue(longer >= shorter);
            Assert.AreEqual(0, shorter.CompareTo(equal));
            Assert.AreEqual(shorter.GetHashCode(), equal.GetHashCode());
        }

        [Test]
        public void ValueConstruction_RejectsNegativeMilliseconds()
        {
            AssertArgumentOutOfRange(
                delegate { new RuntimeInstant(-1); },
                "milliseconds");
            AssertArgumentOutOfRange(
                delegate { new RuntimeDuration(-1); },
                "milliseconds");
        }

        [Test]
        public void Construction_StartsRunningAtZeroOrExactInitialTime()
        {
            ManualRuntimeClock zeroClock = new ManualRuntimeClock();
            ManualRuntimeClock initialClock =
                new ManualRuntimeClock(new RuntimeInstant(1234));

            Assert.AreEqual(RuntimeInstant.Zero, zeroClock.CurrentTime);
            Assert.IsFalse(zeroClock.IsPaused);
            Assert.AreEqual(1234, initialClock.CurrentTime.Milliseconds);
            Assert.IsFalse(initialClock.IsPaused);
        }

        [Test]
        public void Advance_IsExactMonotonicAndZeroIsIdempotent()
        {
            ManualRuntimeClock clock =
                new ManualRuntimeClock(new RuntimeInstant(10));

            clock.Advance(new RuntimeDuration(25));
            RuntimeInstant advanced = clock.CurrentTime;
            clock.Advance(RuntimeDuration.Zero);
            clock.Advance(RuntimeDuration.Zero);

            Assert.AreEqual(35, advanced.Milliseconds);
            Assert.AreEqual(advanced, clock.CurrentTime);
        }

        [Test]
        public void PauseAndResume_AreIdempotentAndStateNeutral()
        {
            ManualRuntimeClock clock =
                new ManualRuntimeClock(new RuntimeInstant(10));

            clock.Pause();
            clock.Pause();
            Assert.IsTrue(clock.IsPaused);
            Assert.AreEqual(10, clock.CurrentTime.Milliseconds);

            clock.Resume();
            clock.Resume();
            Assert.IsFalse(clock.IsPaused);
            Assert.AreEqual(10, clock.CurrentTime.Milliseconds);
        }

        [Test]
        public void AdvanceWhilePaused_IsIgnoredWithoutHiddenAccumulation()
        {
            ManualRuntimeClock clock =
                new ManualRuntimeClock(new RuntimeInstant(10));
            clock.Pause();

            clock.Advance(new RuntimeDuration(25));
            clock.Advance(RuntimeDuration.Zero);
            clock.Resume();

            Assert.AreEqual(10, clock.CurrentTime.Milliseconds);
            clock.Advance(new RuntimeDuration(1));
            Assert.AreEqual(11, clock.CurrentTime.Milliseconds);
        }

        [Test]
        public void AdvanceWhilePaused_IgnoresOtherwiseOverflowingDuration()
        {
            ManualRuntimeClock clock =
                new ManualRuntimeClock(new RuntimeInstant(long.MaxValue));
            clock.Pause();

            clock.Advance(new RuntimeDuration(1));

            Assert.AreEqual(long.MaxValue, clock.CurrentTime.Milliseconds);
        }

        [Test]
        public void Advance_ThrowsOnOverflowAndPreservesCurrentTime()
        {
            ManualRuntimeClock clock =
                new ManualRuntimeClock(
                    new RuntimeInstant(long.MaxValue - 1));
            clock.Advance(new RuntimeDuration(1));
            RuntimeInstant maximum = clock.CurrentTime;

            AssertOverflow(
                delegate
                {
                    clock.Advance(new RuntimeDuration(1));
                });

            Assert.AreEqual(long.MaxValue, maximum.Milliseconds);
            Assert.AreEqual(maximum, clock.CurrentTime);
        }

        [Test]
        public void Initialize_IsRepeatablySuccessfulAndStateNeutral()
        {
            ManualRuntimeClock clock =
                new ManualRuntimeClock(new RuntimeInstant(20));
            clock.Pause();

            ServiceInitializationResult first = clock.Initialize();
            ServiceInitializationResult second = clock.Initialize();

            Assert.IsTrue(first.IsSuccessful);
            Assert.IsTrue(second.IsSuccessful);
            Assert.AreEqual(20, clock.CurrentTime.Milliseconds);
            Assert.IsTrue(clock.IsPaused);
        }

        [Test]
        public void Shutdown_IsIdempotentTerminalAndLeavesReadsAvailable()
        {
            ManualRuntimeClock clock =
                new ManualRuntimeClock(new RuntimeInstant(20));
            clock.Pause();

            clock.Shutdown();
            clock.Shutdown();

            Assert.AreEqual(20, clock.CurrentTime.Milliseconds);
            Assert.IsTrue(clock.IsPaused);
            AssertObjectDisposed(delegate { clock.Pause(); });
            AssertObjectDisposed(delegate { clock.Resume(); });
            AssertObjectDisposed(
                delegate
                {
                    clock.Advance(new RuntimeDuration(1));
                });
            Assert.AreEqual(20, clock.CurrentTime.Milliseconds);
            Assert.IsTrue(clock.IsPaused);
        }

        [Test]
        public void InitializeAfterShutdown_ReturnsFailureWithoutRevival()
        {
            ManualRuntimeClock clock =
                new ManualRuntimeClock(new RuntimeInstant(20));
            clock.Shutdown();

            ServiceInitializationResult result = clock.Initialize();

            Assert.IsFalse(result.IsSuccessful);
            Assert.IsFalse(string.IsNullOrEmpty(result.FailureReason));
            Assert.AreEqual(20, clock.CurrentTime.Milliseconds);
            Assert.IsFalse(clock.IsPaused);
            AssertObjectDisposed(delegate { clock.Resume(); });
        }

        [Test]
        public void Composition_OwnsClockThroughRuntimeServiceContract()
        {
            ManualRuntimeClock clock =
                new ManualRuntimeClock(new RuntimeInstant(5));
            IRuntimeClock readClock = clock;
            IRuntimeClockControl control = clock;

            RuntimeCompositionResult result =
                RuntimeCompositionRoot.Compose(clock);

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(5, readClock.CurrentTime.Milliseconds);
            control.Advance(new RuntimeDuration(2));
            Assert.AreEqual(7, readClock.CurrentTime.Milliseconds);

            result.Runtime.Dispose();

            Assert.IsTrue(result.Runtime.ShutdownResult.IsSuccessful);
            Assert.AreEqual(7, readClock.CurrentTime.Milliseconds);
            AssertObjectDisposed(
                delegate
                {
                    control.Advance(new RuntimeDuration(1));
                });
        }

        [Test]
        public void SeparateInstances_ShareNoMutableState()
        {
            ManualRuntimeClock first = new ManualRuntimeClock();
            ManualRuntimeClock second = new ManualRuntimeClock();

            first.Advance(new RuntimeDuration(10));
            first.Pause();

            Assert.AreEqual(10, first.CurrentTime.Milliseconds);
            Assert.IsTrue(first.IsPaused);
            Assert.AreEqual(RuntimeInstant.Zero, second.CurrentTime);
            Assert.IsFalse(second.IsPaused);

            second.Advance(new RuntimeDuration(3));

            Assert.AreEqual(10, first.CurrentTime.Milliseconds);
            Assert.AreEqual(3, second.CurrentTime.Milliseconds);
        }

        [Test]
        public void ManualClock_DoesNotAdvanceWithoutExplicitControl()
        {
            ManualRuntimeClock clock =
                new ManualRuntimeClock(new RuntimeInstant(42));

            RuntimeInstant firstRead = clock.CurrentTime;
            RuntimeInstant secondRead = clock.CurrentTime;

            Assert.AreEqual(firstRead, secondRead);
            Assert.AreEqual(42, secondRead.Milliseconds);
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
    }
}
