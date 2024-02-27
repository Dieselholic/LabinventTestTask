namespace LabinventTestTask.RabbitMQ.Service.Options
{
    public class RabbitMQOptions
    {
        public required string HostName { get; init; }
        public required string Username { get; init; }
        public required string Password { get; init; }
        public required string QueueName { get; init; }
        public required bool IsQueueDurable { get; init; }
        public required bool IsQueueExclusive { get; init; }
        public required bool HasAutoDelete { get; init; }

    }
}
