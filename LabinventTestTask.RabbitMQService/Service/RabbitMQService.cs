using RabbitMQ.Client;
using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using LabinventTestTask.Common.LoggerService;
using LabinventTestTask.RabbitMQ.Service.Options;

namespace LabinventTestTask.RabbitMQ.Service
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly RabbitMQOptions _options;
        private readonly ILoggerService _loggerService;
        private readonly ConnectionFactory _connectionFactory;
        private IConnection? _connection;
        private IModel? _channel;

        public bool IsAlive
        {
            get
            {
                return TryCreateConnection() && TryCreateChannel();
            }
        }

        public RabbitMQService(IOptions<RabbitMQOptions> options, ILoggerService loggerService)
        {
            _options = options.Value;
            _loggerService = loggerService;
            _connectionFactory = new ConnectionFactory()
            {
                HostName = _options.HostName,
                UserName = _options.Username,
                Password = _options.Password
            };
        }

        public void SendToRabbitMQ(string jsonData)
        {
            _loggerService.LogInformation($"Entered method.", GetType().Name);

            if (!IsAlive)
            {
                return;
            }

            try
            {
                if (!TryDeclageQueue())
                {
                    return;
                }

                var body = Encoding.UTF8.GetBytes(jsonData);

                _loggerService.LogInformation($"Attempting to publish data to queue...", GetType().Name);

                _channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: _options.QueueName,
                    basicProperties: null,
                    body: body);

                _loggerService.LogInformation($"Success", GetType().Name);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, GetType().Name);
                throw;
            }
        }

        public void CreateDPConsumer(Func<string, Task> dPMethod)
        {
            _loggerService.LogInformation($"Entered method.", GetType().Name);
            
            if (!IsAlive)
            {
                return;
            }

            try
            {
                if (!TryDeclageQueue())
                {
                    return;
                }

                _loggerService.LogInformation($"Attempting to create consumer...", GetType().Name);

                var consumer = new EventingBasicConsumer(_channel);

                _loggerService.LogInformation($"Attempting to add recieve event to consumer...", GetType().Name);

                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    _loggerService.LogInformation($"Recieved message \"{message}\"", GetType().Name);

                    await dPMethod(message);

                    _channel?.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };

                _loggerService.LogInformation($"Attempting to start consume...", GetType().Name);

                _channel.BasicConsume(queue: _options.QueueName, autoAck: false, consumer: consumer);

                _loggerService.LogInformation($"Success", GetType().Name);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, GetType().Name);
                throw;
            }
        }

        private bool TryCreateConnection()
        {
            if (_connection is not null && _connection.IsOpen)
            {
                return true;
            }

            try
            {
                _loggerService.LogInformation("Attempting to create connection...", GetType().Name);
                _connection = _connectionFactory.CreateConnection();
                return true;
            }
            catch (Exception)
            {
                _loggerService.LogInformation($"Failed to create connection.", GetType().Name);
            }

            return false;
        }

        private bool TryCreateChannel()
        {
            if (_channel is not null && _channel.IsOpen)
            {
                return true;
            }

            try
            {
                _loggerService.LogInformation("Attempting to create channel...", GetType().Name);
                _channel = _connection!.CreateModel();
                return true;
            }
            catch (Exception)
            {
                _loggerService.LogInformation($"Failed to create channel.", GetType().Name);
            }

            return false;
        }

        private bool TryDeclageQueue()
        {
            if (_channel?.CurrentQueue is not null)
            {
                return true;
            }

            try
            {
                _loggerService.LogInformation($"Attempting to declare queue...", GetType().Name);
                _channel?.QueueDeclare(
                    _options.QueueName,
                    _options.IsQueueDurable,
                    _options.IsQueueExclusive,
                    _options.HasAutoDelete,
                    null);
                return true;
            }
            catch (Exception)
            {
                _loggerService.LogInformation($"Failed to declare queue.", GetType().Name);
            }

            return false;
        }
    }
}
