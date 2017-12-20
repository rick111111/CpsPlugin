using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DesktopProjectDebug
{
    [ComVisible(false)]
    internal class DebuggableProjectConfig :
        IVsDebuggableProjectCfg,
        IVsDebuggableProjectCfg2
    {
        Guid CLSID_ComPlusOnlyDebugEngine4 = new Guid("{FB0D4648-F776-4980-95F8-BB7F36EBC1EE}");

        internal SnapshotDebugConfig SnapshotDebugConfig { get; set; }

        public DebuggableProjectConfig()
        {
            SnapshotDebugConfig = new SnapshotDebugConfig("", "", "");
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

        /// <summary>
        /// IVsDebuggableProjectCfg
        /// </summary>
        public int DebugLaunch(uint grfLaunch)
        {
            VsDebugTargetInfo4 info = new VsDebugTargetInfo4();
            info.dlo = (uint)DEBUG_LAUNCH_OPERATION.DLO_CreateProcess;
            info.guidLaunchDebugEngine = CLSID_ComPlusOnlyDebugEngine4;

            // Note. A bug in the debugger will cause a failure if a PID is set but the exe name
            // is NULL. So we just blindly set it here.
            info.bstrExe = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "cmd.exe");
            info.bstrArg = null;
            info.bstrCurDir = null;
            info.LaunchFlags = grfLaunch;

            IVsDebugger4 debugger = ServiceProvider.GlobalProvider.GetService(typeof(SVsShellDebugger)) as IVsDebugger4;
            var debugTargetInfos = new VsDebugTargetInfo4[] { info };
            // Debugger will throw on failure
            var tpi = new VsDebugTargetProcessInfo[1];
            debugger.LaunchDebugTargets4(1, debugTargetInfos, tpi);
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

        /// <summary>
        /// IVsDebuggableProjectCfg2. We override to prevent inner project system from playing with
        /// their hosting process
        /// </summary>
        public int OnBeforeDebugLaunch(uint grfLaunch)
        {
            return VSConstants.S_OK;
        }
    }
}
