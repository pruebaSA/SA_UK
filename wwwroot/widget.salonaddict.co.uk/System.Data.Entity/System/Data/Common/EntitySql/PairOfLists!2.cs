namespace System.Data.Common.EntitySql
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal sealed class PairOfLists<L, R>
    {
        private List<L> _leftValues;
        private List<R> _rightValues;

        internal PairOfLists()
        {
            this._leftValues = new List<L>();
            this._rightValues = new List<R>();
        }

        internal PairOfLists(List<L> leftValues, List<R> rightValues)
        {
            this._leftValues = leftValues;
            this._rightValues = rightValues;
        }

        internal void Add(L left, R right)
        {
            this._leftValues.Add(left);
            this._rightValues.Add(right);
        }

        internal int Count =>
            this._leftValues.Count;

        internal Pair<L, R> this[int index]
        {
            set
            {
                this.Left[index] = value.Left;
                this.Right[index] = value.Right;
            }
        }

        internal List<L> Left =>
            this._leftValues;

        internal List<R> Right =>
            this._rightValues;
    }
}

