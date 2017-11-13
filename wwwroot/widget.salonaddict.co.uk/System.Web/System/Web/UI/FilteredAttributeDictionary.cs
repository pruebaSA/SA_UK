namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Web;

    internal sealed class FilteredAttributeDictionary : IDictionary, ICollection, IEnumerable
    {
        private IDictionary _data;
        private string _filter;
        private ParsedAttributeCollection _owner;

        internal FilteredAttributeDictionary(ParsedAttributeCollection owner, string filter)
        {
            this._filter = filter;
            this._owner = owner;
            this._data = new ListDictionary(StringComparer.OrdinalIgnoreCase);
        }

        public void Add(string key, string value)
        {
            this._owner.AddFilteredAttribute(this._filter, key, value);
        }

        public void Clear()
        {
            this._owner.ClearFilter(this._filter);
        }

        public bool Contains(string key) => 
            this._data.Contains(key);

        public void Remove(string key)
        {
            this._owner.RemoveFilteredAttribute(this._filter, key);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this._data.CopyTo(array, index);
        }

        void IDictionary.Add(object key, object value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (!(key is string))
            {
                throw new ArgumentException(System.Web.SR.GetString("FilteredAttributeDictionary_ArgumentMustBeString"), "key");
            }
            if (!(value is string))
            {
                throw new ArgumentException(System.Web.SR.GetString("FilteredAttributeDictionary_ArgumentMustBeString"), "value");
            }
            if (value == null)
            {
                value = string.Empty;
            }
            this.Add(key.ToString(), value.ToString());
        }

        void IDictionary.Clear()
        {
            this.Clear();
        }

        bool IDictionary.Contains(object key)
        {
            if (!(key is string))
            {
                throw new ArgumentException(System.Web.SR.GetString("FilteredAttributeDictionary_ArgumentMustBeString"), "key");
            }
            return this.Contains(key.ToString());
        }

        IDictionaryEnumerator IDictionary.GetEnumerator() => 
            this._data.GetEnumerator();

        void IDictionary.Remove(object key)
        {
            this.Remove(key.ToString());
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this._data.GetEnumerator();

        internal IDictionary Data =>
            this._data;

        public string Filter =>
            this._filter;

        public string this[string key]
        {
            get => 
                ((string) this._data[key]);
            set
            {
                this._owner.ReplaceFilteredAttribute(this._filter, key, value);
            }
        }

        int ICollection.Count =>
            this._data.Count;

        bool ICollection.IsSynchronized =>
            this._data.IsSynchronized;

        object ICollection.SyncRoot =>
            this._data.SyncRoot;

        bool IDictionary.IsFixedSize =>
            false;

        bool IDictionary.IsReadOnly =>
            false;

        object IDictionary.this[object key]
        {
            get
            {
                if (!(key is string))
                {
                    throw new ArgumentException(System.Web.SR.GetString("FilteredAttributeDictionary_ArgumentMustBeString"), "key");
                }
                return this[key.ToString()];
            }
            set
            {
                if (!(key is string))
                {
                    throw new ArgumentException(System.Web.SR.GetString("FilteredAttributeDictionary_ArgumentMustBeString"), "key");
                }
                if (!(value is string))
                {
                    throw new ArgumentException(System.Web.SR.GetString("FilteredAttributeDictionary_ArgumentMustBeString"), "value");
                }
                this[key.ToString()] = value.ToString();
            }
        }

        ICollection IDictionary.Keys =>
            this._data.Keys;

        ICollection IDictionary.Values =>
            this._data.Values;
    }
}

