namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.ServiceModel;

    internal class XPathConjunctExpr : XPathExpr
    {
        internal XPathConjunctExpr(XPathExprType type, ValueDataType returnType, XPathExpr left, XPathExpr right) : base(type, returnType)
        {
            if ((left == null) || (right == null))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new QueryCompileException(QueryCompileError.InvalidExpression));
            }
            base.SubExpr.Add(left);
            base.SubExpr.Add(right);
        }

        internal XPathExpr Left =>
            base.SubExpr[0];

        internal XPathExpr Right =>
            base.SubExpr[1];
    }
}

