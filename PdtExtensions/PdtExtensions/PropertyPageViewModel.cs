using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Debugger.Parallel.Extension
{
    public class PropertyPageViewModel : ViewModelBase
    {
        public PropertyPageViewModel(SnapshotDebugConfig config)
        {
            SetConfiguration(config);
        }

        public void SetConfiguration(SnapshotDebugConfig config)
        {
            if (config != null)
            {
                AppServiceName = config.WebsiteName;
            }
            else
            {
                AppServiceName = Resources.AppServiceSelectionHyperlink;
            }
        }

        private string _appServiceName;
        public string AppServiceName
        {
            get
            {
                return _appServiceName;
            }
            set
            {
                ChangePropertyValue(ref _appServiceName, value);
            }
        }
    }
}
