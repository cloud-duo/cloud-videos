namespace cloud_videos.Models
{
    public class HistoryEntry
    {
        public string VideoUrl { get; set; }
        public string AudioUrl { get; set; }

        public HistoryEntry()
        {
        }

        public HistoryEntry(string videoUrl, string audioUrl)
        {
            VideoUrl = videoUrl;
            AudioUrl = audioUrl;
        }
    }
}