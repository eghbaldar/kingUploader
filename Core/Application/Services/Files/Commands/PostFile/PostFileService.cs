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
                // before inserting a new one, I will delete all recods, because I only need the last one!
                _context.Files.RemoveRange(_context.Files.Where(x => x.Filename == req.Filename).Select(x => x));
                //
                Core.Domain.File.Files newFile = new Core.Domain.File.Files();

                newFile.FilePart = req.FilePart;
                newFile.Filename = req.Filename;
                newFile.Start = req.Start;

                _context.Files.Add(newFile);
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
