using System;
using System.Collections.Generic;

namespace DesktopProjectDebug
{
    [Serializable]
    internal sealed class SnapshotDebugUserSettings
    {
        private const string DefaultVersion = "1.0";

        public const string UserSettingKey = "SnapshotDebugConfigUserSetting";

        public SnapshotDebugUserSettings(IEnumerable<SnapshotDebugConfig> configs)
        {
            Assumes.ThrowIfNull(configs, nameof(configs));

            Version = DefaultVersion;
            SnapshotDebugConfigList = configs;
        }
        
        public static bool VersionMatch(string version)
        {
            return DefaultVersion == version;
        }
        
        public string Version { get; }
        
        public IEnumerable<SnapshotDebugConfig> SnapshotDebugConfigList { get; }
    }
}
