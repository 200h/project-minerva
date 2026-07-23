using System;

namespace Minerva.Core
{
    /// <summary>
    /// Exposes narrow capabilities and owns one successfully composed runtime lifetime.
    /// </summary>
    public sealed class ComposedRuntime : IDisposable
    {
        private readonly IComposedRuntimeLifetime _lifetime;

        internal ComposedRuntime(
            IComposedRuntimeLifetime lifetime,
            IEventPublisher eventPublisher,
            IEventSubscriber eventSubscriber)
        {
            _lifetime = lifetime;
            EventPublisher = eventPublisher;
            EventSubscriber = eventSubscriber;
        }

        /// <summary>
        /// Gets the runtime-owned event publication capability.
        /// </summary>
        public IEventPublisher EventPublisher { get; private set; }

        /// <summary>
        /// Gets the runtime-owned event subscription capability.
        /// </summary>
        public IEventSubscriber EventSubscriber { get; private set; }

        /// <summary>
        /// Gets shutdown diagnostics after this lifetime has been disposed.
        /// </summary>
        public RuntimeShutdownResult ShutdownResult
        {
            get { return _lifetime.ShutdownResult; }
        }

        /// <summary>
        /// Shuts down all owned services in reverse order exactly once.
        /// Event capabilities remain referentially stable but reject new work afterward.
        /// </summary>
        public void Dispose()
        {
            _lifetime.Dispose();
        }
    }

    internal interface IComposedRuntimeLifetime : IDisposable
    {
        RuntimeShutdownResult ShutdownResult { get; }
    }
}
