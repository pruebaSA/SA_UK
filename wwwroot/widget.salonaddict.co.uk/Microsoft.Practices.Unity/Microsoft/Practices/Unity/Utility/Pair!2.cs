namespace Microsoft.Practices.Unity.Utility
{
    using System;

    public class Pair<TFirst, TSecond>
    {
        private TFirst first;
        private TSecond second;

        public Pair(TFirst first, TSecond second)
        {
            this.first = first;
            this.second = second;
        }

        public TFirst First =>
            this.first;

        public TSecond Second =>
            this.second;
    }
}

