namespace System.Web.Util
{
    using System;
    using System.Reflection;

    internal sealed class Tuple
    {
        private object[] _items;

        public Tuple(params object[] items)
        {
            this._items = items;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Tuple tuple = (Tuple) obj;
            if (tuple != this)
            {
                if (tuple._items.Length != this._items.Length)
                {
                    return false;
                }
                for (int i = 0; i < this._items.Length; i++)
                {
                    if (!tuple[i].Equals(this[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            if (this._items.Length == 0)
            {
                return 0;
            }
            HashCodeCombiner combiner = new HashCodeCombiner();
            for (int i = 0; i < this._items.Length; i++)
            {
                combiner.AddObject(this._items[i]);
            }
            return combiner.CombinedHash32;
        }

        public object this[int index] =>
            this._items[index];
    }
}

