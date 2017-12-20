using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace DesktopProjectDebug
{
    public class AspNetProjectPropertyPage : IPropertyPage
    {
        public const string PropertyPageGuidString = "3F1E4810-F43A-47EF-8294-7978848C45B3";
        public static Guid PropertyPageGuid = new Guid(PropertyPageGuidString);

        private const int WM_SETFOCUS = 0x0007;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int DLGC_WANTARROWS = 0x0001; 
        private const int DLGC_WANTTAB = 0x0002;
        private const int DLGC_WANTCHARS = 0x0080;

        private IPropertyPageSite _pageSite;
        private DialogPageElementHost _elementHost;

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
            Assumes.ThrowIfNull(rect, nameof(rect));

            _elementHost = new DialogPageElementHost(PropertyPageControl.FirstElement, PropertyPageControl.LastElement)
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
            Assumes.ThrowIfNull(pageInfo, nameof(pageInfo));

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

        public void SetObjects(uint cObjects, object[] ppunk)
        {
        }

        public void Show(uint nCmdShow)
        {
        }

        public void Move(RECT[] rect)
        {
            Assumes.ThrowIfNull(rect, nameof(rect));

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

        /// <summary>
        /// Subclass of ElementHost designed to work around focus issues in native Win32 property page frames
        /// </summary>
        private class DialogPageElementHost : ElementHost
        {
            private UIElement _firstElement;
            private UIElement _lastElement;

            public DialogPageElementHost(UIElement fistElement, UIElement lastElement)
            {
                Assumes.ThrowIfNull(fistElement, nameof(fistElement));

                _firstElement = fistElement;
                _lastElement = lastElement;
            }

            protected override void WndProc(ref System.Windows.Forms.Message m)
            {
                if (m.Msg == WM_SETFOCUS)
                {
                    // If our WPF element host receives focus, just set it to our first element
                    _firstElement.Focus();
                    return;
                }                

                base.WndProc(ref m);
            }
        }

        [DllImport("User32.dll", EntryPoint = "SetParent")]
        internal static extern int SetParent(int windowHandle, int parentWindowHandle);
    }
}
