using LabinventTestTask.Infrastructure.Database;
using LabinventTestTask.Domain.Entities;
using LabinventTestTask.Common.LoggerService;
using Microsoft.EntityFrameworkCore;
using LabinventTestTask.RabbitMQ.Service;
using System.Text.Json;

namespace DataProcessorService.DataProcessing
{
    public class DataProcessingService : IDataProcessingService
    {
        private readonly IRabbitMQService _rabbitMQService;
        private readonly ILoggerService _loggerService;
        private readonly DataContext _dbContext;
        private readonly IHostApplicationLifetime _appLifetime;

        public DataProcessingService(
            IRabbitMQService rabbitMQService,
            ILoggerService loggerService,
            DataContext dbContext,
            IHostApplicationLifetime appLifetime)
        {
            _rabbitMQService = rabbitMQService;
            _loggerService = loggerService;
            _dbContext = dbContext;
            _loggerService.LogInformation($"Service has been started...", GetType().Name);
            _appLifetime = appLifetime;
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

                _rabbitMQService.CreateDPConsumer(ProcessMessageAsync);
            }
            catch (Exception ex)
            {
                _loggerService.LogFatal(ex, GetType().Name);
                _appLifetime.StopApplication();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken token)
        {
            _loggerService.LogInformation($"Service has been stopped.", GetType().Name);
            return Task.CompletedTask;
        }

        private async Task ProcessMessageAsync(string message)
        {
            _loggerService.LogInformation($"Entered method.", GetType().Name);

            try
            {
                var listOfModuleData = new List<ModuleData>();

                listOfModuleData = JsonSerializer.Deserialize<List<ModuleData>>(message)
                    ?? throw new ArgumentException($"Argument {nameof(message)} was null.");

                if (listOfModuleData.Count <= 0)
                {
                    throw new ArgumentException($"Argument {nameof(message)} doent's contain any data.");
                }

                await SendToDatabase(listOfModuleData);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, GetType().Name);
            }
        }

        private async Task SendToDatabase(List<ModuleData> listOfModuleData)
        {
            _loggerService.LogInformation($"Entered method.", GetType().Name);

            try
            {
                foreach (var moduleData in listOfModuleData)
                {
                    _loggerService.LogInformation($"Attempting to apply data to database...", GetType().Name);

                    var existingData = await _dbContext.ModuleData.FirstOrDefaultAsync(md => md.ModuleCategoryID == moduleData.ModuleCategoryID);

                    if (existingData != null)
                    {
                        existingData.ModuleState = moduleData.ModuleState;
                        _dbContext.Entry(existingData).State = EntityState.Modified;
                    }
                    else
                    {
                        var newData = new ModuleData(moduleData.ModuleCategoryID, moduleData.ModuleState);
                        _dbContext.ModuleData.Add(newData);
                    }
                }

                await _dbContext.SaveChangesAsync();

                _loggerService.LogInformation($"Success.", GetType().Name);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, GetType().Name);
                throw;
            }
        }
    }
}
