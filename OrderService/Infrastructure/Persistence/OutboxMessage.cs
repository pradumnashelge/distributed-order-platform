using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Infrastructure.Persistence
{
    /// <summary>
    /// Represents an outbox message stored in the database for reliable messaging.
    /// </summary>
    public sealed class OutboxMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
        public string Payload { get; set; } = string.Empty;

        public DateTime OccurredOn { get; set; }

        public bool Processed { get; set; }
    }
}