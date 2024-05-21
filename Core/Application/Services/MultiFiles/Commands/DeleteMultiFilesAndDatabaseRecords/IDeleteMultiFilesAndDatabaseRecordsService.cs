using KingUploader.Core.Application.Interfaces.Context;

namespace KingUploader.Core.Application.Services.MultiFiles.Commands.DeleteMultiFilesAndDatabaseRecords
{
    public interface IDeleteMultiFilesAndDatabaseRecordsService
    {
        bool Execute();
    }
    public class DeleteMultiFilesAndDatabaseRecordsService: IDeleteMultiFilesAndDatabaseRecordsService
    {
        private readonly IDataBaseContext _context;
        public DeleteMultiFilesAndDatabaseRecordsService(IDataBaseContext context)
        {
            _context = context;
        }
        public bool Execute()
        {
            _context.MultiFiles.RemoveRange(_context.MultiFiles.Select(x => x));
            _context.SaveChanges();
            // delete files physically
            string projectPath = Directory.GetCurrentDirectory();
            string directoryPath = "/multifiles/"; // Replace with your desired directory path

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
