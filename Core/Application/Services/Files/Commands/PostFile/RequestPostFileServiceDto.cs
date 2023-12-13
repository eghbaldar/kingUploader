namespace KingUploader.Core.Application.Services.Files.Commands.PostFile
{
    public class RequestPostFileServiceDto
    {
        public string Filename { get; set; }
        public int FilePart { get; set; }
        public string Start { get; set; } // the Byte-Index of file // the first value is ZERO
    }
}
