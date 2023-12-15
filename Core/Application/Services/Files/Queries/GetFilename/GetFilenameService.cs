using KingUploader.Core.Application.Interfaces.Context;

namespace KingUploader.Core.Application.Services.Files.Queries.GetFilename
{
    public class GetFilenameService: IGetFilenameService
    {
        private readonly IDataBaseContext _context;
        public GetFilenameService(IDataBaseContext context)
        {
            _context = context;
        }
        public string Execute()
        {
            return _context.Files.First().Filename;
        }
    }
}
