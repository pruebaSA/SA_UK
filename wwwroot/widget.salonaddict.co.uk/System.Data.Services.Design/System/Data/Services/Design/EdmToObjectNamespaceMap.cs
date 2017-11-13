namespace System.Data.Services.Design
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Design.Common;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public class EdmToObjectNamespaceMap
    {
        private Dictionary<string, string> _map = new Dictionary<string, string>();

        internal EdmToObjectNamespaceMap()
        {
        }

        public void Add(string edmNamespace, string objectNamespace)
        {
            EDesignUtil.CheckStringArgument(edmNamespace, "edmNamespace");
            EDesignUtil.CheckArgumentNull<string>(objectNamespace, "objectNamespace");
            this._map.Add(edmNamespace, objectNamespace);
        }

        public void Clear()
        {
            this._map.Clear();
        }

        public bool Contains(string edmNamespace) => 
            this._map.ContainsKey(edmNamespace);

        public bool Remove(string edmNamespace) => 
            this._map.Remove(edmNamespace);

        public bool TryGetObjectNamespace(string edmNamespace, out string objectNamespace) => 
            this._map.TryGetValue(edmNamespace, out objectNamespace);

        public int Count =>
            this._map.Count;

        public ICollection<string> EdmNamespaces =>
            this._map.Keys;

        public string this[string edmNamespace]
        {
            get => 
                this._map[edmNamespace];
            set
            {
                this._map[edmNamespace] = value;
            }
        }
    }
}

