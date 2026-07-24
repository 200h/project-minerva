namespace Minerva.Core
{
    /// <summary>
    /// Identifies the semantic outcome of one runtime-state mutation attempt.
    /// </summary>
    public enum RuntimeStateMutationStatus
    {
        /// <summary>
        /// The proposed value was accepted and became authoritative.
        /// </summary>
        Changed,

        /// <summary>
        /// The proposed value compared equal and no mutation occurred.
        /// </summary>
        Unchanged,

        /// <summary>
        /// Validation rejected the proposed value.
        /// </summary>
        Rejected
    }
}
