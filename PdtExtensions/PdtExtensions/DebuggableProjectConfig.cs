using Microsoft.VisualStudio.Debugger.Interop.Internal;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Debugger.Parallel.Extension
{
    [ComVisible(false)]
    internal class DebuggableProjectConfig :
        IVsDebuggableProjectCfg,
        IVsDebuggableProjectCfg2
    {
        private ISnapshotDebugConfigManager _snapshotDebugConfigManager;
        private Guid _projectGuid;

        public DebuggableProjectConfig(Guid projectGuid, ISnapshotDebugConfigManager configManager)
        {
            _projectGuid = projectGuid;
            _snapshotDebugConfigManager = configManager;
        }

        public virtual int get_DisplayName(out string name)
        {
            name = Resources.DebugConfigDisplayName;

            return VSConstants.S_OK;
        }

        public virtual int get_IsDebugOnly(out int fDebug)
        {
            fDebug = 0;
            return VSConstants.S_OK;
        }

        public virtual int get_IsReleaseOnly(out int fRelease)
        {
            fRelease = 0;
            return VSConstants.S_OK;
        }

        public virtual int EnumOutputs(out IVsEnumOutputs eo)
        {
            eo = null;
            return VSConstants.E_NOTIMPL;
        }
        public virtual int get_BuildableProjectCfg(out IVsBuildableProjectCfg pb)
        {
            pb = null;
            return VSConstants.E_NOTIMPL;
        }

        public virtual int get_CanonicalName(out string name)
        {
            name = "Debug";
            return VSConstants.S_OK;
        }

        public virtual int get_IsPackaged(out int pkgd)
        {
            pkgd = 0;
            return VSConstants.S_OK;
        }

        public virtual int get_IsSpecifyingOutputSupported(out int f)
        {
            f = 0;
            return VSConstants.E_NOTIMPL;
        }

        public virtual int get_Platform(out Guid platform)
        {
            platform = Guid.Empty;
            return VSConstants.E_NOTIMPL;
        }

        public virtual int get_ProjectCfgProvider(out IVsProjectCfgProvider p)
        {
            p = null;
            return VSConstants.E_NOTIMPL;
        }

        public virtual int get_RootURL(out string root)
        {
            root = "";
            return VSConstants.E_NOTIMPL;
        }

        public virtual int get_TargetCodePage(out uint target)
        {
            target = 0;
            return VSConstants.E_NOTIMPL;
        }

        public virtual int get_UpdateSequenceNumber(ULARGE_INTEGER[] li)
        {
            return VSConstants.E_NOTIMPL;
        }

        public virtual int OpenOutput(string name, out IVsOutput output)
        {
            output = null;
            return VSConstants.E_NOTIMPL;
        }

        public int DebugLaunch(uint grfLaunch)
        {
            if (_snapshotDebugConfigManager != null)
            {
                SnapshotDebugConfig config = _snapshotDebugConfigManager.EnsureConfigurationExist(_projectGuid);
                IProductionDebuggerInternal debugger = ServiceProvider.GlobalProvider.GetService(typeof(SVsShellDebugger)) as IProductionDebuggerInternal;
                if (config != null && debugger != null)
                {
                    debugger.LaunchProductionDebugWithAzureTools(config.ResourceId, config.WebsiteName, config.Subscription);
                }
            }
           
            return VSConstants.S_OK;
        }

        /// <summary>
        /// IVsDebuggableProjectCfg
        /// Return whether it is OK to launch the debugger
        /// </summary>
        public int QueryDebugLaunch(uint grfLaunch, out int pfCanLaunch)
        {
            pfCanLaunch = 1;
            return VSConstants.S_OK;
        }

        public int OnBeforeDebugLaunch(uint grfLaunch)
        {
            return VSConstants.S_OK;
        }
    }
}
