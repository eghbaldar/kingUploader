using KingUploader.Core.Application.Services.Files.Commands.PostFile;

namespace KingUploader.Core.Application.Interfaces.Facades
{
    public interface IFilesFacade
    {
        public PostFileService PostFileService { get; }
    }
}
