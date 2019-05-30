using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cloud_videos.Models
{
    public class TtsRequest
    {
        public IEnumerable<string> Text { get; set; }
    }
}