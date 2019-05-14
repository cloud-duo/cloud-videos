using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace cloud_videos.Helpers
{
    public static class ConfigHelper
    {
        public static string GetStorageConnectionString()
        {
            using (var r = new StreamReader(System.Web.HttpContext.Current.Request.MapPath("config.json")))
            {
                var json = r.ReadToEnd();
                var items = JsonConvert.DeserializeObject<dynamic>(json);
                return items.key;
            }
        }
    }
}