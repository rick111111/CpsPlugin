using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopProjectDebug
{
    internal class SnapshotDebugConfigManager
    {
        public const string ConfigSettingKey = "SnapShotDebugConfigSetting";

        private MRUList<SnapshotDebugConfig> _configList = new MRUList<SnapshotDebugConfig>();

        public IReadOnlyCollection<SnapshotDebugConfig> GetConfigList()
        {
            return _configList.GetItems();
        }

        public void SaveConfigSettings(Stream stream)
        {

        }

        public void LoadConfigSettings(Stream stream)
        {

        }

        private class MRUList<T> where T : class
        {
            private const int MaxLength = 15;
            private List<T> _itemList = new List<T>();

            public IReadOnlyCollection<T> GetItems()
            {
                return _itemList;
            }

            public void VisitItem(T item)
            {
                _itemList.Remove(item);
                _itemList.Insert(0, item);

                while (_itemList.Count > MaxLength)
                {
                    _itemList.RemoveAt(_itemList.Count - 1);
                }
            }
        }
    }
}
