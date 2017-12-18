namespace DesktopProjectDebug
{
    public class SnapshotDebugConfig
    {
        public SnapshotDebugConfig(string subscription = "", string resourceId = "", string websiteName = "")
        {
            Subscription = subscription;
            ResourceId = resourceId;
            WebsiteName = websiteName;
        }

        public string Subscription { get; set; }
        
        public string ResourceId { get; set; }

        public string WebsiteName { get; set; }
    }
}
