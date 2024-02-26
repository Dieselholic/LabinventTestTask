namespace LabinventTestTask.RabbitMQ.Service
{
    public interface IRabbitMQService
    {
        void SendToRabbitMQ(string jsonData);

        void CreateDPConsumer(Func<string, Task> dPMethod);

        bool IsAlive { get; }
    }
}
