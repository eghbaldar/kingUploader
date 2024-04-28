using KingUploader.Core.Application.Interfaces.Context;
using KingUploader.Core.Application.Services.Common;
using Microsoft.AspNetCore.Http;

namespace KingUploader.Core.Application.Services.Files.Commands.PostFile
{
    public class PostFileService : IPostFileService
    {
        private readonly IDataBaseContext _context;
        public PostFileService(IDataBaseContext context)
        {
            _context = context;

        }
        public ResultPostFileServiceDto Execute(RequestPostFileServiceDto req)
        {
            var resultCheckFileSizeExtension = CheckSizeExtension(req.FilePartCount,req.Filename);
            if(!resultCheckFileSizeExtension.Success) { return new ResultPostFileServiceDto { Message = resultCheckFileSizeExtension.Message, Result = 0 }; };
            /////////////////////////////////////////////////////////////
            int filepartcountfromdatabase = Upload(req.File,req.Filename);

            if (filepartcountfromdatabase > 0)
            {
                var file = _context.Files.Where(x => x.Filename == req.Filename).FirstOrDefault();
                if (file != null) // update
                {

                    file.FilePart = filepartcountfromdatabase;
                    file.Start = req.Start;
                    file.UploadDatetime = DateTime.Now;
                    if (file.FilePart == req.FilePartCount) file.Done = true;

                    if (_context.SaveChanges() > 0)
                    {
                        if (file.FilePart == req.FilePartCount)
                        {
                            return new ResultPostFileServiceDto
                            {
                                Result = 2 // {2}=Upload Finished Successfully
                            };
                        }
                        else
                        {
                            return new ResultPostFileServiceDto
                            {
                                Result = 1 // {1}=Upload is going on.
                            };
                        }
                    }
                    else
                    {
                        return new ResultPostFileServiceDto
                        {
                            Result = 0 // {0}=Upload failed!
                        };
                    }
                }
                else
                {  // insert
                    Core.Domain.File.Files newFile = new Core.Domain.File.Files();

                    newFile.FilePart = filepartcountfromdatabase;
                    newFile.Filename = req.Filename;
                    newFile.Start = req.Start;
                    newFile.FilePartCount = req.FilePartCount;
                    if (req.FilePartCount == 1) newFile.Done = true; // if the total part fo a file (FilePartCount) is only one part

                    _context.Files.Add(newFile);
                    if (_context.SaveChanges() > 0)
                    {
                        if (req.FilePartCount == 1)
                        {
                            return new ResultPostFileServiceDto
                            {
                                Result = 2 // {2}=Upload Finished Successfully
                            };
                        }
                        else
                        {
                            return new ResultPostFileServiceDto
                            {
                                Result = 1 // {1}=Upload is going on.
                            };
                        }
                    }
                    else
                    {
                        return new ResultPostFileServiceDto
                        {
                            Result = 0 // {0}=Upload failed!
                        };
                    }
                }
            }
            else
            {
                return new ResultPostFileServiceDto
                {
                    Result = 0// {0}=Upload failed!
                };
            }
        }
        private int Upload(IFormFile file,string orginalFilename)
        {
            try
            {
                // create folder
                string folder = $@"wwwroot\files\";
                var uploadRootFolder = Path.Combine(Environment.CurrentDirectory, folder);
                if (!Directory.Exists(uploadRootFolder)) Directory.CreateDirectory(uploadRootFolder);
                // end

                int filepartcountfromdatabase = GetLastFilePart(orginalFilename);

                string filename = String.Format("{0}.part{1}", orginalFilename, filepartcountfromdatabase);
                string filePath = Path.Combine(uploadRootFolder, filename);

                // if file exists, first of all delete and then upload it again
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                else
                {
                    using var fileStream = new FileStream(filePath, FileMode.CreateNew);
                    {
                        file.CopyTo(fileStream);
                    }
                }
                return filepartcountfromdatabase;
            }
            catch (Exception ex)
            {
                return 0;
            }

        }
        private int GetLastFilePart(string filename)
        {
            var FilePart = _context.Files
            .Where(x => x.Filename == filename && x.Done == false) // false => the file uploading process is not completed!
            .OrderByDescending(x => x.FilePart)
            .Select(x => x.FilePart)
            .FirstOrDefault();

            if (FilePart != 0) // FilePart:!0 => the file uploading process is not completed!
            {
                return FilePart + 1;
            }
            else
                return 1; // FilePart:0 => first record
        }
        private ResultDto CheckSizeExtension(int FilePartCount,string filename)
        {
            // check file size & extension
            if ((FilePartCount * Constants.Chunck * 1024) > 5000000000)
            // why 100? because based on our principle, we are going to separate each chunk to 100 kb
            // why 1024? becaue we need the byte unit to compare the two values
            {
                return new ResultDto
                {
                    Success = false,
                    Message = "(server side) => Check your file size! (must less than 50-MB)",
                };
            }
            else
            {
                if (Path.GetExtension(filename).Replace(".", "").ToLower() != "zip".ToLower())
                {
                    return new ResultDto
                    {
                        Success = false,
                        Message = "(server side) => Check your file extension!",
                    };
                }
            }
            return new ResultDto
            {
                Success = true,
            };
        }
    }
}
