namespace KingUploader.Core.Application.Services.Files.Commands.PostFile
{
    public interface IPostFileService
    {
        public bool Execute(RequestPostFileServiceDto req);
    }
}
