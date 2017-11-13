namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbExpressionBinding
    {
        private ExpressionLink _expr;
        private string _varName;
        private TypeUsage _varType;

        internal DbExpressionBinding(DbCommandTree cmdTree, DbExpression input, string varName)
        {
            EntityUtil.CheckArgumentNull<string>(varName, "varName");
            this._expr = new ExpressionLink("Expression", cmdTree, input);
            if (string.IsNullOrEmpty(varName))
            {
                throw EntityUtil.Argument(Strings.Cqt_Binding_VariableNameNotValid, "varName");
            }
            TypeUsage elementType = null;
            if (!TypeHelpers.TryGetCollectionElementType(input.ResultType, out elementType))
            {
                throw EntityUtil.Argument(Strings.Cqt_Binding_CollectionRequired, "input");
            }
            this._varName = varName;
            this._varType = elementType;
        }

        internal static void Check(string strName, DbExpressionBinding binding, DbCommandTree owner)
        {
            if (binding == null)
            {
                throw EntityUtil.ArgumentNull(strName);
            }
            if (owner != binding.Expression.CommandTree)
            {
                throw EntityUtil.Argument(Strings.Cqt_General_TreeMismatch, strName);
            }
        }

        public DbExpression Expression
        {
            get => 
                this._expr.Expression;
            internal set
            {
                this._expr.Expression = value;
            }
        }

        internal DbVariableReferenceExpression Variable =>
            this._expr.Expression.CommandTree.CreateVariableReferenceExpression(this._varName, this._varType);

        public string VariableName =>
            this._varName;

        public TypeUsage VariableType =>
            this._varType;
    }
}

