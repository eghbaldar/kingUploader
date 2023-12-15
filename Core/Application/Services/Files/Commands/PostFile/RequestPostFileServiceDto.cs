namespace KingUploader.Core.Application.Services.Files.Commands.PostFile
{
    public class RequestPostFileServiceDto
    {
        public string Filename { get; set; }
        public int FilePart { get; set; }
        public string Start { get; set; } // the Byte-Index of file // the first value is ZERO
        public int FilePartCount { get; set; } // All parts of the file that are going to be separated by the basis of "each chunk of file = (1024*100)" will be stored.
    }
}
