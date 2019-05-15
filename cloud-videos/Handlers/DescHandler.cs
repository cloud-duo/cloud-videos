using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using cloud_videos.Helpers;
using cloud_videos.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.WindowsAzure.Storage;

namespace cloud_videos.Handlers
{
    public class DescHandler
    {
        private const string ShareName = "images";

        private static readonly List<VisualFeatureTypes> Features =
            new List<VisualFeatureTypes>
            {
                VisualFeatureTypes.Categories,
                VisualFeatureTypes.Description,
                VisualFeatureTypes.Tags
            };

        private static async Task<MemoryStream> GetImage(string filename)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigHelper.GetStorageConnectionString());

            var fileClient = storageAccount.CreateCloudFileClient();

            var share = fileClient.GetShareReference(ShareName);

            await share.CreateIfNotExistsAsync();

            var rootDirectory = share.GetRootDirectoryReference();

            var cloudFile = rootDirectory.GetFileReference(filename);

            var stream = new MemoryStream();

            await cloudFile.DownloadToStreamAsync(stream);

            return stream;
        }

        public async Task<ImageAnalysis> Run(string filename)
        {
            var computerVision =
                new ComputerVisionClient(new ApiKeyServiceClientCredentials(ConfigHelper.GetSubscriptionKey()))
                {
                    Endpoint = "https://northeurope.api.cognitive.microsoft.com/"
                };

            var imageStream = await GetImage(filename);

            var image = Image.FromStream(imageStream);

            var analysis = await computerVision.AnalyzeImageInStreamAsync(image.ToStream(ImageFormat.Jpeg), Features);

            return analysis;
        }
    }
}