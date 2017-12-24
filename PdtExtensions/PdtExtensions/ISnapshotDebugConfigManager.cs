using System;

namespace Microsoft.VisualStudio.Debugger.Parallel.Extension
{
    public interface ISnapshotDebugConfigManager
    {
        event EventHandler<Guid> ConfigurationChanged;

        SnapshotDebugConfig EnsureConfigurationExist(Guid projectGuid);

        SnapshotDebugConfig GetConfiguration(Guid projectGuid);
    }
}