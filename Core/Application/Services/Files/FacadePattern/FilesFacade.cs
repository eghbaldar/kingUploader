using KingUploader.Core.Application.Interfaces.Context;
using KingUploader.Core.Application.Interfaces.Facades;
using KingUploader.Core.Application.Services.Files.Commands.PostFile;

namespace KingUploader.Core.Application.Services.Files.FacadePattern
{
    public class FilesFacade: IFilesFacade
    {
        private readonly IDataBaseContext _context;
        public FilesFacade(IDataBaseContext context)
        {
            _context = context;
        }

        private PostFileService _postFileService;
        public PostFileService PostFileService
        {
            get { return _postFileService = _postFileService ?? new PostFileService(_context); }
        }
    }
}
