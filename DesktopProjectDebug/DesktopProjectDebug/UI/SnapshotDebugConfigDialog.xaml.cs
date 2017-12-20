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

        public bool Result { get; private set; }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            this.Close();
        }
    }
}
