using System;
using System.Collections.Generic;

namespace Minerva.Core
{
    /// <summary>
    /// Owns explicit service composition and deterministic runtime lifecycle ordering.
    /// </summary>
    public sealed class RuntimeBootstrap : IDisposable
    {
        private readonly List<IRuntimeService> _services;
        private readonly List<IRuntimeService> _initializedServices;
        private bool _hasInitializationRun;

        /// <summary>
        /// Creates an empty runtime bootstrap.
        /// </summary>
        public RuntimeBootstrap()
        {
            _services = new List<IRuntimeService>();
            _initializedServices = new List<IRuntimeService>();
            State = RuntimeLifecycleState.Created;
        }

        /// <summary>
        /// Gets the bootstrap's current lifecycle state.
        /// </summary>
        public RuntimeLifecycleState State { get; private set; }

        /// <summary>
        /// Gets the result of the sole initialization attempt, when one has occurred.
        /// </summary>
        public RuntimeInitializationResult InitializationResult { get; private set; }

        /// <summary>
        /// Gets the result of shutdown, when shutdown has occurred.
        /// </summary>
        public RuntimeShutdownResult ShutdownResult { get; private set; }

        /// <summary>
        /// Registers a service at the end of the deterministic initialization order.
        /// </summary>
        public void Register(IRuntimeService service)
        {
            ThrowIfDisposed();

            if (_hasInitializationRun)
            {
                throw new InvalidOperationException("Services cannot be registered after initialization has begun.");
            }

            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            for (int index = 0; index < _services.Count; index++)
            {
                if (object.ReferenceEquals(_services[index], service))
                {
                    throw new InvalidOperationException("The same service instance cannot be registered more than once.");
                }
            }

            _services.Add(service);
        }

        /// <summary>
        /// Initializes registered services in registration order exactly once.
        /// </summary>
        public RuntimeInitializationResult Initialize()
        {
            ThrowIfDisposed();

            if (_hasInitializationRun)
            {
                throw new InvalidOperationException("Runtime initialization can only run once.");
            }

            _hasInitializationRun = true;
            State = RuntimeLifecycleState.Initializing;

            for (int index = 0; index < _services.Count; index++)
            {
                IRuntimeService service = _services[index];
                ServiceInitializationResult serviceResult;

                try
                {
                    serviceResult = service.Initialize();
                }
                catch (Exception exception)
                {
                    return FailInitialization(service, exception.Message);
                }

                if (serviceResult == null)
                {
                    return FailInitialization(service, "The service returned no initialization result.");
                }

                if (!serviceResult.IsSuccessful)
                {
                    return FailInitialization(service, serviceResult.FailureReason);
                }

                _initializedServices.Add(service);
            }

            InitializationResult = RuntimeInitializationResult.Success();
            State = RuntimeLifecycleState.Running;
            return InitializationResult;
        }

        /// <summary>
        /// Shuts down successfully initialized services once, in reverse order.
        /// </summary>
        public RuntimeShutdownResult Shutdown()
        {
            if (State == RuntimeLifecycleState.Disposed ||
                State == RuntimeLifecycleState.ShutDown ||
                State == RuntimeLifecycleState.ShutdownFailed)
            {
                return ShutdownResult;
            }

            List<RuntimeShutdownFailure> failures = new List<RuntimeShutdownFailure>();

            for (int index = _initializedServices.Count - 1; index >= 0; index--)
            {
                IRuntimeService service = _initializedServices[index];

                try
                {
                    service.Shutdown();
                }
                catch (Exception exception)
                {
                    failures.Add(new RuntimeShutdownFailure(
                        service.GetType(),
                        exception.GetType(),
                        exception.Message));
                }
            }

            _initializedServices.Clear();
            ShutdownResult = new RuntimeShutdownResult(failures);
            State = ShutdownResult.IsSuccessful
                ? RuntimeLifecycleState.ShutDown
                : RuntimeLifecycleState.ShutdownFailed;
            return ShutdownResult;
        }

        /// <summary>
        /// Shuts down initialized services and permanently disposes this bootstrap.
        /// </summary>
        public void Dispose()
        {
            if (State == RuntimeLifecycleState.Disposed)
            {
                return;
            }

            Shutdown();
            State = RuntimeLifecycleState.Disposed;
        }

        private RuntimeInitializationResult FailInitialization(
            IRuntimeService service,
            string failureReason)
        {
            InitializationResult = RuntimeInitializationResult.Failure(
                service.GetType(),
                failureReason);
            State = RuntimeLifecycleState.InitializationFailed;
            return InitializationResult;
        }

        private void ThrowIfDisposed()
        {
            if (State == RuntimeLifecycleState.Disposed)
            {
                throw new ObjectDisposedException("RuntimeBootstrap");
            }
        }
    }
}
