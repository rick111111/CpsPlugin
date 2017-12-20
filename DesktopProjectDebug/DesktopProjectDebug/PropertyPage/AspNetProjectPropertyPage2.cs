using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using HwndSource = System.Windows.Interop.HwndSource;
using HwndSourceParameters = System.Windows.Interop.HwndSourceParameters;

namespace DesktopProjectDebug
{
    public class AspNetProjectPropertyPage2 : IPropertyPage
    {
        public const string PropertyPageGuidString = "3F1E4810-F43A-47EF-8294-7978848C45B3";
        public static Guid PropertyPageGuid = new Guid(PropertyPageGuidString);

        private const int WM_SETFOCUS = 0x0007;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int DLGC_WANTARROWS = 0x0001; 
        private const int DLGC_WANTTAB = 0x0002;
        private const int DLGC_WANTCHARS = 0x0080;

        private const int WS_VISIBLE = 0x10000000;
        private const int WS_CHILD = 0x40000000;


        private IPropertyPageSite _pageSite;
        private DialogPageElementHost _elementHost;
        private HwndSource _hwndSource;

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

            HwndSourceParameters parameters = new HwndSourceParameters()
            {
                PositionX = rect[0].left,
                PositionY = rect[0].right,
                Width = 450,
                Height = 150,
                ParentWindow = hWndParent,
                WindowStyle = WS_VISIBLE | WS_CHILD
            };

            _hwndSource = new HwndSource(parameters);
            _hwndSource.RootVisual = PropertyPageControl;
        }

        public void Deactivate()
        {
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

        private IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Indicate character, arrow, and tab input should be handled in the WPF control.
            if (msg == WM_KEYUP || msg == WM_KEYDOWN)
            {
                // Special case up/down and tab/shift tab when on the first or last item
                bool shouldHandle = true;
                Key key = KeyInterop.KeyFromVirtualKey(wParam.ToInt32());
                IInputElement currentElement = Keyboard.FocusedElement;
                if (currentElement != null)
                {
                    switch (key)
                    {
                        case Key.Tab:
                            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            {
                                shouldHandle = currentElement != PropertyPageControl.FirstElement;
                            }
                            else
                            {
                                shouldHandle = currentElement != PropertyPageControl.LastElement;
                            }
                            break;
                    }
                }

                if (shouldHandle)
                {
                    int dlgCode = DLGC_WANTARROWS | DLGC_WANTCHARS | DLGC_WANTTAB;
                    handled = true;
                    return new IntPtr(dlgCode);
                }
            }

            return IntPtr.Zero;
        }
    }
}
