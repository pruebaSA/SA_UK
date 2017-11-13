namespace MS.Internal.Utility
{
    using MS.Internal;
    using System;

    internal class WeakReferenceKey<T>
    {
        private int _hashCode;
        private WeakReference _item;

        public WeakReferenceKey(T item)
        {
            Invariant.Assert(item != null);
            this._item = new WeakReference(item);
            this._hashCode = item.GetHashCode();
        }

        public override bool Equals(object o)
        {
            if (o == this)
            {
                return true;
            }
            WeakReferenceKey<T> key = o as WeakReferenceKey<T>;
            if (key == null)
            {
                return false;
            }
            T item = this.Item;
            if (item == null)
            {
                return false;
            }
            return ((this._hashCode == key._hashCode) && object.Equals(item, key.Item));
        }

        public override int GetHashCode() => 
            this._hashCode;

        public T Item =>
            ((T) this._item.Target);
    }
}

