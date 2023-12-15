namespace KingUploader.Core.Application.Services.Files.Queries.GetLastFilePart
{
    public class RequestGetLastFilePartDto
    {
        public string Filename { get; set; }
        public int FilePartCount { get; set; } // All parts count of a file!
    }
}
