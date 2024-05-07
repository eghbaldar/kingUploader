using KingUploader.Core.Application.Interfaces.Facades;
using KingUploader.Core.Application.Services.Files.Commands.PostFile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Text;

namespace KingUploader.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFilesFacade _filesFacade;

        public HomeController(IFilesFacade filesFacade)
        {
            _filesFacade = filesFacade;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        ///////////////////////////////////////////////////// Large Module
        [HttpGet]
        public IActionResult Large()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Upload(string Filename, string Start, int FilePartCount)
        {
            IFormFile formFile = Request.Form.Files[0];

            var result = _filesFacade.PostFileService
                .Execute(new Core.Application.Services.Files.Commands.PostFile.RequestPostFileServiceDto
                {
                    Filename = Filename,
                    //FilePart = filepartcountfromdatabase,
                    Start = Start,
                    FilePartCount = FilePartCount,
                    File = formFile,
                });
            /////////////////////////////////////////
            System.Threading.Thread.Sleep(3000);
            ////////////////////////////////////////

            return Json(result);
        }

        [HttpPost]
        public IActionResult CheckResume(string filename)
        {
            return Json(_filesFacade.GetCheckResumeService.Execute());
        }

        public class resultDto
        {
            public bool success { get; set; }
            public string message { get; set; }
        }

        [HttpPost]
        public bool Merge()
        {
            string filenameMain = _filesFacade.GetFilenameService.Execute();
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

        [HttpPost]
        public IActionResult Delete()
        {
            return Json(_filesFacade.DeleteFileAndDatabaseRecordsService.Execute());
        }

        ///////////////////////////////////////////////////// Small Module
        [HttpGet]
        public IActionResult Small()
        {
            return View();
        }
    }
}