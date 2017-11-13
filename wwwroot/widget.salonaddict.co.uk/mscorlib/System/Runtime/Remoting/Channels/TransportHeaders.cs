namespace System.Runtime.Remoting.Channels
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [Serializable, ComVisible(true), SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.Infrastructure), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public class TransportHeaders : ITransportHeaders
    {
        private ArrayList _headerList = new ArrayList(6);

        public IEnumerator GetEnumerator() => 
            this._headerList.GetEnumerator();

        public object this[object key]
        {
            get
            {
                string strB = (string) key;
                foreach (DictionaryEntry entry in this._headerList)
                {
                    if (string.Compare((string) entry.Key, strB, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return entry.Value;
                    }
                }
                return null;
            }
            set
            {
                if (key != null)
                {
                    string strB = (string) key;
                    for (int i = this._headerList.Count - 1; i >= 0; i--)
                    {
                        DictionaryEntry entry = (DictionaryEntry) this._headerList[i];
                        string strA = (string) entry.Key;
                        if (string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            this._headerList.RemoveAt(i);
                            break;
                        }
                    }
                    if (value != null)
                    {
                        this._headerList.Add(new DictionaryEntry(key, value));
                    }
                }
            }
        }
    }
}

