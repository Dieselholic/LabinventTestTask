using LabinventTestTask.Infrastructure.Database;
using DataProcessorService.DataProcessing;
using Microsoft.EntityFrameworkCore;
using LabinventTestTask.RabbitMQ.Service;
using LabinventTestTask.Common.OptionsSetup;
using LabinventTestTask.Common.LoggerService.Options;
using LabinventTestTask.Common.LoggerService;
using LabinventTestTask.RabbitMQ.Service.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")),
        ServiceLifetime.Singleton);

builder.Services.ConfigureOptions<OptionsSetup<LoggerServiceOptions>>();
builder.Services.ConfigureOptions<OptionsSetup<RabbitMQOptions>>();

builder.Services.AddSingleton<ILoggerService, LoggerService>();
builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();
builder.Services.AddSingleton<IDataProcessingService, DataProcessingService>();

builder.Services.AddHostedService(provider => provider.GetRequiredService<IDataProcessingService>());

var app = builder.Build();

app.Run();