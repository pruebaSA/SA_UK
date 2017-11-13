namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class Pair<L, R>
    {
        internal L Left;
        internal R Right;

        internal Pair(L left, R right)
        {
            this.Left = left;
            this.Right = right;
        }
    }
}

