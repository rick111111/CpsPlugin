using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.Versioning;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Web.Application;

namespace DesktopProjectDebug
{
    [Export(typeof(IWebProjectDebugExtension))]
    [ExportMetadata("Order", 200)]
    internal class WebProjectDebuggerExtension : IWebProjectDebugExtension, IWebProjectDebugExtension2
    {
        private const string ExtensionIdentifier = "SnapshotDebugExtension";

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
                return PropertyPageForm.PropertyPageGuid.ToString("B");
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
                DebuggableProjectConfig debugcfg = GetProjectConfig(project);

                if (debugcfg != null)
                {
                    SnapshotDebugConfigDialog dialog = new SnapshotDebugConfigDialog(debugcfg.SnapshotDebugConfig);
                    dialog.ShowDialog();
                }
            }
        }

        public bool IsSupported(IVsHierarchy project)
        {
            FrameworkName fwname = GetProjectTargetFramework(project);
            if (fwname != null && fwname.Version.Major >= 4 && fwname.Version.Minor >= 6 && fwname.Version.Build >= 1)
            {
                return true;
            }

            return false;
        }

        private DebuggableProjectConfig GetProjectConfig(IVsHierarchy project)
        {
            Guid projectGuid = GetProjectGuid(project);

            if (Assumes.Verify(projectGuid != Guid.Empty))
            {
                if (!_debugConfigs.TryGetValue(projectGuid, out var debugConfig))
                {
                    debugConfig = new DebuggableProjectConfig();
                    _debugConfigs[projectGuid] = debugConfig;
                }

                return debugConfig;
            }

            return null;
        }

        private Guid GetProjectGuid(IVsHierarchy project)
        {
            if (Assumes.Verify(project is IVsProject))
            {
                int result = project.GetGuidProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ProjectIDGuid, out Guid guid);
                if (result == VSConstants.S_OK)
                {
                    return guid;
                }
            }

            return Guid.Empty;
        }

        private FrameworkName GetProjectTargetFramework(IVsHierarchy project)
        {
            if (Assumes.Verify(project is IVsProject))
            {
                int result = project.GetProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID4.VSHPROPID_TargetFrameworkMoniker, out object obj);
                string framework = obj as string;
                if (result == VSConstants.S_OK && !string.IsNullOrEmpty(framework))
                {
                    return new FrameworkName(framework);
                }
            }

            return null;
        }
    }
}
