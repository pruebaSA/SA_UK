namespace System.Data.Common.EntitySql
{
    using System;

    internal sealed class AggregateAstNodeInfo
    {
        private MethodExpr _methodExpr;
        private int _scopeIndex;
        internal const int NonCorrelatedScope = -2147483648;

        internal AggregateAstNodeInfo(MethodExpr methodAstNode)
        {
            this._methodExpr = methodAstNode;
            this._scopeIndex = -2147483648;
        }

        internal void AssertMethodExprEquivalent(MethodExpr other)
        {
        }

        internal int ScopeIndex
        {
            get => 
                this._scopeIndex;
            set
            {
                this._scopeIndex = value;
            }
        }
    }
}

