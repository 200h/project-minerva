using System;
using System.Collections.Generic;
using Minerva.Core;
using NUnit.Framework;

namespace Minerva.Tests.Runtime
{
    public sealed class InMemoryEventBusTests
    {
        [Test]
        public void Publish_InvokesSameTypeSubscribersInRegistrationOrder()
        {
            List<string> calls = new List<string>();
            InMemoryEventBus eventBus = new InMemoryEventBus();
            eventBus.Subscribe<TestEvent>(delegate { calls.Add("first"); });
            eventBus.Subscribe<TestEvent>(delegate { calls.Add("second"); });

            EventPublicationResult result = eventBus.Publish(new TestEvent(1));

            CollectionAssert.AreEqual(new string[] { "first", "second" }, calls);
            Assert.IsTrue(result.IsComplete);
            Assert.IsTrue(result.IsSuccessful);
        }

        [Test]
        public void Publish_UsesExactEventTypeOnly()
        {
            int baseCalls = 0;
            int derivedCalls = 0;
            InMemoryEventBus eventBus = new InMemoryEventBus();
            eventBus.Subscribe<BaseTestEvent>(delegate { baseCalls++; });
            eventBus.Subscribe<DerivedTestEvent>(delegate { derivedCalls++; });

            eventBus.Publish(new DerivedTestEvent());

            Assert.AreEqual(0, baseCalls);
            Assert.AreEqual(1, derivedCalls);
        }

        [Test]
        public void Publish_RejectsMessageWhoseRuntimeTypeDiffersFromGenericType()
        {
            InMemoryEventBus eventBus = new InMemoryEventBus();

            AssertArgumentException(
                delegate { eventBus.Publish<BaseTestEvent>(new DerivedTestEvent()); });
        }

        [Test]
        public void Subscribe_RejectsNullHandler()
        {
            InMemoryEventBus eventBus = new InMemoryEventBus();

            AssertArgumentNull(
                delegate { eventBus.Subscribe<TestEvent>(null); },
                "handler");
        }

        [Test]
        public void Publish_RejectsNullMessage()
        {
            InMemoryEventBus eventBus = new InMemoryEventBus();

            AssertArgumentNull(
                delegate { eventBus.Publish<TestEvent>(null); },
                "eventMessage");
        }

        [Test]
        public void Subscription_DisposeIsIdempotentAndStopsDelivery()
        {
            int calls = 0;
            InMemoryEventBus eventBus = new InMemoryEventBus();
            IDisposable subscription =
                eventBus.Subscribe<TestEvent>(delegate { calls++; });

            subscription.Dispose();
            subscription.Dispose();
            eventBus.Publish(new TestEvent(1));

            Assert.AreEqual(0, calls);
        }

        [Test]
        public void Dispatch_SkipsHandlerUnsubscribedBeforeItsTurn()
        {
            List<string> calls = new List<string>();
            InMemoryEventBus eventBus = new InMemoryEventBus();
            IDisposable secondSubscription = null;
            eventBus.Subscribe<TestEvent>(
                delegate
                {
                    calls.Add("first");
                    secondSubscription.Dispose();
                });
            secondSubscription = eventBus.Subscribe<TestEvent>(
                delegate { calls.Add("second"); });

            eventBus.Publish(new TestEvent(1));

            CollectionAssert.AreEqual(new string[] { "first" }, calls);
        }

        [Test]
        public void Dispatch_DoesNotInvokeHandlerSubscribedDuringCurrentEvent()
        {
            List<string> calls = new List<string>();
            InMemoryEventBus eventBus = new InMemoryEventBus();
            bool hasSubscribed = false;
            eventBus.Subscribe<TestEvent>(
                delegate
                {
                    calls.Add("first");
                    if (!hasSubscribed)
                    {
                        hasSubscribed = true;
                        eventBus.Subscribe<TestEvent>(
                            delegate { calls.Add("late"); });
                    }
                });

            eventBus.Publish(new TestEvent(1));
            CollectionAssert.AreEqual(new string[] { "first" }, calls);

            eventBus.Publish(new TestEvent(2));
            CollectionAssert.AreEqual(
                new string[] { "first", "first", "late" },
                calls);
        }

        [Test]
        public void NestedPublications_DrainFifoAfterCurrentEvent()
        {
            List<string> calls = new List<string>();
            InMemoryEventBus eventBus = new InMemoryEventBus();
            eventBus.Subscribe<FirstTestEvent>(
                delegate
                {
                    calls.Add("first:one");
                    eventBus.Publish(new SecondTestEvent());
                });
            eventBus.Subscribe<FirstTestEvent>(
                delegate
                {
                    calls.Add("first:two");
                    eventBus.Publish(new ThirdTestEvent());
                });
            eventBus.Subscribe<SecondTestEvent>(
                delegate { calls.Add("second"); });
            eventBus.Subscribe<ThirdTestEvent>(
                delegate { calls.Add("third"); });

            eventBus.Publish(new FirstTestEvent());

            CollectionAssert.AreEqual(
                new string[] { "first:one", "first:two", "second", "third" },
                calls);
        }

        [Test]
        public void NestedPublication_ResultCompletesWhenOuterQueueDrains()
        {
            InMemoryEventBus eventBus = new InMemoryEventBus();
            EventPublicationResult nestedResult = null;
            bool wasCompleteInsideHandler = true;
            eventBus.Subscribe<FirstTestEvent>(
                delegate
                {
                    nestedResult = eventBus.Publish(new SecondTestEvent());
                    wasCompleteInsideHandler = nestedResult.IsComplete;
                });

            EventPublicationResult outerResult =
                eventBus.Publish(new FirstTestEvent());

            Assert.IsFalse(wasCompleteInsideHandler);
            Assert.IsTrue(outerResult.IsComplete);
            Assert.IsTrue(nestedResult.IsComplete);
            Assert.IsTrue(nestedResult.IsSuccessful);
        }

        [Test]
        public void NestedPublications_DoNotGrowDispatchCallStack()
        {
            const int PublicationCount = 2000;
            int calls = 0;
            InMemoryEventBus eventBus = new InMemoryEventBus();
            eventBus.Subscribe<TestEvent>(
                delegate(TestEvent eventMessage)
                {
                    calls++;
                    if (eventMessage.Value < PublicationCount)
                    {
                        eventBus.Publish(
                            new TestEvent(eventMessage.Value + 1));
                    }
                });

            eventBus.Publish(new TestEvent(1));

            Assert.AreEqual(PublicationCount, calls);
        }

        [Test]
        public void SubscriberFailure_IsReportedAndRemainingHandlerRuns()
        {
            int successfulCalls = 0;
            InMemoryEventBus eventBus = new InMemoryEventBus();
            ThrowingSubscriber subscriber = new ThrowingSubscriber();
            eventBus.Subscribe<TestEvent>(subscriber.Handle);
            eventBus.Subscribe<TestEvent>(delegate { successfulCalls++; });

            EventPublicationResult result = eventBus.Publish(new TestEvent(1));

            Assert.IsTrue(result.IsComplete);
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(1, result.FailureCount);
            Assert.AreEqual(1, successfulCalls);

            EventSubscriberFailure failure = result.GetFailure(0);
            Assert.AreEqual(typeof(TestEvent), failure.EventType);
            Assert.AreEqual(typeof(ThrowingSubscriber), failure.SubscriberType);
            Assert.AreEqual("Handle", failure.SubscriberMethodName);
            Assert.AreEqual(typeof(InvalidOperationException), failure.ExceptionType);
            Assert.AreEqual("subscriber failure", failure.FailureReason);
        }

        [Test]
        public void Publish_WithNoSubscribersReturnsEmptySuccess()
        {
            InMemoryEventBus eventBus = new InMemoryEventBus();

            EventPublicationResult result = eventBus.Publish(new TestEvent(1));

            Assert.IsTrue(result.IsComplete);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(0, result.FailureCount);
            Assert.AreEqual(typeof(TestEvent), result.EventType);
        }

        [Test]
        public void Dispose_IsIdempotentAndRejectsNewWork()
        {
            int calls = 0;
            InMemoryEventBus eventBus = new InMemoryEventBus();
            eventBus.Subscribe<TestEvent>(delegate { calls++; });

            eventBus.Dispose();
            eventBus.Dispose();

            AssertObjectDisposed(
                delegate { eventBus.Subscribe<TestEvent>(delegate { }); });
            AssertObjectDisposed(
                delegate { eventBus.Publish(new TestEvent(1)); });
            Assert.AreEqual(0, calls);
        }

        [Test]
        public void Dispose_DuringDispatchCompletesAlreadyQueuedPublication()
        {
            InMemoryEventBus eventBus = new InMemoryEventBus();
            EventPublicationResult nestedResult = null;
            int nestedCalls = 0;
            eventBus.Subscribe<FirstTestEvent>(
                delegate
                {
                    nestedResult = eventBus.Publish(new SecondTestEvent());
                    eventBus.Dispose();
                });
            eventBus.Subscribe<SecondTestEvent>(
                delegate { nestedCalls++; });

            EventPublicationResult outerResult =
                eventBus.Publish(new FirstTestEvent());

            Assert.IsTrue(outerResult.IsComplete);
            Assert.IsTrue(nestedResult.IsComplete);
            Assert.IsTrue(nestedResult.IsSuccessful);
            Assert.AreEqual(0, nestedCalls);
        }

        [Test]
        public void SeparateInstances_DoNotShareSubscriptions()
        {
            int firstCalls = 0;
            InMemoryEventBus firstBus = new InMemoryEventBus();
            firstBus.Subscribe<TestEvent>(delegate { firstCalls++; });
            firstBus.Dispose();

            InMemoryEventBus secondBus = new InMemoryEventBus();
            EventPublicationResult result =
                secondBus.Publish(new TestEvent(1));

            Assert.AreEqual(0, firstCalls);
            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(0, result.FailureCount);
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

        private static void AssertArgumentException(TestDelegate action)
        {
            try
            {
                action();
                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException)
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

        private sealed class TestEvent : IEvent
        {
            public TestEvent(int value)
            {
                Value = value;
            }

            public int Value { get; private set; }
        }

        private class BaseTestEvent : IEvent
        {
        }

        private sealed class DerivedTestEvent : BaseTestEvent
        {
        }

        private sealed class FirstTestEvent : IEvent
        {
        }

        private sealed class SecondTestEvent : IEvent
        {
        }

        private sealed class ThirdTestEvent : IEvent
        {
        }

        private sealed class ThrowingSubscriber
        {
            public void Handle(TestEvent eventMessage)
            {
                throw new InvalidOperationException("subscriber failure");
            }
        }
    }
}
