using System;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.Debugger.Parallel.Extension
{
    /// <summary>
    /// Interaction logic for PropertyPageControl.xaml
    /// </summary>
    public sealed partial class PropertyPageControl : UserControl
    {
        private Guid _projectGuid;
        private ISnapshotDebugConfigManager _snapshotDebugConfigManager;
        
        public PropertyPageControl()
        {
            InitializeComponent();
        }

        public void Initialize(Guid projectGuid, ISnapshotDebugConfigManager configManager)
        {
            _projectGuid = projectGuid;
            _snapshotDebugConfigManager = configManager;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            _snapshotDebugConfigManager?.CreateNewConfiguration(_projectGuid);
        }
    }
}
