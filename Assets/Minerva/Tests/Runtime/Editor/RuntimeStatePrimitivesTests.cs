using System;
using System.Collections.Generic;
using Minerva.Core;
using NUnit.Framework;

namespace Minerva.Tests.Runtime
{
    public sealed class RuntimeStatePrimitivesTests
    {
        [Test]
        public void Identity_RejectsInvalidIdentifiers()
        {
            AssertArgumentNull(
                delegate { new RuntimeStateIdentity(null, "state"); },
                "ownerId");
            AssertArgumentException(
                delegate { new RuntimeStateIdentity("", "state"); },
                "ownerId");
            AssertArgumentException(
                delegate { new RuntimeStateIdentity(" \t", "state"); },
                "ownerId");
            AssertArgumentNull(
                delegate { new RuntimeStateIdentity("owner", null); },
                "stateId");
            AssertArgumentException(
                delegate { new RuntimeStateIdentity("owner", ""); },
                "stateId");
            AssertArgumentException(
                delegate { new RuntimeStateIdentity("owner", "\r\n"); },
                "stateId");
        }

        [Test]
        public void Identity_UsesOrdinalCaseSensitiveEqualityAndStableHashing()
        {
            RuntimeStateIdentity first =
                new RuntimeStateIdentity("owner", "state");
            RuntimeStateIdentity equal =
                new RuntimeStateIdentity("owner", "state");
            RuntimeStateIdentity differentOwnerCase =
                new RuntimeStateIdentity("Owner", "state");
            RuntimeStateIdentity differentStateCase =
                new RuntimeStateIdentity("owner", "State");

            Assert.IsTrue(first.Equals(equal));
            Assert.IsTrue(first.Equals((object)equal));
            Assert.AreEqual(first.GetHashCode(), equal.GetHashCode());
            Assert.IsFalse(first.Equals(differentOwnerCase));
            Assert.IsFalse(first.Equals(differentStateCase));
            Assert.IsFalse(first.Equals(null));
            Assert.AreEqual("owner/state", first.ToString());
        }

        [Test]
        public void Create_ReturnsSeparateNarrowCapabilities()
        {
            RuntimeStateIdentity identity =
                new RuntimeStateIdentity("owner", "score");
            RuntimeStateCapabilities<int> capabilities =
                RuntimeState.Create<int>(identity, 10);

            Assert.AreSame(identity, capabilities.State.Identity);
            Assert.AreEqual(10, capabilities.State.Value);
            Assert.IsFalse(
                capabilities.State is IRuntimeStateMutator<int>);
            Assert.IsFalse(
                capabilities.Mutator is IRuntimeState<int>);
        }

        [Test]
        public void Create_RejectsNullIdentityBeforeReturningCapabilities()
        {
            AssertArgumentNull(
                delegate { RuntimeState.Create<int>(null, 1); },
                "identity");
        }

        [Test]
        public void Create_ValidatesInitialValueWithNoCurrentValue()
        {
            RecordingValidator<int> validator =
                new RecordingValidator<int>(
                    RuntimeStateValidationResult.Accepted());
            RuntimeStateIdentity identity =
                new RuntimeStateIdentity("owner", "score");

            RuntimeStateCapabilities<int> capabilities =
                RuntimeState.Create<int>(
                    identity,
                    5,
                    null,
                    validator,
                    null);

            Assert.AreEqual(1, validator.CallCount);
            Assert.AreSame(identity, validator.LastContext.Identity);
            Assert.IsFalse(validator.LastContext.HasCurrentValue);
            Assert.AreEqual(0, validator.LastContext.CurrentValue);
            Assert.AreEqual(5, validator.LastContext.ProposedValue);
            Assert.AreEqual(5, capabilities.State.Value);
        }

        [Test]
        public void Create_RejectedInitialValueThrowsActionableArgumentException()
        {
            RecordingValidator<int> validator =
                new RecordingValidator<int>(
                    RuntimeStateValidationResult.Rejected(
                        "Initial score is invalid."));

            try
            {
                RuntimeState.Create<int>(
                    new RuntimeStateIdentity("owner", "score"),
                    -1,
                    null,
                    validator,
                    null);
                Assert.Fail("Expected ArgumentException.");
            }
            catch (ArgumentException exception)
            {
                Assert.AreEqual("initialValue", exception.ParamName);
                StringAssert.Contains(
                    "Initial score is invalid.",
                    exception.Message);
            }
        }

        [Test]
        public void Create_PropagatesInitialValidatorException()
        {
            ThrowingValidator<int> validator =
                new ThrowingValidator<int>("initial failure");

            AssertInvalidOperation(
                delegate
                {
                    RuntimeState.Create<int>(
                        new RuntimeStateIdentity("owner", "score"),
                        1,
                        null,
                        validator,
                        null);
                },
                "initial failure");
        }

        [Test]
        public void TrySet_EqualValueSkipsValidationAndPublication()
        {
            RecordingValidator<int> validator =
                new RecordingValidator<int>(
                    RuntimeStateValidationResult.Accepted());
            RecordingPublisher publisher = new RecordingPublisher();
            RuntimeStateCapabilities<int> capabilities =
                RuntimeState.Create<int>(
                    new RuntimeStateIdentity("owner", "score"),
                    7,
                    null,
                    validator,
                    publisher);
            validator.Reset();

            RuntimeStateMutationResult<int> result =
                capabilities.Mutator.TrySet(7);

            Assert.AreEqual(
                RuntimeStateMutationStatus.Unchanged,
                result.Status);
            Assert.AreEqual(7, capabilities.State.Value);
            Assert.AreEqual(0, validator.CallCount);
            Assert.AreEqual(0, publisher.PublishCount);
            Assert.IsNull(result.Change);
            Assert.IsNull(result.RejectionReason);
            Assert.IsTrue(result.IsPublicationConfigured);
            Assert.IsFalse(result.WasPublicationAttempted);
            Assert.IsNull(result.PublicationResult);
            Assert.IsNull(result.PublicationFailure);
        }

        [Test]
        public void TrySet_CustomComparerControlsUnchangedBehavior()
        {
            RuntimeStateCapabilities<string> capabilities =
                RuntimeState.Create<string>(
                    new RuntimeStateIdentity("owner", "name"),
                    "Alice",
                    StringComparer.OrdinalIgnoreCase);

            RuntimeStateMutationResult<string> result =
                capabilities.Mutator.TrySet("ALICE");

            Assert.AreEqual(
                RuntimeStateMutationStatus.Unchanged,
                result.Status);
            Assert.AreEqual("Alice", capabilities.State.Value);
        }

        [Test]
        public void TrySet_DifferingValueValidatesOnceWithCurrentValue()
        {
            RecordingValidator<int> validator =
                new RecordingValidator<int>(
                    RuntimeStateValidationResult.Accepted());
            RuntimeStateIdentity identity =
                new RuntimeStateIdentity("owner", "score");
            RuntimeStateCapabilities<int> capabilities =
                RuntimeState.Create<int>(
                    identity,
                    3,
                    null,
                    validator,
                    null);
            validator.Reset();

            RuntimeStateMutationResult<int> result =
                capabilities.Mutator.TrySet(4);

            Assert.AreEqual(1, validator.CallCount);
            Assert.AreSame(identity, validator.LastContext.Identity);
            Assert.IsTrue(validator.LastContext.HasCurrentValue);
            Assert.AreEqual(3, validator.LastContext.CurrentValue);
            Assert.AreEqual(4, validator.LastContext.ProposedValue);
            Assert.AreEqual(
                RuntimeStateMutationStatus.Changed,
                result.Status);
        }

        [Test]
        public void TrySet_RejectedValuePreservesStateAndReturnsReason()
        {
            MutableValidator<int> validator =
                new MutableValidator<int>(
                    RuntimeStateValidationResult.Accepted());
            RuntimeStateCapabilities<int> capabilities =
                RuntimeState.Create<int>(
                    new RuntimeStateIdentity("owner", "score"),
                    3,
                    null,
                    validator,
                    null);
            validator.Result = RuntimeStateValidationResult.Rejected(
                "Score cannot be four.");

            RuntimeStateMutationResult<int> result =
                capabilities.Mutator.TrySet(4);

            Assert.AreEqual(
                RuntimeStateMutationStatus.Rejected,
                result.Status);
            Assert.AreEqual(3, capabilities.State.Value);
            Assert.AreEqual(
                "Score cannot be four.",
                result.RejectionReason);
            Assert.IsNull(result.Change);
            Assert.IsFalse(result.IsPublicationConfigured);
            Assert.IsFalse(result.WasPublicationAttempted);
            Assert.IsNull(result.PublicationResult);
            Assert.IsNull(result.PublicationFailure);
        }

        [Test]
        public void TrySet_ValidatorExceptionPropagatesWithoutMutation()
        {
            SwitchableThrowingValidator<int> validator =
                new SwitchableThrowingValidator<int>();
            RuntimeStateCapabilities<int> capabilities =
                RuntimeState.Create<int>(
                    new RuntimeStateIdentity("owner", "score"),
                    3,
                    null,
                    validator,
                    null);
            validator.ShouldThrow = true;

            AssertInvalidOperation(
                delegate { capabilities.Mutator.TrySet(4); },
                "mutation failure");
            Assert.AreEqual(3, capabilities.State.Value);
        }

        [Test]
        public void TrySet_ChangedResultContainsOneCompletedChange()
        {
            RuntimeStateIdentity identity =
                new RuntimeStateIdentity("owner", "score");
            RuntimeStateCapabilities<int> capabilities =
                RuntimeState.Create<int>(identity, 1);

            RuntimeStateMutationResult<int> first =
                capabilities.Mutator.TrySet(2);
            RuntimeStateMutationResult<int> second =
                capabilities.Mutator.TrySet(3);

            Assert.AreEqual(
                RuntimeStateMutationStatus.Changed,
                first.Status);
            Assert.AreSame(identity, first.Change.Identity);
            Assert.AreEqual(1, first.Change.PreviousValue);
            Assert.AreEqual(2, first.Change.CurrentValue);
            Assert.AreEqual(3, capabilities.State.Value);
            Assert.AreNotSame(first.Change, second.Change);
            Assert.IsNull(first.RejectionReason);
            Assert.IsFalse(first.IsPublicationConfigured);
            Assert.IsFalse(first.WasPublicationAttempted);
        }

        [Test]
        public void NullAndDefaultValuesAreAcceptedWithoutImplicitPolicy()
        {
            RuntimeStateCapabilities<string> referenceState =
                RuntimeState.Create<string>(
                    new RuntimeStateIdentity("owner", "name"),
                    null);
            RuntimeStateCapabilities<int?> nullableState =
                RuntimeState.Create<int?>(
                    new RuntimeStateIdentity("owner", "optional-score"),
                    null);

            RuntimeStateMutationResult<string> referenceResult =
                referenceState.Mutator.TrySet("Alice");
            RuntimeStateMutationResult<int?> nullableResult =
                nullableState.Mutator.TrySet(0);

            Assert.AreEqual(
                RuntimeStateMutationStatus.Changed,
                referenceResult.Status);
            Assert.IsNull(referenceResult.Change.PreviousValue);
            Assert.AreEqual("Alice", referenceResult.Change.CurrentValue);
            Assert.AreEqual(
                RuntimeStateMutationStatus.Changed,
                nullableResult.Status);
            Assert.IsFalse(nullableResult.Change.PreviousValue.HasValue);
            Assert.AreEqual(0, nullableResult.Change.CurrentValue.Value);
        }

        [Test]
        public void ValidatorMayExplicitlyRejectNullWithoutImplicitCoercion()
        {
            NullRejectingValidator validator = new NullRejectingValidator();
            RuntimeStateCapabilities<string> capabilities =
                RuntimeState.Create<string>(
                    new RuntimeStateIdentity("owner", "name"),
                    "Alice",
                    null,
                    validator,
                    null);

            RuntimeStateMutationResult<string> result =
                capabilities.Mutator.TrySet(null);

            Assert.AreEqual(
                RuntimeStateMutationStatus.Rejected,
                result.Status);
            Assert.AreEqual("Alice", capabilities.State.Value);
            Assert.AreEqual(
                "Name cannot be null.",
                result.RejectionReason);
        }

        [Test]
        public void ValidationResult_RejectsMissingReason()
        {
            AssertArgumentNull(
                delegate { RuntimeStateValidationResult.Rejected(null); },
                "rejectionReason");
            AssertArgumentException(
                delegate { RuntimeStateValidationResult.Rejected(" "); },
                "rejectionReason");
        }

        [Test]
        public void TrySet_PublishesCompletedChangeAfterValueIsAuthoritative()
        {
            InMemoryEventBus eventBus = new InMemoryEventBus();
            RuntimeStateCapabilities<int> capabilities =
                RuntimeState.Create<int>(
                    new RuntimeStateIdentity("owner", "score"),
                    1,
                    null,
                    null,
                    eventBus);
            int observedValue = -1;
            RuntimeStateChange<int> observedChange = null;
            eventBus.Subscribe<RuntimeStateChange<int>>(
                delegate(RuntimeStateChange<int> change)
                {
                    observedValue = capabilities.State.Value;
                    observedChange = change;
                });

            RuntimeStateMutationResult<int> result =
                capabilities.Mutator.TrySet(2);

            Assert.AreEqual(2, observedValue);
            Assert.AreSame(result.Change, observedChange);
            Assert.IsTrue(result.IsPublicationConfigured);
            Assert.IsTrue(result.WasPublicationAttempted);
            Assert.IsNotNull(result.PublicationResult);
            Assert.IsTrue(result.PublicationResult.IsComplete);
            Assert.IsTrue(result.PublicationResult.IsSuccessful);
            Assert.IsNull(result.PublicationFailure);
        }

        [Test]
        public void TrySet_SubscriberFailureDoesNotRollBackState()
        {
            InMemoryEventBus eventBus = new InMemoryEventBus();
            RuntimeStateCapabilities<int> capabilities =
                RuntimeState.Create<int>(
                    new RuntimeStateIdentity("owner", "score"),
                    1,
                    null,
                    null,
                    eventBus);
            eventBus.Subscribe<RuntimeStateChange<int>>(
                delegate { throw new InvalidOperationException("subscriber"); });

            RuntimeStateMutationResult<int> result =
                capabilities.Mutator.TrySet(2);

            Assert.AreEqual(2, capabilities.State.Value);
            Assert.AreEqual(
                RuntimeStateMutationStatus.Changed,
                result.Status);
            Assert.IsTrue(result.WasPublicationAttempted);
            Assert.IsNotNull(result.PublicationResult);
            Assert.IsFalse(result.PublicationResult.IsSuccessful);
            Assert.AreEqual(1, result.PublicationResult.FailureCount);
            Assert.IsNull(result.PublicationFailure);
        }

        [Test]
        public void TrySet_PublisherExceptionReturnsDiagnosticWithoutRollback()
        {
            ThrowingPublisher publisher =
                new ThrowingPublisher("publisher failure");
            RuntimeStateCapabilities<int> capabilities =
                RuntimeState.Create<int>(
                    new RuntimeStateIdentity("owner", "score"),
                    1,
                    null,
                    null,
                    publisher);

            RuntimeStateMutationResult<int> result =
                capabilities.Mutator.TrySet(2);

            Assert.AreEqual(2, capabilities.State.Value);
            Assert.AreEqual(
                RuntimeStateMutationStatus.Changed,
                result.Status);
            Assert.IsTrue(result.WasPublicationAttempted);
            Assert.IsNull(result.PublicationResult);
            Assert.IsNotNull(result.PublicationFailure);
            Assert.AreEqual(
                typeof(InvalidOperationException),
                result.PublicationFailure.ExceptionType);
            Assert.AreEqual(
                "publisher failure",
                result.PublicationFailure.FailureReason);
        }

        [Test]
        public void ReentrantMutation_UsesQueuedNestedPublicationOrder()
        {
            InMemoryEventBus eventBus = new InMemoryEventBus();
            RuntimeStateCapabilities<int> capabilities =
                RuntimeState.Create<int>(
                    new RuntimeStateIdentity("owner", "score"),
                    0,
                    null,
                    null,
                    eventBus);
            List<int> observed = new List<int>();
            RuntimeStateMutationResult<int> nestedResult = null;
            bool nestedWasCompleteInsideHandler = true;
            eventBus.Subscribe<RuntimeStateChange<int>>(
                delegate(RuntimeStateChange<int> change)
                {
                    observed.Add(change.CurrentValue);
                    if (change.CurrentValue == 1)
                    {
                        nestedResult = capabilities.Mutator.TrySet(2);
                        nestedWasCompleteInsideHandler =
                            nestedResult.PublicationResult.IsComplete;
                    }
                });

            RuntimeStateMutationResult<int> outerResult =
                capabilities.Mutator.TrySet(1);

            CollectionAssert.AreEqual(new int[] { 1, 2 }, observed);
            Assert.AreEqual(2, capabilities.State.Value);
            Assert.IsFalse(nestedWasCompleteInsideHandler);
            Assert.IsTrue(outerResult.PublicationResult.IsComplete);
            Assert.IsTrue(nestedResult.PublicationResult.IsComplete);
            Assert.IsTrue(nestedResult.PublicationResult.IsSuccessful);
        }

        [Test]
        public void EqualIdentities_DoNotShareStateOrPublication()
        {
            RuntimeStateIdentity firstIdentity =
                new RuntimeStateIdentity("owner", "score");
            RuntimeStateIdentity secondIdentity =
                new RuntimeStateIdentity("owner", "score");
            RecordingPublisher firstPublisher = new RecordingPublisher();
            RecordingPublisher secondPublisher = new RecordingPublisher();
            RuntimeStateCapabilities<int> first =
                RuntimeState.Create<int>(
                    firstIdentity,
                    1,
                    null,
                    null,
                    firstPublisher);
            RuntimeStateCapabilities<int> second =
                RuntimeState.Create<int>(
                    secondIdentity,
                    1,
                    null,
                    null,
                    secondPublisher);

            first.Mutator.TrySet(2);

            Assert.AreEqual(firstIdentity, secondIdentity);
            Assert.AreEqual(2, first.State.Value);
            Assert.AreEqual(1, second.State.Value);
            Assert.AreEqual(1, firstPublisher.PublishCount);
            Assert.AreEqual(0, secondPublisher.PublishCount);
        }

        [Test]
        public void StateIntegratesWithComposedRuntimeEventCapabilities()
        {
            RuntimeCompositionResult composition =
                RuntimeCompositionRoot.Compose();
            RuntimeStateCapabilities<int> capabilities =
                RuntimeState.Create<int>(
                    new RuntimeStateIdentity("owner", "score"),
                    1,
                    null,
                    null,
                    composition.Runtime.EventPublisher);
            int observed = 0;
            composition.Runtime.EventSubscriber
                .Subscribe<RuntimeStateChange<int>>(
                    delegate(RuntimeStateChange<int> change)
                    {
                        observed = change.CurrentValue;
                    });

            RuntimeStateMutationResult<int> result =
                capabilities.Mutator.TrySet(2);

            Assert.AreEqual(2, observed);
            Assert.IsTrue(result.PublicationResult.IsSuccessful);

            composition.Runtime.Dispose();
        }

        [Test]
        public void RepeatedConstructionSharesNoMutableState()
        {
            RuntimeStateIdentity identity =
                new RuntimeStateIdentity("owner", "score");
            RuntimeStateCapabilities<int> first =
                RuntimeState.Create<int>(identity, 1);
            RuntimeStateCapabilities<int> second =
                RuntimeState.Create<int>(identity, 1);

            first.Mutator.TrySet(2);

            Assert.AreEqual(2, first.State.Value);
            Assert.AreEqual(1, second.State.Value);
            Assert.AreNotSame(first.State, second.State);
            Assert.AreNotSame(first.Mutator, second.Mutator);
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

        private static void AssertInvalidOperation(
            TestDelegate action,
            string message)
        {
            try
            {
                action();
                Assert.Fail("Expected InvalidOperationException.");
            }
            catch (InvalidOperationException exception)
            {
                Assert.AreEqual(message, exception.Message);
            }
        }

        private sealed class RecordingValidator<T> :
            IRuntimeStateValidator<T>
        {
            private readonly RuntimeStateValidationResult _result;

            public RecordingValidator(RuntimeStateValidationResult result)
            {
                _result = result;
            }

            public int CallCount { get; private set; }

            public RuntimeStateValidationContext<T> LastContext
            {
                get;
                private set;
            }

            public RuntimeStateValidationResult Validate(
                RuntimeStateValidationContext<T> context)
            {
                CallCount++;
                LastContext = context;
                return _result;
            }

            public void Reset()
            {
                CallCount = 0;
                LastContext = null;
            }
        }

        private sealed class MutableValidator<T> :
            IRuntimeStateValidator<T>
        {
            public MutableValidator(RuntimeStateValidationResult result)
            {
                Result = result;
            }

            public RuntimeStateValidationResult Result { get; set; }

            public RuntimeStateValidationResult Validate(
                RuntimeStateValidationContext<T> context)
            {
                return Result;
            }
        }

        private sealed class ThrowingValidator<T> :
            IRuntimeStateValidator<T>
        {
            private readonly string _message;

            public ThrowingValidator(string message)
            {
                _message = message;
            }

            public RuntimeStateValidationResult Validate(
                RuntimeStateValidationContext<T> context)
            {
                throw new InvalidOperationException(_message);
            }
        }

        private sealed class SwitchableThrowingValidator<T> :
            IRuntimeStateValidator<T>
        {
            public bool ShouldThrow { get; set; }

            public RuntimeStateValidationResult Validate(
                RuntimeStateValidationContext<T> context)
            {
                if (ShouldThrow)
                {
                    throw new InvalidOperationException("mutation failure");
                }

                return RuntimeStateValidationResult.Accepted();
            }
        }

        private sealed class NullRejectingValidator :
            IRuntimeStateValidator<string>
        {
            public RuntimeStateValidationResult Validate(
                RuntimeStateValidationContext<string> context)
            {
                if (context.ProposedValue == null)
                {
                    return RuntimeStateValidationResult.Rejected(
                        "Name cannot be null.");
                }

                return RuntimeStateValidationResult.Accepted();
            }
        }

        private sealed class RecordingPublisher : IEventPublisher
        {
            public int PublishCount { get; private set; }

            public EventPublicationResult Publish<TEvent>(
                TEvent eventMessage)
                where TEvent : IEvent
            {
                PublishCount++;
                return new InMemoryEventBus().Publish(eventMessage);
            }
        }

        private sealed class ThrowingPublisher : IEventPublisher
        {
            private readonly string _message;

            public ThrowingPublisher(string message)
            {
                _message = message;
            }

            public EventPublicationResult Publish<TEvent>(
                TEvent eventMessage)
                where TEvent : IEvent
            {
                throw new InvalidOperationException(_message);
            }
        }
    }
}
