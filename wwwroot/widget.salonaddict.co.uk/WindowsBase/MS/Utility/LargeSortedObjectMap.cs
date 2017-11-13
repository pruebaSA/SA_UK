namespace MS.Utility
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Windows;

    internal sealed class LargeSortedObjectMap : FrugalMapBase
    {
        internal int _count;
        private FrugalMapBase.Entry[] _entries;
        private int _lastKey = 0x7fffffff;
        private const int MINSIZE = 2;

        private int FindInsertIndex(int key, out bool found)
        {
            int index = 0;
            if ((this._count > 0) && (key <= this._lastKey))
            {
                int num2 = this._count - 1;
                do
                {
                    int num3 = (num2 + index) / 2;
                    if (key <= this._entries[num3].Key)
                    {
                        num2 = num3;
                    }
                    else
                    {
                        index = num3 + 1;
                    }
                }
                while (index < num2);
                found = key == this._entries[index].Key;
                return index;
            }
            index = this._count;
            found = false;
            return index;
        }

        public override void GetKeyValuePair(int index, out int key, out object value)
        {
            if (index < this._count)
            {
                value = this._entries[index].Value;
                key = this._entries[index].Key;
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
            bool flag;
            int index = this.FindInsertIndex(key, out flag);
            if (flag)
            {
                this._entries[index].Value = value;
                return FrugalMapStoreState.Success;
            }
            if (this._entries != null)
            {
                if (this._entries.Length <= this._count)
                {
                    int length = this._entries.Length;
                    FrugalMapBase.Entry[] destinationArray = new FrugalMapBase.Entry[length + (length >> 1)];
                    Array.Copy(this._entries, 0, destinationArray, 0, this._entries.Length);
                    this._entries = destinationArray;
                }
            }
            else
            {
                this._entries = new FrugalMapBase.Entry[2];
            }
            if (index < this._count)
            {
                Array.Copy(this._entries, index, this._entries, index + 1, this._count - index);
            }
            else
            {
                this._lastKey = key;
            }
            this._entries[index].Key = key;
            this._entries[index].Value = value;
            this._count++;
            return FrugalMapStoreState.Success;
        }

        public override void Iterate(ArrayList list, FrugalMapIterationCallback callback)
        {
            if (this._count > 0)
            {
                for (int i = 0; i < this._count; i++)
                {
                    callback(list, this._entries[i].Key, this._entries[i].Value);
                }
            }
        }

        public override void Promote(FrugalMapBase newMap)
        {
            for (int i = 0; i < this._entries.Length; i++)
            {
                if (newMap.InsertEntry(this._entries[i].Key, this._entries[i].Value) != FrugalMapStoreState.Success)
                {
                    throw new ArgumentException(System.Windows.SR.Get("FrugalMap_TargetMapCannotHoldAllData", new object[] { this.ToString(), newMap.ToString() }), "newMap");
                }
            }
        }

        public override void RemoveEntry(int key)
        {
            bool flag;
            int destinationIndex = this.FindInsertIndex(key, out flag);
            if (flag)
            {
                int length = (this._count - destinationIndex) - 1;
                if (length > 0)
                {
                    Array.Copy(this._entries, destinationIndex + 1, this._entries, destinationIndex, length);
                }
                else if (this._count > 1)
                {
                    this._lastKey = this._entries[this._count - 2].Key;
                }
                else
                {
                    this._lastKey = 0x7fffffff;
                }
                this._entries[this._count - 1].Key = 0x7fffffff;
                this._entries[this._count - 1].Value = DependencyProperty.UnsetValue;
                this._count--;
            }
        }

        public override object Search(int key)
        {
            bool flag;
            int index = this.FindInsertIndex(key, out flag);
            if (flag)
            {
                return this._entries[index].Value;
            }
            return DependencyProperty.UnsetValue;
        }

        public override void Sort()
        {
        }

        public override int Count =>
            this._count;
    }
}

