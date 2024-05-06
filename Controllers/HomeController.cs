using KingUploader.Core.Application.Interfaces.Facades;
using KingUploader.Core.Application.Services.Files.Commands.PostFile;
using KingUploader.Hubs;
using KingUploader.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System.Text;

namespace KingUploader.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFilesFacade _filesFacade;
        private readonly IHubContext<BroadCastHub> _hubContext;

        public HomeController(IFilesFacade filesFacade, IHubContext<BroadCastHub> hubContext)
        {
            _filesFacade = filesFacade;
            _hubContext = hubContext;
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
            System.Threading.Thread.Sleep(100);
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
        public async Task<JsonResult> UploadFile()
        {
            try
            {

                var counter = 0;
                var currentCount = 0;

                var data = new List<RecordItem>();
                var postedFile = Request.Form.Files;

                await Task.Delay(500);
                currentCount++;
                SendFeedBack(currentCount, counter);

                if (postedFile.Count <= 0 || postedFile == null)
                    return Json(new { error = true, message = "Empty File was uploaded" });

                if (postedFile[0] == null || postedFile[0].Length <= 0)
                {
                    return Json(new { error = true, message = "Empty File was uploaded" });
                }

                await Task.Delay(500);
                currentCount++;
                SendFeedBack(currentCount, counter);


                var fileInfo = new FileInfo(postedFile[0].FileName);
                var extention = fileInfo.Extension;
                if (extention.ToLower() != ".csv")
                {
                    return Json(new { error = true, message = "invalid file format" });
                }

                using (StreamReader sr = new StreamReader(postedFile[0].OpenReadStream()))
                {
                    await Task.Delay(500);
                    currentCount++;
                    SendFeedBack(currentCount, counter);

                    while (!sr.EndOfStream)
                    {
                        String Info = sr.ReadLine();
                        String[] Records;
                        if (Info.Contains('\"'))
                        {
                            var row = string.Empty;
                            var model = Info.Replace("\"", "#*").Split('#');
                            foreach (var item in model)
                            {
                                var d = item.Replace("*,", ",");
                                if (d.Contains("*"))
                                {
                                    row += d.Replace("*", "").Replace(",", "");
                                }
                                else
                                {
                                    row += d;
                                }
                            }
                            Records = new String[row.Split(new char[] { ',' }).Length];
                            row.Split(new char[] { ',' }).CopyTo(Records, 0);

                        }
                        else
                        {
                            Records = new String[Info.Split(new char[] { ',' }).Length];
                            Info.Split(new char[] { ',' }).CopyTo(Records, 0);
                        }
                        var strAmount = Records[7].ToString().Trim();
                        decimal output;
                        if (string.IsNullOrEmpty(strAmount) || !decimal.TryParse(strAmount, out output)) continue;



                        var datafile = new RecordItem()
                        {
                            Company = Records[1].ToString().Trim(),
                            Category = Records[3].ToString().Trim(),
                            City = Records[4].ToString().Trim(),
                            Date = Records[6].ToString().Trim(),
                            Currency = Records[8].ToString().Trim(),
                            Amount = decimal.Parse(Records[7].ToString().Trim()),

                        };

                        data.Add(datafile);

                        counter++;
                        SendFeedBack(currentCount, counter);
                    }

                    sr.Close();
                    sr.Dispose();

                    await Task.Delay(500);
                    currentCount++;
                    SendFeedBack(currentCount, counter);

                }
                await Task.Delay(500);
                currentCount++;
                SendFeedBack(currentCount, counter);

                return Json(new { error = false, data = data });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    error = true,
                    message = ex.InnerException != null ?
                    ex.InnerException.Message : ex.Message
                });
            }

        }

        private async void SendFeedBack(int currentCount, int UploadCount)
        {
            var totalCount = 4;
            var feedBackModel = new FeedbackModel()
            {
                currentCount = currentCount,
                currentPercent = (currentCount * 100 / totalCount).ToString(),
                UploadCount = UploadCount,
            };
            await _hubContext.Clients.All.SendAsync("feedBack", feedBackModel);
        }
    }
}