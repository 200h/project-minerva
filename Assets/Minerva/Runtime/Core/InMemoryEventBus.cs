using System;
using System.Collections.Generic;

namespace Minerva.Core
{
    /// <summary>
    /// Provides deterministic, single-threaded, in-memory event dispatch.
    /// </summary>
    public sealed class InMemoryEventBus : IEventBus
    {
        private readonly Dictionary<Type, List<ISubscription>> _subscriptions;
        private readonly Queue<IPendingPublication> _pendingPublications;
        private bool _isDispatching;
        private bool _isDisposed;

        /// <summary>
        /// Creates an empty event bus with instance-owned subscriptions.
        /// </summary>
        public InMemoryEventBus()
        {
            _subscriptions = new Dictionary<Type, List<ISubscription>>();
            _pendingPublications = new Queue<IPendingPublication>();
        }

        /// <summary>
        /// Subscribes a handler in registration order for its exact event type.
        /// </summary>
        public IDisposable Subscribe<TEvent>(Action<TEvent> handler)
            where TEvent : IEvent
        {
            ThrowIfDisposed();

            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            Type eventType = typeof(TEvent);
            List<ISubscription> eventSubscriptions;

            if (!_subscriptions.TryGetValue(eventType, out eventSubscriptions))
            {
                eventSubscriptions = new List<ISubscription>();
                _subscriptions.Add(eventType, eventSubscriptions);
            }

            Subscription<TEvent> subscription =
                new Subscription<TEvent>(this, eventType, handler);
            eventSubscriptions.Add(subscription);
            return subscription;
        }

        /// <summary>
        /// Publishes an event immediately or queues it behind the active dispatch.
        /// </summary>
        public EventPublicationResult Publish<TEvent>(TEvent eventMessage)
            where TEvent : IEvent
        {
            ThrowIfDisposed();

            if (object.ReferenceEquals(eventMessage, null))
            {
                throw new ArgumentNullException("eventMessage");
            }

            Type eventType = typeof(TEvent);
            if (eventMessage.GetType() != eventType)
            {
                throw new ArgumentException(
                    "The published message must have the exact generic event type.",
                    "eventMessage");
            }

            EventPublicationResult result = new EventPublicationResult(eventType);
            _pendingPublications.Enqueue(
                new PendingPublication<TEvent>(eventMessage, result));

            if (_isDispatching)
            {
                return result;
            }

            DrainPendingPublications();
            return result;
        }

        /// <summary>
        /// Permanently rejects new work and removes every subscription.
        /// Already queued publications finish without delivery.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;

            foreach (KeyValuePair<Type, List<ISubscription>> pair in _subscriptions)
            {
                List<ISubscription> eventSubscriptions = pair.Value;
                for (int index = 0; index < eventSubscriptions.Count; index++)
                {
                    eventSubscriptions[index].Deactivate();
                }
            }

            _subscriptions.Clear();
        }

        private void DrainPendingPublications()
        {
            _isDispatching = true;

            try
            {
                while (_pendingPublications.Count > 0)
                {
                    _pendingPublications.Dequeue().Dispatch(this);
                }
            }
            finally
            {
                _isDispatching = false;
            }
        }

        private void Dispatch<TEvent>(
            TEvent eventMessage,
            EventPublicationResult result)
            where TEvent : IEvent
        {
            List<ISubscription> eventSubscriptions;
            if (!_subscriptions.TryGetValue(typeof(TEvent), out eventSubscriptions))
            {
                result.IsComplete = true;
                return;
            }

            ISubscription[] eligibleSubscriptions = eventSubscriptions.ToArray();

            for (int index = 0; index < eligibleSubscriptions.Length; index++)
            {
                Subscription<TEvent> subscription =
                    (Subscription<TEvent>)eligibleSubscriptions[index];

                if (!subscription.IsActive)
                {
                    continue;
                }

                try
                {
                    subscription.Invoke(eventMessage);
                }
                catch (Exception exception)
                {
                    result.AddFailure(new EventSubscriberFailure(
                        typeof(TEvent),
                        subscription.SubscriberType,
                        subscription.SubscriberMethodName,
                        exception.GetType(),
                        exception.Message));
                }
            }

            result.IsComplete = true;
        }

        private void RemoveSubscription(Type eventType, ISubscription subscription)
        {
            List<ISubscription> eventSubscriptions;
            if (!_subscriptions.TryGetValue(eventType, out eventSubscriptions))
            {
                return;
            }

            eventSubscriptions.Remove(subscription);

            if (eventSubscriptions.Count == 0)
            {
                _subscriptions.Remove(eventType);
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("InMemoryEventBus");
            }
        }

        private interface ISubscription
        {
            bool IsActive { get; }

            void Deactivate();
        }

        private sealed class Subscription<TEvent> : ISubscription, IDisposable
            where TEvent : IEvent
        {
            private InMemoryEventBus _owner;
            private Action<TEvent> _handler;
            private readonly Type _eventType;

            public Subscription(
                InMemoryEventBus owner,
                Type eventType,
                Action<TEvent> handler)
            {
                _owner = owner;
                _eventType = eventType;
                _handler = handler;
                SubscriberType = handler.Target == null
                    ? handler.Method.DeclaringType
                    : handler.Target.GetType();
                SubscriberMethodName = handler.Method.Name;
            }

            public bool IsActive
            {
                get { return _handler != null; }
            }

            public Type SubscriberType { get; private set; }

            public string SubscriberMethodName { get; private set; }

            public void Invoke(TEvent eventMessage)
            {
                _handler(eventMessage);
            }

            public void Dispose()
            {
                if (!IsActive)
                {
                    return;
                }

                InMemoryEventBus owner = _owner;
                Deactivate();
                owner.RemoveSubscription(_eventType, this);
            }

            public void Deactivate()
            {
                _handler = null;
                _owner = null;
            }
        }

        private interface IPendingPublication
        {
            void Dispatch(InMemoryEventBus eventBus);
        }

        private sealed class PendingPublication<TEvent> : IPendingPublication
            where TEvent : IEvent
        {
            private readonly TEvent _eventMessage;
            private readonly EventPublicationResult _result;

            public PendingPublication(
                TEvent eventMessage,
                EventPublicationResult result)
            {
                _eventMessage = eventMessage;
                _result = result;
            }

            public void Dispatch(InMemoryEventBus eventBus)
            {
                eventBus.Dispatch(_eventMessage, _result);
            }
        }
    }
}
