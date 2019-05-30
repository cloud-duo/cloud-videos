using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using cloud_videos.Helpers;
using cloud_videos.Models;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
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
                    Endpoint = "https://francecentral.api.cognitive.microsoft.com/"
                };

            var imageStream = await GetImage(filename);

            var image = Image.FromStream(imageStream);

            var analysis = await computerVision.AnalyzeImageInStreamAsync(image.ToStream(ImageFormat.Jpeg), Features);

            return analysis;
        }

        public async Task<Dictionary<int, string>> RunG(DescRequest request)
        {
            var computerVision =
                new ComputerVisionClient(new ApiKeyServiceClientCredentials(ConfigHelper.GetSubscriptionKey()))
                {
                    Endpoint = "https://francecentral.api.cognitive.microsoft.com/"
                };
            var analysisList = new Dictionary<int, string>();

            var storageClient =
                await StorageClient.CreateAsync(
                    GoogleCredential.FromFile(HttpContext.Current.Server.MapPath("~\\keys.json")));

            for (var i = 0; i < request.Count; i++)
            {
                var imageStream = new MemoryStream();

                await storageClient.DownloadObjectAsync("galeata_magica_123", $"{request.Filename}/{i}.jpg",
                    imageStream);

                var image = Image.FromStream(imageStream);

                var analysis =
                    await computerVision.AnalyzeImageInStreamAsync(image.ToStream(ImageFormat.Jpeg), Features);

                var text = analysis.Description.Captions.FirstOrDefault()?.Text ?? string.Empty;

                analysisList.Add(i, text);
            }
            return analysisList;
        }
    }
}