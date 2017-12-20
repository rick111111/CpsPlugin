using System;
using System.Diagnostics;

namespace DesktopProjectDebug
{
    [Serializable]
    [DebuggerDisplay("SiteName = {WebsiteName}, ResourceGroup = {ResourceId}")]
    public sealed class SnapshotDebugConfig
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
        
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            SnapshotDebugConfig c = (SnapshotDebugConfig)obj;
            return Subscription == c.Subscription && ResourceId == c.ResourceId && WebsiteName == c.WebsiteName;
        }

        public override int GetHashCode()
        {
            return Subscription.GetHashCode() ^ ResourceId.GetHashCode() ^ WebsiteName.GetHashCode();
        }
        public static bool operator ==(SnapshotDebugConfig c1, SnapshotDebugConfig c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(SnapshotDebugConfig c1, SnapshotDebugConfig c2)
        {
            return !c1.Equals(c2);
        }
    }
}
