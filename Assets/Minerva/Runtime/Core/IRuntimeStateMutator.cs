namespace Minerva.Core
{
    /// <summary>
    /// Grants explicit authority to propose mutations for one typed state value.
    /// </summary>
    public interface IRuntimeStateMutator<T>
    {
        /// <summary>
        /// Validates and applies a proposed value when it represents an accepted change.
        /// </summary>
        RuntimeStateMutationResult<T> TrySet(T proposedValue);
    }
}
