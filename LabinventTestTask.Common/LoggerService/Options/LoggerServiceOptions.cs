namespace LabinventTestTask.Common.LoggerService.Options
{
    public class LoggerServiceOptions
    {
        public required string LogsFilePath { get; init; }

        public required bool IsStackTraceOn { get; init; }
    }
}
