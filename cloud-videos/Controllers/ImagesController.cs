using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using cloud_videos.Handlers;
using cloud_videos.Helpers;
using cloud_videos.Models;
using Microsoft.WindowsAzure.Storage;

namespace cloud_videos.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ImagesController : ApiController
    {
        private const string ShareName = "images";

        [HttpPost]
        [Route("api/images")]
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

                var cloudFile = rootDirectory.GetFileReference(cloudFilename + ".jpg");

                await cloudFile.UploadFromByteArrayAsync(buffer, 0, buffer.Length);
            }

            return Ok(new UploadResult(cloudFilename));
        }

        [HttpGet]
        [Route("api/images/desc/{filename}")]
        public async Task<IHttpActionResult> Description(string filename)
        {
            return Ok(await new DescHandler().Run(filename + ".jpg"));
        }

        [HttpPost]
        [Route("api/images/descg")]
        public async Task<IHttpActionResult> DescriptionG(DescRequest request)
        {
            return Ok(await new DescHandler().RunG(request));
        }

        [HttpPost]
        [Route("api/images/tts")]
        public async Task<IHttpActionResult> TextToSpeech(TtsRequest ttsRequest)
        {
            return Ok(await new TtsHandler().Run(ttsRequest.Text));
        }
    }
}