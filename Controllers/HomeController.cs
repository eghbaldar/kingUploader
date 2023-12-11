using KingUploader.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Text;

namespace KingUploader.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Upload(long FilePartCount, long ChunkSize, string Filename)
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
            string FilePartName = String.Format("{0}.{2}.part{1}",
            Filename, FilePartCount.ToString(), TotalFileParts.ToString());
            //FilePartName = Path.Combine(folder, FilePartName);

            string filename = FilePartName;//Guid.NewGuid() + formFile.FileName;
            string filePath = Path.Combine(uploadRootFolder, filename);

            using var fileStream = new FileStream(filePath, FileMode.CreateNew);
            {
                formFile.CopyTo(fileStream);
            }
            /////////////////////////////////////////
            System.Threading.Thread.Sleep(100);
            return Json(new resultDto
            {
                success = true,
            });
        }

        public class resultDto
        {
            public bool success { get; set; }
        }

        [HttpPost]
        public bool Merge()
        {
            string filenameMain = "1.rar";
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