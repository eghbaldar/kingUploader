using KingUploader.Core.Application.Interfaces.Context;

namespace KingUploader.Core.Application.Services.Files.Commands.DeleteFileAndDatabaseRecords
{
    public class DeleteFileAndDatabaseRecordsService : IDeleteFileAndDatabaseRecordsService
    {
        private readonly IDataBaseContext _context;
        public DeleteFileAndDatabaseRecordsService(IDataBaseContext context)
        {
            _context = context;
        }
        public bool Execute()
        {
            _context.Files.RemoveRange(_context.Files.Select(x => x));
            _context.SaveChanges();
            // delete files physically
            string projectPath = Directory.GetCurrentDirectory();
            string directoryPath = "/files/"; // Replace with your desired directory path

            // Create a DirectoryInfo object for the specified directory
            DirectoryInfo directoryInfo = new DirectoryInfo(projectPath + "/wwwroot/" + directoryPath + "/");

            // Delete all files within the directory
            foreach (FileInfo file in directoryInfo.EnumerateFiles())
            {
                file.Delete();
            }

            // Optionally, delete all subdirectories as well (including their files)
            foreach (DirectoryInfo subDirectory in directoryInfo.EnumerateDirectories())
            {
                subDirectory.Delete(recursive: true);
            }
            return true;
        }
    }
}