using Confluent.Kafka;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Outbox;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Infrastructure
{
    /// <summary>
    /// Registers infrastructure services.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers DB, messaging, and outbox services.
        /// </summary>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // EF Core - SQLite
            services.AddDbContext<OrderDbContext>(opts =>
                opts.UseSqlite(configuration.GetConnectionString("Sqlite")));

            // MassTransit - RabbitMQ
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitHost = configuration["RabbitMq:Host"] ?? "rabbitmq://localhost";
                    var user = configuration["RabbitMq:Username"] ?? "guest";
                    var pass = configuration["RabbitMq:Password"] ?? "guest";

                    cfg.Host(new Uri(rabbitHost), h =>
                    {
                        h.Username(user);
                        h.Password(pass);
                    });

                    // Configure durable exchange for order-created (MassTransit will manage topology)
                    cfg.Publish<OrderService.Domain.Events.OrderCreatedEvent>(p => p.ExchangeType = "fanout");
                });
            });

            // Kafka producer
            var kafkaServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
            var producerConfig = new ProducerConfig { BootstrapServers = kafkaServers };
            services.AddSingleton(new KafkaProducer(producerConfig, "order-events"));

            // MassTransit publisher adapter
            services.AddScoped<MassTransitPublisher>();

            // Outbox publisher background service
            services.AddSingleton<IOutboxPublisher, OutboxPublisherService>();

            return services;
        }
    }
}