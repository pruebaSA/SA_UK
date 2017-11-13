namespace System.Resources
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [Serializable, ComVisible(true)]
    public class ResourceSet : IDisposable, IEnumerable
    {
        private Hashtable _caseInsensitiveTable;
        [NonSerialized]
        protected IResourceReader Reader;
        protected Hashtable Table;

        protected ResourceSet()
        {
            this.Table = new Hashtable(0);
        }

        internal ResourceSet(bool junk)
        {
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public ResourceSet(Stream stream)
        {
            this.Reader = new ResourceReader(stream);
            this.Table = new Hashtable();
            this.ReadResources();
        }

        public ResourceSet(IResourceReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            this.Reader = reader;
            this.Table = new Hashtable();
            this.ReadResources();
        }

        public ResourceSet(string fileName)
        {
            this.Reader = new ResourceReader(fileName);
            this.Table = new Hashtable();
            this.ReadResources();
        }

        public virtual void Close()
        {
            this.Dispose(true);
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                IResourceReader reader = this.Reader;
                this.Reader = null;
                if (reader != null)
                {
                    reader.Close();
                }
            }
            this.Reader = null;
            this._caseInsensitiveTable = null;
            this.Table = null;
        }

        public virtual Type GetDefaultReader() => 
            typeof(ResourceReader);

        public virtual Type GetDefaultWriter() => 
            typeof(ResourceWriter);

        [ComVisible(false)]
        public virtual IDictionaryEnumerator GetEnumerator() => 
            this.GetEnumeratorHelper();

        private IDictionaryEnumerator GetEnumeratorHelper()
        {
            Hashtable table = this.Table;
            return table?.GetEnumerator();
        }

        public virtual object GetObject(string name)
        {
            Hashtable table = this.Table;
            if (table == null)
            {
                throw new ObjectDisposedException(null, Environment.GetResourceString("ObjectDisposed_ResourceSet"));
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            return table[name];
        }

        public virtual object GetObject(string name, bool ignoreCase)
        {
            Hashtable table = this.Table;
            if (table == null)
            {
                throw new ObjectDisposedException(null, Environment.GetResourceString("ObjectDisposed_ResourceSet"));
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            object obj2 = table[name];
            if ((obj2 != null) || !ignoreCase)
            {
                return obj2;
            }
            Hashtable hashtable2 = this._caseInsensitiveTable;
            return hashtable2?[name];
        }

        public virtual string GetString(string name)
        {
            string str;
            Hashtable table = this.Table;
            if (table == null)
            {
                throw new ObjectDisposedException(null, Environment.GetResourceString("ObjectDisposed_ResourceSet"));
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            try
            {
                str = (string) table[name];
            }
            catch (InvalidCastException)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ResourceNotString_Name", new object[] { name }));
            }
            return str;
        }

        public virtual string GetString(string name, bool ignoreCase)
        {
            string str;
            string str2;
            Hashtable table = this.Table;
            if (table == null)
            {
                throw new ObjectDisposedException(null, Environment.GetResourceString("ObjectDisposed_ResourceSet"));
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            try
            {
                str = (string) table[name];
            }
            catch (InvalidCastException)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ResourceNotString_Name", new object[] { name }));
            }
            if ((str != null) || !ignoreCase)
            {
                return str;
            }
            Hashtable hashtable2 = this._caseInsensitiveTable;
            if (hashtable2 == null)
            {
                hashtable2 = new Hashtable(StringComparer.OrdinalIgnoreCase);
                IDictionaryEnumerator enumerator = table.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    hashtable2.Add(enumerator.Key, enumerator.Value);
                }
                this._caseInsensitiveTable = hashtable2;
            }
            try
            {
                str2 = (string) hashtable2[name];
            }
            catch (InvalidCastException)
            {
                throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ResourceNotString_Name", new object[] { name }));
            }
            return str2;
        }

        protected virtual void ReadResources()
        {
            IDictionaryEnumerator enumerator = this.Reader.GetEnumerator();
            while (enumerator.MoveNext())
            {
                object obj2 = enumerator.Value;
                this.Table.Add(enumerator.Key, obj2);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumeratorHelper();
    }
}

