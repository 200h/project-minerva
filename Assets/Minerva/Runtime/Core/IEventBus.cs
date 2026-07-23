using System;

namespace Minerva.Core
{
    /// <summary>
    /// Combines strongly typed event publication, subscription, and cleanup.
    /// </summary>
    public interface IEventBus : IEventPublisher, IEventSubscriber, IDisposable
    {
    }
}
