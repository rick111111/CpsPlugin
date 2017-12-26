using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Web.Application;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.Versioning;

namespace Microsoft.VisualStudio.Debugger.Parallel.Extension
{
    /// <summary>
    /// Web Project Extension for Asp.Net Desktop scenario, add launch target and debug option page for snapshot debugging
    /// </summary>
    [Export(typeof(IWebProjectDebugExtension))]
    [ExportMetadata("Order", 200)]
    internal class WebProjectDebuggerExtension : IWebProjectDebugExtension, IWebProjectDebugExtension2
    {
        private const string ExtensionIdentifier = "SnapshotDebugExtension";

        [Import]
        private Lazy<ISnapshotDebugConfigManager> _snapshotDebugConfigManager = null;

        // create one IVsDebuggableProjectCfg object per project
        private Dictionary<Guid, DebuggableProjectConfig> _debugConfigs = new Dictionary<Guid, DebuggableProjectConfig>();
        
        public string DebugTargetMenuCommand
        {
            get 
            {
                return Resources.DebugExtensionTargetMenuCommand;
            }
        }

        public string DebugExtensionIdentifier
        {
            get
            {
                return ExtensionIdentifier;
            }
        }

        public string SettingsPageGuid
        {
            get
            {
                return WebProjectPropertyPage.PropertyPageGuid.ToString("B");
            }
        }

        public IVsDebuggableProjectCfg GetDebuggableConfig(IVsHierarchy project)
        {
            return GetProjectConfig(project);
        }

        public void SetActive(IVsHierarchy project, bool isActive)
        {
            if (isActive)
            {                
                Guid projectGuid = Utils.GetProjectGuid(project);
                if (projectGuid != Guid.Empty)
                {
                    SnapshotDebugConfig config = _snapshotDebugConfigManager.Value?.GetConfiguration(projectGuid);
                    if (config == null)
                    {
                        _snapshotDebugConfigManager.Value?.CreateNewConfiguration(projectGuid);
                    }
                }
            }
        }

        /// <summary>
        /// Only support desktop CLR version 4.6.1 and above
        /// </summary>
        public bool IsSupported(IVsHierarchy project)
        {
            FrameworkName fwname = Utils.GetProjectTargetFramework(project);
            if (fwname != null && fwname.Version.Major >= 4 && fwname.Version.Minor >= 6 && fwname.Version.Build >= 1)
            {
                return true;
            }

            return false;
        }

        private DebuggableProjectConfig GetProjectConfig(IVsHierarchy project)
        {
            Guid projectGuid = Utils.GetProjectGuid(project);

            if (projectGuid != Guid.Empty)
            {
                if (!_debugConfigs.TryGetValue(projectGuid, out var debugConfig))
                {
                    debugConfig = new DebuggableProjectConfig(projectGuid, _snapshotDebugConfigManager.Value);
                    _debugConfigs[projectGuid] = debugConfig;
                }

                return debugConfig;
            }

            return null;
        }
    }
}
