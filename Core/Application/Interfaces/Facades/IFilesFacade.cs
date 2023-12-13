using KingUploader.Core.Application.Services.Files.Commands.PostFile;
using KingUploader.Core.Application.Services.Files.Queries.GetCheckResume;
using KingUploader.Core.Application.Services.Files.Queries.GetLastFilePart;

namespace KingUploader.Core.Application.Interfaces.Facades
{
    public interface IFilesFacade
    {
        public PostFileService PostFileService { get; }
        public GetLastFilePartService GetLastFilePartService { get; }
        public GetCheckResumeService GetCheckResumeService { get; }
    }
}
