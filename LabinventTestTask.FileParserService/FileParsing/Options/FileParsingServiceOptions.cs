namespace LabinventTestTask.FileParserService.FileParsing.Options
{
    public class FileParsingServiceOptions
    {
        public required string FileStoragePath { get; init; }
        public required int ServiceTimeoutMs { get; init; }
    }
}
