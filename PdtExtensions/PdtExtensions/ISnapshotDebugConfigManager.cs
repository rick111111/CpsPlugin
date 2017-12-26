using System;

namespace Microsoft.VisualStudio.Debugger.Parallel.Extension
{
    public interface ISnapshotDebugConfigManager
    {
        event EventHandler<Guid> ConfigurationChanged;

        SnapshotDebugConfig GetConfiguration(Guid projectGuid);

        SnapshotDebugConfig CreateNewConfiguration(Guid projectGuid);
    }
}