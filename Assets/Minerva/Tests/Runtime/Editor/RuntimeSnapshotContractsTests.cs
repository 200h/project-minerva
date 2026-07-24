using System;
using System.Collections.Generic;
using Minerva.Core;
using NUnit.Framework;

namespace Minerva.Tests.Runtime
{
    public sealed class RuntimeSnapshotContractsTests
    {
        [Test]
        public void Identity_RejectsInvalidIdentifiers()
        {
            AssertArgumentNull(
                delegate
                {
                    new RuntimeSnapshotContributionIdentity(null, "state");
                },
                "ownerId");
            AssertArgumentException(
                delegate
                {
                    new RuntimeSnapshotContributionIdentity("", "state");
                },
                "ownerId");
            AssertArgumentException(
                delegate
                {
                    new RuntimeSnapshotContributionIdentity(" \t", "state");
                },
                "ownerId");
            AssertArgumentNull(
                delegate
                {
                    new RuntimeSnapshotContributionIdentity("owner", null);
                },
                "contributionId");
            AssertArgumentException(
                delegate
                {
                    new RuntimeSnapshotContributionIdentity("owner", "\r\n");
                },
                "contributionId");
        }

        [Test]
        public void Identity_UsesOrdinalCaseSensitiveEqualityAndStableHashing()
        {
            RuntimeSnapshotContributionIdentity first =
                Identity("owner", "state");
            RuntimeSnapshotContributionIdentity equal =
                Identity("owner", "state");

            Assert.IsTrue(first.Equals(equal));
            Assert.IsTrue(first.Equals((object)equal));
            Assert.AreEqual(first.GetHashCode(), equal.GetHashCode());
            Assert.IsFalse(first.Equals(Identity("Owner", "state")));
            Assert.IsFalse(first.Equals(Identity("owner", "State")));
            Assert.IsFalse(first.Equals(null));
            Assert.AreEqual("owner/state", first.ToString());
        }

        [Test]
        public void Snapshot_DefensivelyCopiesContributionCollection()
        {
            RuntimeSnapshotContribution first =
                Contribution("owner", "first", 1, 10);
            RuntimeSnapshotContribution second =
                Contribution("owner", "second", 1, 20);
            RuntimeSnapshotContribution[] source =
                new RuntimeSnapshotContribution[] { first, second };
            RuntimeSnapshot snapshot = new RuntimeSnapshot(source);

            source[0] = second;

            Assert.AreEqual(2, snapshot.ContributionCount);
            Assert.AreSame(first, snapshot.GetContribution(0));
            Assert.AreSame(second, snapshot.GetContribution(1));
            AssertArgumentOutOfRange(
                delegate { snapshot.GetContribution(-1); },
                "index");
            AssertArgumentOutOfRange(
                delegate { snapshot.GetContribution(2); },
                "index");
        }

        [Test]
        public void Coordinator_RejectsNullEntriesDuplicateIdentityAndInvalidVersions()
        {
            RecordingContributor first =
                Contributor("owner", "state", 1, new List<string>());
            RecordingContributor duplicate =
                Contributor("owner", "state", 1, new List<string>());
            RecordingContributor invalidVersion =
                Contributor("owner", "other", 0, new List<string>());

            AssertArgumentNull(
                delegate
                {
                    new RuntimeSnapshotCoordinator(
                        (IRuntimeSnapshotContributor[])null);
                },
                "contributors");
            AssertArgumentException(
                delegate
                {
                    new RuntimeSnapshotCoordinator(
                        new IRuntimeSnapshotContributor[] { null });
                },
                "contributors");
            AssertArgumentException(
                delegate
                {
                    new RuntimeSnapshotCoordinator(
                        new IRuntimeSnapshotContributor[]
                        {
                            first,
                            duplicate
                        });
                },
                "contributors");
            AssertArgumentOutOfRange(
                delegate
                {
                    new RuntimeSnapshotCoordinator(
                        new IRuntimeSnapshotContributor[]
                        {
                            invalidVersion
                        });
                },
                "contributors");
        }

        [Test]
        public void Capture_UsesRegistrationOrderAndInvokesEachContributorOnce()
        {
            List<string> calls = new List<string>();
            RecordingContributor first =
                Contributor("owner", "first", 1, calls);
            RecordingContributor second =
                Contributor("owner", "second", 2, calls);
            RuntimeSnapshotCoordinator coordinator = Coordinator(first, second);

            RuntimeSnapshotCaptureResult result = coordinator.Capture();

            Assert.IsTrue(result.IsSuccessful);
            Assert.IsNull(result.Failure);
            CollectionAssert.AreEqual(
                new string[] { "capture:first", "capture:second" },
                calls);
            Assert.AreEqual(1, first.CaptureCount);
            Assert.AreEqual(1, second.CaptureCount);
            Assert.AreEqual(2, result.Snapshot.ContributionCount);
            Assert.AreEqual(
                first.Identity,
                result.Snapshot.GetContribution(0).Identity);
            Assert.AreEqual(
                second.Identity,
                result.Snapshot.GetContribution(1).Identity);
        }

        [Test]
        public void Capture_RejectsMalformedReturnedContributionWithoutPartialSnapshot()
        {
            List<string> calls = new List<string>();
            RecordingContributor first =
                Contributor("owner", "first", 1, calls);
            RecordingContributor second =
                Contributor("owner", "second", 1, calls);
            first.CaptureHandler = delegate
            {
                return RuntimeSnapshotContributionCaptureResult.Success(
                    new RuntimeSnapshotContribution(
                        Identity("wrong", "identity"),
                        1,
                        new IntegerData(1)));
            };

            RuntimeSnapshotCaptureResult mismatch =
                Coordinator(first, second).Capture();

            Assert.IsFalse(mismatch.IsSuccessful);
            Assert.IsNull(mismatch.Snapshot);
            Assert.AreEqual(
                RuntimeSnapshotOperationPhase.Capture,
                mismatch.Failure.Phase);
            Assert.AreEqual(first.Identity, mismatch.Failure.Identity);
            Assert.AreEqual(0, second.CaptureCount);

            first.CaptureHandler = delegate
            {
                return RuntimeSnapshotContributionCaptureResult.Success(
                    new RuntimeSnapshotContribution(
                        first.Identity,
                        2,
                        new IntegerData(1)));
            };
            RuntimeSnapshotCaptureResult versionMismatch =
                Coordinator(first).Capture();
            Assert.IsFalse(versionMismatch.IsSuccessful);

            first.CaptureHandler = delegate
            {
                return RuntimeSnapshotContributionCaptureResult.Success(
                    new RuntimeSnapshotContribution(
                        first.Identity,
                        1,
                        null));
            };
            RuntimeSnapshotCaptureResult missingData =
                Coordinator(first).Capture();
            Assert.IsFalse(missingData.IsSuccessful);
        }

        [Test]
        public void Capture_ConvertsReturnedFailureNullResultAndException()
        {
            RecordingContributor contributor =
                Contributor("owner", "state", 1, new List<string>());
            contributor.CaptureHandler = delegate
            {
                return RuntimeSnapshotContributionCaptureResult.Failure(
                    "capture rejected");
            };

            RuntimeSnapshotCaptureResult returnedFailure =
                Coordinator(contributor).Capture();
            Assert.IsFalse(returnedFailure.IsSuccessful);
            Assert.AreEqual(
                "capture rejected",
                returnedFailure.Failure.FailureReason);

            contributor.CaptureHandler = delegate { return null; };
            RuntimeSnapshotCaptureResult nullFailure =
                Coordinator(contributor).Capture();
            Assert.IsFalse(nullFailure.IsSuccessful);
            StringAssert.Contains(
                "no capture result",
                nullFailure.Failure.FailureReason);

            contributor.CaptureHandler = delegate
            {
                throw new InvalidOperationException("capture exception");
            };
            RuntimeSnapshotCaptureResult exceptionFailure =
                Coordinator(contributor).Capture();
            Assert.IsFalse(exceptionFailure.IsSuccessful);
            Assert.AreEqual(
                typeof(InvalidOperationException),
                exceptionFailure.Failure.ExceptionType);
            Assert.AreEqual(
                "capture exception",
                exceptionFailure.Failure.FailureReason);
        }

        [Test]
        public void Restore_StructurallyValidatesCompleteSnapshotBeforePreparation()
        {
            List<string> calls = new List<string>();
            RecordingContributor first =
                Contributor("owner", "first", 1, calls);
            RecordingContributor second =
                Contributor("owner", "second", 1, calls);
            RuntimeSnapshotCoordinator coordinator = Coordinator(first, second);

            AssertStructuralFailure(coordinator.Restore(null));
            AssertStructuralFailure(
                coordinator.Restore(
                    new RuntimeSnapshot(
                        new RuntimeSnapshotContribution[] { null })));
            AssertStructuralFailure(
                coordinator.Restore(
                    Snapshot(
                        new RuntimeSnapshotContribution(
                            null,
                            1,
                            new IntegerData(1)))));
            AssertStructuralFailure(
                coordinator.Restore(
                    Snapshot(
                        new RuntimeSnapshotContribution(
                            first.Identity,
                            0,
                            new IntegerData(1)))));
            AssertStructuralFailure(
                coordinator.Restore(
                    Snapshot(
                        new RuntimeSnapshotContribution(
                            first.Identity,
                            1,
                            null))));
            AssertStructuralFailure(
                coordinator.Restore(
                    Snapshot(
                        Contribution("owner", "unknown", 1, 1))));
            AssertStructuralFailure(
                coordinator.Restore(
                    Snapshot(
                        Contribution("owner", "first", 1, 1))));
            AssertStructuralFailure(
                coordinator.Restore(
                    Snapshot(
                        Contribution("owner", "first", 1, 1),
                        Contribution("owner", "first", 1, 2))));

            Assert.AreEqual(0, first.PrepareCount);
            Assert.AreEqual(0, second.PrepareCount);
            Assert.AreEqual(0, calls.Count);
        }

        [Test]
        public void Restore_MatchesByIdentityButPreparesAndAppliesInRegistrationOrder()
        {
            List<string> calls = new List<string>();
            RecordingContributor first =
                Contributor("owner", "first", 1, calls);
            RecordingContributor second =
                Contributor("owner", "second", 1, calls);
            first.Operation = new RecordingOperation("first", calls);
            second.Operation = new RecordingOperation("second", calls);
            RuntimeSnapshot reversed = Snapshot(
                Contribution("owner", "second", 1, 2),
                Contribution("owner", "first", 1, 1));

            RuntimeSnapshotRestoreResult result =
                Coordinator(first, second).Restore(reversed);

            Assert.IsTrue(result.IsSuccessful);
            CollectionAssert.AreEqual(
                new string[]
                {
                    "prepare:first",
                    "prepare:second",
                    "apply:first",
                    "apply:second",
                    "release:second",
                    "release:first"
                },
                calls);
            Assert.AreEqual(
                first.Identity,
                first.LastPreparedContribution.Identity);
            Assert.AreEqual(
                second.Identity,
                second.LastPreparedContribution.Identity);
        }

        [Test]
        public void Restore_AllowsContributorControlledOlderSchemaCompatibility()
        {
            RecordingContributor contributor =
                Contributor("owner", "state", 2, new List<string>());
            contributor.Operation =
                new RecordingOperation("state", new List<string>());

            RuntimeSnapshotRestoreResult result =
                Coordinator(contributor).Restore(
                    Snapshot(
                        Contribution("owner", "state", 1, 10)));

            Assert.IsTrue(result.IsSuccessful);
            Assert.AreEqual(
                1,
                contributor.LastPreparedContribution.SchemaVersion);
        }

        [Test]
        public void PreparationFailure_ReleasesPriorOperationsInReverseWithoutApplyOrRollback()
        {
            List<string> calls = new List<string>();
            RecordingContributor first =
                Contributor("owner", "first", 1, calls);
            RecordingContributor second =
                Contributor("owner", "second", 1, calls);
            RecordingOperation firstOperation =
                new RecordingOperation("first", calls);
            first.Operation = firstOperation;
            second.PrepareHandler = delegate
            {
                return RuntimeSnapshotPreparationResult.Failure(
                    "preparation rejected");
            };

            RuntimeSnapshotRestoreResult result =
                Coordinator(first, second).Restore(
                    ValidSnapshot(first, second));

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(
                RuntimeSnapshotOperationPhase.Preparation,
                result.PrimaryFailure.Phase);
            Assert.AreEqual(
                "preparation rejected",
                result.PrimaryFailure.FailureReason);
            CollectionAssert.AreEqual(
                new string[]
                {
                    "prepare:first",
                    "prepare:second",
                    "release:first"
                },
                calls);
            Assert.AreEqual(0, firstOperation.ApplyCount);
            Assert.AreEqual(0, firstOperation.RollbackCount);
        }

        [Test]
        public void PreparationException_PreservesPrimaryAndReverseReleaseFailures()
        {
            List<string> calls = new List<string>();
            RecordingContributor first =
                Contributor("owner", "first", 1, calls);
            RecordingContributor second =
                Contributor("owner", "second", 1, calls);
            RecordingOperation firstOperation =
                new RecordingOperation("first", calls);
            firstOperation.ReleaseResult =
                RuntimeSnapshotStepResult.Failure("release failed");
            first.Operation = firstOperation;
            second.PrepareHandler = delegate
            {
                throw new InvalidOperationException("prepare exception");
            };

            RuntimeSnapshotRestoreResult result =
                Coordinator(first, second).Restore(
                    ValidSnapshot(first, second));

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(
                typeof(InvalidOperationException),
                result.PrimaryFailure.ExceptionType);
            Assert.AreEqual(1, result.ReleaseFailureCount);
            Assert.AreEqual(
                "release failed",
                result.GetReleaseFailure(0).FailureReason);
            Assert.AreEqual(0, result.RollbackFailureCount);
        }

        [Test]
        public void ApplyFailure_RollsBackAttemptedIncludingFailingThenReleasesAll()
        {
            List<string> calls = new List<string>();
            int firstState = 1;
            int secondState = 2;
            RecordingContributor first =
                Contributor("owner", "first", 1, calls);
            RecordingContributor second =
                Contributor("owner", "second", 1, calls);
            RecordingContributor third =
                Contributor("owner", "third", 1, calls);
            RecordingOperation firstOperation =
                new RecordingOperation("first", calls);
            firstOperation.ApplyAction = delegate { firstState = 10; };
            firstOperation.RollbackAction = delegate { firstState = 1; };
            RecordingOperation secondOperation =
                new RecordingOperation("second", calls);
            secondOperation.ApplyAction = delegate { secondState = 20; };
            secondOperation.ApplyResult =
                RuntimeSnapshotStepResult.Failure("partial apply failed");
            secondOperation.RollbackAction = delegate { secondState = 2; };
            RecordingOperation thirdOperation =
                new RecordingOperation("third", calls);
            first.Operation = firstOperation;
            second.Operation = secondOperation;
            third.Operation = thirdOperation;

            RuntimeSnapshotRestoreResult result =
                Coordinator(first, second, third).Restore(
                    ValidSnapshot(first, second, third));

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(
                "partial apply failed",
                result.PrimaryFailure.FailureReason);
            Assert.AreEqual(1, firstState);
            Assert.AreEqual(2, secondState);
            Assert.AreEqual(0, thirdOperation.ApplyCount);
            Assert.AreEqual(0, thirdOperation.RollbackCount);
            CollectionAssert.AreEqual(
                new string[]
                {
                    "prepare:first",
                    "prepare:second",
                    "prepare:third",
                    "apply:first",
                    "apply:second",
                    "rollback:second",
                    "rollback:first",
                    "release:third",
                    "release:second",
                    "release:first"
                },
                calls);
        }

        [Test]
        public void ApplyException_PreservesOrderedRollbackAndReleaseFailures()
        {
            List<string> calls = new List<string>();
            RecordingContributor first =
                Contributor("owner", "first", 1, calls);
            RecordingContributor second =
                Contributor("owner", "second", 1, calls);
            RecordingOperation firstOperation =
                new RecordingOperation("first", calls);
            firstOperation.RollbackResult =
                RuntimeSnapshotStepResult.Failure("rollback first");
            firstOperation.ReleaseResult =
                RuntimeSnapshotStepResult.Failure("release first");
            RecordingOperation secondOperation =
                new RecordingOperation("second", calls);
            secondOperation.ApplyException =
                new InvalidOperationException("apply exception");
            secondOperation.RollbackResult =
                RuntimeSnapshotStepResult.Failure("rollback second");
            secondOperation.ReleaseException =
                new InvalidOperationException("release second");
            first.Operation = firstOperation;
            second.Operation = secondOperation;

            RuntimeSnapshotRestoreResult result =
                Coordinator(first, second).Restore(
                    ValidSnapshot(first, second));

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(
                typeof(InvalidOperationException),
                result.PrimaryFailure.ExceptionType);
            Assert.AreEqual("apply exception", result.PrimaryFailure.FailureReason);
            Assert.AreEqual(2, result.RollbackFailureCount);
            Assert.AreEqual(
                second.Identity,
                result.GetRollbackFailure(0).Identity);
            Assert.AreEqual(
                first.Identity,
                result.GetRollbackFailure(1).Identity);
            Assert.AreEqual(2, result.ReleaseFailureCount);
            Assert.AreEqual(
                second.Identity,
                result.GetReleaseFailure(0).Identity);
            Assert.AreEqual(
                first.Identity,
                result.GetReleaseFailure(1).Identity);
        }

        [Test]
        public void RollbackException_IsDiagnosedWithoutReplacingApplyFailureOrStoppingCleanup()
        {
            List<string> calls = new List<string>();
            RecordingContributor first =
                Contributor("owner", "first", 1, calls);
            RecordingContributor second =
                Contributor("owner", "second", 1, calls);
            RecordingContributor third =
                Contributor("owner", "third", 1, calls);
            RecordingOperation firstOperation =
                new RecordingOperation("first", calls);
            RecordingOperation secondOperation =
                new RecordingOperation("second", calls);
            RecordingOperation thirdOperation =
                new RecordingOperation("third", calls);
            InvalidOperationException rollbackException =
                new InvalidOperationException("rollback exception");
            secondOperation.ApplyResult =
                RuntimeSnapshotStepResult.Failure("primary apply failure");
            secondOperation.RollbackException = rollbackException;
            first.Operation = firstOperation;
            second.Operation = secondOperation;
            third.Operation = thirdOperation;
            RuntimeSnapshotCoordinator coordinator =
                Coordinator(first, second, third);

            RuntimeSnapshotRestoreResult result =
                coordinator.Restore(ValidSnapshot(first, second, third));

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(
                RuntimeSnapshotOperationPhase.Apply,
                result.PrimaryFailure.Phase);
            Assert.AreEqual(second.Identity, result.PrimaryFailure.Identity);
            Assert.AreEqual(
                "primary apply failure",
                result.PrimaryFailure.FailureReason);
            Assert.IsNull(result.PrimaryFailure.ExceptionType);
            Assert.AreEqual(1, result.RollbackFailureCount);
            RuntimeSnapshotDiagnostic rollbackFailure =
                result.GetRollbackFailure(0);
            Assert.AreEqual(
                RuntimeSnapshotOperationPhase.Rollback,
                rollbackFailure.Phase);
            Assert.AreEqual(second.Identity, rollbackFailure.Identity);
            Assert.AreEqual(
                typeof(InvalidOperationException),
                rollbackFailure.ExceptionType);
            Assert.AreEqual(
                rollbackException.Message,
                rollbackFailure.FailureReason);
            System.Reflection.FieldInfo[] diagnosticFields =
                typeof(RuntimeSnapshotDiagnostic).GetFields(
                    System.Reflection.BindingFlags.Instance
                    | System.Reflection.BindingFlags.Public
                    | System.Reflection.BindingFlags.NonPublic);
            for (int index = 0; index < diagnosticFields.Length; index++)
            {
                Assert.IsFalse(
                    typeof(Exception).IsAssignableFrom(
                        diagnosticFields[index].FieldType),
                    "Diagnostic field retains an exception object: "
                        + diagnosticFields[index].Name);
            }

            Assert.AreEqual(1, firstOperation.RollbackCount);
            Assert.AreEqual(1, secondOperation.RollbackCount);
            Assert.AreEqual(0, thirdOperation.RollbackCount);
            Assert.AreEqual(1, firstOperation.ReleaseCount);
            Assert.AreEqual(1, secondOperation.ReleaseCount);
            Assert.AreEqual(1, thirdOperation.ReleaseCount);
            CollectionAssert.AreEqual(
                new string[]
                {
                    "prepare:first",
                    "prepare:second",
                    "prepare:third",
                    "apply:first",
                    "apply:second",
                    "rollback:second",
                    "rollback:first",
                    "release:third",
                    "release:second",
                    "release:first"
                },
                calls);

            RuntimeSnapshotCaptureResult subsequentCapture =
                coordinator.Capture();

            Assert.IsTrue(subsequentCapture.IsSuccessful);
        }

        [Test]
        public void SuccessfulApply_WithReleaseFailureReportsFailureWithoutRollback()
        {
            List<string> calls = new List<string>();
            RecordingContributor contributor =
                Contributor("owner", "state", 1, calls);
            RecordingOperation operation =
                new RecordingOperation("state", calls);
            operation.ReleaseResult =
                RuntimeSnapshotStepResult.Failure("release failed");
            contributor.Operation = operation;

            RuntimeSnapshotRestoreResult result =
                Coordinator(contributor).Restore(
                    ValidSnapshot(contributor));

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(
                RuntimeSnapshotOperationPhase.Release,
                result.PrimaryFailure.Phase);
            Assert.AreEqual(0, result.RollbackFailureCount);
            Assert.AreEqual(1, result.ReleaseFailureCount);
            Assert.AreEqual(1, operation.ApplyCount);
            Assert.AreEqual(0, operation.RollbackCount);
        }

        [Test]
        public void RestoreOperation_EnforcesApplyRollbackAndReleaseLifecycle()
        {
            RecordingOperation operation =
                new RecordingOperation("state", new List<string>());

            Assert.IsFalse(operation.Rollback().IsSuccessful);
            Assert.IsTrue(operation.Apply().IsSuccessful);
            Assert.IsFalse(operation.Apply().IsSuccessful);
            Assert.IsTrue(operation.Rollback().IsSuccessful);
            Assert.IsTrue(operation.Rollback().IsSuccessful);
            Assert.IsFalse(operation.Apply().IsSuccessful);
            Assert.IsTrue(operation.Release().IsSuccessful);
            Assert.IsTrue(operation.Release().IsSuccessful);
            Assert.IsFalse(operation.Apply().IsSuccessful);
            Assert.IsFalse(operation.Rollback().IsSuccessful);
            Assert.AreEqual(1, operation.ApplyCount);
            Assert.AreEqual(1, operation.RollbackCount);
            Assert.AreEqual(1, operation.ReleaseCount);
        }

        [Test]
        public void Coordinator_ConvertsNullOperationStepResultsToDiagnostics()
        {
            RecordingContributor contributor =
                Contributor("owner", "state", 1, new List<string>());
            RecordingOperation operation =
                new RecordingOperation("state", new List<string>());
            operation.ApplyResult = null;
            contributor.Operation = operation;

            RuntimeSnapshotRestoreResult result =
                Coordinator(contributor).Restore(
                    ValidSnapshot(contributor));

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(
                RuntimeSnapshotOperationPhase.Apply,
                result.PrimaryFailure.Phase);
            StringAssert.Contains(
                "no step result",
                result.PrimaryFailure.FailureReason);
        }

        [Test]
        public void SameCoordinatorReentrancy_IsRejectedAndIdleStateIsRestored()
        {
            RecordingContributor contributor =
                Contributor("owner", "state", 1, new List<string>());
            RuntimeSnapshotCoordinator coordinator = Coordinator(contributor);
            contributor.CaptureHandler = delegate
            {
                coordinator.Restore(ValidSnapshot(contributor));
                return contributor.DefaultCapture();
            };

            RuntimeSnapshotCaptureResult reentrant = coordinator.Capture();

            Assert.IsFalse(reentrant.IsSuccessful);
            Assert.AreEqual(
                typeof(InvalidOperationException),
                reentrant.Failure.ExceptionType);

            contributor.CaptureHandler = contributor.DefaultCapture;
            RuntimeSnapshotCaptureResult afterFailure = coordinator.Capture();
            Assert.IsTrue(afterFailure.IsSuccessful);
        }

        [Test]
        public void SameCoordinatorRestoreReentrancy_IsConvertedDuringPreparation()
        {
            RecordingContributor contributor =
                Contributor("owner", "state", 1, new List<string>());
            RuntimeSnapshotCoordinator coordinator = Coordinator(contributor);
            RuntimeSnapshot snapshot = ValidSnapshot(contributor);
            contributor.PrepareHandler = delegate
            {
                coordinator.Capture();
                return RuntimeSnapshotPreparationResult.Success(
                    new RecordingOperation("state", new List<string>()));
            };

            RuntimeSnapshotRestoreResult result =
                coordinator.Restore(snapshot);

            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(
                typeof(InvalidOperationException),
                result.PrimaryFailure.ExceptionType);
            Assert.AreEqual(
                RuntimeSnapshotOperationPhase.Preparation,
                result.PrimaryFailure.Phase);
        }

        [Test]
        public void DifferentCoordinatorMayOperateDuringParticipantCallback()
        {
            RecordingContributor otherContributor =
                Contributor("other", "state", 1, new List<string>());
            RuntimeSnapshotCoordinator other =
                Coordinator(otherContributor);
            bool otherCaptureSucceeded = false;
            RecordingContributor contributor =
                Contributor("owner", "state", 1, new List<string>());
            contributor.CaptureHandler = delegate
            {
                otherCaptureSucceeded = other.Capture().IsSuccessful;
                return contributor.DefaultCapture();
            };

            RuntimeSnapshotCaptureResult result =
                Coordinator(contributor).Capture();

            Assert.IsTrue(result.IsSuccessful);
            Assert.IsTrue(otherCaptureSucceeded);
        }

        [Test]
        public void SeparateCoordinatorsShareNoMutableState()
        {
            RecordingContributor firstContributor =
                Contributor("owner", "state", 1, new List<string>());
            RecordingContributor secondContributor =
                Contributor("owner", "state", 1, new List<string>());
            RuntimeSnapshotCoordinator first = Coordinator(firstContributor);
            RuntimeSnapshotCoordinator second = Coordinator(secondContributor);

            first.Capture();

            Assert.AreEqual(1, firstContributor.CaptureCount);
            Assert.AreEqual(0, secondContributor.CaptureCount);
            Assert.IsTrue(second.Capture().IsSuccessful);
        }

        [Test]
        public void CoordinatorCoexistsWithAcceptedRuntimeCompositionWithoutOwnership()
        {
            RuntimeCompositionResult composition =
                RuntimeCompositionRoot.Compose();
            RecordingContributor contributor =
                Contributor("owner", "state", 1, new List<string>());
            RuntimeSnapshotCoordinator coordinator = Coordinator(contributor);

            Assert.IsTrue(composition.IsSuccessful);
            Assert.IsTrue(coordinator.Capture().IsSuccessful);

            composition.Runtime.Dispose();
            Assert.IsTrue(composition.Runtime.ShutdownResult.IsSuccessful);
            Assert.IsTrue(coordinator.Capture().IsSuccessful);
        }

        [Test]
        public void DiagnosticCollectionsAreReadOnlyByApiAndBoundsChecked()
        {
            RecordingContributor contributor =
                Contributor("owner", "state", 1, new List<string>());
            RecordingOperation operation =
                new RecordingOperation("state", new List<string>());
            operation.ApplyResult =
                RuntimeSnapshotStepResult.Failure("apply failed");
            operation.RollbackResult =
                RuntimeSnapshotStepResult.Failure("rollback failed");
            operation.ReleaseResult =
                RuntimeSnapshotStepResult.Failure("release failed");
            contributor.Operation = operation;

            RuntimeSnapshotRestoreResult result =
                Coordinator(contributor).Restore(
                    ValidSnapshot(contributor));

            Assert.AreEqual(1, result.RollbackFailureCount);
            Assert.AreEqual(1, result.ReleaseFailureCount);
            AssertArgumentOutOfRange(
                delegate { result.GetRollbackFailure(1); },
                "index");
            AssertArgumentOutOfRange(
                delegate { result.GetReleaseFailure(-1); },
                "index");
        }

        private static RuntimeSnapshotCoordinator Coordinator(
            params IRuntimeSnapshotContributor[] contributors)
        {
            return new RuntimeSnapshotCoordinator(contributors);
        }

        private static RecordingContributor Contributor(
            string owner,
            string contribution,
            int version,
            List<string> calls)
        {
            return new RecordingContributor(
                Identity(owner, contribution),
                version,
                calls);
        }

        private static RuntimeSnapshotContributionIdentity Identity(
            string owner,
            string contribution)
        {
            return new RuntimeSnapshotContributionIdentity(
                owner,
                contribution);
        }

        private static RuntimeSnapshotContribution Contribution(
            string owner,
            string contribution,
            int version,
            int value)
        {
            return new RuntimeSnapshotContribution(
                Identity(owner, contribution),
                version,
                new IntegerData(value));
        }

        private static RuntimeSnapshot Snapshot(
            params RuntimeSnapshotContribution[] contributions)
        {
            return new RuntimeSnapshot(contributions);
        }

        private static RuntimeSnapshot ValidSnapshot(
            params RecordingContributor[] contributors)
        {
            RuntimeSnapshotContribution[] contributions =
                new RuntimeSnapshotContribution[contributors.Length];
            for (int index = 0; index < contributors.Length; index++)
            {
                contributions[index] =
                    new RuntimeSnapshotContribution(
                        contributors[index].Identity,
                        contributors[index].CurrentSchemaVersion,
                        new IntegerData(index));
            }

            return new RuntimeSnapshot(contributions);
        }

        private static void AssertStructuralFailure(
            RuntimeSnapshotRestoreResult result)
        {
            Assert.IsFalse(result.IsSuccessful);
            Assert.AreEqual(
                RuntimeSnapshotOperationPhase.StructuralValidation,
                result.PrimaryFailure.Phase);
            Assert.AreEqual(0, result.RollbackFailureCount);
            Assert.AreEqual(0, result.ReleaseFailureCount);
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

        private sealed class IntegerData : IRuntimeSnapshotData
        {
            public IntegerData(int value)
            {
                Value = value;
            }

            public int Value { get; private set; }
        }

        private sealed class RecordingContributor :
            IRuntimeSnapshotContributor
        {
            private readonly List<string> _calls;

            public RecordingContributor(
                RuntimeSnapshotContributionIdentity identity,
                int currentSchemaVersion,
                List<string> calls)
            {
                Identity = identity;
                CurrentSchemaVersion = currentSchemaVersion;
                _calls = calls;
                Operation =
                    new RecordingOperation(
                        identity == null ? "unknown" : identity.ContributionId,
                        calls);
            }

            public RuntimeSnapshotContributionIdentity Identity
            {
                get;
                private set;
            }

            public int CurrentSchemaVersion { get; private set; }

            public int CaptureCount { get; private set; }

            public int PrepareCount { get; private set; }

            public RuntimeSnapshotContribution LastPreparedContribution
            {
                get;
                private set;
            }

            public Func<RuntimeSnapshotContributionCaptureResult>
                CaptureHandler { get; set; }

            public Func<RuntimeSnapshotPreparationResult>
                PrepareHandler { get; set; }

            public RecordingOperation Operation { get; set; }

            public RuntimeSnapshotContributionCaptureResult Capture()
            {
                CaptureCount++;
                _calls.Add("capture:" + Identity.ContributionId);
                if (CaptureHandler != null)
                {
                    return CaptureHandler();
                }

                return DefaultCapture();
            }

            public RuntimeSnapshotContributionCaptureResult DefaultCapture()
            {
                return RuntimeSnapshotContributionCaptureResult.Success(
                    new RuntimeSnapshotContribution(
                        Identity,
                        CurrentSchemaVersion,
                        new IntegerData(CaptureCount)));
            }

            public RuntimeSnapshotPreparationResult PrepareRestore(
                RuntimeSnapshotContribution contribution)
            {
                PrepareCount++;
                LastPreparedContribution = contribution;
                _calls.Add("prepare:" + Identity.ContributionId);
                if (PrepareHandler != null)
                {
                    return PrepareHandler();
                }

                return RuntimeSnapshotPreparationResult.Success(Operation);
            }
        }

        private sealed class RecordingOperation :
            RuntimeSnapshotRestoreOperation
        {
            private readonly string _name;
            private readonly List<string> _calls;

            public RecordingOperation(string name, List<string> calls)
            {
                _name = name;
                _calls = calls;
                ApplyResult = RuntimeSnapshotStepResult.Success();
                RollbackResult = RuntimeSnapshotStepResult.Success();
                ReleaseResult = RuntimeSnapshotStepResult.Success();
            }

            public int ApplyCount { get; private set; }

            public int RollbackCount { get; private set; }

            public int ReleaseCount { get; private set; }

            public Action ApplyAction { get; set; }

            public Action RollbackAction { get; set; }

            public RuntimeSnapshotStepResult ApplyResult { get; set; }

            public RuntimeSnapshotStepResult RollbackResult { get; set; }

            public RuntimeSnapshotStepResult ReleaseResult { get; set; }

            public Exception ApplyException { get; set; }

            public Exception RollbackException { get; set; }

            public Exception ReleaseException { get; set; }

            protected override RuntimeSnapshotStepResult ApplyCore()
            {
                ApplyCount++;
                _calls.Add("apply:" + _name);
                if (ApplyAction != null)
                {
                    ApplyAction();
                }

                if (ApplyException != null)
                {
                    throw ApplyException;
                }

                return ApplyResult;
            }

            protected override RuntimeSnapshotStepResult RollbackCore()
            {
                RollbackCount++;
                _calls.Add("rollback:" + _name);
                if (RollbackAction != null)
                {
                    RollbackAction();
                }

                if (RollbackException != null)
                {
                    throw RollbackException;
                }

                return RollbackResult;
            }

            protected override RuntimeSnapshotStepResult ReleaseCore()
            {
                ReleaseCount++;
                _calls.Add("release:" + _name);
                if (ReleaseException != null)
                {
                    throw ReleaseException;
                }

                return ReleaseResult;
            }
        }
    }
}
