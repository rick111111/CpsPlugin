using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;
using System.Runtime.InteropServices;

namespace DesktopProjectDebug
{
    public partial class PropertyPage : UserControl, IPropertyPage
    {
        private IPropertyPageSite _site = null;

        public PropertyPage()
        {
            InitializeComponent();
        }

        public void SetPageSite(IPropertyPageSite pPageSite)
        {
            _site = pPageSite;
        }

        public void Activate(IntPtr hWndParent, RECT[] pRect, int bModal)
        {
            Win32Methods.SetParent(this.Handle, hWndParent);
        }

        public void Deactivate()
        {
        }

        public void GetPageInfo(PROPPAGEINFO[] pPageInfo)
        {
            PROPPAGEINFO info = new PROPPAGEINFO();

            info.cb = (uint)Marshal.SizeOf(typeof(PROPPAGEINFO));
            info.dwHelpContext = 0;
            info.pszDocString = null;
            info.pszHelpFile = null;
            info.pszTitle = "Desktop Project Debugger";
            info.SIZE.cx = this.Size.Width;
            info.SIZE.cy = this.Size.Height;
            if (pPageInfo != null && pPageInfo.Length > 0)
                pPageInfo[0] = info;
        }

        public void SetObjects(uint cObjects, object[] ppunk)
        {
        }

        public void Show(uint nCmdShow)
        {
            if (nCmdShow != SW_HIDE)
                this.Show();
            else
                this.Hide();
        }

        private const int SW_HIDE = 0;

        void IPropertyPage.Move(RECT[] pRect)
        {
            if (pRect == null || pRect.Length <= 0)
                throw new ArgumentNullException("pRect");

            Microsoft.VisualStudio.OLE.Interop.RECT r = pRect[0];

            this.Location = new Point(r.left, r.top);
        }

        public int IsPageDirty()
        {
            return VSConstants.S_FALSE;
        }

        public int Apply()
        {
            throw new NotImplementedException();
        }

        public void Help(string pszHelpDir)
        {
        }

        public int TranslateAccelerator(MSG[] pMsg)
        {
            if (pMsg == null)
                return VSConstants.E_POINTER;

            Message m = Message.Create(pMsg[0].hwnd, (int)pMsg[0].message, pMsg[0].wParam, pMsg[0].lParam);
            bool used = false;

            // Preprocessing should be passed to the control whose handle the message refers to.
            Control target = Control.FromChildHandle(m.HWnd);
            if (target != null)
                used = target.PreProcessMessage(ref m);

            if (used)
            {
                pMsg[0].message = (uint)m.Msg;
                pMsg[0].wParam = m.WParam;
                pMsg[0].lParam = m.LParam;
                // Returning S_OK indicates we handled the message ourselves
                return VSConstants.S_OK;
            }


            // Returning S_FALSE indicates we have not handled the message
            int result = 0;
            if (this._site != null)
                result = _site.TranslateAccelerator(pMsg);
            return result;
        }
    }
}
