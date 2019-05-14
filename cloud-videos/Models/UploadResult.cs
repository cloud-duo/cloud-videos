namespace cloud_videos.Models
{
    public class UploadResult
    {
        public UploadResult(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; set; }
    }
}