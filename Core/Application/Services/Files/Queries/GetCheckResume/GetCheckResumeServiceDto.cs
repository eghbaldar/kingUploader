namespace KingUploader.Core.Application.Services.Files.Queries.GetCheckResume
{
    public class GetCheckResumeServiceDto
    {
        public long Id { get; set; }
        public string Filename { get; set; }
        public long FilePartCount { get; set; } // the total file part count based on chunk size
        public long FilePart { get; set; }
        public string Start { get; set; } // the Byte-Index of file // the first value is ZERO
        public DateTime UploadDatetime { get; set; } = DateTime.Now;
        public bool Resume { get; set; } // False => there is no the left file
    }
}
