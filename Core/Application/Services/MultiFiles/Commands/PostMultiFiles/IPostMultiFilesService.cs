using KingUploader.Core.Application.Services.Files.Commands.PostFile;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace KingUploader.Core.Application.Services.MultiFiles.Commands.PostMultiFiles
{
    public interface IPostMultiFilesService
    {
        Task<ResultPostMultiFilesServiceDto> Execute(RequestPostMultiFilesServiceDto req);
    }

}
