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

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
