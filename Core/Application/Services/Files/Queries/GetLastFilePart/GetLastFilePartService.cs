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
            {
                var FilePartResume = _context.Files.Where(x => x.Done == false).FirstOrDefault();
                if (FilePartResume != null) // upload paused!
                    return FilePart + 1;
                else
                    return FilePart + 1; // upload finished!
            }
            else
                return 1; //first record
        }
    }
}
