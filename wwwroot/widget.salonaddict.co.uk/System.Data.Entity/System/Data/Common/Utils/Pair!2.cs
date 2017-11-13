namespace System.Data.Common.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class Pair<TFirst, TSecond> : InternalBase
    {
        private readonly TFirst first;
        private readonly TSecond second;

        internal Pair(TFirst first, TSecond second)
        {
            this.first = first;
            this.second = second;
        }

        public bool Equals(Pair<TFirst, TSecond> other) => 
            (this.first.Equals(other.first) && this.second.Equals(other.second));

        public override bool Equals(object other)
        {
            Pair<TFirst, TSecond> pair = other as Pair<TFirst, TSecond>;
            return ((pair != null) && this.Equals(pair));
        }

        public override int GetHashCode() => 
            ((this.first.GetHashCode() << 5) ^ this.second.GetHashCode());

        internal override void ToCompactString(StringBuilder builder)
        {
            builder.Append("<");
            builder.Append(this.first.ToString());
            builder.Append(", " + this.second.ToString());
            builder.Append(">");
        }

        internal TFirst First =>
            this.first;

        internal TSecond Second =>
            this.second;

        internal class PairComparer : IEqualityComparer<Pair<TFirst, TSecond>>
        {
            private static readonly EqualityComparer<TFirst> firstComparer;
            internal static readonly Pair<TFirst, TSecond>.PairComparer Instance;
            private static readonly EqualityComparer<TSecond> secondComparer;

            static PairComparer()
            {
                Pair<TFirst, TSecond>.PairComparer.Instance = new Pair<TFirst, TSecond>.PairComparer();
                Pair<TFirst, TSecond>.PairComparer.firstComparer = EqualityComparer<TFirst>.Default;
                Pair<TFirst, TSecond>.PairComparer.secondComparer = EqualityComparer<TSecond>.Default;
            }

            private PairComparer()
            {
            }

            public bool Equals(Pair<TFirst, TSecond> x, Pair<TFirst, TSecond> y) => 
                (Pair<TFirst, TSecond>.PairComparer.firstComparer.Equals(x.First, y.First) && Pair<TFirst, TSecond>.PairComparer.secondComparer.Equals(x.Second, y.Second));

            public int GetHashCode(Pair<TFirst, TSecond> source) => 
                source.GetHashCode();
        }
    }
}

