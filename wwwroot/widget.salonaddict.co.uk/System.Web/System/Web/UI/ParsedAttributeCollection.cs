namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Web.Util;

    internal sealed class ParsedAttributeCollection : IDictionary, ICollection, IEnumerable
    {
        private IDictionary _allFiltersDictionary;
        private IDictionary _filterTable = new ListDictionary(StringComparer.OrdinalIgnoreCase);

        internal ParsedAttributeCollection()
        {
        }

        public void AddFilteredAttribute(string filter, string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw ExceptionUtil.ParameterNullOrEmpty("name");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (filter == null)
            {
                filter = string.Empty;
            }
            if (this._allFiltersDictionary != null)
            {
                this._allFiltersDictionary.Add(Util.CreateFilteredName(filter, name), value);
            }
            FilteredAttributeDictionary dictionary = (FilteredAttributeDictionary) this._filterTable[filter];
            if (dictionary == null)
            {
                dictionary = new FilteredAttributeDictionary(this, filter);
                this._filterTable[filter] = dictionary;
            }
            dictionary.Data.Add(name, value);
        }

        public void ClearFilter(string filter)
        {
            if (filter == null)
            {
                filter = string.Empty;
            }
            if (this._allFiltersDictionary != null)
            {
                ArrayList list = new ArrayList();
                foreach (string str in this._allFiltersDictionary.Keys)
                {
                    string str2;
                    if (StringUtil.EqualsIgnoreCase(Util.ParsePropertyDeviceFilter(str, out str2), filter))
                    {
                        list.Add(str);
                    }
                }
                foreach (string str4 in list)
                {
                    this._allFiltersDictionary.Remove(str4);
                }
            }
            this._filterTable.Remove(filter);
        }

        public ICollection GetFilteredAttributeDictionaries() => 
            this._filterTable.Values;

        public void RemoveFilteredAttribute(string filter, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw ExceptionUtil.ParameterNullOrEmpty("name");
            }
            if (filter == null)
            {
                filter = string.Empty;
            }
            if (this._allFiltersDictionary != null)
            {
                this._allFiltersDictionary.Remove(Util.CreateFilteredName(filter, name));
            }
            FilteredAttributeDictionary dictionary = (FilteredAttributeDictionary) this._filterTable[filter];
            if (dictionary != null)
            {
                dictionary.Data.Remove(name);
            }
        }

        public void ReplaceFilteredAttribute(string filter, string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw ExceptionUtil.ParameterNullOrEmpty("name");
            }
            if (filter == null)
            {
                filter = string.Empty;
            }
            if (this._allFiltersDictionary != null)
            {
                this._allFiltersDictionary[Util.CreateFilteredName(filter, name)] = value;
            }
            FilteredAttributeDictionary dictionary = (FilteredAttributeDictionary) this._filterTable[filter];
            if (dictionary == null)
            {
                dictionary = new FilteredAttributeDictionary(this, filter);
                this._filterTable[filter] = dictionary;
            }
            dictionary.Data[name] = value;
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.AllFiltersDictionary.CopyTo(array, index);
        }

        void IDictionary.Add(object key, object value)
        {
            string str;
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (value == null)
            {
                value = string.Empty;
            }
            string filter = Util.ParsePropertyDeviceFilter(key.ToString(), out str);
            this.AddFilteredAttribute(filter, str, value.ToString());
        }

        void IDictionary.Clear()
        {
            this.AllFiltersDictionary.Clear();
            this._filterTable.Clear();
        }

        bool IDictionary.Contains(object key) => 
            this.AllFiltersDictionary.Contains(key);

        IDictionaryEnumerator IDictionary.GetEnumerator() => 
            this.AllFiltersDictionary.GetEnumerator();

        void IDictionary.Remove(object key)
        {
            string str;
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            string filter = Util.ParsePropertyDeviceFilter(key.ToString(), out str);
            this.RemoveFilteredAttribute(filter, str);
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.AllFiltersDictionary.GetEnumerator();

        private IDictionary AllFiltersDictionary
        {
            get
            {
                if (this._allFiltersDictionary == null)
                {
                    this._allFiltersDictionary = new ListDictionary(StringComparer.OrdinalIgnoreCase);
                    foreach (FilteredAttributeDictionary dictionary in this._filterTable.Values)
                    {
                        foreach (DictionaryEntry entry in (IEnumerable) dictionary)
                        {
                            string introduced6 = Util.CreateFilteredName(dictionary.Filter, entry.Key.ToString());
                            this._allFiltersDictionary[introduced6] = entry.Value;
                        }
                    }
                }
                return this._allFiltersDictionary;
            }
        }

        int ICollection.Count =>
            this.AllFiltersDictionary.Count;

        bool ICollection.IsSynchronized =>
            this.AllFiltersDictionary.IsSynchronized;

        object ICollection.SyncRoot =>
            this.AllFiltersDictionary.SyncRoot;

        bool IDictionary.IsFixedSize =>
            false;

        bool IDictionary.IsReadOnly =>
            false;

        object IDictionary.this[object key]
        {
            get => 
                this.AllFiltersDictionary[key];
            set
            {
                string str;
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                string filter = Util.ParsePropertyDeviceFilter(key.ToString(), out str);
                this.ReplaceFilteredAttribute(filter, str, value.ToString());
            }
        }

        ICollection IDictionary.Keys =>
            this.AllFiltersDictionary.Keys;

        ICollection IDictionary.Values =>
            this.AllFiltersDictionary.Values;
    }
}

