namespace System.Data.Linq.SqlClient
{
    using System;

    internal class SqlTypeConverter : SqlVisitor
    {
        protected SqlFactory sql;

        internal SqlTypeConverter(SqlFactory sql)
        {
            this.sql = sql;
        }

        private bool BothTypesAreStrings(ProviderType oldSqlType, ProviderType newSqlType) => 
            (oldSqlType.IsSameTypeFamily(this.sql.TypeProvider.From(typeof(string))) && newSqlType.IsSameTypeFamily(this.sql.TypeProvider.From(typeof(string))));

        private SqlExpression ConvertBitToString(SqlExpression expr, Type resultClrType)
        {
            SqlWhen[] whens = new SqlWhen[1];
            bool flag = true;
            whens[0] = new SqlWhen(expr, this.sql.ValueFromObject(flag.ToString(), false, expr.SourceExpression));
            bool flag2 = false;
            return new SqlSearchedCase(resultClrType, whens, this.sql.ValueFromObject(flag2.ToString(), false, expr.SourceExpression), expr.SourceExpression);
        }

        private SqlExpression ConvertDoubleToString(SqlExpression expr, Type resultClrType)
        {
            SqlExpression expression = this.sql.FunctionCall(typeof(void), "NVARCHAR", new SqlExpression[] { this.sql.ValueFromObject(30, false, expr.SourceExpression) }, expr.SourceExpression);
            return this.sql.FunctionCall(resultClrType, "CONVERT", new SqlExpression[] { expression, expr, this.sql.ValueFromObject(2, false, expr.SourceExpression) }, expr.SourceExpression);
        }

        private bool OldWillFitInNew(ProviderType oldSqlType, ProviderType newSqlType)
        {
            int? size;
            int? nullable2;
            if ((!newSqlType.IsLargeType && newSqlType.HasSizeOrIsLarge) && ((!oldSqlType.IsLargeType && oldSqlType.HasSizeOrIsLarge) && newSqlType.HasSizeOrIsLarge))
            {
                size = newSqlType.Size;
                nullable2 = oldSqlType.Size;
            }
            return ((size.GetValueOrDefault() < nullable2.GetValueOrDefault()) || (size.HasValue & nullable2.HasValue));
        }

        private bool StringConversionIsNeeded(ProviderType oldSqlType, ProviderType newSqlType)
        {
            if (!this.BothTypesAreStrings(oldSqlType, newSqlType))
            {
                return true;
            }
            bool flag = oldSqlType.IsFixedSize || newSqlType.IsFixedSize;
            return (!newSqlType.HasSizeOrIsLarge || (this.OldWillFitInNew(oldSqlType, newSqlType) && flag));
        }

        private bool StringConversionIsSafe(ProviderType oldSqlType, ProviderType newSqlType)
        {
            if (this.BothTypesAreStrings(oldSqlType, newSqlType) && newSqlType.HasSizeOrIsLarge)
            {
                return this.OldWillFitInNew(oldSqlType, newSqlType);
            }
            return true;
        }

        internal override SqlExpression VisitUnaryOperator(SqlUnary uo)
        {
            uo.Operand = this.VisitExpression(uo.Operand);
            if (uo.NodeType == SqlNodeType.Convert)
            {
                ProviderType sqlType = uo.Operand.SqlType;
                ProviderType newSqlType = uo.SqlType;
                Type nonNullableType = TypeSystem.GetNonNullableType(uo.Operand.ClrType);
                Type clrType = TypeSystem.GetNonNullableType(uo.ClrType);
                if (clrType == typeof(char))
                {
                    if (nonNullableType == typeof(bool))
                    {
                        throw Error.ConvertToCharFromBoolNotSupported();
                    }
                    if (sqlType.IsNumeric)
                    {
                        return this.sql.FunctionCall(uo.ClrType, "NCHAR", new SqlExpression[] { uo.Operand }, uo.SourceExpression);
                    }
                    if (!this.StringConversionIsSafe(sqlType, newSqlType))
                    {
                        throw Error.UnsafeStringConversion(sqlType.ToQueryString(), newSqlType.ToQueryString());
                    }
                    if (this.StringConversionIsNeeded(sqlType, newSqlType))
                    {
                        uo.SetSqlType(this.sql.TypeProvider.From(uo.ClrType, sqlType.HasSizeOrIsLarge ? sqlType.Size : null));
                    }
                    return uo;
                }
                if (((nonNullableType == typeof(char)) && (sqlType.IsChar || sqlType.IsString)) && newSqlType.IsNumeric)
                {
                    return this.sql.FunctionCall(clrType, this.sql.TypeProvider.From(typeof(int)), "UNICODE", new SqlExpression[] { uo.Operand }, uo.SourceExpression);
                }
                if (clrType != typeof(string))
                {
                    return uo;
                }
                if (nonNullableType == typeof(double))
                {
                    return this.ConvertDoubleToString(uo.Operand, uo.ClrType);
                }
                if (nonNullableType == typeof(bool))
                {
                    return this.ConvertBitToString(uo.Operand, uo.ClrType);
                }
                if (!this.StringConversionIsSafe(sqlType, newSqlType))
                {
                    throw Error.UnsafeStringConversion(sqlType.ToQueryString(), newSqlType.ToQueryString());
                }
                if (this.StringConversionIsNeeded(sqlType, newSqlType))
                {
                    uo.SetSqlType(this.sql.TypeProvider.From(uo.ClrType, sqlType.HasSizeOrIsLarge ? sqlType.Size : null));
                }
            }
            return uo;
        }
    }
}

