using KingUploader.Core.Application.Interfaces.Context;

namespace KingUploader.Core.Application.Services.Files.Queries.GetLastFilePart
{
    public class GetLastFilePartService : IGetLastFilePartService
    {
        private readonly IDataBaseContext _context;
        public GetLastFilePartService(IDataBaseContext context)
        {
            _context = context;
        }
        public int Execute(RequestGetLastFilePartDto req)
        {
            var FilePart = _context.Files
             .Where(x => x.Filename == req.Filename && x.Done == false) // false => the file uploading process is not completed!
             .OrderByDescending(x => x.FilePart)
             .Select(x => x.FilePart)
             .FirstOrDefault();

            if (FilePart != 0) // !0 => the file uploading process is not completed!
            {
                return FilePart + 1;
            }
            else
                return 1; // 0 => first record
        }
    }
}
