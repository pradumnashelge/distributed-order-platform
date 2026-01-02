namespace OrderService.Infrastructure.Outbox
{
    /// <summary>
    /// Outbox publisher contract.
    /// </summary>
    public interface IOutboxPublisher
    {
        /// <summary>
        /// Starts the background publishing loop. The implementation should stop when cancellation token is triggered.
        /// </summary>
        /// <param name="applicationStopping">Token provided by host to signal shutdown.</param>
        void StartPublishing(CancellationToken applicationStopping);
    }
}