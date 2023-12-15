using KingUploader.Core.Application.Interfaces.Context;

namespace KingUploader.Core.Application.Services.Files.Commands.PostFile
{
    public class PostFileService : IPostFileService
    {
        private readonly IDataBaseContext _context;
        public PostFileService(IDataBaseContext context)
        {
            _context = context;

        }
        public bool Execute(RequestPostFileServiceDto req)
        {
            try
            {
                var file = _context.Files.Where(x => x.Filename == req.Filename).FirstOrDefault();
                if (file != null) // update
                {
                    file.FilePart= req.FilePart;
                    file.Start= req.Start;
                    file.UploadDatetime = DateTime.Now;

                    _context.SaveChanges();

                    return true;
                }
                else
                {  // insert
                    Core.Domain.File.Files newFile = new Core.Domain.File.Files();

                    newFile.FilePart = req.FilePart;
                    newFile.Filename = req.Filename;
                    newFile.Start = req.Start;
                    newFile.FilePartCount = req.FilePartCount;

                    _context.Files.Add(newFile);
                    _context.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
