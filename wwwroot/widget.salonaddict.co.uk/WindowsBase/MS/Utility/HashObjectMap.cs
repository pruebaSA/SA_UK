namespace MS.Utility
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Windows;

    internal sealed class HashObjectMap : FrugalMapBase
    {
        internal Hashtable _entries;
        internal const int MINSIZE = 0xa3;
        private static object NullValue = new object();

        public override void GetKeyValuePair(int index, out int key, out object value)
        {
            if (index < this._entries.Count)
            {
                IDictionaryEnumerator enumerator = this._entries.GetEnumerator();
                enumerator.MoveNext();
                for (int i = 0; i < index; i++)
                {
                    enumerator.MoveNext();
                }
                key = (int) enumerator.Key;
                if ((enumerator.Value != NullValue) && (enumerator.Value != null))
                {
                    value = enumerator.Value;
                }
                else
                {
                    value = DependencyProperty.UnsetValue;
                }
            }
            else
            {
                value = DependencyProperty.UnsetValue;
                key = 0x7fffffff;
                throw new ArgumentOutOfRangeException("index");
            }
        }

        public override FrugalMapStoreState InsertEntry(int key, object value)
        {
            if (this._entries == null)
            {
                this._entries = new Hashtable(0xa3);
            }
            this._entries[key] = ((value != NullValue) && (value != null)) ? value : NullValue;
            return FrugalMapStoreState.Success;
        }

        public override void Iterate(ArrayList list, FrugalMapIterationCallback callback)
        {
            IDictionaryEnumerator enumerator = this._entries.GetEnumerator();
            while (enumerator.MoveNext())
            {
                object unsetValue;
                int key = (int) enumerator.Key;
                if ((enumerator.Value != NullValue) && (enumerator.Value != null))
                {
                    unsetValue = enumerator.Value;
                }
                else
                {
                    unsetValue = DependencyProperty.UnsetValue;
                }
                callback(list, key, unsetValue);
            }
        }

        public override void Promote(FrugalMapBase newMap)
        {
            throw new InvalidOperationException(System.Windows.SR.Get("FrugalMap_CannotPromoteBeyondHashtable"));
        }

        public override void RemoveEntry(int key)
        {
            this._entries.Remove(key);
        }

        public override object Search(int key)
        {
            object obj2 = this._entries[key];
            if ((obj2 != NullValue) && (obj2 != null))
            {
                return obj2;
            }
            return DependencyProperty.UnsetValue;
        }

        public override void Sort()
        {
        }

        public override int Count =>
            this._entries.Count;
    }
}

