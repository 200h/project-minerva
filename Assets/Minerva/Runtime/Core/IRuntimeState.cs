namespace Minerva.Core
{
    /// <summary>
    /// Exposes read-only access to one strongly typed authoritative value.
    /// </summary>
    public interface IRuntimeState<T>
    {
        /// <summary>
        /// Gets the stable owner-qualified identity of the value.
        /// </summary>
        RuntimeStateIdentity Identity { get; }

        /// <summary>
        /// Gets the current authoritative value.
        /// </summary>
        T Value { get; }
    }
}
