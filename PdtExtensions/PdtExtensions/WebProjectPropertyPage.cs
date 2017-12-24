using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using System;
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

        private PropertyPageControl _propertyPageControl;
        internal PropertyPageControl PropertyPageControl
        {
            get
            {
                if (_propertyPageControl == null)
                {
                    _propertyPageControl = new PropertyPageControl();
                }
                return _propertyPageControl;
            }
        }

        public void SetPageSite(IPropertyPageSite pageSite)
        {
            // pageSite value will be null on clean up
            _pageSite = pageSite;
        }

        public void Activate(IntPtr hWndParent, RECT[] rect, int modal)
        {
            _elementHost = new ElementHost()
            {
                Child = PropertyPageControl,
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
            if (_elementHost != null)
            {
                _elementHost.Child = null;
                _elementHost.Dispose();
                _elementHost = null;
            }

            _propertyPageControl = null;
            _pageSite = null;
        }

        public void GetPageInfo(PROPPAGEINFO[] pageInfo)
        {
            PROPPAGEINFO newPageInfo = new PROPPAGEINFO();
            newPageInfo.cb = (uint)Marshal.SizeOf(typeof(PROPPAGEINFO));
            newPageInfo.pszTitle = Resources.DebugPropertyPageTitle;
            newPageInfo.SIZE.cx = (int)PropertyPageControl.Width;
            newPageInfo.SIZE.cy = (int)PropertyPageControl.Height;
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
                foreach (IVsBrowseObject obj in ppunk.OfType<IVsBrowseObject>())
                {
                    if (obj.GetProjectItem(out IVsHierarchy vsHierarchy, out uint _) == VSConstants.S_OK && vsHierarchy is IVsProject)
                    {
                        PropertyPageControl.ProjectGuid = Utils.GetProjectGuid(vsHierarchy);
                    }
                }
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
