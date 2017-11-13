namespace System.Data.Linq.SqlClient
{
    using System;

    internal class LiteralValidator : SqlVisitor
    {
        internal override SqlExpression VisitBinaryOperator(SqlBinary bo)
        {
            bo.Left = this.VisitExpression(bo.Left);
            return bo;
        }

        internal override SqlExpression VisitValue(SqlValue value)
        {
            if (((!value.IsClientSpecified && value.ClrType.IsClass) && ((value.ClrType != typeof(string)) && (value.ClrType != typeof(Type)))) && (value.Value != null))
            {
                throw Error.ClassLiteralsNotAllowed(value.ClrType);
            }
            return value;
        }
    }
}

