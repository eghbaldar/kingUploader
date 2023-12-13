using KingUploader.Core.Application.Interfaces.Context;

namespace KingUploader.Core.Application.Services.Files.Queries.GetCheckResume
{
    public class GetCheckResumeService: IGetCheckResumeService
    {
        private readonly IDataBaseContext _context;
        public GetCheckResumeService(IDataBaseContext context)
        {
            _context = context;
        }
        public GetCheckResumeServiceDto Execute()
        {
            var file = _context.Files.Where(x => x.Done == false).FirstOrDefault();
            if (file != null)
            {
                return new GetCheckResumeServiceDto
                {
                    Id = file.Id,
                    Filename = file.Filename,
                    FilePart = file.FilePart,
                    Start = file.Start,
                    UploadDatetime = file.UploadDatetime,
                    Resume = true,
                };
            }
            else
                return new GetCheckResumeServiceDto
                {
                    Resume = false,
                };

        }
    }
}
