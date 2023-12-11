using System.ComponentModel.DataAnnotations;

namespace KingUploader.Core.Domain.File
{
    public class Files
    {
        [Key]
        public long Id{ get; set; }
        public string Filename { get; set; }
        public int FilePart { get; set; }
        public DateTime UploadDatetime { get; set; } = DateTime.Now;
    }
}
