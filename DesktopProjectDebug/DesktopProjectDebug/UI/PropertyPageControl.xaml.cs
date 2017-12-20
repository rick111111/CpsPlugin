using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ElementHost = System.Windows.Forms.Integration.ElementHost;

namespace DesktopProjectDebug
{
    /// <summary>
    /// Interaction logic for PropertyPageControl.xaml
    /// </summary>
    public sealed partial class PropertyPageControl : UserControl, IDisposable
    {
        public UIElement LastElement { get; set; }

        public UIElement FirstElement { get; set; }

        private PropertyPageViewModel _viewModel;
        private SnapshotDebugConfigManager _configManager;

        public PropertyPageControl(SnapshotDebugConfigManager configManager)
        {
            Assumes.ThrowIfNull(configManager, nameof(configManager));

            InitializeComponent();

            _configManager = configManager;
            _configManager.ConfigListChanged += ConfigManager_ConfigListChanged;

            // Find the stack panel children for lookup later. Finding the elements in the stack
            // panel is more robust to future change in case somebody adds another radio button
            // Note: System.Linq doesn't work on a UIElementCollection
            foreach (UIElement child in rootPanel.Children)
            {
                if (FirstElement == null)
                {
                    FirstElement = child;
                }
                LastElement = child;
            }

            _viewModel = new PropertyPageViewModel();
            _viewModel.SnapshotDebugConfigList = VSPackage1.ConfigManager.GetConfigList();
            this.DataContext = _viewModel;
        }

        private void ConfigManager_ConfigListChanged(object sender, System.EventArgs e)
        {
            _viewModel.SnapshotDebugConfigList = VSPackage1.ConfigManager.GetConfigList();
            azureResourceCombo.SelectedIndex = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _configManager.PromptForNewConfig();
        }

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SnapshotDebugConfig config = azureResourceCombo.SelectedItem as SnapshotDebugConfig;
            if (config != null)
            {
                _configManager.VisitConfig(config);
            }
        }

        public void Dispose()
        {
            _configManager.ConfigListChanged -= ConfigManager_ConfigListChanged;
        }
    }
}
