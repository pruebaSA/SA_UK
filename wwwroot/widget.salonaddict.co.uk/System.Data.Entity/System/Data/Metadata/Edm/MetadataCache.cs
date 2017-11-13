namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.QueryCache;
    using System.Data.Entity;
    using System.Data.Mapping;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Threading;

    internal static class MetadataCache
    {
        private static readonly Dictionary<string, EdmMetadataEntry> _edmLevelCache = new Dictionary<string, EdmMetadataEntry>(StringComparer.OrdinalIgnoreCase);
        private static readonly object _edmLevelLock = new object();
        private static readonly List<StoreMetadataEntry> _metadataEntriesRemovedFromCache = new List<StoreMetadataEntry>();
        private static readonly Dictionary<string, StoreMetadataEntry> _storeLevelCache = new Dictionary<string, StoreMetadataEntry>(StringComparer.OrdinalIgnoreCase);
        private static readonly object _storeLevelLock = new object();
        private const int cleanupPeriod = 0x493e0;
        private static Timer timer = new Timer(new TimerCallback(MetadataCache.PeriodicCleanupCallback), null, 0x493e0, 0x493e0);

        internal static void Clear()
        {
            lock (_edmLevelLock)
            {
                _edmLevelCache.Clear();
            }
            lock (_storeLevelLock)
            {
                foreach (StoreMetadataEntry entry in _storeLevelCache.Values)
                {
                    if (entry.IsEntryStillValid())
                    {
                        _metadataEntriesRemovedFromCache.Add(entry);
                    }
                    else
                    {
                        entry.Clear();
                    }
                }
                _storeLevelCache.Clear();
            }
        }

        private static void DoCacheClean<T>(Dictionary<string, T> cache, object objectToLock) where T: MetadataEntry
        {
            if (cache != null)
            {
                List<KeyValuePair<string, T>> list = null;
                lock (objectToLock)
                {
                    if ((objectToLock == _storeLevelLock) && (_metadataEntriesRemovedFromCache.Count != 0))
                    {
                        for (int i = _metadataEntriesRemovedFromCache.Count - 1; 0 <= i; i--)
                        {
                            if (!_metadataEntriesRemovedFromCache[i].IsEntryStillValid())
                            {
                                _metadataEntriesRemovedFromCache[i].CleanupQueryCache();
                                _metadataEntriesRemovedFromCache.RemoveAt(i);
                            }
                        }
                    }
                    foreach (KeyValuePair<string, T> pair in cache)
                    {
                        if (pair.Value.PeriodicCleanUpThread())
                        {
                            if (list == null)
                            {
                                list = new List<KeyValuePair<string, T>>();
                            }
                            list.Add(pair);
                        }
                    }
                    if (list != null)
                    {
                        for (int j = 0; j < list.Count; j++)
                        {
                            KeyValuePair<string, T> pair2 = list[j];
                            pair2.Value.Clear();
                            KeyValuePair<string, T> pair3 = list[j];
                            cache.Remove(pair3.Key);
                        }
                    }
                }
            }
        }

        private static T GetCacheEntry<T>(Dictionary<string, T> cache, string cacheKey, object objectToLock, IMetadataEntryConstructor<T> metadataEntry, out object entryToken) where T: MetadataEntry
        {
            T local;
            lock (objectToLock)
            {
                if (cache.TryGetValue(cacheKey, out local))
                {
                    entryToken = local.EnsureToken();
                    return local;
                }
                local = metadataEntry.GetMetadataEntry();
                entryToken = local.EnsureToken();
                cache.Add(cacheKey, local);
            }
            return local;
        }

        internal static EdmItemCollection GetOrCreateEdmItemCollection(string cacheKey, MetadataArtifactLoader loader, out object entryToken)
        {
            EdmMetadataEntry entry = GetCacheEntry<EdmMetadataEntry>(_edmLevelCache, cacheKey, _edmLevelLock, new EdmMetadataEntryConstructor(), out entryToken);
            LoadItemCollection<EdmMetadataEntry>(new EdmItemCollectionLoader(loader), entry);
            return entry.EdmItemCollection;
        }

        internal static StorageMappingItemCollection GetOrCreateStoreAndMappingItemCollections(string cacheKey, MetadataArtifactLoader loader, EdmItemCollection edmItemCollection, out object entryToken)
        {
            StoreMetadataEntry entry = GetCacheEntry<StoreMetadataEntry>(_storeLevelCache, cacheKey, _storeLevelLock, new StoreMetadataEntryConstructor(), out entryToken);
            LoadItemCollection<StoreMetadataEntry>(new StoreItemCollectionLoader(edmItemCollection, loader), entry);
            return entry.StorageMappingItemCollection;
        }

        private static void LoadItemCollection<T>(IItemCollectionLoader<T> itemCollectionLoader, T entry) where T: MetadataEntry
        {
            bool flag = true;
            if (!entry.IsLoaded)
            {
                lock (entry)
                {
                    if (!entry.IsLoaded)
                    {
                        itemCollectionLoader.LoadItemCollection(entry);
                        flag = false;
                    }
                }
            }
            if (flag)
            {
                entry.CheckFilePermission();
            }
        }

        private static void PeriodicCleanupCallback(object state)
        {
            DoCacheClean<EdmMetadataEntry>(_edmLevelCache, _edmLevelLock);
            DoCacheClean<StoreMetadataEntry>(_storeLevelCache, _storeLevelLock);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct EdmItemCollectionLoader : MetadataCache.IItemCollectionLoader<MetadataCache.EdmMetadataEntry>
        {
            private MetadataArtifactLoader _loader;
            public EdmItemCollectionLoader(MetadataArtifactLoader loader)
            {
                this._loader = loader;
            }

            public void LoadItemCollection(MetadataCache.EdmMetadataEntry entry)
            {
                entry.LoadEdmItemCollection(this._loader);
            }
        }

        private class EdmMetadataEntry : MetadataCache.MetadataEntry
        {
            internal void LoadEdmItemCollection(MetadataArtifactLoader loader)
            {
                List<XmlReader> xmlReaders = loader.CreateReaders(DataSpace.CSpace);
                try
                {
                    System.Data.Metadata.Edm.EdmItemCollection itemCollection = new System.Data.Metadata.Edm.EdmItemCollection(xmlReaders, loader.GetPaths(DataSpace.CSpace));
                    List<string> paths = new List<string>();
                    loader.CollectFilePermissionPaths(paths, DataSpace.CSpace);
                    FileIOPermission filePermissions = null;
                    if (paths.Count > 0)
                    {
                        filePermissions = new FileIOPermission(FileIOPermissionAccess.Read, paths.ToArray());
                    }
                    base.UpdateMetadataEntry(itemCollection, filePermissions);
                }
                finally
                {
                    Helper.DisposeXmlReaders(xmlReaders);
                }
            }

            internal System.Data.Metadata.Edm.EdmItemCollection EdmItemCollection =>
                ((System.Data.Metadata.Edm.EdmItemCollection) base.ItemCollection);
        }

        [StructLayout(LayoutKind.Sequential, Size=1)]
        private struct EdmMetadataEntryConstructor : MetadataCache.IMetadataEntryConstructor<MetadataCache.EdmMetadataEntry>
        {
            public MetadataCache.EdmMetadataEntry GetMetadataEntry() => 
                new MetadataCache.EdmMetadataEntry();
        }

        private interface IItemCollectionLoader<T> where T: MetadataCache.MetadataEntry
        {
            void LoadItemCollection(T entry);
        }

        private interface IMetadataEntryConstructor<T>
        {
            T GetMetadataEntry();
        }

        private abstract class MetadataEntry
        {
            private WeakReference _entryTokenReference = new WeakReference(null);
            private FileIOPermission _filePermissions;
            private System.Data.Metadata.Edm.ItemCollection _itemCollection;
            private bool _markEntryForCleanup;
            private WeakReference _weakReferenceItemCollection = new WeakReference(null);

            internal MetadataEntry()
            {
            }

            internal void CheckFilePermission()
            {
                if (this._filePermissions != null)
                {
                    this._filePermissions.Demand();
                }
            }

            internal virtual void Clear()
            {
            }

            internal object EnsureToken()
            {
                object target = this._entryTokenReference.Target;
                System.Data.Metadata.Edm.ItemCollection items = (System.Data.Metadata.Edm.ItemCollection) this._weakReferenceItemCollection.Target;
                if (!this._entryTokenReference.IsAlive)
                {
                    if (this._itemCollection == null)
                    {
                        if (this._weakReferenceItemCollection.IsAlive)
                        {
                            this._itemCollection = items;
                        }
                        else
                        {
                            this._filePermissions = null;
                        }
                    }
                    target = new object();
                    this._entryTokenReference.Target = target;
                    this._markEntryForCleanup = false;
                }
                return target;
            }

            internal bool IsEntryStillValid() => 
                this._entryTokenReference.IsAlive;

            internal bool PeriodicCleanUpThread()
            {
                if (this._markEntryForCleanup)
                {
                    if (this._itemCollection == null)
                    {
                        if (!this._weakReferenceItemCollection.IsAlive)
                        {
                            this._filePermissions = null;
                            return true;
                        }
                    }
                    else
                    {
                        this._itemCollection = null;
                    }
                }
                else if (!this._entryTokenReference.IsAlive)
                {
                    this._markEntryForCleanup = true;
                }
                return false;
            }

            protected void UpdateMetadataEntry(System.Data.Metadata.Edm.ItemCollection itemCollection, FileIOPermission filePermissions)
            {
                this._weakReferenceItemCollection.Target = itemCollection;
                this._filePermissions = filePermissions;
                this._itemCollection = itemCollection;
            }

            internal bool IsLoaded =>
                (this._itemCollection != null);

            protected System.Data.Metadata.Edm.ItemCollection ItemCollection =>
                this._itemCollection;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct StoreItemCollectionLoader : MetadataCache.IItemCollectionLoader<MetadataCache.StoreMetadataEntry>
        {
            private EdmItemCollection _edmItemCollection;
            private MetadataArtifactLoader _loader;
            internal StoreItemCollectionLoader(EdmItemCollection edmItemCollection, MetadataArtifactLoader loader)
            {
                if ((loader.GetPaths(DataSpace.SSpace) == null) || (loader.GetPaths(DataSpace.SSpace).Count == 0))
                {
                    throw EntityUtil.Metadata(Strings.AtleastOneSSDLNeeded);
                }
                this._edmItemCollection = edmItemCollection;
                this._loader = loader;
            }

            public void LoadItemCollection(MetadataCache.StoreMetadataEntry entry)
            {
                entry.LoadStoreCollection(this._edmItemCollection, this._loader);
            }
        }

        private class StoreMetadataEntry : MetadataCache.MetadataEntry
        {
            private QueryCacheManager _queryCacheManager;

            internal StoreMetadataEntry()
            {
            }

            internal void CleanupQueryCache()
            {
                if (this._queryCacheManager != null)
                {
                    this._queryCacheManager.Dispose();
                    this._queryCacheManager = null;
                }
            }

            internal override void Clear()
            {
                this.CleanupQueryCache();
                base.Clear();
            }

            internal void LoadStoreCollection(EdmItemCollection edmItemCollection, MetadataArtifactLoader loader)
            {
                StoreItemCollection storeCollection = null;
                IEnumerable<XmlReader> xmlReaders = loader.CreateReaders(DataSpace.SSpace);
                try
                {
                    storeCollection = new StoreItemCollection(xmlReaders, loader.GetPaths(DataSpace.SSpace));
                }
                finally
                {
                    Helper.DisposeXmlReaders(xmlReaders);
                }
                if (this._queryCacheManager != null)
                {
                    this._queryCacheManager.Clear();
                }
                this._queryCacheManager = storeCollection.QueryCacheManager;
                System.Data.Mapping.StorageMappingItemCollection itemCollection = null;
                IEnumerable<XmlReader> enumerable2 = loader.CreateReaders(DataSpace.CSSpace);
                try
                {
                    itemCollection = new System.Data.Mapping.StorageMappingItemCollection(edmItemCollection, storeCollection, enumerable2, loader.GetPaths(DataSpace.CSSpace));
                }
                finally
                {
                    Helper.DisposeXmlReaders(enumerable2);
                }
                List<string> paths = new List<string>();
                loader.CollectFilePermissionPaths(paths, DataSpace.SSpace);
                loader.CollectFilePermissionPaths(paths, DataSpace.CSSpace);
                FileIOPermission filePermissions = null;
                if (paths.Count > 0)
                {
                    filePermissions = new FileIOPermission(FileIOPermissionAccess.Read, paths.ToArray());
                }
                base.UpdateMetadataEntry(itemCollection, filePermissions);
            }

            internal System.Data.Mapping.StorageMappingItemCollection StorageMappingItemCollection =>
                ((System.Data.Mapping.StorageMappingItemCollection) base.ItemCollection);
        }

        [StructLayout(LayoutKind.Sequential, Size=1)]
        private struct StoreMetadataEntryConstructor : MetadataCache.IMetadataEntryConstructor<MetadataCache.StoreMetadataEntry>
        {
            public MetadataCache.StoreMetadataEntry GetMetadataEntry() => 
                new MetadataCache.StoreMetadataEntry();
        }
    }
}

