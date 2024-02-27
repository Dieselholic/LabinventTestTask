using LabinventTestTask.Infrastructure.Database;
using DataProcessorService.DataProcessing;
using Microsoft.EntityFrameworkCore;
using LabinventTestTask.RabbitMQ.Service;
using LabinventTestTask.Common.OptionsSetup;
using LabinventTestTask.Common.LoggerService.Options;
using LabinventTestTask.Common.LoggerService;
using LabinventTestTask.RabbitMQ.Service.Options;
using LabinventTestTask.Domain.Entities;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<DataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.ConfigureOptions<OptionsSetup<LoggerServiceOptions>>();
builder.Services.ConfigureOptions<OptionsSetup<RabbitMQOptions>>();

builder.Services.AddSingleton<ILoggerService, LoggerService>();
builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();
builder.Services.AddSingleton<IDataProcessingService, DataProcessingService>();

builder.Services.AddHostedService(provider => provider.GetRequiredService<IDataProcessingService>());

var app = builder.Build();

app.Map("/", async (DataContext context) =>
{
    var sb = new StringBuilder();

    sb.AppendLine($"|{nameof(ModuleData.ModuleCategoryID),-20}|{nameof(ModuleData.ModuleState),-15}|");

    var tableLine = new string('-', sb.Length - 2);

    sb.Insert(0, tableLine + '\n');
    sb.AppendLine(tableLine);
    await foreach (var module in context.ModuleData)
    {
        sb.AppendLine($"|{module.ModuleCategoryID,-20}|{module.ModuleState,-15}|");
    }

    sb.AppendLine(tableLine);

    sb.Insert(0, $"Updated: {DateTime.Now:HH:mm:ss}.\n");

    return sb.ToString();
});

app.Run();