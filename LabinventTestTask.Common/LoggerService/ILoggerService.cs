using System.Runtime.CompilerServices;

namespace LabinventTestTask.Common.LoggerService
{
    public interface ILoggerService
    {
        public void LogInformation(string message, string serviceName, [CallerMemberName] string memberName = "");
        public void LogError(Exception ex, string serviceName, [CallerMemberName] string memberName = "");
        public void LogFatal(Exception ex, string serviceName, [CallerMemberName] string memberName = "");
    }
}
