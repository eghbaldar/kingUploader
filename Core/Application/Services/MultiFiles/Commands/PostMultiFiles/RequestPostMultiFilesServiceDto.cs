namespace KingUploader.Core.Application.Services.MultiFiles.Commands.PostMultiFiles
{
    public class RequestPostMultiFilesServiceDto
    {
        public IFormFile File { get; set; }
        public string Filename { get; set; }
        public int FilePart { get; set; }
        public string Start { get; set; } // the Byte-Index of file // the first value is ZERO
        public int FilePartCount { get; set; } // All parts of the file that are going to be separated by the basis of "each chunk of file = (1024*100)" will be stored.
        public Guid SpecificFolderName { get; set; }
        public string TotalFileSize { get; set; }
        public string OriginalFileExtension { get; set; }
        public string[] acceptableExensions { get; set; } // acceptable extension
        public string AcceptableFileSize { get; set; } // acceptable fileSize
    }

}
