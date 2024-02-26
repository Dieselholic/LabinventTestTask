using LabinventTestTask.FileParserService.FileParsing;
using LabinventTestTask.RabbitMQ.Service;
using LabinventTestTask.Common.OptionsSetup;
using LabinventTestTask.Common.LoggerService.Options;
using LabinventTestTask.Common.LoggerService;
using LabinventTestTask.RabbitMQ.Service.Options;
using LabinventTestTask.FileParserService.FileParsing.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureOptions<OptionsSetup<LoggerServiceOptions>>();
builder.Services.ConfigureOptions<OptionsSetup<RabbitMQOptions>>();
builder.Services.ConfigureOptions<OptionsSetup<FileParsingServiceOptions>>();

builder.Services.AddSingleton<ILoggerService, LoggerService>();
builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();
builder.Services.AddSingleton<IFileParsingService, FileParsingService>();

builder.Services.AddHostedService(provider => provider.GetRequiredService<IFileParsingService>());

var app = builder.Build();

app.Run();
