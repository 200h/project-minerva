namespace Minerva.Core
{
    /// <summary>
    /// Validates an initial or proposed typed runtime-state value.
    /// </summary>
    public interface IRuntimeStateValidator<T>
    {
        /// <summary>
        /// Returns an accepted or actionable rejected result for the supplied context.
        /// </summary>
        RuntimeStateValidationResult Validate(
            RuntimeStateValidationContext<T> context);
    }
}
