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

namespace Microsoft.VisualStudio.Debugger.Parallel.Extension
{
    /// <summary>
    /// Interaction logic for PropertyPageControl.xaml
    /// </summary>
    public partial class PropertyPageControl : UserControl
    {
        public Guid ProjectGuid { get; set; }

        public PropertyPageControl()
        {
            InitializeComponent();
        }
    }
}
