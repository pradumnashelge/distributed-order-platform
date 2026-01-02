using System.Text;
using System.Text.Json;
using Confluent.Kafka;

namespace OrderService.Infrastructure.Messaging
{
    /// <summary>
    /// Simple Kafka producer that serializes events as JSON strings.
    /// </summary>
    public sealed class KafkaProducer : IDisposable
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topic;
        private readonly JsonSerializerOptions _serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public KafkaProducer(ProducerConfig config, string topic)
        {
            _producer = new ProducerBuilder<string, string>(config).Build();
            _topic = topic;
        }

        /// <summary>
        /// Publishes a message to Kafka as JSON.
        /// </summary>
        /// <typeparam name="T">Event type.</typeparam>
        /// <param name="key">Message key.</param>
        /// <param name="message">Event object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task ProduceAsync<T>(string key, T message, CancellationToken cancellationToken)
        {
            var payload = JsonSerializer.Serialize(message, _serializerOptions);
            var msg = new Message<string, string> { Key = key, Value = payload };

            // Fire and await delivery result to ensure durability in this implementation
            using var _ = cancellationToken.Register(() => { /* noop, Producer's async send not cancellable */ });

            var result = await _producer.ProduceAsync(_topic, msg, cancellationToken).ConfigureAwait(false);
            if (result.Status != PersistenceStatus.Persisted)
            {
                throw new InvalidOperationException($"Kafka message delivery failed: {result.Status}");
            }
        }
        
        public void Dispose() => _producer.Dispose();
    }
}