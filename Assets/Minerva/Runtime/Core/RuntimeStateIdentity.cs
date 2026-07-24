using System;

namespace Minerva.Core
{
    /// <summary>
    /// Identifies one runtime-state value by stable owner and owner-local identifiers.
    /// </summary>
    public sealed class RuntimeStateIdentity : IEquatable<RuntimeStateIdentity>
    {
        /// <summary>
        /// Creates an immutable owner-qualified identity.
        /// </summary>
        public RuntimeStateIdentity(string ownerId, string stateId)
        {
            ValidateIdentifier(ownerId, "ownerId");
            ValidateIdentifier(stateId, "stateId");

            OwnerId = ownerId;
            StateId = stateId;
        }

        /// <summary>
        /// Gets the stable identifier of the authoritative owner.
        /// </summary>
        public string OwnerId { get; private set; }

        /// <summary>
        /// Gets the stable state identifier within the authoritative owner.
        /// </summary>
        public string StateId { get; private set; }

        /// <summary>
        /// Determines whether another identity has the same ordinal identifiers.
        /// </summary>
        public bool Equals(RuntimeStateIdentity other)
        {
            return !object.ReferenceEquals(other, null)
                && string.Equals(
                    OwnerId,
                    other.OwnerId,
                    StringComparison.Ordinal)
                && string.Equals(
                    StateId,
                    other.StateId,
                    StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether another object is an equal runtime-state identity.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as RuntimeStateIdentity);
        }

        /// <summary>
        /// Returns a deterministic ordinal hash of both identifiers.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = CombineOrdinalHash(hash, OwnerId);
                hash = CombineOrdinalHash(hash, StateId);
                return hash;
            }
        }

        /// <summary>
        /// Returns the owner-qualified stable identity for diagnostics.
        /// </summary>
        public override string ToString()
        {
            return OwnerId + "/" + StateId;
        }

        private static int CombineOrdinalHash(int hash, string value)
        {
            unchecked
            {
                for (int index = 0; index < value.Length; index++)
                {
                    hash = (hash * 31) + value[index];
                }

                return hash;
            }
        }

        private static void ValidateIdentifier(
            string identifier,
            string parameterName)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (identifier.Trim().Length == 0)
            {
                throw new ArgumentException(
                    "A stable runtime-state identifier cannot be empty or whitespace.",
                    parameterName);
            }
        }
    }
}
