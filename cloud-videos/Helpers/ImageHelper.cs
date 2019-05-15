using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace cloud_videos.Helpers
{
    public static class ImageHelper
    {
        public static Stream ToStream(this Image image, ImageFormat format)
        {
            var stream = new MemoryStream();
            image.Save(stream, format);
            stream.Position = 0;
            return stream;
        }
    }
}