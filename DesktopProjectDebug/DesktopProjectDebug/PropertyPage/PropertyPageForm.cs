﻿using System;
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
    [Guid(PropertyPageGuidString)]
    public partial class PropertyPageForm : UserControl, IPropertyPage
    {
        public const string PropertyPageGuidString = "3F1E4810-F43A-47EF-8294-7978848C45B3";
        public static Guid PropertyPageGuid = new Guid(PropertyPageGuidString);

        private const int SW_HIDE = 0;
        private IPropertyPageSite _site = null;

        public PropertyPageForm()
        {
            InitializeComponent();
        }

        public void SetPageSite(IPropertyPageSite pPageSite)
        {
            _site = pPageSite;
        }

        public void Activate(IntPtr hWndParent, RECT[] pRect, int bModal)
        {
            this.SuspendLayout();

            var parentControl = Control.FromHandle(hWndParent);
            if (parentControl != null)
            {
                this.Parent = parentControl;
            }

            Win32Methods.SetParent(this.Handle, hWndParent);

            this.ResumeLayout();
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
            info.pszTitle = Resources.DebugPropertyPageTitle;
            info.SIZE.cx = this.Size.Width;
            info.SIZE.cy = this.Size.Height;
            if (pPageInfo != null && pPageInfo.Length > 0)
            {
                pPageInfo[0] = info;
            }
        }

        public void SetObjects(uint cObjects, object[] ppunk)
        {
        }

        public void Show(uint nCmdShow)
        {
            if (nCmdShow != SW_HIDE)
            {
                this.Show();
            }
            else
            {
                this.Hide();
            }
        }


        void IPropertyPage.Move(RECT[] pRect)
        {
            if (pRect == null || pRect.Length <= 0)
                throw new ArgumentNullException("pRect");

            RECT r = pRect[0];

            this.Location = new Point(r.left, r.top);
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
            return VSConstants.S_OK;
        }
    }
}