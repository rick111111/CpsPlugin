using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HwndSource = System.Windows.Interop.HwndSource;

namespace DesktopProjectDebug
{
    /// <summary>
    /// Interaction logic for PropertyPageWpf.xaml
    /// </summary>
    [Guid("3F1E4810-F43A-47EF-8294-7978848C45B3")]
    public partial class PropertyPageWpf : UserControl, IPropertyPage
    {
        private const int WM_SETFOCUS = 0x0007;
        private const int WM_GETDLGCODE = 0x0087;
        private const int DLGC_WANTARROWS = 0x0001;
        private const int DLGC_WANTTAB = 0x0002;
        private const int DLGC_WANTCHARS = 0x0080;

        private IPropertyPageSite pageSite;
        private DialogPageElementHost elementHost;
        private UIElement firstStackPanelChild;
        private UIElement lastStackPanelChild;
        private SnapshotDebugConfiguration configuration = new SnapshotDebugConfiguration();

        public PropertyPageWpf()
        {
            InitializeComponent();

            // TODO implement data context
            this.DataContext = new PropertyPageViewModel();
        }

        public void SetPageSite(IPropertyPageSite pPageSite)
        {
            pageSite = pPageSite;
        }

        public void Activate(IntPtr hWndParent, RECT[] rect, int bModal)
        {
            Assumes.ThrowIfNull(rect, nameof(rect));

            elementHost = new DialogPageElementHost(selectedAppServiceTextBox)
            {
                Child = this,
                Left = rect[0].left,
                Top = rect[0].top,
                Width = rect[0].right - rect[0].left,
                Height = rect[0].bottom - rect[0].top
            };

            SetParent((int)this.elementHost.Handle, (int)hWndParent);

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
                if (firstStackPanelChild == null)
                    firstStackPanelChild = child;
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

            if (elementHost != null)
            {
                elementHost.Child = null;
                elementHost.Dispose();
                elementHost = null;
            }

            configuration = null;
            pageSite = null;
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

            elementHost.Width = rect[0].right - rect[0].left;
            elementHost.Height = rect[0].bottom - rect[0].top;
            elementHost.Location = new System.Drawing.Point(rect[0].left, rect[0].top);
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

        public int TranslateAccelerator(MSG[] pMsg)
        {
            return pageSite.TranslateAccelerator(pMsg);
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
                                shouldHandle = currentElement != firstStackPanelChild;
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
