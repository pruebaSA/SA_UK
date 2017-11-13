namespace System.Data.Common.Internal.Materialization
{
    using System;
    using System.Linq.Expressions;

    internal class TranslatorResult
    {
        private readonly Type RequestedType;
        private readonly System.Linq.Expressions.Expression ReturnedExpression;

        internal TranslatorResult(System.Linq.Expressions.Expression returnedExpression, Type requestedType)
        {
            this.RequestedType = requestedType;
            this.ReturnedExpression = returnedExpression;
        }

        internal System.Linq.Expressions.Expression Expression =>
            Translator.Emit_EnsureType(this.ReturnedExpression, this.RequestedType);
    }
}

