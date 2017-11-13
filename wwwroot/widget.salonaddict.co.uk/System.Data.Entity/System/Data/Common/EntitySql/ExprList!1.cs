namespace System.Data.Common.EntitySql
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    internal sealed class ExprList<T> : Expr, IEnumerable<T>, IEnumerable
    {
        private List<T> _exprList;

        internal ExprList()
        {
            this._exprList = new List<T>();
        }

        internal ExprList(T item)
        {
            this._exprList = new List<T>();
            this._exprList.Add(item);
        }

        internal ExprList<T> Add(T item)
        {
            this._exprList.Add(item);
            return (ExprList<T>) this;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => 
            this._exprList.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            this._exprList.GetEnumerator();

        internal int Count =>
            this._exprList.Count;

        internal List<T> Expressions =>
            this._exprList;

        internal T this[int index] =>
            this._exprList[index];
    }
}

