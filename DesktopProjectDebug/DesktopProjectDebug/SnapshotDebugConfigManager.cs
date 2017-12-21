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
        /// <summary>
        /// Maintains one MRU list per project
        /// </summary>
        private Dictionary<Guid, MRUList<SnapshotDebugConfig>> _mruDictionary = new Dictionary<Guid, MRUList<SnapshotDebugConfig>>();

        public event EventHandler<IEnumerable<Guid>> ConfigListChanged;

        public IReadOnlyCollection<SnapshotDebugConfig> GetConfigList(Guid projectGuid)
        {
            return GetMRUList(projectGuid)?.GetItems() ?? new List<SnapshotDebugConfig>();
        }

        public SnapshotDebugConfig PromptForNewConfig(Guid projectGuid)
        {
            SnapshotDebugConfig config = new SnapshotDebugConfig();
            SnapshotDebugConfigDialog dialog = new SnapshotDebugConfigDialog(config);
            if (dialog.ShowDialog() == true)
            {
                VisitConfig(projectGuid, config);
            }

            return config;
        }

        /// <summary>
        /// Update MRU list when user visit a configuration or created a new configuration
        /// </summary>
        public void VisitConfig(Guid projectGuid, SnapshotDebugConfig config)
        {
            MRUList<SnapshotDebugConfig> mruList = GetMRUList(projectGuid);
            mruList.VisitItem(config);

            ConfigListChanged?.Invoke(this, new Guid[] { projectGuid });
        }

        private MRUList<SnapshotDebugConfig> GetMRUList(Guid projectGuid)
        {
            MRUList<SnapshotDebugConfig> mruList;
            if (!_mruDictionary.TryGetValue(projectGuid, out mruList))
            {
                mruList = new MRUList<SnapshotDebugConfig>();
                _mruDictionary[projectGuid] = mruList;
            }

            return mruList;
        }

        /// <summary>
        /// Save snapshot debug setting into stream
        /// </summary>
        public bool SaveConfigSettings(Stream stream)
        {
            Assumes.ThrowIfNull(stream, nameof(stream));

            Dictionary<Guid, IEnumerable<SnapshotDebugConfig>> configs = _mruDictionary.
                ToDictionary(p => p.Key, p => p.Value.GetItems() as IEnumerable<SnapshotDebugConfig>);

            SnapshotDebugUserSettings userSetting = new SnapshotDebugUserSettings(configs);
            if (userSetting.DebugConfigs.Count() == 0)
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

        /// <summary>
        /// Load snapshot debug setting from stream
        /// </summary>
        public bool LoadConfigSettings(Stream stream)
        {
            Assumes.ThrowIfNull(stream, nameof(stream));

            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                if (stream.Length > 0)
                {
                    SnapshotDebugUserSettings userSettings = formatter.Deserialize(stream) as SnapshotDebugUserSettings;
                    if (SnapshotDebugUserSettings.VersionMatch(userSettings?.Version))
                    {
                        if (userSettings.DebugConfigs?.Count() > 0)
                        {
                            _mruDictionary = userSettings.DebugConfigs.ToDictionary(p => p.Key, p => new MRUList<SnapshotDebugConfig>(p.Value.ToList()));

                            // Guid.empty means all MRU lists are changed
                            ConfigListChanged?.Invoke(this, _mruDictionary.Select(p => p.Key));
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

        /// <summary>
        /// Basic MRU list implementation
        /// </summary>
        private sealed class MRUList<T> where T : class
        {
            private const int MaxLength = 10;
            private List<T> _itemList;
            private object _lockObject = new object();

            public MRUList()
            {
                _itemList = new List<T>();
            }

            public MRUList(List<T> lt)
            {
                _itemList = lt;
            }

            public IReadOnlyCollection<T> GetItems()
            {
                lock (_lockObject)
                {
                    return _itemList;
                }
            }

            public void VisitItem(T item)
            {
                lock (_lockObject)
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
}
