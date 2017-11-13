namespace MS.Utility
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Windows;

    internal sealed class ArrayObjectMap : FrugalMapBase
    {
        private ushort _count;
        private FrugalMapBase.Entry[] _entries;
        private bool _sorted;
        private const int GROWTH = 3;
        private const int MAXSIZE = 15;
        private const int MINSIZE = 9;

        private int Compare(int left, int right) => 
            (this._entries[left].Key - this._entries[right].Key);

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
            for (int i = 0; i < this._count; i++)
            {
                if (this._entries[i].Key == key)
                {
                    this._entries[i].Value = value;
                    return FrugalMapStoreState.Success;
                }
            }
            if (15 <= this._count)
            {
                return FrugalMapStoreState.SortedArray;
            }
            if (this._entries != null)
            {
                this._sorted = false;
                if (this._entries.Length <= this._count)
                {
                    FrugalMapBase.Entry[] destinationArray = new FrugalMapBase.Entry[this._entries.Length + 3];
                    Array.Copy(this._entries, 0, destinationArray, 0, this._entries.Length);
                    this._entries = destinationArray;
                }
            }
            else
            {
                this._entries = new FrugalMapBase.Entry[9];
                this._sorted = true;
            }
            this._entries[this._count].Key = key;
            this._entries[this._count].Value = value;
            this._count = (ushort) (this._count + 1);
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

        private int Partition(int left, int right)
        {
            FrugalMapBase.Entry entry;
            int num = right;
            int index = left - 1;
            int num3 = right;
        Label_0008:
            while (this.Compare(++index, num) < 0)
            {
            }
            while (this.Compare(num, --num3) < 0)
            {
                if (num3 == left)
                {
                    break;
                }
            }
            if (index < num3)
            {
                entry = this._entries[num3];
                this._entries[num3] = this._entries[index];
                this._entries[index] = entry;
                goto Label_0008;
            }
            entry = this._entries[right];
            this._entries[right] = this._entries[index];
            this._entries[index] = entry;
            return index;
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

        private void QSort(int left, int right)
        {
            if (left < right)
            {
                int num = this.Partition(left, right);
                this.QSort(left, num - 1);
                this.QSort(num + 1, right);
            }
        }

        public override void RemoveEntry(int key)
        {
            for (int i = 0; i < this._count; i++)
            {
                if (this._entries[i].Key == key)
                {
                    int length = (this._count - i) - 1;
                    if (length > 0)
                    {
                        Array.Copy(this._entries, i + 1, this._entries, i, length);
                    }
                    this._entries[this._count - 1].Key = 0x7fffffff;
                    this._entries[this._count - 1].Value = DependencyProperty.UnsetValue;
                    this._count = (ushort) (this._count - 1);
                    return;
                }
            }
        }

        public override object Search(int key)
        {
            for (int i = 0; i < this._count; i++)
            {
                if (key == this._entries[i].Key)
                {
                    return this._entries[i].Value;
                }
            }
            return DependencyProperty.UnsetValue;
        }

        public override void Sort()
        {
            if (!this._sorted && (this._count > 1))
            {
                this.QSort(0, this._count - 1);
                this._sorted = true;
            }
        }

        public override int Count =>
            this._count;
    }
}

