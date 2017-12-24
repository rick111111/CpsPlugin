using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.Versioning;

namespace Microsoft.VisualStudio.Debugger.Parallel.Extension
{
    public static class Utils
    {
        /// <summary>
        /// Get project Guid from IVsProject
        /// </summary>
        public static Guid GetProjectGuid(IVsHierarchy project)
        {
            if (project is IVsProject)
            {
                int result = project.GetGuidProperty(VSConstants.VSITEMID_ROOT, (int)__VSHPROPID.VSHPROPID_ProjectIDGuid, out Guid guid);
                if (result == VSConstants.S_OK)
                {
                    return guid;
                }
            }

            return Guid.Empty;
        }

        /// <summary>
        /// Get Target framework setting from IVsProject
        /// </summary>
        public static FrameworkName GetProjectTargetFramework(IVsHierarchy project)
        {
            if (project is IVsProject)
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
