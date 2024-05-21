using KingUploader.Core.Application.Services.MultiFiles.Commands.DeleteMultiFilesAndDatabaseRecords;
using KingUploader.Core.Application.Services.MultiFiles.Commands.PostMultiFiles;

namespace KingUploader.Core.Application.Interfaces.Facades
{
    public interface IMultiFilesFacade
    {
        public PostMultiFilesService PostMultiFilesService { get; }
        public DeleteMultiFilesAndDatabaseRecordsService DeleteMultiFilesAndDatabaseRecordsService { get; }
    }
}
