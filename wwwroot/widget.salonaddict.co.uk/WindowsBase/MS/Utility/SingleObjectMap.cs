namespace MS.Utility
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Windows;

    internal sealed class SingleObjectMap : FrugalMapBase
    {
        private FrugalMapBase.Entry _loneEntry;

        public SingleObjectMap()
        {
            this._loneEntry.Key = 0x7fffffff;
            this._loneEntry.Value = DependencyProperty.UnsetValue;
        }

        public override void GetKeyValuePair(int index, out int key, out object value)
        {
            if (index == 0)
            {
                value = this._loneEntry.Value;
                key = this._loneEntry.Key;
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
            if ((0x7fffffff != this._loneEntry.Key) && (key != this._loneEntry.Key))
            {
                return FrugalMapStoreState.ThreeObjectMap;
            }
            this._loneEntry.Key = key;
            this._loneEntry.Value = value;
            return FrugalMapStoreState.Success;
        }

        public override void Iterate(ArrayList list, FrugalMapIterationCallback callback)
        {
            if (this.Count == 1)
            {
                callback(list, this._loneEntry.Key, this._loneEntry.Value);
            }
        }

        public override void Promote(FrugalMapBase newMap)
        {
            if (newMap.InsertEntry(this._loneEntry.Key, this._loneEntry.Value) != FrugalMapStoreState.Success)
            {
                throw new ArgumentException(System.Windows.SR.Get("FrugalMap_TargetMapCannotHoldAllData", new object[] { this.ToString(), newMap.ToString() }), "newMap");
            }
        }

        public override void RemoveEntry(int key)
        {
            if (key == this._loneEntry.Key)
            {
                this._loneEntry.Key = 0x7fffffff;
                this._loneEntry.Value = DependencyProperty.UnsetValue;
            }
        }

        public override object Search(int key)
        {
            if (key == this._loneEntry.Key)
            {
                return this._loneEntry.Value;
            }
            return DependencyProperty.UnsetValue;
        }

        public override void Sort()
        {
        }

        public override int Count
        {
            get
            {
                if (0x7fffffff != this._loneEntry.Key)
                {
                    return 1;
                }
                return 0;
            }
        }
    }
}

