using KingUploader.Core.Application.Services.Common;

namespace KingUploader.Core.Application.Services.FileSignatureValidator
{
    public class FileSignatureValidator
    {
        public static ResultDto ValidateFileSignature(IFormFile file)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    // Copy the contents of the IFormFile to the MemoryStream
                    file.CopyTo(ms);

                    // Get the bytes from the MemoryStream
                    byte[] fileBytes = ms.ToArray();

                    // Check the file signature
                    if (IsPDF(fileBytes) || IsMP4(fileBytes) || IsZIP(fileBytes) || IsRAR(fileBytes))
                    {
                        return new ResultDto
                        {
                            Message = "File signature is valid.",
                            Success = true,
                        };
                    }
                    else
                    {
                        return new ResultDto
                        {
                            Message = "Invalid file signature.",
                            Success = false,
                        }; ;
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultDto
                {
                    Message ="Error validating file signature: " + ex.Message,
                    Success = false,
                };
            }
        }

        private static bool IsPDF(byte[] header)
        {
            // Check if the file signature matches a PDF file
            return header[0] == 0x25 && header[1] == 0x50 && header[2] == 0x44 && header[3] == 0x46;
        }

        private static bool IsMP4(byte[] header)
        {
            // Check if the file signature matches an MP4 file
            return header[0] == 0x66 && header[1] == 0x74 && header[2] == 0x79 && header[3] == 0x70;
        }
        private static bool IsZIP(byte[] header)
        {
            // Check if the file signature matches a ZIP file
            // ZIP file signature: PKZIP file format signature (0x50 0x4B 0x03 0x04)
            return header[0] == 0x50 && header[1] == 0x4B && header[2] == 0x03 && header[3] == 0x04;
        }

        private static bool IsRAR(byte[] header)
        {
            // Check if the file signature matches a RAR file
            // RAR file signature: RAR file format signature (0x52 0x61 0x72 0x21)
            return header[0] == 0x52 && header[1] == 0x61 && header[2] == 0x72 && header[3] == 0x21;
        }
    }
}
