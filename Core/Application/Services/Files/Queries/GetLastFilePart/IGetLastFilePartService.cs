namespace KingUploader.Core.Application.Services.Files.Queries.GetLastFilePart
{
    public interface IGetLastFilePartService
    {
        public int Execute(RequestGetLastFilePartDto req);
    }
}
