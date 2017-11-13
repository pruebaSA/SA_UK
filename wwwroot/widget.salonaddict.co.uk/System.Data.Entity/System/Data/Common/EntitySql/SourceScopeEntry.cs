namespace System.Data.Common.EntitySql
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.CommandTrees;

    internal class SourceScopeEntry : ScopeEntry
    {
        private DbExpression _aggregateExpression;
        private DbVariableReferenceExpression _groupVar;
        private bool _isGroupVarDirty;
        private bool _isSourceVarDirty;
        private List<string> _prefixNames;
        private string _varName;
        private DbVariableReferenceExpression _varRef;
        private string _varTag;

        internal SourceScopeEntry(ScopeEntryKind scopeEntryKind, DbExpressionBinding sourceBinding, string varTag) : base(scopeEntryKind, sourceBinding.Variable)
        {
            this._prefixNames = new List<string>();
            this._isSourceVarDirty = true;
            this._isGroupVarDirty = true;
            this._varName = sourceBinding.VariableName;
            this._varRef = sourceBinding.Variable;
            this._varTag = varTag;
        }

        internal SourceScopeEntry(ScopeEntryKind scopeEntryKind, string varName, DbVariableReferenceExpression sourceVar) : base(scopeEntryKind, sourceVar)
        {
            this._prefixNames = new List<string>();
            this._isSourceVarDirty = true;
            this._isGroupVarDirty = true;
            this._varName = varName;
            this._varRef = sourceVar;
        }

        internal SourceScopeEntry(ScopeEntryKind scopeEntryKind, string varName, DbVariableReferenceExpression sourceVar, SourceVarKind varKind) : base(scopeEntryKind, sourceVar)
        {
            this._prefixNames = new List<string>();
            this._isSourceVarDirty = true;
            this._isGroupVarDirty = true;
            this._varName = varName;
            this._varRef = sourceVar;
            base._varKind = varKind;
        }

        internal void AddBindingPrefix(string prefixName)
        {
            this._isGroupVarDirty = true;
            this._isSourceVarDirty = true;
            this._prefixNames.Insert(0, prefixName);
        }

        private DbExpression RecalculateSourceVar(DbVariableReferenceExpression baseExpression)
        {
            DbCommandTree commandTree = baseExpression.CommandTree;
            DbExpression instance = baseExpression;
            if (this._prefixNames.Count > 0)
            {
                for (int i = 1; i < this._prefixNames.Count; i++)
                {
                    instance = commandTree.CreatePropertyExpression(this._prefixNames[i], instance);
                }
                return commandTree.CreatePropertyExpression(this._varName, instance);
            }
            return baseExpression;
        }

        internal void SetNewSourceBinding(DbVariableReferenceExpression sourceVar)
        {
            this._isGroupVarDirty = true;
            this._isSourceVarDirty = true;
            this._varRef = sourceVar;
        }

        internal DbExpression AggregateExpression
        {
            get
            {
                if (this._isGroupVarDirty)
                {
                    this._aggregateExpression = this.RecalculateSourceVar(this._groupVar);
                    this._isGroupVarDirty = false;
                }
                return this._aggregateExpression;
            }
        }

        internal override DbExpression Expression
        {
            get
            {
                if (this._isSourceVarDirty)
                {
                    base._expression = this.RecalculateSourceVar(this._varRef);
                    this._isSourceVarDirty = false;
                }
                return base._expression;
            }
        }

        internal DbVariableReferenceExpression GroupVarExpression
        {
            get => 
                this._groupVar;
            set
            {
                this._groupVar = value;
            }
        }

        internal string VarTag =>
            this._varTag;
    }
}

