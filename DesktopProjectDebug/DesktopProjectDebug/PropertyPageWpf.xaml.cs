using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopProjectDebug
{
    /// <summary>
    /// Interaction logic for PropertyPageWpf.xaml
    /// </summary>
    public partial class PropertyPageWpf : UserControl, IPropertyPage
    {
        public PropertyPageWpf()
        {
            InitializeComponent();
        }

        public void SetPageSite(IPropertyPageSite pPageSite)
        {
            throw new NotImplementedException();
        }

        public void Activate(IntPtr hWndParent, RECT[] pRect, int bModal)
        {
            throw new NotImplementedException();
        }

        public void Deactivate()
        {
            throw new NotImplementedException();
        }

        public void GetPageInfo(PROPPAGEINFO[] pPageInfo)
        {
            throw new NotImplementedException();
        }

        public void SetObjects(uint cObjects, object[] ppunk)
        {
            throw new NotImplementedException();
        }

        public void Show(uint nCmdShow)
        {
            throw new NotImplementedException();
        }

        public void Move(RECT[] pRect)
        {
            throw new NotImplementedException();
        }

        public int IsPageDirty()
        {
            throw new NotImplementedException();
        }

        public int Apply()
        {
            throw new NotImplementedException();
        }

        public void Help(string pszHelpDir)
        {
            throw new NotImplementedException();
        }

        public int TranslateAccelerator(MSG[] pMsg)
        {
            throw new NotImplementedException();
        }
    }
}
