namespace System.Data.Common.CommandTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common.CommandTrees.Internal;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;

    public sealed class DbFunctionExpression : DbExpression
    {
        private IList<DbExpression> _args;
        private EdmFunction _functionInfo;
        private ExpressionLink _lambdaBody;

        internal DbFunctionExpression(DbCommandTree cmdTree, EdmFunction function, DbExpression lambdaBody, IList<DbExpression> args) : base(cmdTree, DbExpressionKind.Function)
        {
            if (lambdaBody == null)
            {
                cmdTree.TypeHelper.CheckFunction(function);
            }
            if (!function.IsComposableAttribute)
            {
                throw EntityUtil.Argument(Strings.Cqt_Function_NonComposableInExpression, "function");
            }
            if (!string.IsNullOrEmpty(function.CommandTextAttribute))
            {
                throw EntityUtil.Argument(Strings.Cqt_Function_CommandTextInExpression, "function");
            }
            if ((function.ReturnParameter == null) || TypeSemantics.IsNullOrNullType(function.ReturnParameter.TypeUsage))
            {
                throw EntityUtil.Argument(Strings.Cqt_Function_VoidResultInvalid, "function");
            }
            EntityUtil.CheckArgumentNull<IList<DbExpression>>(args, "args");
            this._args = new ExpressionList("Arguments", cmdTree, function.Parameters, args);
            this._functionInfo = function;
            if (lambdaBody != null)
            {
                this._lambdaBody = new ExpressionLink("LambdaBody", cmdTree, lambdaBody);
            }
            base.ResultType = function.ReturnParameter.TypeUsage;
        }

        public override void Accept(DbExpressionVisitor visitor)
        {
            if (visitor == null)
            {
                throw EntityUtil.ArgumentNull("visitor");
            }
            visitor.Visit(this);
        }

        public override TResultType Accept<TResultType>(DbExpressionVisitor<TResultType> visitor) => 
            visitor?.Visit(this);

        public IList<DbExpression> Arguments =>
            this._args;

        public EdmFunction Function =>
            this._functionInfo;

        internal bool IsLambda =>
            (this._lambdaBody != null);

        internal DbExpression LambdaBody
        {
            get
            {
                if (this._lambdaBody != null)
                {
                    return this._lambdaBody.Expression;
                }
                return null;
            }
            set
            {
                if (this._lambdaBody == null)
                {
                    throw EntityUtil.NotSupported(Strings.Cqt_Function_BodyOnlyValidForLambda);
                }
                this._lambdaBody.Expression = value;
            }
        }

        internal static string LambdaFunctionName =>
            "Lambda";

        internal static string LambdaFunctionNameSpace =>
            "System.Data.Common.CommandTrees.LambdaFunctions";
    }
}

