using KingUploader.Core.Application.Services.Files.Commands.DeleteFileAndDatabaseRecords;
using KingUploader.Core.Application.Services.Files.Commands.PostFile;
using KingUploader.Core.Application.Services.Files.Queries.GetCheckResume;
using KingUploader.Core.Application.Services.Files.Queries.GetFilename;

namespace KingUploader.Core.Application.Interfaces.Facades
{
    public interface IFilesFacade
    {
        public PostFileService PostFileService { get; }
        public GetCheckResumeService GetCheckResumeService { get; }
        public GetFilenameService GetFilenameService { get; }
        public DeleteFileAndDatabaseRecordsService DeleteFileAndDatabaseRecordsService { get; }
    }
}
