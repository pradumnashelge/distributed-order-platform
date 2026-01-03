using Confluent.Kafka;
using AnalyticsService.Infrastructure;
using System.Text.Json;

namespace AnalyticsService.Consumers;

public class OrderEventConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly BlobStorageService _blobStorage;
    private readonly ILogger<OrderEventConsumer> _logger;

    public OrderEventConsumer(
        IConfiguration configuration,
        BlobStorageService blobStorage,
        ILogger<OrderEventConsumer> logger)
    {
        _configuration = configuration;
        _blobStorage = blobStorage;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = "analytics-service-group",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe(_configuration["Kafka:Topic"]);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                var payload = consumeResult.Message.Value;

                var orderEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(payload);

                if (orderEvent != null)
                {
                    var fileName =
                        $"orders/{orderEvent.CreatedAt:yyyy/MM/dd}/{orderEvent.OrderId}.json";

                    await _blobStorage.SaveAsync(fileName, payload);

                    _logger.LogInformation(
                        "Stored analytics event for Order {OrderId}",
                        orderEvent.OrderId);
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kafka consumption error");
            }
        }
    }
}
