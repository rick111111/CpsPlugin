using System.Windows;

namespace DesktopProjectDebug
{
    /// <summary>
    /// Interaction logic for SnapshotDebugConfigDialog.xaml
    /// </summary>
    public partial class SnapshotDebugConfigDialog : Window
    {
        public SnapshotDebugConfigDialog(SnapshotDebugConfig config)
        {
            InitializeComponent();

            this.DataContext = config;
        }
    }
}
