namespace System.Web.Util
{
    using System;

    internal sealed class Pair<TFirst, TSecond>
    {
        private readonly TFirst _first;
        private readonly TSecond _second;

        public Pair(TFirst first, TSecond second)
        {
            this._first = first;
            this._second = second;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            Pair<TFirst, TSecond> pair = obj as Pair<TFirst, TSecond>;
            if ((pair == null) || (((pair._first != null) || (this._first != null)) && ((pair._first == null) || !pair._first.Equals(this._first))))
            {
                return false;
            }
            return (((pair._second == null) && (this._second == null)) || ((pair._second != null) && pair._second.Equals(this._second)));
        }

        public override int GetHashCode()
        {
            int num = (this._first == null) ? 0 : this._first.GetHashCode();
            int num2 = (this._second == null) ? 0 : this._second.GetHashCode();
            return HashCodeCombiner.CombineHashCodes(num, num2);
        }

        public TFirst First =>
            this._first;

        public TSecond Second =>
            this._second;
    }
}

