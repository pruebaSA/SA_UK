namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class SavePoint
    {
        private int _scopeIndex;

        internal SavePoint(int scopeIndex)
        {
            this._scopeIndex = scopeIndex;
        }

        internal bool ContainsScope(int scopeIndex) => 
            (scopeIndex >= this._scopeIndex);

        internal int ScopeIndex =>
            this._scopeIndex;
    }
}

