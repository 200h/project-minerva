using System;

namespace Minerva.Core
{
    /// <summary>
    /// Identifies one snapshot contribution by stable owner and owner-local identifiers.
    /// </summary>
    public sealed class RuntimeSnapshotContributionIdentity :
        IEquatable<RuntimeSnapshotContributionIdentity>
    {
        /// <summary>
        /// Creates an immutable owner-qualified contribution identity.
        /// </summary>
        public RuntimeSnapshotContributionIdentity(
            string ownerId,
            string contributionId)
        {
            ValidateIdentifier(ownerId, "ownerId");
            ValidateIdentifier(contributionId, "contributionId");

            OwnerId = ownerId;
            ContributionId = contributionId;
        }

        /// <summary>
        /// Gets the stable identifier of the authoritative owner.
        /// </summary>
        public string OwnerId { get; private set; }

        /// <summary>
        /// Gets the stable contribution identifier within the owner.
        /// </summary>
        public string ContributionId { get; private set; }

        /// <summary>
        /// Determines whether another identity has the same ordinal identifiers.
        /// </summary>
        public bool Equals(RuntimeSnapshotContributionIdentity other)
        {
            return !object.ReferenceEquals(other, null)
                && string.Equals(
                    OwnerId,
                    other.OwnerId,
                    StringComparison.Ordinal)
                && string.Equals(
                    ContributionId,
                    other.ContributionId,
                    StringComparison.Ordinal);
        }

        /// <summary>
        /// Determines whether another object is an equal contribution identity.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as RuntimeSnapshotContributionIdentity);
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
                hash = CombineOrdinalHash(hash, ContributionId);
                return hash;
            }
        }

        /// <summary>
        /// Returns the owner-qualified identity for diagnostics.
        /// </summary>
        public override string ToString()
        {
            return OwnerId + "/" + ContributionId;
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
                    "A stable snapshot contribution identifier cannot be empty or whitespace.",
                    parameterName);
            }
        }
    }
}
