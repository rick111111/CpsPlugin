using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using HwndSource = System.Windows.Interop.HwndSource;

namespace DesktopProjectDebug
{
    /// <summary>
    /// Interaction logic for PropertyPageWpf.xaml
    /// </summary>
    public partial class PropertyPageWpf : UserControl, IPropertyPage
    {
        private const int WM_SETFOCUS = 0x0007;
        private const int WM_GETDLGCODE = 0x0087;
        private const int DLGC_WANTARROWS = 0x0001;
        private const int DLGC_WANTTAB = 0x0002;
        private const int DLGC_WANTCHARS = 0x0080;

        private IPropertyPageSite _pageSite;
        private DialogPageElementHost _elementHost;
        private UIElement _firstStackPanelChild;
        private UIElement lastStackPanelChild;
        private SnapshotDebugConfig configuration = new SnapshotDebugConfig();

        public PropertyPageWpf()
        {
            InitializeComponent();

            // TODO implement data context
            this.DataContext = new PropertyPageViewModel();
        }

        public void SetPageSite(IPropertyPageSite pageSite)
        {
            _pageSite = pageSite;
        }

        public void Activate(IntPtr hWndParent, RECT[] rect, int modal)
        {
            Assumes.ThrowIfNull(rect, nameof(rect));

            _elementHost = new DialogPageElementHost(selectedAppServiceTextBox)
            {
                Child = this,
                Left = rect[0].left,
                Top = rect[0].top,
                Width = rect[0].right - rect[0].left,
                Height = rect[0].bottom - rect[0].top
            };

            SetParent((int)_elementHost.Handle, (int)hWndParent);

            var hwndSource = HwndSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.AddHook(this.HwndSourceHook);
            }

            // Find the stack panel children for lookup later. Finding the elements in the stack
            // panel is more robust to future change in case somebody adds another radio button
            // Note: System.Linq doesn't work on a UIElementCollection
            foreach (UIElement child in rootPanel.Children)
            {
                if (_firstStackPanelChild == null)
                    _firstStackPanelChild = child;
                lastStackPanelChild = child;
            }
        }

        public void Deactivate()
        {
            HwndSource hwndSource = HwndSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.RemoveHook(this.HwndSourceHook);
            }

            if (_elementHost != null)
            {
                _elementHost.Child = null;
                _elementHost.Dispose();
                _elementHost = null;
            }

            configuration = null;
            _pageSite = null;
        }

        public void GetPageInfo(PROPPAGEINFO[] pageInfo)
        {
            Assumes.ThrowIfNull(pageInfo, nameof(pageInfo));

            PROPPAGEINFO newPageInfo = new PROPPAGEINFO();
            newPageInfo.cb = (uint)Marshal.SizeOf(typeof(PROPPAGEINFO));
            newPageInfo.pszTitle = "Snapshot Debugger Configuration";
            newPageInfo.SIZE.cx = (int)DesiredSize.Width;
            newPageInfo.SIZE.cy = (int)DesiredSize.Height;
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
            private UIElement _controlToFocus;

            public DialogPageElementHost(UIElement controlToFocus)
            {
                _controlToFocus = controlToFocus;
            }

            /// <inheritdoc />
            protected override void WndProc(ref System.Windows.Forms.Message m)
            {
                if (m.Msg == WM_SETFOCUS)
                {
                    // If our WPF element host receives focus, just set it to our first element
                    _controlToFocus.Focus();
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
            if (msg == WM_GETDLGCODE)
            {
                // Special case up/down and tab/shift tab when on the first or last item
                bool shouldHandle = true;
                var key = KeyInterop.KeyFromVirtualKey(wParam.ToInt32());
                IInputElement currentElement = Keyboard.FocusedElement;
                if (currentElement != null)
                {
                    switch (key)
                    {
                        case Key.Tab:
                            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                            {
                                shouldHandle = currentElement != _firstStackPanelChild;
                            }
                            else
                            {
                                shouldHandle = currentElement != lastStackPanelChild;
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
