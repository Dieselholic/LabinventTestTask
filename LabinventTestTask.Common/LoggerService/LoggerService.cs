using LabinventTestTask.Common.LoggerService.Options;
using Microsoft.Extensions.Options;
using Serilog;
using System.Runtime.CompilerServices;

namespace LabinventTestTask.Common.LoggerService
{
    public class LoggerService : ILoggerService
    {
        private readonly ILogger _logger;
        private readonly LoggerServiceOptions _options;

        public LoggerService(IOptions<LoggerServiceOptions> options)
        {
            _options = options.Value;
            _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(_options.LogsFilePath, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public void LogInformation(string message, string serviceName, [CallerMemberName] string memberName = "")
        {
            _logger.Information("| SERVICE : {serviceName,-25}| METHOD : {methodName,-25}| MESSAGE : {message}", serviceName, memberName, message);
        }

        public void LogError(Exception ex, string serviceName, [CallerMemberName] string memberName = "")
        {
            if (_options.IsStackTraceOn)
            {
                _logger.Error(ex, "| SERVICE : {serviceName,-25}| METHOD : {methodName,-25}| ERROR MESSAGE : {message}\nSTACK TRACE : {stackTrace}", serviceName, memberName, ex.Message, ex.StackTrace);
            }
            else
            {
                _logger.Error(ex, "| SERVICE : {serviceName,-25}| METHOD : {methodName,-25}| ERROR MESSAGE : {message}", serviceName, memberName, ex.Message);
            }
        }

        public void LogFatal(Exception ex, string serviceName, [CallerMemberName] string memberName = "")
        {
            if (_options.IsStackTraceOn)
            {
                _logger.Fatal(ex, "| SERVICE : {serviceName,-25}| METHOD : {methodName,-25}| ERROR MESSAGE : {message}\nSTACK TRACE : {stackTrace}", serviceName, memberName, ex.Message, ex.StackTrace);
            }
            else
            {
                _logger.Fatal(ex, "| SERVICE : {serviceName,-25}| METHOD : {methodName,-25}| ERROR MESSAGE : {message}", serviceName, memberName, ex.Message);
            }
        }
    }
}
