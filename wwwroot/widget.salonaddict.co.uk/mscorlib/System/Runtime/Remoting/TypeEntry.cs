namespace System.Runtime.Remoting
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public class TypeEntry
    {
        private string _assemblyName;
        private RemoteAppEntry _cachedRemoteAppEntry;
        private string _typeName;

        protected TypeEntry()
        {
        }

        internal void CacheRemoteAppEntry(RemoteAppEntry entry)
        {
            this._cachedRemoteAppEntry = entry;
        }

        internal RemoteAppEntry GetRemoteAppEntry() => 
            this._cachedRemoteAppEntry;

        public string AssemblyName
        {
            get => 
                this._assemblyName;
            set
            {
                this._assemblyName = value;
            }
        }

        public string TypeName
        {
            get => 
                this._typeName;
            set
            {
                this._typeName = value;
            }
        }
    }
}

