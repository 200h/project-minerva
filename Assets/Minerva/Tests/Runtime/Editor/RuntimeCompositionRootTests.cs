using System;
using System.Collections.Generic;
using Minerva.Core;
using NUnit.Framework;

namespace Minerva.Tests.Runtime
{
    public sealed class RuntimeCompositionRootTests
    {
        [Test]
        public void Compose_ReturnsInitializedRuntimeAndNarrowCapabilities()
        {
            RuntimeCompositionResult result = RuntimeCompositionRoot.Compose();

            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNotNull(result.Runtime);
            Assert.IsTrue(result.InitializationResult.IsSuccessful);
            Assert.IsNull(result.CleanupResult);
            Assert.IsNotNull(result.Runtime.EventPublisher);
            Assert.IsNotNull(result.Runtime.EventSubscriber);
            Assert.IsFalse(result.Runtime.EventPublisher is InMemoryEventBus);
            Assert.IsFalse(result.Runtime.EventSubscriber is InMemoryEventBus);

            result.Runtime.Dispose();
        }

        [Test]
        public void Compose_InitializesTransferredServicesOnceInSuppliedOrder()
        {
            List<string> calls = new List<string>();
            RecordingService first = new RecordingService("first", calls);
            RecordingService second = new RecordingService("second", calls);

            RuntimeCompositionResult result =
                RuntimeCompositionRoot.Compose(first, second);

            CollectionAssert.AreEqual(
                new string[] { "initialize:first", "initialize:second" },
                calls);

            result.Runtime.Dispose();
        }

        [Test]
        public void Dispose_ShutsDownServicesInReverseOrderBeforeEventBus()
        {
            List<string> calls = new List<string>();
            PublishingShutdownService first =
                new PublishingShutdownService("first", calls);
            PublishingShutdownService second =
                new PublishingShutdownService("second", calls);
            RuntimeCompositionResult result =
                RuntimeCompositionRoot.Compose(first, second);
            first.Publisher = result.Runtime.EventPublisher;
            second.Publisher = result.Runtime.EventPublisher;

            result.Runtime.Dispose();

            CollectionAssert.AreEqual(
                new string[]
                {
                    "initialize:first",
                    "initialize:second",
                    "shutdown:second",
                    "publish:second",
                    "shutdown:first",
                    "publish:first"
                },
                calls);
            Assert.IsTrue(result.Runtime.ShutdownResult.IsSuccessful);
            AssertObjectDisposed(
                delegate
                {
                    result.Runtime.EventPublisher.Publish(new TestEvent());
                });
        }

        [Test]
        public void Dispose_IsIdempotentAndDoesNotRepeatOwnedCleanup()
        {
            List<string> calls = new List<string>();
            RecordingService service = new RecordingService("service", calls);
            RuntimeCompositionResult result =
                RuntimeCompositionRoot.Compose(service);

            result.Runtime.Dispose();
            RuntimeShutdownResult firstResult =
                result.Runtime.ShutdownResult;
            result.Runtime.Dispose();

            Assert.AreSame(firstResult, result.Runtime.ShutdownResult);
            CollectionAssert.AreEqual(
                new string[]
                {
                    "initialize:service",
                    "shutdown:service"
                },
                calls);
        }

        [Test]
        public void ReportedFailure_ReturnsNoRuntimeAndPreservesDiagnostics()
        {
            List<string> calls = new List<string>();
            RecordingService initialized =
                new RecordingService("initialized", calls);
            FailingService failing =
                new FailingService("reported failure", calls);

            RuntimeCompositionResult result =
                RuntimeCompositionRoot.Compose(initialized, failing);

            Assert.IsFalse(result.IsSuccessful);
            Assert.IsNull(result.Runtime);
            Assert.IsFalse(result.InitializationResult.IsSuccessful);
            Assert.AreEqual(
                typeof(FailingService),
                result.InitializationResult.FailedServiceType);
            Assert.AreEqual(
                "reported failure",
                result.InitializationResult.FailureReason);
            Assert.IsTrue(result.CleanupResult.IsSuccessful);
            CollectionAssert.AreEqual(
                new string[]
                {
                    "initialize:initialized",
                    "initialize:failing",
                    "shutdown:initialized"
                },
                calls);
        }

        [Test]
        public void NullResultFailure_ReturnsNoRuntimeAndCleansUp()
        {
            List<string> calls = new List<string>();
            RecordingService initialized =
                new RecordingService("initialized", calls);

            RuntimeCompositionResult result =
                RuntimeCompositionRoot.Compose(
                    initialized,
                    new NullResultService());

            Assert.IsFalse(result.IsSuccessful);
            Assert.IsNull(result.Runtime);
            Assert.AreEqual(
                typeof(NullResultService),
                result.InitializationResult.FailedServiceType);
            Assert.AreEqual(
                "The service returned no initialization result.",
                result.InitializationResult.FailureReason);
            Assert.IsTrue(result.CleanupResult.IsSuccessful);
            CollectionAssert.AreEqual(
                new string[]
                {
                    "initialize:initialized",
                    "shutdown:initialized"
                },
                calls);
        }

        [Test]
        public void ThrownFailure_ReturnsNoRuntimeAndCleansUp()
        {
            List<string> calls = new List<string>();
            RecordingService initialized =
                new RecordingService("initialized", calls);

            RuntimeCompositionResult result =
                RuntimeCompositionRoot.Compose(
                    initialized,
                    new ThrowingInitializationService());

            Assert.IsFalse(result.IsSuccessful);
            Assert.IsNull(result.Runtime);
            Assert.AreEqual(
                typeof(ThrowingInitializationService),
                result.InitializationResult.FailedServiceType);
            Assert.AreEqual(
                "thrown failure",
                result.InitializationResult.FailureReason);
            Assert.IsTrue(result.CleanupResult.IsSuccessful);
            CollectionAssert.AreEqual(
                new string[]
                {
                    "initialize:initialized",
                    "shutdown:initialized"
                },
                calls);
        }

        [Test]
        public void StartupAndCleanupFailuresRemainSeparatelyInspectable()
        {
            List<string> calls = new List<string>();
            ThrowingShutdownService cleanupFailure =
                new ThrowingShutdownService(calls);
            FailingService startupFailure =
                new FailingService("startup failure", calls);

            RuntimeCompositionResult result =
                RuntimeCompositionRoot.Compose(
                    cleanupFailure,
                    startupFailure);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(
                "startup failure",
                result.InitializationResult.FailureReason);
            Assert.IsFalse(result.CleanupResult.IsSuccessful);
            Assert.AreEqual(1, result.CleanupResult.FailureCount);
            RuntimeShutdownFailure failure =
                result.CleanupResult.GetFailure(0);
            Assert.AreEqual(
                typeof(ThrowingShutdownService),
                failure.ServiceType);
            Assert.AreEqual(
                typeof(InvalidOperationException),
                failure.ExceptionType);
            Assert.AreEqual("cleanup failure", failure.FailureReason);
        }

        [Test]
        public void SeparateRuntimes_DoNotShareEventSubscriptions()
        {
            int firstCalls = 0;
            RuntimeCompositionResult first =
                RuntimeCompositionRoot.Compose();
            first.Runtime.EventSubscriber.Subscribe<TestEvent>(
                delegate { firstCalls++; });

            RuntimeCompositionResult second =
                RuntimeCompositionRoot.Compose();
            second.Runtime.EventPublisher.Publish(new TestEvent());

            Assert.AreEqual(0, firstCalls);

            first.Runtime.Dispose();
            second.Runtime.Dispose();
        }

        [Test]
        public void Compose_RejectsInvalidServiceListsBeforeOwnershipTransfer()
        {
            RecordingService service =
                new RecordingService("service", new List<string>());

            AssertArgumentNull(
                delegate
                {
                    RuntimeCompositionRoot.Compose(
                        (IRuntimeService[])null);
                },
                "services");
            AssertArgumentException(
                delegate
                {
                    RuntimeCompositionRoot.Compose(
                        new IRuntimeService[] { null });
                },
                "services");
            AssertArgumentException(
                delegate
                {
                    RuntimeCompositionRoot.Compose(service, service);
                },
                "services");
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

        private static void AssertArgumentException(
            TestDelegate action,
            string parameterName)
        {
            try
            {
                action();
                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException exception)
            {
                Assert.AreEqual(parameterName, exception.ParamName);
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

        private sealed class TestEvent : IEvent
        {
        }

        private class RecordingService : IRuntimeService
        {
            private readonly string _name;
            private readonly List<string> _calls;

            public RecordingService(string name, List<string> calls)
            {
                _name = name;
                _calls = calls;
            }

            public virtual ServiceInitializationResult Initialize()
            {
                _calls.Add("initialize:" + _name);
                return ServiceInitializationResult.Success();
            }

            public virtual void Shutdown()
            {
                _calls.Add("shutdown:" + _name);
            }
        }

        private sealed class PublishingShutdownService :
            RecordingService
        {
            private readonly string _name;
            private readonly List<string> _calls;

            public PublishingShutdownService(
                string name,
                List<string> calls)
                : base(name, calls)
            {
                _name = name;
                _calls = calls;
            }

            public IEventPublisher Publisher { private get; set; }

            public override void Shutdown()
            {
                _calls.Add("shutdown:" + _name);
                Publisher.Publish(new TestEvent());
                _calls.Add("publish:" + _name);
            }
        }

        private sealed class FailingService : IRuntimeService
        {
            private readonly string _failureReason;
            private readonly List<string> _calls;

            public FailingService(
                string failureReason,
                List<string> calls)
            {
                _failureReason = failureReason;
                _calls = calls;
            }

            public ServiceInitializationResult Initialize()
            {
                _calls.Add("initialize:failing");
                return ServiceInitializationResult.Failure(
                    _failureReason);
            }

            public void Shutdown()
            {
                _calls.Add("shutdown:failing");
            }
        }

        private sealed class NullResultService : IRuntimeService
        {
            public ServiceInitializationResult Initialize()
            {
                return null;
            }

            public void Shutdown()
            {
            }
        }

        private sealed class ThrowingInitializationService :
            IRuntimeService
        {
            public ServiceInitializationResult Initialize()
            {
                throw new InvalidOperationException("thrown failure");
            }

            public void Shutdown()
            {
            }
        }

        private sealed class ThrowingShutdownService : IRuntimeService
        {
            private readonly List<string> _calls;

            public ThrowingShutdownService(List<string> calls)
            {
                _calls = calls;
            }

            public ServiceInitializationResult Initialize()
            {
                _calls.Add("initialize:cleanup-failure");
                return ServiceInitializationResult.Success();
            }

            public void Shutdown()
            {
                _calls.Add("shutdown:cleanup-failure");
                throw new InvalidOperationException("cleanup failure");
            }
        }
    }
}
