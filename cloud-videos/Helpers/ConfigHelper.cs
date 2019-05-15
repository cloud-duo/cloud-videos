using System.Collections.Generic;
using System.IO;
using System.Web;
using Newtonsoft.Json;

namespace cloud_videos.Helpers
{
    public static class ConfigHelper
    {
        public static string GetStorageConnectionString()
        {
            using (var r = new StreamReader(HttpContext.Current.Server.MapPath("~\\config.json")))
            {
                var json = r.ReadToEnd();
                var items = JsonConvert.DeserializeObject<dynamic>(json);
                return items.key;
            }
        }

        public static string GetSubscriptionKey()
        {
            using (var r = new StreamReader(HttpContext.Current.Server.MapPath("~\\config.json")))
            {
                var json = r.ReadToEnd();
                var items = JsonConvert.DeserializeObject<dynamic>(json);
                return items.subkey;
            }
        }

        public static string GetTextToSpeechKey()
        {
            using (var r = new StreamReader(HttpContext.Current.Server.MapPath("~\\config.json")))
            {
                var json = r.ReadToEnd();
                var items = JsonConvert.DeserializeObject<dynamic>(json);
                return items.texttospeechkey;
            }
        }
    }
}