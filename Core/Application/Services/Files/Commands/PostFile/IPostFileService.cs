namespace KingUploader.Core.Application.Services.Files.Commands.PostFile
{
    public interface IPostFileService
    {
        public ResultPostFileServiceDto Execute(RequestPostFileServiceDto req);
    }
}
