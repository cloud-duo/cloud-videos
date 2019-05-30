using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Web;
using cloud_videos.Helpers;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.CognitiveServices.Speech;
using Microsoft.WindowsAzure.Storage;
using NAudio.Wave;

namespace cloud_videos.Handlers
{
    public class TtsHandler
    {

        public async Task<string> Run(IEnumerable<string> text)
        {
            // Creates an instance of a speech config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            // The default language is "en-us".

            var config = SpeechConfig.FromSubscription(ConfigHelper.GetTextToSpeechKey(), "francecentral");
            config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Audio16Khz64KBitRateMonoMp3);


            // Creates a speech synthesizer using the default speaker as audio output.
            using (var synthesizer = new SpeechSynthesizer(config))
            {
                var storageClient =
                    await StorageClient.CreateAsync(
                        GoogleCredential.FromFile(HttpContext.Current.Server.MapPath("~\\keys.json")));

                using (var result = await synthesizer.SpeakTextAsync(text.Aggregate((i, j) => i + "\n" + j)))
                {
                    var fileName = Guid.NewGuid() + ".mp3";

                    await storageClient.UploadObjectAsync("galeata_magica_123", fileName, null,
                        new MemoryStream(result.AudioData));


                    return fileName;
                }
            }
        }
    }
}