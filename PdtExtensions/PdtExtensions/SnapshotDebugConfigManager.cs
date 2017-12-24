using Microsoft.VisualStudio.Web.WindowsAzure.CommonContracts;
using Microsoft.VisualStudio.WindowsAzure.MicrosoftWeb;
using Microsoft.WindowsAzure.Client.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace Microsoft.VisualStudio.Debugger.Parallel.Extension
{
    [Export(typeof(ISnapshotDebugConfigManager))]
    public sealed class SnapshotDebugConfigManager : ISnapshotDebugConfigManager
    {
        [Import]
        private Lazy<IAzureServiceSelectionDialogFactory> _azureServiceSelectionDialogFactory = null;

        /// <summary>
        /// A dictionary of debug configurations using project Guid as key
        /// </summary>
        private Dictionary<Guid, SnapshotDebugConfig> _configDictionary = new Dictionary<Guid, SnapshotDebugConfig>();

        public event EventHandler<Guid> ConfigurationChanged;

        public SnapshotDebugConfig GetConfiguration(Guid projectGuid)
        {
            if (_configDictionary.ContainsKey(projectGuid))
            {
                return _configDictionary[projectGuid];
            }
            else
            {
                return null;
            }
        }

        public SnapshotDebugConfig EnsureConfigurationExist(Guid projectGuid)
        {
            SnapshotDebugConfig config = GetConfiguration(projectGuid);
            if (config == null)
            {
                config = PromptForConfiguration(projectGuid);
                if (config != null)
                {
                    _configDictionary[projectGuid] = config;
                    ConfigurationChanged?.Invoke(this, projectGuid);
                }
            }

            return config;
        }

        private SnapshotDebugConfig PromptForConfiguration(Guid projectGuid)
        {
            IAzureServiceSelectionDialog selectionDialog = _azureServiceSelectionDialogFactory.Value?.CreateDialog(AppServiceAssetSelectionParameters.Default,
                new AzureServiceDescription()
                {
                    Title = Resources.AppServiceDialogTitle,
                    Description = Resources.AppServiceDialogDescription
                });

            if (selectionDialog?.ShowModal() == true)
            {
                IEntity entity = selectionDialog.Result;
                return new SnapshotDebugConfig()
                {
                    Subscription = entity.ToString(),
                    ResourceId = entity.Id,
                    WebsiteName = entity.Name
                };
            }

            return null;
        }

        private sealed class AzureServiceDescription : IAzureServiceDescription
        {
            public string Title { get; set; }

            public string Description { get; set; }

            public ImageSource Icon24Square { get; set; } = null;

            public ImageSource Icon48Square { get; set; } = null;
        }
    }
}
