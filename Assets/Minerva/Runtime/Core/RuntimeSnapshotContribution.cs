namespace Minerva.Core
{
    /// <summary>
    /// Carries one owner-defined, versioned snapshot contribution.
    /// </summary>
    public sealed class RuntimeSnapshotContribution
    {
        /// <summary>
        /// Creates a contribution record for later coordinator validation.
        /// </summary>
        /// <remarks>
        /// The constructor deliberately preserves structurally invalid decoded input so
        /// the coordinator can reject the complete aggregate before participant calls.
        /// </remarks>
        public RuntimeSnapshotContribution(
            RuntimeSnapshotContributionIdentity identity,
            int schemaVersion,
            IRuntimeSnapshotData data)
        {
            Identity = identity;
            SchemaVersion = schemaVersion;
            Data = data;
        }

        /// <summary>
        /// Gets the stable owner-qualified contribution identity.
        /// </summary>
        public RuntimeSnapshotContributionIdentity Identity
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the owner-defined schema version.
        /// </summary>
        public int SchemaVersion { get; private set; }

        /// <summary>
        /// Gets the immutable detached contribution data.
        /// </summary>
        public IRuntimeSnapshotData Data { get; private set; }
    }
}
