using KingUploader.Core.Application.Interfaces.Facades;
using KingUploader.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Text;

namespace KingUploader.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFilesFacade _filesFacade;
        public HomeController(IFilesFacade filesFacade)
        {
            _filesFacade= filesFacade;
        }
        /// <summary>
        /// ////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Upload(long ChunkSize, string Filename,string Start)
        {
            IFormFile formFile = Request.Form.Files[0];

            ///////////////////////////////////////
            string folder = $@"wwwroot\files\";
            var uploadRootFolder = Path.Combine(Environment.CurrentDirectory, folder);
            if (!Directory.Exists(uploadRootFolder))
            {
                Directory.CreateDirectory(uploadRootFolder);
            }

            // calculate the number of files that will be created
            int TotalFileParts = 0;
            if (formFile.Length < ChunkSize)
            {
                TotalFileParts = 1;
            }
            else
            {
                float PreciseFileParts = ((float)formFile.Length / (float)ChunkSize);
                TotalFileParts = (int)Math.Ceiling(PreciseFileParts);
            }
            //

            int filepartcountfromdatabase = _filesFacade.GetLastFilePartService.Execute(new Core.Application.Services.Files.Queries.GetLastFilePart.RequestGetLastFilePartDto
            {
                Filename = Filename,
            }) ;

            string FilePartName = String.Format("{0}.{1}.part{2}",
            Filename, TotalFileParts.ToString(), filepartcountfromdatabase);
            //FilePartName = Path.Combine(folder, FilePartName);

            string filename = FilePartName;//Guid.NewGuid() + formFile.FileName;
            string filePath = Path.Combine(uploadRootFolder, filename);

            using var fileStream = new FileStream(filePath, FileMode.CreateNew);
            {
                formFile.CopyTo(fileStream);
            }
            _filesFacade.PostFileService.Execute(new Core.Application.Services.Files.Commands.PostFile.RequestPostFileServiceDto
            {
                Filename = Filename,
                FilePart= filepartcountfromdatabase,
                Start= Start,
            });
            /////////////////////////////////////////
            System.Threading.Thread.Sleep(100);
            ////////////////////////////////////////

            return Json(new resultDto
            {
                success = true,
            });
        }

        [HttpPost]
        public IActionResult CheckResume(string filename)
        {
            return Json(_filesFacade.GetCheckResumeService.Execute());
        }

        public class resultDto
        {
            public bool success { get; set; }
        }

        [HttpPost]
        public bool Merge()
        {
            string filenameMain = "test.jpg";
            string folder = $@"wwwroot\files\";
            var uploadRootFolder = Path.Combine(Environment.CurrentDirectory, folder);
            bool Output = false;
            try
            {
                string[] tmpfiles = Directory.GetFiles(uploadRootFolder, "*.part*");

                var sortedTmpfiles = tmpfiles
                    .Select(x =>
                    new
                    {
                        key = int.Parse(x.Replace("part", "").Substring(x.Replace("part", "").LastIndexOf("."), x.Replace("part", "").Length - (x.Replace("part", "").LastIndexOf("."))).Replace(".", ""))
                        ,
                        value = x
                    })                   
                    .OrderBy(p => p.key)
                    .ToList();

                var files = Directory.EnumerateFiles(folder).OrderByDescending(filename => filename);


                FileStream outPutFile = null;
                string PrevFileName = "";
                foreach (var tempFile in sortedTmpfiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(tempFile.value);
                    string baseFileName = fileName.Substring(0, fileName.IndexOf(Convert.ToChar(".")));
                    string extension = Path.GetExtension(filenameMain);
                    if (!PrevFileName.Equals(baseFileName))
                    {
                        if (outPutFile != null)
                        {
                            outPutFile.Flush();
                            outPutFile.Close();
                        }
                        outPutFile = new FileStream(uploadRootFolder + "\\" + baseFileName + extension, FileMode.OpenOrCreate, FileAccess.Write);
                    }
                    int bytesRead = 0;
                    byte[] buffer = new byte[1024];
                    FileStream inputTempFile = new FileStream(tempFile.value, FileMode.OpenOrCreate, FileAccess.Read);
                    while ((bytesRead = inputTempFile.Read(buffer, 0, 1024)) > 0)
                        outPutFile.Write(buffer, 0, bytesRead);
                    inputTempFile.Close();
                    //File.Delete(tempFile);
                    PrevFileName = baseFileName;
                }
                outPutFile.Close();
            }
            catch
            {
                return false;
            }
            return Output;
        }

    }
}