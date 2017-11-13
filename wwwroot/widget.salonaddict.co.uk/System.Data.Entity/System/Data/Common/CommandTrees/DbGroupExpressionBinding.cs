namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbGroupExpressionBinding
    {
        private ExpressionLink _expr;
        private string _groupVarName;
        private string _varName;
        private TypeUsage _varType;

        internal DbGroupExpressionBinding(DbCommandTree cmdTree, DbExpression input, string varName, string groupVarName)
        {
            EntityUtil.CheckArgumentNull<string>(varName, "varName");
            EntityUtil.CheckArgumentNull<string>(groupVarName, "groupVarName");
            this._expr = new ExpressionLink("Expression", cmdTree, input);
            if (string.IsNullOrEmpty(varName))
            {
                throw EntityUtil.Argument(Strings.Cqt_Binding_VariableNameNotValid, "varName");
            }
            if (string.IsNullOrEmpty(groupVarName))
            {
                throw EntityUtil.Argument(Strings.Cqt_GroupBinding_GroupVariableNameNotValid, "groupVarName");
            }
            TypeUsage elementType = null;
            if (!TypeHelpers.TryGetCollectionElementType(input.ResultType, out elementType))
            {
                throw EntityUtil.Argument(Strings.Cqt_GroupBinding_CollectionRequired, "input");
            }
            this._varName = varName;
            this._varType = elementType;
            this._groupVarName = groupVarName;
        }

        internal static void Check(string strName, DbGroupExpressionBinding binding, DbCommandTree owner)
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

        internal DbVariableReferenceExpression GroupVariable =>
            this.Expression.CommandTree.CreateVariableReferenceExpression(this._groupVarName, this._varType);

        public string GroupVariableName =>
            this._groupVarName;

        public TypeUsage GroupVariableType =>
            this._varType;

        internal DbVariableReferenceExpression Variable =>
            this._expr.Expression.CommandTree.CreateVariableReferenceExpression(this._varName, this._varType);

        public string VariableName =>
            this._varName;

        public TypeUsage VariableType =>
            this._varType;
    }
}

