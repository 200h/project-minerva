using System;
using System.Collections.Generic;
using Minerva.Core;
using NUnit.Framework;

namespace Minerva.Tests.Runtime
{
    public sealed class RuntimeBootstrapTests
    {
        [Test]
        public void Initialize_UsesRegistrationOrder()
        {
            List<string> calls = new List<string>();
            RuntimeBootstrap runtime = new RuntimeBootstrap();
            runtime.Register(new RecordingService("first", calls));
            runtime.Register(new RecordingService("second", calls));

            RuntimeInitializationResult result = runtime.Initialize();

            Assert.IsTrue(result.IsSuccessful);
            CollectionAssert.AreEqual(
                new string[] { "initialize:first", "initialize:second" },
                calls);
            Assert.AreEqual(RuntimeLifecycleState.Running, runtime.State);
        }

        [Test]
        public void Register_RejectsSameInstanceTwice()
        {
            RuntimeBootstrap runtime = new RuntimeBootstrap();
            RecordingService service = new RecordingService("service", new List<string>());
            runtime.Register(service);

            AssertInvalidOperation(delegate { runtime.Register(service); });
        }

        [Test]
        public void Initialize_StopsAtReportedFailureAndDescribesIt()
        {
            List<string> calls = new List<string>();
            RuntimeBootstrap runtime = new RuntimeBootstrap();
            runtime.Register(new RecordingService("first", calls));
            FailingService failingService = new FailingService("reported failure", calls);
            runtime.Register(failingService);
            runtime.Register(new RecordingService("never", calls));

            RuntimeInitializationResult result = runtime.Initialize();

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(typeof(FailingService), result.FailedServiceType);
            Assert.AreEqual("reported failure", result.FailureReason);
            CollectionAssert.AreEqual(
                new string[] { "initialize:first", "initialize:failing" },
                calls);
            Assert.AreEqual(RuntimeLifecycleState.InitializationFailed, runtime.State);
        }

        [Test]
        public void Initialize_StopsAtExceptionAndDescribesIt()
        {
            RuntimeBootstrap runtime = new RuntimeBootstrap();
            ThrowingService service = new ThrowingService();
            runtime.Register(service);

            RuntimeInitializationResult result = runtime.Initialize();

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(typeof(ThrowingService), result.FailedServiceType);
            Assert.AreEqual("exception failure", result.FailureReason);
        }

        [Test]
        public void Shutdown_StopsOnlySuccessfulServicesInReverseOrder()
        {
            List<string> calls = new List<string>();
            RuntimeBootstrap runtime = new RuntimeBootstrap();
            runtime.Register(new RecordingService("first", calls));
            runtime.Register(new RecordingService("second", calls));
            runtime.Register(new FailingService("failure", calls));

            runtime.Initialize();
            runtime.Shutdown();

            CollectionAssert.AreEqual(
                new string[]
                {
                    "initialize:first",
                    "initialize:second",
                    "initialize:failing",
                    "shutdown:second",
                    "shutdown:first"
                },
                calls);
            Assert.AreEqual(RuntimeLifecycleState.ShutDown, runtime.State);
        }

        [Test]
        public void Shutdown_IsIdempotent()
        {
            List<string> calls = new List<string>();
            RuntimeBootstrap runtime = new RuntimeBootstrap();
            runtime.Register(new RecordingService("service", calls));
            runtime.Initialize();

            runtime.Shutdown();
            runtime.Shutdown();

            CollectionAssert.AreEqual(
                new string[] { "initialize:service", "shutdown:service" },
                calls);
        }

        [Test]
        public void Initialize_RejectsSecondAttempt()
        {
            RuntimeBootstrap runtime = new RuntimeBootstrap();
            runtime.Initialize();

            AssertInvalidOperation(delegate { runtime.Initialize(); });
        }

        [Test]
        public void SeparateInstances_CanBeConstructedInitializedAndDisposedRepeatedly()
        {
            for (int index = 0; index < 3; index++)
            {
                List<string> calls = new List<string>();
                RuntimeBootstrap runtime = new RuntimeBootstrap();
                runtime.Register(new RecordingService("service", calls));

                Assert.IsTrue(runtime.Initialize().IsSuccessful);
                runtime.Dispose();
                runtime.Dispose();

                CollectionAssert.AreEqual(
                    new string[] { "initialize:service", "shutdown:service" },
                    calls);
                Assert.AreEqual(RuntimeLifecycleState.Disposed, runtime.State);
            }
        }

        private static void AssertInvalidOperation(TestDelegate action)
        {
            try
            {
                action();
                Assert.Fail("Expected InvalidOperationException.");
            }
            catch (InvalidOperationException)
            {
            }
        }

        private sealed class RecordingService : IRuntimeService
        {
            private readonly string _name;
            private readonly List<string> _calls;

            public RecordingService(string name, List<string> calls)
            {
                _name = name;
                _calls = calls;
            }

            public ServiceInitializationResult Initialize()
            {
                _calls.Add("initialize:" + _name);
                return ServiceInitializationResult.Success();
            }

            public void Shutdown()
            {
                _calls.Add("shutdown:" + _name);
            }
        }

        private sealed class FailingService : IRuntimeService
        {
            private readonly string _reason;
            private readonly List<string> _calls;

            public FailingService(string reason, List<string> calls)
            {
                _reason = reason;
                _calls = calls;
            }

            public ServiceInitializationResult Initialize()
            {
                _calls.Add("initialize:failing");
                return ServiceInitializationResult.Failure(_reason);
            }

            public void Shutdown()
            {
                _calls.Add("shutdown:failing");
            }
        }

        private sealed class ThrowingService : IRuntimeService
        {
            public ServiceInitializationResult Initialize()
            {
                throw new InvalidOperationException("exception failure");
            }

            public void Shutdown()
            {
            }
        }
    }
}
