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
                Core.Domain.File.Files file = new Core.Domain.File.Files();
                file.Filename = req.Filename;
                file.FilePart = req.FilePart;
                _context.Files.Add(file);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
