namespace System.Collections.Specialized
{
    using System;
    using System.Collections;
    using System.ComponentModel.Design.Serialization;
    using System.Globalization;
    using System.Reflection;

    [Serializable, DesignerSerializer("System.Diagnostics.Design.StringDictionaryCodeDomSerializer, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public class StringDictionary : IEnumerable
    {
        internal Hashtable contents = new Hashtable();

        public virtual void Add(string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            this.contents.Add(key.ToLower(CultureInfo.InvariantCulture), value);
        }

        public virtual void Clear()
        {
            this.contents.Clear();
        }

        public virtual bool ContainsKey(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            return this.contents.ContainsKey(key.ToLower(CultureInfo.InvariantCulture));
        }

        public virtual bool ContainsValue(string value) => 
            this.contents.ContainsValue(value);

        public virtual void CopyTo(Array array, int index)
        {
            this.contents.CopyTo(array, index);
        }

        public virtual IEnumerator GetEnumerator() => 
            this.contents.GetEnumerator();

        public virtual void Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            this.contents.Remove(key.ToLower(CultureInfo.InvariantCulture));
        }

        public virtual int Count =>
            this.contents.Count;

        public virtual bool IsSynchronized =>
            this.contents.IsSynchronized;

        public virtual string this[string key]
        {
            get
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                return (string) this.contents[key.ToLower(CultureInfo.InvariantCulture)];
            }
            set
            {
                if (key == null)
                {
                    throw new ArgumentNullException("key");
                }
                this.contents[key.ToLower(CultureInfo.InvariantCulture)] = value;
            }
        }

        public virtual ICollection Keys =>
            this.contents.Keys;

        public virtual object SyncRoot =>
            this.contents.SyncRoot;

        public virtual ICollection Values =>
            this.contents.Values;
    }
}

