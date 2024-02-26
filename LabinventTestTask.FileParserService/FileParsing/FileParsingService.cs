using LabinventTestTask.Domain.DTO;
using LabinventTestTask.Common.LoggerService;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using LabinventTestTask.RabbitMQ.Service;
using System.Xml.Linq;
using LabinventTestTask.FileParserService.FileParsing.Options;

namespace LabinventTestTask.FileParserService.FileParsing
{
    public class FileParsingService : IFileParsingService
    {
        private readonly FileParsingServiceOptions _options;
        private readonly ILoggerService _loggerService;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly string _xmlStoragePath;
        private readonly Timer _timer;

        public FileParsingService(
            IOptions<FileParsingServiceOptions> options,
            ILoggerService loggerService,
            IRabbitMQService rabbitMQService,
            IHostApplicationLifetime appLifetime)
        {
            _options = options.Value;
            _loggerService = loggerService;
            _rabbitMQService = rabbitMQService;
            _loggerService.LogInformation($"Service has been started...", GetType().Name);
            _appLifetime = appLifetime;
            _timer = new Timer(TimedFileParsing);
            _xmlStoragePath = Path.Combine(Environment.CurrentDirectory, _options.FileStoragePath);
        }

        public Task StartAsync(CancellationToken token)
        {
            _loggerService.LogInformation($"Entered method.", GetType().Name);

            try
            {
                _loggerService.LogInformation($"Attempt to connect...", GetType().Name);

                while (!_rabbitMQService.IsAlive)
                {
                    _loggerService.LogInformation($"Timeout 10000 ms...", GetType().Name);
                    Task.Delay(TimeSpan.FromMilliseconds(10000), token).GetAwaiter().GetResult();
                }

                _loggerService.LogInformation($"Connected successfully.", GetType().Name);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, GetType().Name);
                _appLifetime.StopApplication();
            }

            _timer.Change(TimeSpan.Zero, TimeSpan.FromMilliseconds(_options.ServiceTimeoutMs));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken token)
        {
            _loggerService.LogInformation($"Service has been stopped.", GetType().Name);
            return Task.CompletedTask;
        }

        private void TimedFileParsing(object? state)
        {
            _loggerService.LogInformation($"Entered method.", GetType().Name);
            
            try
            {
                if (Directory.Exists(_xmlStoragePath))
                {
                    foreach (var filePath in Directory.GetFiles(_xmlStoragePath, "*.xml"))
                    {
                        ThreadPool.QueueUserWorkItem(ProcessFile, filePath);
                    }
                }
                else
                {
                    throw new ArgumentException("Incorrect xml-storage path.");
                }
            }
            catch (Exception ex)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _loggerService.LogFatal(ex, GetType().Name);
                _appLifetime.StopApplication();
            }

            return;
        }

        private void ProcessFile(object? state)
        {
            _loggerService.LogInformation($"Thread {Environment.CurrentManagedThreadId} entered method.", GetType().Name);

            try
            {
                if (state is not string filePath)
                {
                    throw new ArgumentException($"{nameof(filePath)} was null.");
                }

                _loggerService.LogInformation($"Thread {Environment.CurrentManagedThreadId} is attempting to parse xml-file with filepath {filePath}...", GetType().Name);

                if (!File.Exists(filePath))
                {
                    var ex = new ArgumentException($"Filepath is invalid or file doesn't exist.");
                    ex.Data.Add(nameof(filePath), filePath);
                    throw ex;
                }

                var xDocData = XDocument.Load(filePath);
                var modyfiedData = GetModyfiedData(xDocData);
                var jsonData = JsonConvert.SerializeObject(modyfiedData);

                _loggerService.LogInformation($"Thread {Environment.CurrentManagedThreadId} sends parsed json data to queue.", GetType().Name);

                _rabbitMQService.SendToRabbitMQ(jsonData);

                _loggerService.LogInformation($"Thread {Environment.CurrentManagedThreadId} successfully sent data to queue.", GetType().Name);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, GetType().Name);
                throw;
            }
        }

        private IEnumerable<ModuleDataDTO> GetModyfiedData(XDocument xDoc)
        {
            _loggerService.LogInformation($"Entered method.", GetType().Name);

            var random = new Random();

            foreach (var categoryId in xDoc.Descendants("ModuleCategoryID"))
            {
                yield return new ModuleDataDTO(categoryId.Value, GetRandomModuleState(random));
            }
        }

        private static string GetRandomModuleState(Random random)
        {
            var states = Enum.GetNames(typeof(ModuleStatesEnum));
            return states[random.Next(states.Length)];
        }
    }
}
