using System;
using System.Collections.Generic;

namespace Minerva.Core
{
    internal sealed class RuntimeStateCell<T>
    {
        private readonly IEqualityComparer<T> _comparer;
        private readonly IRuntimeStateValidator<T> _validator;
        private readonly IEventPublisher _publisher;
        private T _value;

        public RuntimeStateCell(
            RuntimeStateIdentity identity,
            T initialValue,
            IEqualityComparer<T> comparer,
            IRuntimeStateValidator<T> validator,
            IEventPublisher publisher)
        {
            if (identity == null)
            {
                throw new ArgumentNullException("identity");
            }

            Identity = identity;
            _comparer = comparer ?? EqualityComparer<T>.Default;
            _validator = validator;
            _publisher = publisher;

            ValidateInitialValue(initialValue);
            _value = initialValue;
        }

        private RuntimeStateIdentity Identity { get; set; }

        private T Value
        {
            get { return _value; }
        }

        public RuntimeStateCapabilities<T> CreateCapabilities()
        {
            return new RuntimeStateCapabilities<T>(
                new ReadCapability(this),
                new MutationCapability(this));
        }

        private RuntimeStateMutationResult<T> TrySet(T proposedValue)
        {
            if (_comparer.Equals(_value, proposedValue))
            {
                return RuntimeStateMutationResult<T>.Unchanged(
                    _publisher != null);
            }

            RuntimeStateValidationResult validationResult =
                ValidateMutation(proposedValue);
            if (!validationResult.IsAccepted)
            {
                return RuntimeStateMutationResult<T>.Rejected(
                    validationResult.RejectionReason,
                    _publisher != null);
            }

            T previousValue = _value;
            _value = proposedValue;

            RuntimeStateChange<T> change = new RuntimeStateChange<T>(
                Identity,
                previousValue,
                proposedValue);

            if (_publisher == null)
            {
                return RuntimeStateMutationResult<T>.ChangedWithoutPublication(
                    change);
            }

            try
            {
                EventPublicationResult publicationResult =
                    _publisher.Publish<RuntimeStateChange<T>>(change);
                if (publicationResult == null)
                {
                    throw new InvalidOperationException(
                        "The event publisher returned no publication result.");
                }

                return RuntimeStateMutationResult<T>.ChangedWithPublication(
                    change,
                    publicationResult);
            }
            catch (Exception exception)
            {
                RuntimeStatePublicationFailure publicationFailure =
                    new RuntimeStatePublicationFailure(
                        exception.GetType(),
                        exception.Message);
                return RuntimeStateMutationResult<T>.ChangedWithFailure(
                    change,
                    publicationFailure);
            }
        }

        private void ValidateInitialValue(T initialValue)
        {
            if (_validator == null)
            {
                return;
            }

            RuntimeStateValidationResult validationResult =
                RequireValidationResult(
                    _validator.Validate(
                        new RuntimeStateValidationContext<T>(
                            Identity,
                            false,
                            default(T),
                            initialValue)));

            if (!validationResult.IsAccepted)
            {
                throw new ArgumentException(
                    validationResult.RejectionReason,
                    "initialValue");
            }
        }

        private RuntimeStateValidationResult ValidateMutation(T proposedValue)
        {
            if (_validator == null)
            {
                return RuntimeStateValidationResult.Accepted();
            }

            return RequireValidationResult(
                _validator.Validate(
                    new RuntimeStateValidationContext<T>(
                        Identity,
                        true,
                        _value,
                        proposedValue)));
        }

        private static RuntimeStateValidationResult RequireValidationResult(
            RuntimeStateValidationResult validationResult)
        {
            if (validationResult == null)
            {
                throw new InvalidOperationException(
                    "The runtime-state validator returned no validation result.");
            }

            return validationResult;
        }

        private sealed class ReadCapability : IRuntimeState<T>
        {
            private readonly RuntimeStateCell<T> _cell;

            public ReadCapability(RuntimeStateCell<T> cell)
            {
                _cell = cell;
            }

            public RuntimeStateIdentity Identity
            {
                get { return _cell.Identity; }
            }

            public T Value
            {
                get { return _cell.Value; }
            }
        }

        private sealed class MutationCapability : IRuntimeStateMutator<T>
        {
            private readonly RuntimeStateCell<T> _cell;

            public MutationCapability(RuntimeStateCell<T> cell)
            {
                _cell = cell;
            }

            public RuntimeStateMutationResult<T> TrySet(T proposedValue)
            {
                return _cell.TrySet(proposedValue);
            }
        }
    }
}
