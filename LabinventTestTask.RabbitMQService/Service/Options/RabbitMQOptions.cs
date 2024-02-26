namespace LabinventTestTask.RabbitMQ.Service.Options
{
    public class RabbitMQOptions
    {
        public required string RabbitMQHost { get; init; }
        public required string RabbitMQQueue { get; init; }
        public required string Username { get; init; }
        public required string Password { get; init; }
    }
}
