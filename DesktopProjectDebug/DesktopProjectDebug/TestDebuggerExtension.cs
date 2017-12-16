using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Web.Application;

namespace DesktopProjectDebug
{
    [Export(typeof(IWebProjectDebugExtension))]
    [ExportMetadata("Order", 150)]
    internal class TestDebuggerExtension : IWebProjectDebugExtension, IWebProjectDebugExtension2
    {
        Dictionary<IVsHierarchy, DebuggableProjectCfg> _debugCfgs = new Dictionary<IVsHierarchy, DebuggableProjectCfg>();

        public TestDebuggerExtension()
        {
        }

        public string DebugTargetMenuCommand
        {
            get 
            {
                return "Desktop Project Debugger";
            }
        }

        public string DebugExtensionIdentifier
        {
            get
            {
                return "DesktopProjectDebugger";
            }
        }

        public string SettingsPageGuid
        {
            get
            {
                return "{3F1E4810-F43A-47EF-8294-7978848C45B3}";
            }
        }

        public IVsDebuggableProjectCfg GetDebuggableConfig(IVsHierarchy project)
        {
            if (!_debugCfgs.TryGetValue(project, out var debugcfg))
            {
                debugcfg = new DebuggableProjectCfg(project, "cmd.exe");
            }

            return debugcfg;
        }
        public void SetActive(IVsHierarchy project, bool isActive)
        {
            if(isActive)
                MessageBox.Show("Test Debugger selected");
            else
                MessageBox.Show("Test Debugger deselected");
        }

        public bool IsSupported(IVsHierarchy project)
        {
            return true;
        }
    }
}
