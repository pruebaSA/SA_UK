namespace System.Web.Security
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Web;

    [Serializable, AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class MembershipUserCollection : ICollection, IEnumerable
    {
        private Hashtable _Indices;
        private bool _ReadOnly;
        private ArrayList _Values;

        public MembershipUserCollection()
        {
            this._Indices = new Hashtable(10, StringComparer.CurrentCultureIgnoreCase);
            this._Values = new ArrayList();
        }

        private MembershipUserCollection(Hashtable indices, ArrayList values)
        {
            this._Indices = (Hashtable) indices.Clone();
            this._Values = (ArrayList) values.Clone();
        }

        public void Add(MembershipUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (this._ReadOnly)
            {
                throw new NotSupportedException();
            }
            int num = this._Values.Add(user);
            try
            {
                this._Indices.Add(user.UserName, num);
            }
            catch
            {
                this._Values.RemoveAt(num);
                throw;
            }
        }

        public void Clear()
        {
            this._Values.Clear();
            this._Indices.Clear();
        }

        public void CopyTo(MembershipUser[] array, int index)
        {
            this._Values.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator() => 
            this._Values.GetEnumerator();

        public void Remove(string name)
        {
            if (this._ReadOnly)
            {
                throw new NotSupportedException();
            }
            object obj2 = this._Indices[name];
            if ((obj2 != null) && (obj2 is int))
            {
                int index = (int) obj2;
                if (index < this._Values.Count)
                {
                    this._Values.RemoveAt(index);
                    this._Indices.Remove(name);
                    ArrayList list = new ArrayList();
                    foreach (DictionaryEntry entry in this._Indices)
                    {
                        if (((int) entry.Value) > index)
                        {
                            list.Add(entry.Key);
                        }
                    }
                    foreach (string str in list)
                    {
                        this._Indices[str] = ((int) this._Indices[str]) - 1;
                    }
                }
            }
        }

        public void SetReadOnly()
        {
            if (!this._ReadOnly)
            {
                this._ReadOnly = true;
                this._Values = ArrayList.ReadOnly(this._Values);
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this._Values.CopyTo(array, index);
        }

        public int Count =>
            this._Values.Count;

        public bool IsSynchronized =>
            false;

        public MembershipUser this[string name]
        {
            get
            {
                object obj2 = this._Indices[name];
                if ((obj2 == null) || !(obj2 is int))
                {
                    return null;
                }
                int num = (int) obj2;
                if (num >= this._Values.Count)
                {
                    return null;
                }
                return (MembershipUser) this._Values[num];
            }
        }

        public object SyncRoot =>
            this;
    }
}

