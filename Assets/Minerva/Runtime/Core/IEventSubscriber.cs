using System;

namespace Minerva.Core
{
    /// <summary>
    /// Registers strongly typed handlers for exact event types.
    /// </summary>
    public interface IEventSubscriber
    {
        /// <summary>
        /// Subscribes a handler and returns its explicit lifetime handle.
        /// </summary>
        IDisposable Subscribe<TEvent>(Action<TEvent> handler)
            where TEvent : IEvent;
    }
}
