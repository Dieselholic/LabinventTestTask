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

        private bool TryRebootIfDead()
        {
            if (_channel is not null && _channel.IsOpen)
            {
                return true;
            }

            if (_connection is null || !_connection.IsOpen)
            {
                if (!TryCreateConnection())
                {
                    return false;
                }
            }

            if (TryCreateChannel() && _channel!.IsOpen)
            {
                return true;
            }
         
            return false;
        }

        public bool IsAlive
        {
            get
            {
                return TryRebootIfDead();
            }
        }

        public RabbitMQService(IOptions<RabbitMQOptions> options, ILoggerService loggerService)
        {
            _options = options.Value;
            _loggerService = loggerService;
            _loggerService.LogInformation($"Hostname: {_options.RabbitMQHost}; Username: {_options.Username}.",GetType().Name);
            _connectionFactory = new ConnectionFactory()
            {
                HostName = _options.RabbitMQHost,
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
                _loggerService.LogInformation($"Attempting to declare queue...", GetType().Name);

                _channel?.QueueDeclare(
                    queue: _options.RabbitMQQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var body = Encoding.UTF8.GetBytes(jsonData);

                _loggerService.LogInformation($"Attempting to publish data to queue...", GetType().Name);

                _channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: _options.RabbitMQQueue,
                    basicProperties: null,
                    body: body);

                _loggerService.LogInformation("Success.", GetType().Name);
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
                _loggerService.LogInformation($"Attempting to declare queue...", GetType().Name);

                _channel?.QueueDeclare(
                    queue: _options.RabbitMQQueue,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

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

                _channel.BasicConsume(queue: _options.RabbitMQQueue, autoAck: false, consumer: consumer);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, GetType().Name);
                throw;
            }

            _loggerService.LogInformation("Success.", GetType().Name);
        }

        private bool TryCreateConnection()
        {
            try
            {
                _loggerService.LogInformation("Attempting to create connection...", GetType().Name);
                _connection = _connectionFactory.CreateConnection();
            }
            catch (Exception)
            {
                _loggerService.LogInformation($"Connection is not alive.", GetType().Name);
                return false;
            }

            return true;
        }

        private bool TryCreateChannel()
        {
            try
            {
                if (_connection != null)
                {
                    _loggerService.LogInformation("Attempting to create channel...", GetType().Name);
                    _channel = _connection.CreateModel();
                    return true;
                }
            }
            catch (Exception)
            {
                _loggerService.LogInformation($"Connection is not alive.", GetType().Name);
            }

            return false;
        }
    }
}
