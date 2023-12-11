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
             .Where(x => x.Filename == req.Filename)
             .OrderByDescending(x => x.FilePart)
             .Select(x => x.FilePart)
             .FirstOrDefault();

            if (FilePart != 0)
                return FilePart + 1;
            else
                return 1; //first record

            //var files = _context.Files
            //    .Where(x => x.Filename == req.Filename)
            //    .OrderByDescending(x => x.FilePart)
            //    .Select(x => x.FilePart)
            //    .First();

            //if (files != null)
            //    return files;
            //else
            //    return 1; //first record
        }
    }
}
