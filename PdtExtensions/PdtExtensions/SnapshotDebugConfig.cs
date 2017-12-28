using Microsoft.WindowsAzure.Client.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Debugger.Parallel.Extension
{
    [Serializable]
    [DebuggerDisplay("SiteName = {WebsiteName}, ResourceGroup = {ResourceId}")]
    public sealed class SnapshotDebugConfig
    {
        private const string BearerTokenPrefix = "Bearer ";

        public string ResourceId { get; }

        public string WebsiteName { get; }

        private IARMRoot _entityRoot;

        public SnapshotDebugConfig(string resourceId, string websiteName, IARMRoot entityRoot)
        {
            ResourceId = resourceId;
            WebsiteName = websiteName;
            _entityRoot = entityRoot;
        }

        public async Task<string> GetBearerToken()
        {
            string token = string.Empty;
            if (_entityRoot != null)
            {
                token = await _entityRoot?.AuthenticationProvider.GetBearerTokenAsync(_entityRoot?.EnvironmentSettings.ARMServiceManagementEndpointUri);
            }

            if (!string.IsNullOrEmpty(token) && !token.StartsWith(BearerTokenPrefix))
            {
                token = BearerTokenPrefix + token;
            }

            return token;
        }

        public override bool Equals(object obj)
        {
            var config = obj as SnapshotDebugConfig;
            return config != null &&
                   ResourceId == config.ResourceId &&
                   WebsiteName == config.WebsiteName;
        }

        public override int GetHashCode()
        {
            var hashCode = 1723940257;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ResourceId);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(WebsiteName);
            return hashCode;
        }

        public static bool operator ==(SnapshotDebugConfig c1, SnapshotDebugConfig c2)
        {
            if (ReferenceEquals(c1, c2))
            {
                return true;
            }
            if (ReferenceEquals(c1, null))
            {
                return false;
            }
            if (ReferenceEquals(c2, null))
            {
                return false;
            }

            return c1.Equals(c2);
        }

        public static bool operator !=(SnapshotDebugConfig c1, SnapshotDebugConfig c2)
        {
            return !(c1 == c2);
        }
    }
}
