using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using cloud_videos.Helpers;
using cloud_videos.Models;
using Microsoft.WindowsAzure.Storage;

namespace cloud_videos.Controllers
{
    public class VideosController : ApiController
    {
        private const string ShareName = "videos";

        [HttpPost]
        [Route("api/videos")]
        public async Task<IHttpActionResult> Upload()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            var cloudFilename = Guid.NewGuid().ToString();

            foreach (var file in provider.Contents)
            {
                var buffer = await file.ReadAsByteArrayAsync();

                var storageAccount = CloudStorageAccount.Parse(ConfigHelper.GetStorageConnectionString());

                var fileClient = storageAccount.CreateCloudFileClient();

                var share = fileClient.GetShareReference(ShareName);

                await share.CreateIfNotExistsAsync();

                var rootDirectory = share.GetRootDirectoryReference();

                var cloudFile = rootDirectory.GetFileReference(cloudFilename);

                await cloudFile.UploadFromByteArrayAsync(buffer, 0, buffer.Length);
            }

            return Ok(new UploadResult(cloudFilename));
        }
    }
}