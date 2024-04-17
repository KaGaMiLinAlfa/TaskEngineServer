using System;

namespace Worker2.ApiModel.Heartbeat
{
    public class SiteLastHeartbeat
    {
        public string Group { get; set; }

        public string SiteName { get; set; }

        public DateTime LastTime { get; set; }
    }
}
