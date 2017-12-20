using System.Collections.Generic;

namespace DesktopProjectDebug
{
    public class PropertyPageViewModel : ViewModelBase
    {
        private IEnumerable<SnapshotDebugConfig> _snapshotDebugConfigList;
        public IEnumerable<SnapshotDebugConfig> SnapshotDebugConfigList
        {
            get
            {
                return _snapshotDebugConfigList;
            }
            set
            {
                _snapshotDebugConfigList = value;
                OnPropertyChanged();
            }
        }
    }
}
