using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DesktopProjectDebug
{
    /// <summary>
    /// Interaction logic for PropertyPageControl.xaml
    /// </summary>
    public sealed partial class PropertyPageControl : UserControl, IDisposable
    {
        private PropertyPageViewModel _viewModel;
        private SnapshotDebugConfigManager _configManager;

        private Guid _projectGuid;
        public Guid ProjectGuid
        {
            get
            {
                return _projectGuid;
            }
            set
            {
                if (Assumes.Verify(value != null && value != Guid.Empty, "Invalid Project Guid"))
                {
                    _projectGuid = value;
                    _viewModel.SnapshotDebugConfigList = ProductionDebugPackage.DebugConfigManager.GetConfigList(_projectGuid);
                }
            }
        }

        public PropertyPageControl(SnapshotDebugConfigManager configManager)
        {
            Assumes.ThrowIfNull(configManager, nameof(configManager));

            InitializeComponent();

            _configManager = configManager;
            _configManager.ConfigListChanged += ConfigManager_ConfigListChanged; ;

            _viewModel = new PropertyPageViewModel();
            this.DataContext = _viewModel;
        }

        private void ConfigManager_ConfigListChanged(object sender, IEnumerable<Guid> guids)
        {
            if (Assumes.Verify(ProjectGuid != null && ProjectGuid != Guid.Empty, "Invalid Project Guid"))
            {
                if (guids != null && guids.Contains(ProjectGuid))
                {
                    _viewModel.SnapshotDebugConfigList = ProductionDebugPackage.DebugConfigManager.GetConfigList(ProjectGuid);
                    configCombo.SelectedIndex = 0;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Assumes.Verify(ProjectGuid != null && ProjectGuid != Guid.Empty, "Invalid Project Guid"))
            {
                _configManager.PromptForNewConfig(ProjectGuid);
            }
        }

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Assumes.Verify(ProjectGuid != null && ProjectGuid != Guid.Empty, "Invalid Project Guid"))
            {
                SnapshotDebugConfig config = configCombo.SelectedItem as SnapshotDebugConfig;
                if (config != null)
                {
                    _configManager.VisitConfig(ProjectGuid, config);
                }
            }
        }

        public void Dispose()
        {
            _configManager.ConfigListChanged -= ConfigManager_ConfigListChanged;
        }
    }
}
