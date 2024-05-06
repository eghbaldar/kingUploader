using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace KingUploader.Hubs
{
    public class FileUploadHub : Hub
    {
        private readonly Dictionary<string, int> uploadedBytesDictionary = new Dictionary<string, int>();

        public async Task UploadFileChunk(byte[] chunk, string fileId)
        {
            // Process the received file chunk
            // Update the progress

            // Check if the file ID exists in the dictionary
            if (uploadedBytesDictionary.ContainsKey(fileId))
            {
                // Increment the uploaded bytes by the size of the current chunk
                uploadedBytesDictionary[fileId] += chunk.Length;
            }
            else
            {
                // Add a new entry for the file ID and set its initial value to the size of the current chunk
                uploadedBytesDictionary[fileId] = chunk.Length;
            }

            // Calculate the progress percentage
            int totalFileSize = GetTotalFileSize(fileId); // Implement your logic to get the total file size
            int uploadedBytes = uploadedBytesDictionary[fileId];
            int progressPercentage = (int)((double)uploadedBytes / totalFileSize * 100);

            // Send the progress update to connected clients
            await Clients.All.SendAsync("UploadFileChunk", fileId, progressPercentage);
        }

        private int GetTotalFileSize(string fileId)
        {
            // Implement your logic to get the total file size for the specified file ID
            // For example, you might have a database or storage system that tracks file sizes
            // Return the total file size in bytes
            return 0;
        }

        // This method can be used by clients to get the current progress for a specific file
        public async Task<int> GetProgress(string fileId)
        {
            int totalFileSize = GetTotalFileSize(fileId); // Implement your logic to get the total file size
            int uploadedBytes = uploadedBytesDictionary.ContainsKey(fileId) ? uploadedBytesDictionary[fileId] : 0;
            int progressPercentage = (int)((double)uploadedBytes / totalFileSize * 100);

            return progressPercentage;
        }
    }
}
