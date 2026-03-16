using Microsoft.VisualBasic.FileIO;

namespace LogisticsPro_API
{
    public class FileUploadModel
    {
        public IFormFile FileDetails { get; set; }
        public FileType FileType { get; set; }
    }

    public enum FileType
    {
        PDF = 1,
        DOCX = 2
    }
}
