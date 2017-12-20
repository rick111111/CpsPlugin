using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace DesktopProjectDebug
{
    public sealed class SnapshotDebugConfigManager
    {
        private MRUList<SnapshotDebugConfig> _configList = new MRUList<SnapshotDebugConfig>();

        public event EventHandler ConfigListChanged;

        public IReadOnlyCollection<SnapshotDebugConfig> GetConfigList()
        {
            return _configList.GetItems();
        }

        public void PromptForNewConfig()
        {
            SnapshotDebugConfig config = new SnapshotDebugConfig();
            SnapshotDebugConfigDialog dialog = new SnapshotDebugConfigDialog(config);
            dialog.ShowDialog();

            if (dialog.Result)
            {
                VisitConfig(config);
            }
        }

        public void VisitConfig(SnapshotDebugConfig config)
        {
            _configList.VisitItem(config);
            ConfigListChanged?.Invoke(this, EventArgs.Empty);
        }

        public bool SaveConfigSettings(Stream stream)
        {
            Assumes.ThrowIfNull(stream, nameof(stream));

            SnapshotDebugUserSettings userSetting = new SnapshotDebugUserSettings(GetConfigList());
            if (userSetting.SnapshotDebugConfigList.Count() == 0)
            {
                return false;
            }

            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                formatter.Serialize(stream, userSetting);
            }
            catch (SerializationException e)
            {
                Assumes.Fail(e.ToString());
                return false;
            }

            return true;
        }

        public bool LoadConfigSettings(Stream stream)
        {
            Assumes.ThrowIfNull(stream, nameof(stream));

            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                if (stream.Length > 0)
                {
                    SnapshotDebugUserSettings userSettings = formatter.Deserialize(stream) as SnapshotDebugUserSettings;
                    if (SnapshotDebugUserSettings.VersionMatch(userSettings.Version))
                    {
                        if (userSettings.SnapshotDebugConfigList != null && userSettings.SnapshotDebugConfigList.Count() > 0)
                        {
                            _configList.ResetItems(userSettings.SnapshotDebugConfigList.ToList());
                            ConfigListChanged?.Invoke(this, EventArgs.Empty);
                        }

                        return true;
                    }
                }
            }
            catch (SerializationException e)
            {
                Assumes.Fail(e.ToString());
            }

            return false;
        }

        private sealed class MRUList<T> where T : class
        {
            private const int MaxLength = 15;
            private List<T> _itemList = new List<T>();

            public void ResetItems(List<T> itemList)
            {
                Assumes.ThrowIfNull(itemList, nameof(itemList));

                _itemList = itemList;
            }

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
