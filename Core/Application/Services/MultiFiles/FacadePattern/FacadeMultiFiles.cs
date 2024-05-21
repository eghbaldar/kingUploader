using KingUploader.Core.Application.Interfaces.Context;
using KingUploader.Core.Application.Interfaces.Facades;
using KingUploader.Core.Application.Services.MultiFiles.Commands.DeleteMultiFilesAndDatabaseRecords;
using KingUploader.Core.Application.Services.MultiFiles.Commands.PostMultiFiles;

namespace KingUploader.Core.Application.Services.MultiFiles.FacadePattern
{
    public class FacadeMultiFiles:IMultiFilesFacade
    {
        private readonly IDataBaseContext _context;
        public FacadeMultiFiles(IDataBaseContext context)
        {
            _context = context;
        }
        private PostMultiFilesService _postMultiFilesService;
        public PostMultiFilesService PostMultiFilesService
        {
            get { return _postMultiFilesService = _postMultiFilesService ?? new PostMultiFilesService(_context); }
        }
        private DeleteMultiFilesAndDatabaseRecordsService _deleteMultiFilesAndDatabaseRecordsService;
        public DeleteMultiFilesAndDatabaseRecordsService DeleteMultiFilesAndDatabaseRecordsService
        {
            get { return _deleteMultiFilesAndDatabaseRecordsService = _deleteMultiFilesAndDatabaseRecordsService ?? new DeleteMultiFilesAndDatabaseRecordsService(_context); }
        }
    }
}
