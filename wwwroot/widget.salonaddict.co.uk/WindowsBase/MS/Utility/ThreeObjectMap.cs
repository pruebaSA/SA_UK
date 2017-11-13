namespace MS.Utility
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Windows;

    internal sealed class ThreeObjectMap : FrugalMapBase
    {
        private ushort _count;
        private FrugalMapBase.Entry _entry0;
        private FrugalMapBase.Entry _entry1;
        private FrugalMapBase.Entry _entry2;
        private bool _sorted;
        private const int SIZE = 3;

        public override void GetKeyValuePair(int index, out int key, out object value)
        {
            if (index < this._count)
            {
                switch (index)
                {
                    case 0:
                        key = this._entry0.Key;
                        value = this._entry0.Value;
                        return;

                    case 1:
                        key = this._entry1.Key;
                        value = this._entry1.Value;
                        return;

                    case 2:
                        key = this._entry2.Key;
                        value = this._entry2.Value;
                        return;
                }
                key = 0x7fffffff;
                value = DependencyProperty.UnsetValue;
            }
            else
            {
                key = 0x7fffffff;
                value = DependencyProperty.UnsetValue;
                throw new ArgumentOutOfRangeException("index");
            }
        }

        public override FrugalMapStoreState InsertEntry(int key, object value)
        {
            switch (this._count)
            {
                case 1:
                    if (this._entry0.Key != key)
                    {
                        break;
                    }
                    this._entry0.Value = value;
                    return FrugalMapStoreState.Success;

                case 2:
                    if (this._entry0.Key != key)
                    {
                        if (this._entry1.Key != key)
                        {
                            break;
                        }
                        this._entry1.Value = value;
                        return FrugalMapStoreState.Success;
                    }
                    this._entry0.Value = value;
                    return FrugalMapStoreState.Success;

                case 3:
                    if (this._entry0.Key != key)
                    {
                        if (this._entry1.Key == key)
                        {
                            this._entry1.Value = value;
                            return FrugalMapStoreState.Success;
                        }
                        if (this._entry2.Key == key)
                        {
                            this._entry2.Value = value;
                            return FrugalMapStoreState.Success;
                        }
                        break;
                    }
                    this._entry0.Value = value;
                    return FrugalMapStoreState.Success;
            }
            if (3 <= this._count)
            {
                return FrugalMapStoreState.SixObjectMap;
            }
            switch (this._count)
            {
                case 0:
                    this._entry0.Key = key;
                    this._entry0.Value = value;
                    this._sorted = true;
                    break;

                case 1:
                    this._entry1.Key = key;
                    this._entry1.Value = value;
                    this._sorted = false;
                    break;

                case 2:
                    this._entry2.Key = key;
                    this._entry2.Value = value;
                    this._sorted = false;
                    break;
            }
            this._count = (ushort) (this._count + 1);
            return FrugalMapStoreState.Success;
        }

        public override void Iterate(ArrayList list, FrugalMapIterationCallback callback)
        {
            if (this._count > 0)
            {
                if (this._count >= 1)
                {
                    callback(list, this._entry0.Key, this._entry0.Value);
                }
                if (this._count >= 2)
                {
                    callback(list, this._entry1.Key, this._entry1.Value);
                }
                if (this._count == 3)
                {
                    callback(list, this._entry2.Key, this._entry2.Value);
                }
            }
        }

        public override void Promote(FrugalMapBase newMap)
        {
            if (newMap.InsertEntry(this._entry0.Key, this._entry0.Value) != FrugalMapStoreState.Success)
            {
                throw new ArgumentException(System.Windows.SR.Get("FrugalMap_TargetMapCannotHoldAllData", new object[] { this.ToString(), newMap.ToString() }), "newMap");
            }
            if (newMap.InsertEntry(this._entry1.Key, this._entry1.Value) != FrugalMapStoreState.Success)
            {
                throw new ArgumentException(System.Windows.SR.Get("FrugalMap_TargetMapCannotHoldAllData", new object[] { this.ToString(), newMap.ToString() }), "newMap");
            }
            if (newMap.InsertEntry(this._entry2.Key, this._entry2.Value) != FrugalMapStoreState.Success)
            {
                throw new ArgumentException(System.Windows.SR.Get("FrugalMap_TargetMapCannotHoldAllData", new object[] { this.ToString(), newMap.ToString() }), "newMap");
            }
        }

        public override void RemoveEntry(int key)
        {
            switch (this._count)
            {
                case 1:
                    if (this._entry0.Key != key)
                    {
                        break;
                    }
                    this._entry0.Key = 0x7fffffff;
                    this._entry0.Value = DependencyProperty.UnsetValue;
                    this._count = (ushort) (this._count - 1);
                    return;

                case 2:
                    if (this._entry0.Key != key)
                    {
                        if (this._entry1.Key != key)
                        {
                            break;
                        }
                        this._entry1.Key = 0x7fffffff;
                        this._entry1.Value = DependencyProperty.UnsetValue;
                        this._count = (ushort) (this._count - 1);
                        return;
                    }
                    this._entry0 = this._entry1;
                    this._entry1.Key = 0x7fffffff;
                    this._entry1.Value = DependencyProperty.UnsetValue;
                    this._count = (ushort) (this._count - 1);
                    return;

                case 3:
                    if (this._entry0.Key != key)
                    {
                        if (this._entry1.Key == key)
                        {
                            this._entry1 = this._entry2;
                            this._entry2.Key = 0x7fffffff;
                            this._entry2.Value = DependencyProperty.UnsetValue;
                            this._count = (ushort) (this._count - 1);
                            return;
                        }
                        if (this._entry2.Key == key)
                        {
                            this._entry2.Key = 0x7fffffff;
                            this._entry2.Value = DependencyProperty.UnsetValue;
                            this._count = (ushort) (this._count - 1);
                        }
                        break;
                    }
                    this._entry0 = this._entry1;
                    this._entry1 = this._entry2;
                    this._entry2.Key = 0x7fffffff;
                    this._entry2.Value = DependencyProperty.UnsetValue;
                    this._count = (ushort) (this._count - 1);
                    return;

                default:
                    return;
            }
        }

        public override object Search(int key)
        {
            if (this._count > 0)
            {
                if (this._entry0.Key == key)
                {
                    return this._entry0.Value;
                }
                if (this._count > 1)
                {
                    if (this._entry1.Key == key)
                    {
                        return this._entry1.Value;
                    }
                    if ((this._count > 2) && (this._entry2.Key == key))
                    {
                        return this._entry2.Value;
                    }
                }
            }
            return DependencyProperty.UnsetValue;
        }

        public override void Sort()
        {
            if (!this._sorted && (this._count > 1))
            {
                FrugalMapBase.Entry entry;
                if (this._entry0.Key > this._entry1.Key)
                {
                    entry = this._entry0;
                    this._entry0 = this._entry1;
                    this._entry1 = entry;
                }
                if ((this._count > 2) && (this._entry1.Key > this._entry2.Key))
                {
                    entry = this._entry1;
                    this._entry1 = this._entry2;
                    this._entry2 = entry;
                    if (this._entry0.Key > this._entry1.Key)
                    {
                        entry = this._entry0;
                        this._entry0 = this._entry1;
                        this._entry1 = entry;
                    }
                }
                this._sorted = true;
            }
        }

        public override int Count =>
            this._count;
    }
}

