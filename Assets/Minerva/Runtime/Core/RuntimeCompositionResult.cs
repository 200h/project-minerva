namespace Minerva.Core
{
    /// <summary>
    /// Describes an atomic runtime composition attempt without exposing partial resources.
    /// </summary>
    public sealed class RuntimeCompositionResult
    {
        private RuntimeCompositionResult(
            ComposedRuntime runtime,
            RuntimeInitializationResult initializationResult,
            RuntimeShutdownResult cleanupResult)
        {
            Runtime = runtime;
            InitializationResult = initializationResult;
            CleanupResult = cleanupResult;
        }

        /// <summary>
        /// Gets a value indicating whether composition returned a usable runtime.
        /// </summary>
        public bool IsSuccessful
        {
            get { return Runtime != null; }
        }

        /// <summary>
        /// Gets the usable runtime after success, or null after startup failure.
        /// </summary>
        public ComposedRuntime Runtime { get; private set; }

        /// <summary>
        /// Gets the accepted bootstrap initialization outcome.
        /// </summary>
        public RuntimeInitializationResult InitializationResult
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets immediate cleanup diagnostics after startup failure, or null after success.
        /// </summary>
        public RuntimeShutdownResult CleanupResult { get; private set; }

        internal static RuntimeCompositionResult Success(
            ComposedRuntime runtime,
            RuntimeInitializationResult initializationResult)
        {
            return new RuntimeCompositionResult(
                runtime,
                initializationResult,
                null);
        }

        internal static RuntimeCompositionResult Failure(
            RuntimeInitializationResult initializationResult,
            RuntimeShutdownResult cleanupResult)
        {
            return new RuntimeCompositionResult(
                null,
                initializationResult,
                cleanupResult);
        }
    }
}
