using System;
using System.Collections.Generic;

namespace Minerva.Core
{
    /// <summary>
    /// Coordinates deterministic in-memory snapshot capture and transactional restore.
    /// </summary>
    public sealed class RuntimeSnapshotCoordinator
    {
        private readonly IRuntimeSnapshotContributor[] _contributors;
        private readonly RuntimeSnapshotContributionIdentity[] _identities;
        private readonly int[] _schemaVersions;
        private bool _isOperationActive;

        /// <summary>
        /// Creates an immutable registration-ordered contributor boundary.
        /// </summary>
        public RuntimeSnapshotCoordinator(
            IRuntimeSnapshotContributor[] contributors)
        {
            if (contributors == null)
            {
                throw new ArgumentNullException("contributors");
            }

            _contributors =
                new IRuntimeSnapshotContributor[contributors.Length];
            _identities =
                new RuntimeSnapshotContributionIdentity[contributors.Length];
            _schemaVersions = new int[contributors.Length];

            for (int index = 0; index < contributors.Length; index++)
            {
                IRuntimeSnapshotContributor contributor = contributors[index];
                if (contributor == null)
                {
                    throw new ArgumentException(
                        "A snapshot contributor cannot be null.",
                        "contributors");
                }

                RuntimeSnapshotContributionIdentity identity =
                    contributor.Identity;
                if (identity == null)
                {
                    throw new ArgumentException(
                        "A snapshot contributor must expose a stable identity.",
                        "contributors");
                }

                int schemaVersion = contributor.CurrentSchemaVersion;
                if (schemaVersion <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        "contributors",
                        "A snapshot contributor schema version must be positive.");
                }

                for (int earlierIndex = 0;
                    earlierIndex < index;
                    earlierIndex++)
                {
                    if (_identities[earlierIndex].Equals(identity))
                    {
                        throw new ArgumentException(
                            "Duplicate snapshot contribution identity: "
                                + identity + ".",
                            "contributors");
                    }
                }

                _contributors[index] = contributor;
                _identities[index] = identity;
                _schemaVersions[index] = schemaVersion;
            }
        }

        /// <summary>
        /// Captures each contributor exactly once in registration order.
        /// </summary>
        public RuntimeSnapshotCaptureResult Capture()
        {
            EnterOperation();

            try
            {
                RuntimeSnapshotContribution[] contributions =
                    new RuntimeSnapshotContribution[_contributors.Length];

                for (int index = 0;
                    index < _contributors.Length;
                    index++)
                {
                    RuntimeSnapshotContributionCaptureResult result;
                    try
                    {
                        result = _contributors[index].Capture();
                    }
                    catch (Exception exception)
                    {
                        return CaptureFailure(
                            _identities[index],
                            exception);
                    }

                    RuntimeSnapshotDiagnostic validationFailure =
                        ValidateCaptureResult(index, result);
                    if (validationFailure != null)
                    {
                        return new RuntimeSnapshotCaptureResult(
                            null,
                            validationFailure);
                    }

                    contributions[index] = result.Contribution;
                }

                return new RuntimeSnapshotCaptureResult(
                    new RuntimeSnapshot(contributions),
                    null);
            }
            finally
            {
                _isOperationActive = false;
            }
        }

        /// <summary>
        /// Validates, prepares, applies, and cleans up one aggregate restore.
        /// </summary>
        public RuntimeSnapshotRestoreResult Restore(RuntimeSnapshot snapshot)
        {
            EnterOperation();

            try
            {
                RuntimeSnapshotDiagnostic structuralFailure =
                    ValidateStructure(snapshot);
                if (structuralFailure != null)
                {
                    return RestoreFailure(structuralFailure);
                }

                RuntimeSnapshotContribution[] matched =
                    MatchContributions(snapshot);
                IRuntimeSnapshotRestoreOperation[] operations =
                    new IRuntimeSnapshotRestoreOperation[_contributors.Length];
                int preparedCount = 0;

                for (int index = 0;
                    index < _contributors.Length;
                    index++)
                {
                    RuntimeSnapshotPreparationResult result;
                    try
                    {
                        result = _contributors[index].PrepareRestore(
                            matched[index]);
                    }
                    catch (Exception exception)
                    {
                        RuntimeSnapshotDiagnostic primary =
                            ExceptionDiagnostic(
                                RuntimeSnapshotOperationPhase.Preparation,
                                _identities[index],
                                exception);
                        return PreparationFailure(
                            primary,
                            operations,
                            preparedCount);
                    }

                    RuntimeSnapshotDiagnostic failure =
                        ValidatePreparationResult(index, result);
                    if (failure != null)
                    {
                        return PreparationFailure(
                            failure,
                            operations,
                            preparedCount);
                    }

                    operations[index] = result.Operation;
                    preparedCount++;
                }

                return ApplyAndRelease(operations);
            }
            finally
            {
                _isOperationActive = false;
            }
        }

        private RuntimeSnapshotRestoreResult ApplyAndRelease(
            IRuntimeSnapshotRestoreOperation[] operations)
        {
            int attemptedCount = 0;
            RuntimeSnapshotDiagnostic primaryFailure = null;

            for (int index = 0; index < operations.Length; index++)
            {
                attemptedCount++;
                RuntimeSnapshotStepResult result;
                try
                {
                    result = operations[index].Apply();
                }
                catch (Exception exception)
                {
                    primaryFailure = ExceptionDiagnostic(
                        RuntimeSnapshotOperationPhase.Apply,
                        _identities[index],
                        exception);
                    break;
                }

                primaryFailure = ValidateStepResult(
                    RuntimeSnapshotOperationPhase.Apply,
                    _identities[index],
                    result);
                if (primaryFailure != null)
                {
                    break;
                }
            }

            if (primaryFailure != null)
            {
                List<RuntimeSnapshotDiagnostic> rollbackFailures =
                    RollbackAttempted(operations, attemptedCount);
                List<RuntimeSnapshotDiagnostic> releaseFailures =
                    ReleasePrepared(operations, operations.Length);

                return new RuntimeSnapshotRestoreResult(
                    false,
                    primaryFailure,
                    rollbackFailures.ToArray(),
                    releaseFailures.ToArray());
            }

            List<RuntimeSnapshotDiagnostic> successfulReleaseFailures =
                ReleasePrepared(operations, operations.Length);
            if (successfulReleaseFailures.Count > 0)
            {
                return new RuntimeSnapshotRestoreResult(
                    false,
                    successfulReleaseFailures[0],
                    null,
                    successfulReleaseFailures.ToArray());
            }

            return new RuntimeSnapshotRestoreResult(
                true,
                null,
                null,
                null);
        }

        private RuntimeSnapshotRestoreResult PreparationFailure(
            RuntimeSnapshotDiagnostic primaryFailure,
            IRuntimeSnapshotRestoreOperation[] operations,
            int preparedCount)
        {
            List<RuntimeSnapshotDiagnostic> releaseFailures =
                ReleasePrepared(operations, preparedCount);

            return new RuntimeSnapshotRestoreResult(
                false,
                primaryFailure,
                null,
                releaseFailures.ToArray());
        }

        private List<RuntimeSnapshotDiagnostic> RollbackAttempted(
            IRuntimeSnapshotRestoreOperation[] operations,
            int attemptedCount)
        {
            List<RuntimeSnapshotDiagnostic> failures =
                new List<RuntimeSnapshotDiagnostic>();

            for (int index = attemptedCount - 1; index >= 0; index--)
            {
                RuntimeSnapshotStepResult result;
                try
                {
                    result = operations[index].Rollback();
                }
                catch (Exception exception)
                {
                    failures.Add(
                        ExceptionDiagnostic(
                            RuntimeSnapshotOperationPhase.Rollback,
                            _identities[index],
                            exception));
                    continue;
                }

                RuntimeSnapshotDiagnostic failure = ValidateStepResult(
                    RuntimeSnapshotOperationPhase.Rollback,
                    _identities[index],
                    result);
                if (failure != null)
                {
                    failures.Add(failure);
                }
            }

            return failures;
        }

        private List<RuntimeSnapshotDiagnostic> ReleasePrepared(
            IRuntimeSnapshotRestoreOperation[] operations,
            int preparedCount)
        {
            List<RuntimeSnapshotDiagnostic> failures =
                new List<RuntimeSnapshotDiagnostic>();

            for (int index = preparedCount - 1; index >= 0; index--)
            {
                RuntimeSnapshotStepResult result;
                try
                {
                    result = operations[index].Release();
                }
                catch (Exception exception)
                {
                    failures.Add(
                        ExceptionDiagnostic(
                            RuntimeSnapshotOperationPhase.Release,
                            _identities[index],
                            exception));
                    continue;
                }

                RuntimeSnapshotDiagnostic failure = ValidateStepResult(
                    RuntimeSnapshotOperationPhase.Release,
                    _identities[index],
                    result);
                if (failure != null)
                {
                    failures.Add(failure);
                }
            }

            return failures;
        }

        private RuntimeSnapshotDiagnostic ValidateCaptureResult(
            int index,
            RuntimeSnapshotContributionCaptureResult result)
        {
            RuntimeSnapshotContributionIdentity identity =
                _identities[index];

            if (result == null)
            {
                return FailureDiagnostic(
                    RuntimeSnapshotOperationPhase.Capture,
                    identity,
                    "The contributor returned no capture result.");
            }

            if (!result.IsSuccessful)
            {
                return FailureDiagnostic(
                    RuntimeSnapshotOperationPhase.Capture,
                    identity,
                    NormalizeReason(
                        result.FailureReason,
                        "The contributor reported capture failure without an actionable reason."));
            }

            RuntimeSnapshotContribution contribution = result.Contribution;
            if (contribution == null)
            {
                return FailureDiagnostic(
                    RuntimeSnapshotOperationPhase.Capture,
                    identity,
                    "The contributor returned no captured contribution.");
            }

            if (contribution.Identity == null
                || !identity.Equals(contribution.Identity))
            {
                return FailureDiagnostic(
                    RuntimeSnapshotOperationPhase.Capture,
                    identity,
                    "The captured contribution identity does not match its contributor.");
            }

            if (contribution.SchemaVersion != _schemaVersions[index])
            {
                return FailureDiagnostic(
                    RuntimeSnapshotOperationPhase.Capture,
                    identity,
                    "The captured schema version does not match the contributor's current version.");
            }

            if (contribution.Data == null)
            {
                return FailureDiagnostic(
                    RuntimeSnapshotOperationPhase.Capture,
                    identity,
                    "The captured contribution contains no snapshot data.");
            }

            return null;
        }

        private RuntimeSnapshotDiagnostic ValidatePreparationResult(
            int index,
            RuntimeSnapshotPreparationResult result)
        {
            RuntimeSnapshotContributionIdentity identity =
                _identities[index];

            if (result == null)
            {
                return FailureDiagnostic(
                    RuntimeSnapshotOperationPhase.Preparation,
                    identity,
                    "The contributor returned no preparation result.");
            }

            if (!result.IsSuccessful)
            {
                return FailureDiagnostic(
                    RuntimeSnapshotOperationPhase.Preparation,
                    identity,
                    NormalizeReason(
                        result.FailureReason,
                        "The contributor reported preparation failure without an actionable reason."));
            }

            if (result.Operation == null)
            {
                return FailureDiagnostic(
                    RuntimeSnapshotOperationPhase.Preparation,
                    identity,
                    "The contributor returned no prepared restore operation.");
            }

            return null;
        }

        private RuntimeSnapshotDiagnostic ValidateStructure(
            RuntimeSnapshot snapshot)
        {
            if (snapshot == null)
            {
                return FailureDiagnostic(
                    RuntimeSnapshotOperationPhase.StructuralValidation,
                    null,
                    "A snapshot is required for restore.");
            }

            for (int index = 0;
                index < snapshot.ContributionCount;
                index++)
            {
                RuntimeSnapshotContribution contribution =
                    snapshot.GetContribution(index);
                if (contribution == null)
                {
                    return FailureDiagnostic(
                        RuntimeSnapshotOperationPhase.StructuralValidation,
                        null,
                        "A snapshot contribution cannot be null.");
                }

                if (contribution.Identity == null)
                {
                    return FailureDiagnostic(
                        RuntimeSnapshotOperationPhase.StructuralValidation,
                        null,
                        "A snapshot contribution identity cannot be null.");
                }

                if (contribution.SchemaVersion <= 0)
                {
                    return FailureDiagnostic(
                        RuntimeSnapshotOperationPhase.StructuralValidation,
                        contribution.Identity,
                        "A snapshot contribution schema version must be positive.");
                }

                if (contribution.Data == null)
                {
                    return FailureDiagnostic(
                        RuntimeSnapshotOperationPhase.StructuralValidation,
                        contribution.Identity,
                        "A snapshot contribution must contain detached data.");
                }

                for (int earlierIndex = 0;
                    earlierIndex < index;
                    earlierIndex++)
                {
                    RuntimeSnapshotContribution earlier =
                        snapshot.GetContribution(earlierIndex);
                    if (earlier != null
                        && earlier.Identity != null
                        && earlier.Identity.Equals(contribution.Identity))
                    {
                        return FailureDiagnostic(
                            RuntimeSnapshotOperationPhase.StructuralValidation,
                            contribution.Identity,
                            "The snapshot contains a duplicate contribution identity.");
                    }
                }

                if (FindRegisteredIndex(contribution.Identity) < 0)
                {
                    return FailureDiagnostic(
                        RuntimeSnapshotOperationPhase.StructuralValidation,
                        contribution.Identity,
                        "The snapshot contains an unknown contribution.");
                }
            }

            for (int contributorIndex = 0;
                contributorIndex < _identities.Length;
                contributorIndex++)
            {
                if (FindSnapshotContribution(
                    snapshot,
                    _identities[contributorIndex]) == null)
                {
                    return FailureDiagnostic(
                        RuntimeSnapshotOperationPhase.StructuralValidation,
                        _identities[contributorIndex],
                        "The snapshot is missing a registered contribution.");
                }
            }

            return null;
        }

        private RuntimeSnapshotContribution[] MatchContributions(
            RuntimeSnapshot snapshot)
        {
            RuntimeSnapshotContribution[] matched =
                new RuntimeSnapshotContribution[_identities.Length];

            for (int index = 0; index < _identities.Length; index++)
            {
                matched[index] = FindSnapshotContribution(
                    snapshot,
                    _identities[index]);
            }

            return matched;
        }

        private RuntimeSnapshotContribution FindSnapshotContribution(
            RuntimeSnapshot snapshot,
            RuntimeSnapshotContributionIdentity identity)
        {
            for (int index = 0;
                index < snapshot.ContributionCount;
                index++)
            {
                RuntimeSnapshotContribution contribution =
                    snapshot.GetContribution(index);
                if (contribution != null
                    && contribution.Identity != null
                    && contribution.Identity.Equals(identity))
                {
                    return contribution;
                }
            }

            return null;
        }

        private int FindRegisteredIndex(
            RuntimeSnapshotContributionIdentity identity)
        {
            for (int index = 0; index < _identities.Length; index++)
            {
                if (_identities[index].Equals(identity))
                {
                    return index;
                }
            }

            return -1;
        }

        private void EnterOperation()
        {
            if (_isOperationActive)
            {
                throw new InvalidOperationException(
                    "A snapshot coordinator operation is already active.");
            }

            _isOperationActive = true;
        }

        private static RuntimeSnapshotCaptureResult CaptureFailure(
            RuntimeSnapshotContributionIdentity identity,
            Exception exception)
        {
            return new RuntimeSnapshotCaptureResult(
                null,
                ExceptionDiagnostic(
                    RuntimeSnapshotOperationPhase.Capture,
                    identity,
                    exception));
        }

        private static RuntimeSnapshotRestoreResult RestoreFailure(
            RuntimeSnapshotDiagnostic primaryFailure)
        {
            return new RuntimeSnapshotRestoreResult(
                false,
                primaryFailure,
                null,
                null);
        }

        private static RuntimeSnapshotDiagnostic ValidateStepResult(
            RuntimeSnapshotOperationPhase phase,
            RuntimeSnapshotContributionIdentity identity,
            RuntimeSnapshotStepResult result)
        {
            if (result == null)
            {
                return FailureDiagnostic(
                    phase,
                    identity,
                    "The restore operation returned no step result.");
            }

            if (!result.IsSuccessful)
            {
                return FailureDiagnostic(
                    phase,
                    identity,
                    NormalizeReason(
                        result.FailureReason,
                        "The restore operation failed without an actionable reason."));
            }

            return null;
        }

        private static RuntimeSnapshotDiagnostic ExceptionDiagnostic(
            RuntimeSnapshotOperationPhase phase,
            RuntimeSnapshotContributionIdentity identity,
            Exception exception)
        {
            return new RuntimeSnapshotDiagnostic(
                phase,
                identity,
                exception.GetType(),
                NormalizeReason(
                    exception.Message,
                    "A participant threw "
                        + exception.GetType().FullName + "."));
        }

        private static RuntimeSnapshotDiagnostic FailureDiagnostic(
            RuntimeSnapshotOperationPhase phase,
            RuntimeSnapshotContributionIdentity identity,
            string reason)
        {
            return new RuntimeSnapshotDiagnostic(
                phase,
                identity,
                reason);
        }

        private static string NormalizeReason(
            string reason,
            string fallback)
        {
            if (string.IsNullOrEmpty(reason)
                || reason.Trim().Length == 0)
            {
                return fallback;
            }

            return reason;
        }
    }
}
