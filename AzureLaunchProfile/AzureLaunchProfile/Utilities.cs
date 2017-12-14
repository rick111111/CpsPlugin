using Microsoft.VisualStudio.ProjectSystem.Debug;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace Microsoft.VisualStudio.ProductionDebug.LaunchProfile
{
    public static class ProjectCapabilities
    {
        public const string DotNetCoreWeb = "DotNetCoreWeb";
    }

    public static class Utilities
    {
        public static void EnsurePackageLoaded()
        {
            IVsShell vsShell = ServiceProvider.GlobalProvider.GetService(typeof(SVsShell)) as IVsShell;
            IVsPackage loadedPackage;
            vsShell.LoadPackage(typeof(AzureLaunchProfilePackage).GUID, out loadedPackage);
        }

        internal static bool IsSnapshotDebuggerProfile(this ILaunchProfile launchProfile)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(launchProfile?.CommandName, Resources.SnapshotDebuggerLaunchProfilerCmdName);
        }
    }
}
