using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Microsoft.VisualStudio.Debugger.Parallel.Extension
{
    [Guid(PropertyPageGuidString)]
    public class WebProjectPropertyPage : IPropertyPage
    {
        public const string PropertyPageGuidString = "3F1E4810-F43A-47EF-8294-7978848C45B3";
        public static Guid PropertyPageGuid = new Guid(PropertyPageGuidString);

        private IPropertyPageSite _pageSite;
        private ElementHost _elementHost;

        private Guid _projectGuid;
        private PropertyPageControl _propertyPageControl = new PropertyPageControl();
        private PropertyPageViewModel _propertyPageViewModel;

        [Import]
        private ISnapshotDebugConfigManager _snapshotDebugConfigManager = null;

        public void SetPageSite(IPropertyPageSite pageSite)
        {
            // pageSite value will be null on clean up
            _pageSite = pageSite;
        }

        public void Activate(IntPtr hWndParent, RECT[] rect, int modal)
        {
            _elementHost = new ElementHost()
            {
                Child = _propertyPageControl,
                Left = rect[0].left,
                Top = rect[0].top,
                Width = rect[0].right - rect[0].left,
                Height = rect[0].bottom - rect[0].top
            };

            var parentControl = Control.FromHandle(hWndParent);
            if (parentControl != null)
            {
                _elementHost.Parent = parentControl;
            }

            SetParent((int)_elementHost.Handle, (int)hWndParent);
        }

        public void Deactivate()
        {
            if (_snapshotDebugConfigManager != null)
            {
                _snapshotDebugConfigManager.ConfigurationChanged -= SnapshotDebugConfigManager_ConfigurationChanged;
            }

            if (_elementHost != null)
            {
                _elementHost.Child = null;
                _elementHost.Dispose();
                _elementHost = null;
            }

            _pageSite = null;
        }

        public void GetPageInfo(PROPPAGEINFO[] pageInfo)
        {
            PROPPAGEINFO newPageInfo = new PROPPAGEINFO();

            newPageInfo.cb = (uint)Marshal.SizeOf(typeof(PROPPAGEINFO));
            newPageInfo.pszTitle = Resources.DebugPropertyPageTitle;
            newPageInfo.SIZE.cx = (int)_propertyPageControl.Width;
            newPageInfo.SIZE.cy = (int)_propertyPageControl.Height;
            newPageInfo.pszDocString = null;
            newPageInfo.pszHelpFile = null;
            newPageInfo.dwHelpContext = 0;

            pageInfo[0] = newPageInfo;
        }

        /// <summary>
        /// This will be called before Activate()
        /// cObjects will be 0 on clean up.
        /// </summary>
        public void SetObjects(uint cObjects, object[] ppunk)
        {
            if (cObjects > 0)
            {
                // satisfy mef imports
                IComponentModel host = ServiceProvider.GlobalProvider.GetService(typeof(SComponentModel)) as IComponentModel;
                if (host != null)
                {
                    host.DefaultCompositionService.SatisfyImportsOnce(this);
                }

                foreach (IVsBrowseObject obj in ppunk.OfType<IVsBrowseObject>())
                {
                    if (obj.GetProjectItem(out IVsHierarchy vsHierarchy, out uint _) == VSConstants.S_OK && vsHierarchy is IVsProject)
                    {
                        _projectGuid = Utils.GetProjectGuid(vsHierarchy);
                    }
                }

                if (_projectGuid != null && _projectGuid != Guid.Empty && _snapshotDebugConfigManager != null)
                {
                    _propertyPageControl.Initialize(_projectGuid, _snapshotDebugConfigManager);

                    SnapshotDebugConfig config = _snapshotDebugConfigManager.GetConfiguration(_projectGuid);
                    _propertyPageViewModel = new PropertyPageViewModel(config);
                    _propertyPageControl.DataContext = _propertyPageViewModel;

                    _snapshotDebugConfigManager.ConfigurationChanged += SnapshotDebugConfigManager_ConfigurationChanged;
                }
            }
        }

        private void SnapshotDebugConfigManager_ConfigurationChanged(object sender, Guid e)
        {
            if (_projectGuid == e)
            {
                _propertyPageViewModel.SetConfiguration(_snapshotDebugConfigManager.GetConfiguration(_projectGuid));
            }
        }

        public void Show(uint nCmdShow)
        {
        }

        public void Move(RECT[] rect)
        {
            _elementHost.Width = rect[0].right - rect[0].left;
            _elementHost.Height = rect[0].bottom - rect[0].top;
            _elementHost.Location = new System.Drawing.Point(rect[0].left, rect[0].top);
        }

        public int IsPageDirty()
        {
            return VSConstants.S_FALSE;
        }

        public int Apply()
        {
            return VSConstants.S_OK;
        }

        public void Help(string pszHelpDir)
        {
        }

        public int TranslateAccelerator(MSG[] msg)
        {
            return _pageSite.TranslateAccelerator(msg);
        }

        [DllImport("User32.dll", EntryPoint = "SetParent")]
        internal static extern int SetParent(int windowHandle, int parentWindowHandle);
    }
}
