namespace System.Data.Common.EntitySql
{
    using System;
    using System.Data.Common.CommandTrees;

    internal abstract class ScopeEntry
    {
        protected DbExpression _expression;
        private bool _isHidden;
        private ScopeEntryKind _scopeEntryKind;
        protected SourceVarKind _varKind;

        internal ScopeEntry(ScopeEntryKind scopeEntryKind, DbExpression expression)
        {
            this._scopeEntryKind = scopeEntryKind;
            this._expression = expression;
            this._varKind = SourceVarKind.Input;
        }

        internal virtual DbExpression Expression =>
            this._expression;

        internal bool IsHidden =>
            this._isHidden;

        internal ScopeEntryKind Kind
        {
            get => 
                this._scopeEntryKind;
            set
            {
                this._scopeEntryKind = value;
            }
        }

        public SourceVarKind VarKind
        {
            get => 
                this._varKind;
            set
            {
                this._varKind = value;
            }
        }
    }
}

