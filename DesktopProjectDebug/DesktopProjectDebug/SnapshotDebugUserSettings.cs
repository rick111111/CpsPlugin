using System;
using System.Collections.Generic;

namespace DesktopProjectDebug
{
    [Serializable]
    internal sealed class SnapshotDebugUserSettings
    {
        private const string DefaultVersion = "1.0";

        public const string UserSettingKey = "SnapshotDebugConfigUserSetting";

        public SnapshotDebugUserSettings(Dictionary<Guid, IEnumerable<SnapshotDebugConfig>> configs)
        {
            Assumes.ThrowIfNull(configs, nameof(configs));

            Version = DefaultVersion;
            DebugConfigs = configs;
        }
        
        public static bool VersionMatch(string version)
        {
            return DefaultVersion == version;
        }
        
        public string Version { get; }
        
        public Dictionary<Guid, IEnumerable<SnapshotDebugConfig>> DebugConfigs { get; }
    }
}
