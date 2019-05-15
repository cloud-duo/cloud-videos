using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using cloud_videos.Helpers;
using Microsoft.CognitiveServices.Speech;
using Microsoft.WindowsAzure.Storage;

namespace cloud_videos.Handlers
{
    public class TtsHandler
    {
        private const string ShareName = "sounds";

        public async Task<string> Run(string text)
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            // The default language is "en-us".

            var config = SpeechConfig.FromSubscription(ConfigHelper.GetTextToSpeechKey(), "northeurope");

            // Creates a speech synthesizer using the default speaker as audio output.
            using (var synthesizer = new SpeechSynthesizer(config))
            {

                using (var result = await synthesizer.SpeakTextAsync(text))
                {
                    var storageAccount = CloudStorageAccount.Parse(ConfigHelper.GetStorageConnectionString());

                    var fileClient = storageAccount.CreateCloudFileClient();

                    var share = fileClient.GetShareReference(ShareName);

                    await share.CreateIfNotExistsAsync();

                    var rootDirectory = share.GetRootDirectoryReference();

                    var fileName = Guid.NewGuid().ToString() + ".mp3";

                    var cloudFile = rootDirectory.GetFileReference(fileName);

                    await cloudFile.UploadFromByteArrayAsync(result.AudioData, 0, result.AudioData.Length);

                    return fileName;
                }
            }
        }
    }
}