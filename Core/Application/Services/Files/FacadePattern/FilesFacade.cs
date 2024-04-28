using KingUploader.Core.Application.Interfaces.Context;
using KingUploader.Core.Application.Interfaces.Facades;
using KingUploader.Core.Application.Services.Files.Commands.DeleteFileAndDatabaseRecords;
using KingUploader.Core.Application.Services.Files.Commands.PostFile;
using KingUploader.Core.Application.Services.Files.Queries.GetCheckResume;
using KingUploader.Core.Application.Services.Files.Queries.GetFilename;

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
        private GetCheckResumeService _getCheckResumeService;
        public GetCheckResumeService GetCheckResumeService
        {
            get { return _getCheckResumeService = _getCheckResumeService ?? new GetCheckResumeService(_context); }
        }     
        private GetFilenameService _getFilenameService;
        public GetFilenameService GetFilenameService
        {
            get { return _getFilenameService = _getFilenameService ?? new GetFilenameService(_context); }
        }    
        private DeleteFileAndDatabaseRecordsService _deleteFileAndDatabaseRecordsService;
        public DeleteFileAndDatabaseRecordsService DeleteFileAndDatabaseRecordsService
        {
            get { return _deleteFileAndDatabaseRecordsService = _deleteFileAndDatabaseRecordsService ?? new DeleteFileAndDatabaseRecordsService(_context); }
        }
        
    }
}
