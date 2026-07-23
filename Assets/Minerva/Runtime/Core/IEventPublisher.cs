namespace Minerva.Core
{
    /// <summary>
    /// Publishes strongly typed event messages to exact-type subscribers.
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publishes an event or queues it when another publication is being dispatched.
        /// </summary>
        EventPublicationResult Publish<TEvent>(TEvent eventMessage)
            where TEvent : IEvent;
    }
}
