using KingUploader.Core.Application.Interfaces.Context;
using KingUploader.Core.Application.Services.Common;
using KingUploader.Core.Application.Services.Files.Commands.PostFile;
using Microsoft.EntityFrameworkCore;

namespace KingUploader.Core.Application.Services.MultiFiles.Commands.PostMultiFiles
{
    public class ResultPostMultiFilesServiceDto
    {
        public byte Result { get; set; }
        public string Message { get; set; }
    }
    public class RequestPostMultiFilesServiceDto
    {
        public IFormFile File { get; set; }
        public string Filename { get; set; }
        public int FilePart { get; set; }
        public string Start { get; set; } // the Byte-Index of file // the first value is ZERO
        public int FilePartCount { get; set; } // All parts of the file that are going to be separated by the basis of "each chunk of file = (1024*100)" will be stored.
    }
    public interface IPostMultiFilesService
    {
        ResultPostMultiFilesServiceDto Execute(RequestPostMultiFilesServiceDto req);
    }
    public class PostMultiFilesService : IPostMultiFilesService
    {
        private readonly IDataBaseContext _context;
        public PostMultiFilesService(IDataBaseContext context)
        {
            _context = context;
        }
        public ResultPostMultiFilesServiceDto Execute(RequestPostMultiFilesServiceDto req)
        {
            ///////////////////////////////////////////////////////////// check size and extention
            var resultCheckFileSizeExtension = CheckSizeExtension(req.FilePartCount, req.Filename);
            if (!resultCheckFileSizeExtension.Success) { return new ResultPostMultiFilesServiceDto { Message = resultCheckFileSizeExtension.Message, Result = 0 }; };
            /////////////////////////////////////////////////////////////
            int filepartcountfromdatabase = Upload(req.File, req.Filename);

            if (filepartcountfromdatabase > 0)
            {
                var file = _context.MultiFiles.Where(x => x.Filename == req.Filename).FirstOrDefault();
                if (file != null) // update
                {

                    file.FilePart = filepartcountfromdatabase;
                    file.Start = req.Start;
                    file.UploadDate = DateTime.Now;
                    if (file.FilePart == req.FilePartCount) file.Done = true;

                    if (_context.SaveChanges() > 0)
                    {
                        if (file.FilePart == req.FilePartCount)
                        {
                            return new ResultPostMultiFilesServiceDto
                            {
                                Result = 2 // {2}=Upload Finished Successfully
                            };
                        }
                        else
                        {
                            return new ResultPostMultiFilesServiceDto
                            {
                                Result = 1 // {1}=Upload is going on.
                            };
                        }
                    }
                    else
                    {
                        return new ResultPostMultiFilesServiceDto
                        {
                            Result = 0 // {0}=Upload failed!
                        };
                    }
                }
                else
                {
                    ///////////////////////////////////////////////////////////// check file signature
                    //// check only the first chunk of the file is enough!
                    //// To ensure the identity of a file during upload and prevent fake extension files from being uploaded in a C# program, you can implement file signature validation. File signatures, also known as magic numbers, are unique identifiers at the beginning of files that indicate their file type. Here's how you can do it:
                    //// Use C# to read the file's first few bytes to extract its signature. Different file types have different signatures. For example, a PDF file typically starts with the characters %PDF, while an MP4 file starts with ftyp.
                    //// When dealing with large files uploaded in chunks, it's generally sufficient to validate the file signature in the first chunk. This is because most file formats have their signature at the beginning of the file, often within the first few bytes.
                    var resultCheckFileSignature = KingUploader.Core.Application.Services.FileSignatureValidator.FileSignatureValidator.ValidateFileSignature(req.File);
                    if (!resultCheckFileSignature.Success)
                        return new ResultPostMultiFilesServiceDto { Result = 0, Message = "File signature is invalid" };
                    ///////////////////////////////////////////////////////////// 
                    // insert
                    Core.Domain.File.MultiFiles newFile = new Core.Domain.File.MultiFiles();

                    newFile.FilePart = filepartcountfromdatabase;
                    newFile.Filename = req.Filename;
                    newFile.Start = req.Start;
                    newFile.FilePartCount = req.FilePartCount;
                    if (req.FilePartCount == 1) newFile.Done = true; // if the total part of a file (FilePartCount) is only one part

                    _context.MultiFiles.Add(newFile);
                    if (_context.SaveChanges() > 0)
                    {
                        if (req.FilePartCount == 1)
                        {
                            return new ResultPostMultiFilesServiceDto
                            {
                                Result = 2 // {2}=Upload Finished Successfully
                            };
                        }
                        else
                        {
                            return new ResultPostMultiFilesServiceDto
                            {
                                Result = 1 // {1}=Upload is going on.
                            };
                        }
                    }
                    else
                    {
                        return new ResultPostMultiFilesServiceDto
                        {
                            Result = 0 // {0}=Upload failed!
                        };
                    }
                }
            }
            else
            {
                return new ResultPostMultiFilesServiceDto
                {
                    Result = 0// {0}=Upload failed!
                };
            }
        }
        private int Upload(IFormFile file, string orginalFilename)
        {
            try
            {
                // create folder
                string folder = $@"wwwroot\multifiles\";
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
            var FilePart = _context.MultiFiles
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
        private ResultDto CheckSizeExtension(int FilePartCount, string filename)
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
                if (Path.GetExtension(filename).Replace(".", "").ToLower() != "jpg".ToLower())
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
