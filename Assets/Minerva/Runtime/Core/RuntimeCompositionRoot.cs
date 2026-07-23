using System;

namespace Minerva.Core
{
    /// <summary>
    /// Constructs and starts one explicitly owned foundation runtime instance.
    /// </summary>
    public static class RuntimeCompositionRoot
    {
        /// <summary>
        /// Composes a runtime with lifecycle services in the supplied deterministic order.
        /// Ownership of every supplied service transfers to the composition operation after
        /// argument validation succeeds. Callers must not dispose transferred services.
        /// </summary>
        /// <param name="services">
        /// Lifecycle services to initialize after the event-bus lifecycle adapter.
        /// </param>
        /// <returns>
        /// A successful runtime handle or separate initialization and cleanup diagnostics.
        /// </returns>
        public static RuntimeCompositionResult Compose(
            params IRuntimeService[] services)
        {
            ValidateServices(services);

            RuntimeBootstrap bootstrap = new RuntimeBootstrap();
            InMemoryEventBus eventBus = new InMemoryEventBus();
            BootstrapRuntimeLifetime lifetime =
                new BootstrapRuntimeLifetime(bootstrap);

            bootstrap.Register(new EventBusRuntimeService(eventBus));

            for (int index = 0; index < services.Length; index++)
            {
                bootstrap.Register(services[index]);
            }

            RuntimeInitializationResult initializationResult =
                bootstrap.Initialize();

            if (!initializationResult.IsSuccessful)
            {
                lifetime.Dispose();
                return RuntimeCompositionResult.Failure(
                    initializationResult,
                    lifetime.ShutdownResult);
            }

            EventBusCapabilities eventCapabilities =
                new EventBusCapabilities(eventBus);
            ComposedRuntime runtime = new ComposedRuntime(
                lifetime,
                eventCapabilities,
                eventCapabilities);

            return RuntimeCompositionResult.Success(
                runtime,
                initializationResult);
        }

        private static void ValidateServices(IRuntimeService[] services)
        {
            if (services == null)
            {
                throw new ArgumentNullException("services");
            }

            for (int index = 0; index < services.Length; index++)
            {
                IRuntimeService service = services[index];
                if (service == null)
                {
                    throw new ArgumentException(
                        "A composed runtime service cannot be null.",
                        "services");
                }

                for (int earlierIndex = 0;
                    earlierIndex < index;
                    earlierIndex++)
                {
                    if (object.ReferenceEquals(
                        services[earlierIndex],
                        service))
                    {
                        throw new ArgumentException(
                            "The same runtime service cannot be composed more than once.",
                            "services");
                    }
                }
            }
        }

        private sealed class EventBusRuntimeService : IRuntimeService
        {
            private readonly IEventBus _eventBus;

            public EventBusRuntimeService(IEventBus eventBus)
            {
                _eventBus = eventBus;
            }

            public ServiceInitializationResult Initialize()
            {
                return ServiceInitializationResult.Success();
            }

            public void Shutdown()
            {
                _eventBus.Dispose();
            }
        }

        private sealed class EventBusCapabilities :
            IEventPublisher,
            IEventSubscriber
        {
            private readonly IEventBus _eventBus;

            public EventBusCapabilities(IEventBus eventBus)
            {
                _eventBus = eventBus;
            }

            public EventPublicationResult Publish<TEvent>(
                TEvent eventMessage)
                where TEvent : IEvent
            {
                return _eventBus.Publish(eventMessage);
            }

            public IDisposable Subscribe<TEvent>(Action<TEvent> handler)
                where TEvent : IEvent
            {
                return _eventBus.Subscribe(handler);
            }
        }

        private sealed class BootstrapRuntimeLifetime :
            IComposedRuntimeLifetime
        {
            private readonly RuntimeBootstrap _bootstrap;

            public BootstrapRuntimeLifetime(RuntimeBootstrap bootstrap)
            {
                _bootstrap = bootstrap;
            }

            public RuntimeShutdownResult ShutdownResult
            {
                get { return _bootstrap.ShutdownResult; }
            }

            public void Dispose()
            {
                _bootstrap.Dispose();
            }
        }
    }
}
