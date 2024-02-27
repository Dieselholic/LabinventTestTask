namespace LabinventTestTask.Common.LoggerService.Options
{
    public class LoggerServiceOptions
    {
        public required bool IsConsoleLoggingEnabled { get; init; }

        public required bool IsFileLoggingEnabled { get; init; }

        public required string LogsFilePath { get; init; }

    }
}
