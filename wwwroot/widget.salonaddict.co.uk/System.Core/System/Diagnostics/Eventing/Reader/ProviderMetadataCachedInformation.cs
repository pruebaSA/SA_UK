namespace System.Diagnostics.Eventing.Reader
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Security;

    internal class ProviderMetadataCachedInformation
    {
        private Dictionary<ProviderMetadataId, CacheItem> cache;
        private string logfile;
        private int maximumCacheSize;
        private EventLogSession session;

        public ProviderMetadataCachedInformation(EventLogSession session, string logfile, int maximumCacheSize)
        {
            this.session = session;
            this.logfile = logfile;
            this.cache = new Dictionary<ProviderMetadataId, CacheItem>();
            this.maximumCacheSize = maximumCacheSize;
        }

        private void AddCacheEntry(ProviderMetadataId key, ProviderMetadata pm)
        {
            if (this.IsCacheFull())
            {
                this.FlushOldestEntry();
            }
            CacheItem item = new CacheItem(pm);
            this.cache.Add(key, item);
        }

        private void DeleteCacheEntry(ProviderMetadataId key)
        {
            if (this.IsProviderinCache(key))
            {
                CacheItem item = this.cache[key];
                this.cache.Remove(key);
                item.ProviderMetadata.Dispose();
            }
        }

        private void FlushOldestEntry()
        {
            double totalMilliseconds = -10.0;
            DateTime now = DateTime.Now;
            ProviderMetadataId key = null;
            foreach (KeyValuePair<ProviderMetadataId, CacheItem> pair in this.cache)
            {
                TimeSpan span = now.Subtract(pair.Value.TheTime);
                if (span.TotalMilliseconds >= totalMilliseconds)
                {
                    totalMilliseconds = span.TotalMilliseconds;
                    key = pair.Key;
                }
            }
            if (key != null)
            {
                this.DeleteCacheEntry(key);
            }
        }

        [SecurityTreatAsSafe]
        public string GetFormatDescription(string ProviderName, EventLogHandle eventHandle)
        {
            string str;
            lock (this)
            {
                ProviderMetadataId key = new ProviderMetadataId(ProviderName, CultureInfo.CurrentCulture);
                try
                {
                    str = NativeWrapper.EvtFormatMessageRenderName(this.GetProviderMetadata(key).Handle, eventHandle, Microsoft.Win32.UnsafeNativeMethods.EvtFormatMessageFlags.EvtFormatMessageEvent);
                }
                catch (EventLogNotFoundException)
                {
                    str = null;
                }
            }
            return str;
        }

        public string GetFormatDescription(string ProviderName, EventLogHandle eventHandle, string[] values)
        {
            string str;
            lock (this)
            {
                ProviderMetadataId key = new ProviderMetadataId(ProviderName, CultureInfo.CurrentCulture);
                ProviderMetadata providerMetadata = this.GetProviderMetadata(key);
                try
                {
                    str = NativeWrapper.EvtFormatMessageFormatDescription(providerMetadata.Handle, eventHandle, values);
                }
                catch (EventLogNotFoundException)
                {
                    str = null;
                }
            }
            return str;
        }

        [SecurityTreatAsSafe]
        public IEnumerable<string> GetKeywordDisplayNames(string ProviderName, [SecurityTreatAsSafe] EventLogHandle eventHandle)
        {
            lock (this)
            {
                ProviderMetadataId key = new ProviderMetadataId(ProviderName, CultureInfo.CurrentCulture);
                return NativeWrapper.EvtFormatMessageRenderKeywords(this.GetProviderMetadata(key).Handle, eventHandle, Microsoft.Win32.UnsafeNativeMethods.EvtFormatMessageFlags.EvtFormatMessageKeyword);
            }
        }

        [SecurityTreatAsSafe]
        public string GetLevelDisplayName(string ProviderName, [SecurityTreatAsSafe] EventLogHandle eventHandle)
        {
            lock (this)
            {
                ProviderMetadataId key = new ProviderMetadataId(ProviderName, CultureInfo.CurrentCulture);
                return NativeWrapper.EvtFormatMessageRenderName(this.GetProviderMetadata(key).Handle, eventHandle, Microsoft.Win32.UnsafeNativeMethods.EvtFormatMessageFlags.EvtFormatMessageLevel);
            }
        }

        [SecurityTreatAsSafe]
        public string GetOpcodeDisplayName(string ProviderName, [SecurityTreatAsSafe] EventLogHandle eventHandle)
        {
            lock (this)
            {
                ProviderMetadataId key = new ProviderMetadataId(ProviderName, CultureInfo.CurrentCulture);
                return NativeWrapper.EvtFormatMessageRenderName(this.GetProviderMetadata(key).Handle, eventHandle, Microsoft.Win32.UnsafeNativeMethods.EvtFormatMessageFlags.EvtFormatMessageOpcode);
            }
        }

        private ProviderMetadata GetProviderMetadata(ProviderMetadataId key)
        {
            if (!this.IsProviderinCache(key))
            {
                ProviderMetadata metadata;
                try
                {
                    metadata = new ProviderMetadata(key.ProviderName, this.session, key.TheCultureInfo, this.logfile);
                }
                catch (EventLogNotFoundException)
                {
                    metadata = new ProviderMetadata(key.ProviderName, this.session, key.TheCultureInfo);
                }
                this.AddCacheEntry(key, metadata);
                return metadata;
            }
            CacheItem cacheItem = this.cache[key];
            ProviderMetadata providerMetadata = cacheItem.ProviderMetadata;
            try
            {
                providerMetadata.CheckReleased();
                UpdateCacheValueInfoForHit(cacheItem);
            }
            catch (EventLogException)
            {
                this.DeleteCacheEntry(key);
                try
                {
                    providerMetadata = new ProviderMetadata(key.ProviderName, this.session, key.TheCultureInfo, this.logfile);
                }
                catch (EventLogNotFoundException)
                {
                    providerMetadata = new ProviderMetadata(key.ProviderName, this.session, key.TheCultureInfo);
                }
                this.AddCacheEntry(key, providerMetadata);
            }
            return providerMetadata;
        }

        [SecurityTreatAsSafe]
        public string GetTaskDisplayName(string ProviderName, [SecurityTreatAsSafe] EventLogHandle eventHandle)
        {
            lock (this)
            {
                ProviderMetadataId key = new ProviderMetadataId(ProviderName, CultureInfo.CurrentCulture);
                return NativeWrapper.EvtFormatMessageRenderName(this.GetProviderMetadata(key).Handle, eventHandle, Microsoft.Win32.UnsafeNativeMethods.EvtFormatMessageFlags.EvtFormatMessageTask);
            }
        }

        private bool IsCacheFull() => 
            (this.cache.Count == this.maximumCacheSize);

        private bool IsProviderinCache(ProviderMetadataId key) => 
            this.cache.ContainsKey(key);

        private static void UpdateCacheValueInfoForHit(CacheItem cacheItem)
        {
            cacheItem.TheTime = DateTime.Now;
        }

        private class CacheItem
        {
            private System.Diagnostics.Eventing.Reader.ProviderMetadata pm;
            private DateTime theTime;

            public CacheItem(System.Diagnostics.Eventing.Reader.ProviderMetadata pm)
            {
                this.pm = pm;
                this.theTime = DateTime.Now;
            }

            public System.Diagnostics.Eventing.Reader.ProviderMetadata ProviderMetadata =>
                this.pm;

            public DateTime TheTime
            {
                get => 
                    this.theTime;
                set
                {
                    this.theTime = value;
                }
            }
        }

        private class ProviderMetadataId
        {
            private CultureInfo cultureInfo;
            private string providerName;

            public ProviderMetadataId(string providerName, CultureInfo cultureInfo)
            {
                this.providerName = providerName;
                this.cultureInfo = cultureInfo;
            }

            public override bool Equals(object obj)
            {
                ProviderMetadataCachedInformation.ProviderMetadataId id = obj as ProviderMetadataCachedInformation.ProviderMetadataId;
                if (id == null)
                {
                    return false;
                }
                return (this.providerName.Equals(id.providerName) && (this.cultureInfo == id.cultureInfo));
            }

            public override int GetHashCode() => 
                (this.providerName.GetHashCode() ^ this.cultureInfo.GetHashCode());

            public string ProviderName =>
                this.providerName;

            public CultureInfo TheCultureInfo =>
                this.cultureInfo;
        }
    }
}

