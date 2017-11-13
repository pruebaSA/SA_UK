namespace System.Data.Linq.SqlClient
{
    using System;

    internal class ExpectRationalizedBooleans : SqlBooleanMismatchVisitor
    {
        internal ExpectRationalizedBooleans()
        {
        }

        internal override SqlExpression ConvertPredicateToValue(SqlExpression predicateExpression)
        {
            throw Error.ExpectedBitFoundPredicate();
        }

        internal override SqlExpression ConvertValueToPredicate(SqlExpression bitExpression)
        {
            throw Error.ExpectedPredicateFoundBit();
        }
    }
}

