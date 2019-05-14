using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using cloud_videos.Helpers;
using cloud_videos.Models;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace cloud_videos.Handlers
{
    public class DescHandler
    {
        private const string ShareName = "images";

        private async Task<MemoryStream> GetImage(string filename)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigHelper.GetStorageConnectionString());

            var fileClient = storageAccount.CreateCloudFileClient();

            var share = fileClient.GetShareReference(ShareName);

            await share.CreateIfNotExistsAsync();

            var rootDirectory = share.GetRootDirectoryReference();

            var cloudFile = rootDirectory.GetFileReference(filename);

            var stream = new MemoryStream();

            await cloudFile.DownloadToStreamAsync(stream).ConfigureAwait(false);

            return stream;
        }

        public async Task<DescResult> Run(string filename)
        {
            return null;
        }
    }
}