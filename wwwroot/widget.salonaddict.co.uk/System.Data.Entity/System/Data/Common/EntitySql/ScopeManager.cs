namespace System.Data.Common.EntitySql
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;

    internal class ScopeManager
    {
        private IEqualityComparer<string> _keyComparer;
        private List<Scope> _stackedScopes = new List<Scope>();

        internal ScopeManager(IEqualityComparer<string> keyComparer)
        {
            this._keyComparer = keyComparer;
        }

        internal void Add(string key, ScopeEntry value)
        {
            this.CurrentScope.Add(key, value);
        }

        internal ScopeManager EnterScope()
        {
            this._stackedScopes.Add(new Scope(this._keyComparer));
            return this;
        }

        internal Scope GetScopeByIndex(int scopeIndex)
        {
            if ((0 > scopeIndex) || (scopeIndex > this.CurrentScopeIndex))
            {
                throw EntityUtil.EntitySqlError(Strings.InvalidScopeIndex);
            }
            return this._stackedScopes[scopeIndex];
        }

        internal bool IsInCurrentScope(string key) => 
            this.CurrentScope.Contains(key);

        internal void LeaveScope()
        {
            this._stackedScopes.RemoveAt(this.CurrentScopeIndex);
        }

        internal void RemoveFromScope(string key)
        {
            this.CurrentScope.Remove(key);
        }

        internal void RollbackToSavepoint(SavePoint savePoint)
        {
            if (((savePoint.ScopeIndex > this.CurrentScopeIndex) || (savePoint.ScopeIndex < 0)) || (this.CurrentScopeIndex < 0))
            {
                throw EntityUtil.EntitySqlError(Strings.InvalidSavePoint);
            }
            int num = this.CurrentScopeIndex - savePoint.ScopeIndex;
            if (num > 0)
            {
                this._stackedScopes.RemoveRange(savePoint.ScopeIndex + 1, this.CurrentScopeIndex - savePoint.ScopeIndex);
            }
        }

        internal Scope CurrentScope =>
            this._stackedScopes[this.CurrentScopeIndex];

        internal int CurrentScopeIndex =>
            (this._stackedScopes.Count - 1);
    }
}

