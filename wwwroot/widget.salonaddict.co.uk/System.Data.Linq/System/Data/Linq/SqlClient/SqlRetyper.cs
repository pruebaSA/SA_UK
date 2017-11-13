namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;

    internal class SqlRetyper
    {
        private Visitor visitor;

        internal SqlRetyper(TypeSystemProvider typeProvider, MetaModel model)
        {
            this.visitor = new Visitor(typeProvider, model);
        }

        internal SqlNode Retype(SqlNode node) => 
            this.visitor.Visit(node);

        private class Visitor : SqlVisitor
        {
            private SqlFactory sql;
            private TypeSystemProvider typeProvider;

            internal Visitor(TypeSystemProvider typeProvider, MetaModel model)
            {
                this.sql = new SqlFactory(typeProvider, model);
                this.typeProvider = typeProvider;
            }

            private static bool CanDbConvert(Type from, Type to)
            {
                from = TypeSystem.GetNonNullableType(from);
                to = TypeSystem.GetNonNullableType(to);
                if (from == to)
                {
                    return true;
                }
                if (to.IsAssignableFrom(from))
                {
                    return true;
                }
                TypeCode typeCode = Type.GetTypeCode(to);
                TypeCode code2 = Type.GetTypeCode(from);
                switch (typeCode)
                {
                    case TypeCode.Int16:
                        return ((code2 == TypeCode.Byte) || (code2 == TypeCode.SByte));

                    case TypeCode.UInt16:
                        return ((code2 == TypeCode.Byte) || (code2 == TypeCode.SByte));

                    case TypeCode.Int32:
                        switch (code2)
                        {
                            case TypeCode.Byte:
                            case TypeCode.SByte:
                            case TypeCode.Int16:
                                return true;
                        }
                        return (code2 == TypeCode.UInt16);

                    case TypeCode.UInt32:
                        switch (code2)
                        {
                            case TypeCode.Byte:
                            case TypeCode.SByte:
                            case TypeCode.Int16:
                                return true;
                        }
                        return (code2 == TypeCode.UInt16);

                    case TypeCode.Int64:
                        switch (code2)
                        {
                            case TypeCode.Byte:
                            case TypeCode.SByte:
                            case TypeCode.Int16:
                            case TypeCode.UInt16:
                            case TypeCode.Int32:
                                return true;
                        }
                        return (code2 == TypeCode.UInt32);

                    case TypeCode.UInt64:
                        switch (code2)
                        {
                            case TypeCode.Byte:
                            case TypeCode.SByte:
                            case TypeCode.Int16:
                            case TypeCode.UInt16:
                            case TypeCode.Int32:
                                return true;
                        }
                        return (code2 == TypeCode.UInt32);

                    case TypeCode.Double:
                        return (code2 == TypeCode.Single);

                    case TypeCode.Decimal:
                        return ((code2 == TypeCode.Single) || (code2 == TypeCode.Double));
                }
                return false;
            }

            private void CoerceBinaryArgs(ref SqlExpression arg1, ref SqlExpression arg2)
            {
                if ((arg1.SqlType != null) && (arg2.SqlType != null))
                {
                    if (!arg1.SqlType.IsSameTypeFamily(arg2.SqlType))
                    {
                        if ((arg1.ClrType != typeof(bool)) && (arg2.ClrType != typeof(bool)))
                        {
                            if (arg2.NodeType == SqlNodeType.Value)
                            {
                                SqlValue value2 = (SqlValue) arg2;
                                object obj2 = value2.Value;
                                if (!value2.ClrType.IsAssignableFrom(arg1.ClrType))
                                {
                                    obj2 = DBConvert.ChangeType(obj2, arg1.ClrType);
                                }
                                ProviderType sqlType = this.typeProvider.ChangeTypeFamilyTo(arg2.SqlType, arg1.SqlType);
                                arg2 = this.sql.Value(arg1.ClrType, sqlType, obj2, value2.IsClientSpecified, arg2.SourceExpression);
                            }
                            else if (arg1.NodeType == SqlNodeType.Value)
                            {
                                SqlValue value3 = (SqlValue) arg1;
                                object obj3 = value3.Value;
                                if (!value3.ClrType.IsAssignableFrom(arg2.ClrType))
                                {
                                    obj3 = DBConvert.ChangeType(obj3, arg2.ClrType);
                                }
                                ProviderType type2 = this.typeProvider.ChangeTypeFamilyTo(arg1.SqlType, arg2.SqlType);
                                arg1 = this.sql.Value(arg2.ClrType, type2, obj3, value3.IsClientSpecified, arg1.SourceExpression);
                            }
                            else if ((arg2.NodeType == SqlNodeType.ClientParameter) && (arg2.SqlType != arg1.SqlType))
                            {
                                ((SqlClientParameter) arg2).SetSqlType(arg1.SqlType);
                            }
                            else if ((arg1.NodeType == SqlNodeType.ClientParameter) && (arg1.SqlType != arg2.SqlType))
                            {
                                ((SqlClientParameter) arg1).SetSqlType(arg2.SqlType);
                            }
                            else
                            {
                                int num = arg1.SqlType.ComparePrecedenceTo(arg2.SqlType);
                                if (num > 0)
                                {
                                    arg2 = this.sql.UnaryConvert(arg1.ClrType, arg1.SqlType, arg2, arg2.SourceExpression);
                                }
                                else if (num < 0)
                                {
                                    arg1 = this.sql.UnaryConvert(arg2.ClrType, arg2.SqlType, arg1, arg1.SourceExpression);
                                }
                            }
                        }
                    }
                    else if (((arg1.SqlType.HasPrecisionAndScale && arg2.SqlType.HasPrecisionAndScale) && (arg1.SqlType != arg2.SqlType)) || (this.sql.IsSqlHighPrecisionDateTimeType(arg1) || this.sql.IsSqlHighPrecisionDateTimeType(arg2)))
                    {
                        ProviderType bestType = this.typeProvider.GetBestType(arg1.SqlType, arg2.SqlType);
                        SqlSimpleTypeExpression expression = arg1 as SqlSimpleTypeExpression;
                        if (expression != null)
                        {
                            expression.SetSqlType(bestType);
                        }
                        SqlSimpleTypeExpression expression2 = arg2 as SqlSimpleTypeExpression;
                        if (expression2 != null)
                        {
                            expression2.SetSqlType(bestType);
                        }
                    }
                    else if (this.sql.IsSqlDateType(arg1) && !this.sql.IsSqlHighPrecisionDateTimeType(arg2))
                    {
                        SqlSimpleTypeExpression expression3 = arg2 as SqlSimpleTypeExpression;
                        if (expression3 != null)
                        {
                            expression3.SetSqlType(arg1.SqlType);
                        }
                    }
                    else if (this.sql.IsSqlDateType(arg2) && !this.sql.IsSqlHighPrecisionDateTimeType(arg1))
                    {
                        SqlSimpleTypeExpression expression4 = arg1 as SqlSimpleTypeExpression;
                        if (expression4 != null)
                        {
                            expression4.SetSqlType(arg2.SqlType);
                        }
                    }
                }
            }

            private void CoerceToFirst(SqlExpression arg1, ref SqlExpression arg2)
            {
                if ((arg1.SqlType != null) && (arg2.SqlType != null))
                {
                    if (arg2.NodeType == SqlNodeType.Value)
                    {
                        SqlValue value2 = (SqlValue) arg2;
                        arg2 = this.sql.Value(arg1.ClrType, arg1.SqlType, DBConvert.ChangeType(value2.Value, arg1.ClrType), value2.IsClientSpecified, arg2.SourceExpression);
                    }
                    else if ((arg2.NodeType == SqlNodeType.ClientParameter) && (arg2.SqlType != arg1.SqlType))
                    {
                        ((SqlClientParameter) arg2).SetSqlType(arg1.SqlType);
                    }
                    else
                    {
                        arg2 = this.sql.UnaryConvert(arg1.ClrType, arg1.SqlType, arg2, arg2.SourceExpression);
                    }
                }
            }

            internal override SqlStatement VisitAssign(SqlAssign sa)
            {
                base.VisitAssign(sa);
                SqlExpression rValue = sa.RValue;
                this.CoerceToFirst(sa.LValue, ref rValue);
                sa.RValue = rValue;
                return sa;
            }

            internal override SqlExpression VisitBinaryOperator(SqlBinary bo)
            {
                base.VisitBinaryOperator(bo);
                if ((bo.NodeType.IsComparisonOperator() && (bo.Left.ClrType != typeof(bool))) && (bo.Right.ClrType != typeof(bool)))
                {
                    if (bo.Left.NodeType == SqlNodeType.Convert)
                    {
                        SqlUnary left = (SqlUnary) bo.Left;
                        if (CanDbConvert(left.Operand.ClrType, bo.Right.ClrType) && (left.Operand.SqlType.ComparePrecedenceTo(bo.Right.SqlType) != 1))
                        {
                            return this.VisitBinaryOperator(new SqlBinary(bo.NodeType, bo.ClrType, bo.SqlType, left.Operand, bo.Right));
                        }
                    }
                    if (bo.Right.NodeType == SqlNodeType.Convert)
                    {
                        SqlUnary right = (SqlUnary) bo.Right;
                        if (CanDbConvert(right.Operand.ClrType, bo.Left.ClrType) && (right.Operand.SqlType.ComparePrecedenceTo(bo.Left.SqlType) != 1))
                        {
                            return this.VisitBinaryOperator(new SqlBinary(bo.NodeType, bo.ClrType, bo.SqlType, bo.Left, right.Operand));
                        }
                    }
                }
                if ((bo.Right != null) && (bo.NodeType != SqlNodeType.Concat))
                {
                    SqlExpression expression = bo.Left;
                    SqlExpression expression2 = bo.Right;
                    this.CoerceBinaryArgs(ref expression, ref expression2);
                    if ((bo.Left != expression) || (bo.Right != expression2))
                    {
                        bo = this.sql.Binary(bo.NodeType, expression, expression2);
                    }
                    bo.SetSqlType(this.typeProvider.PredictTypeForBinary(bo.NodeType, expression.SqlType, expression2.SqlType));
                }
                if (bo.NodeType.IsComparisonOperator())
                {
                    Func<SqlExpression, SqlExpression, bool> func = (expr, val) => (((val.NodeType == SqlNodeType.Value) || (val.NodeType == SqlNodeType.ClientParameter)) && (((expr.NodeType != SqlNodeType.Value) && (expr.NodeType != SqlNodeType.ClientParameter)) && val.SqlType.IsUnicodeType)) && !expr.SqlType.IsUnicodeType;
                    SqlSimpleTypeExpression expression3 = null;
                    if (func(bo.Left, bo.Right))
                    {
                        expression3 = (SqlSimpleTypeExpression) bo.Right;
                    }
                    else if (func(bo.Right, bo.Left))
                    {
                        expression3 = (SqlSimpleTypeExpression) bo.Left;
                    }
                    if (expression3 != null)
                    {
                        expression3.SetSqlType(expression3.SqlType.GetNonUnicodeEquivalent());
                    }
                }
                return bo;
            }

            internal override SqlExpression VisitColumn(SqlColumn col) => 
                base.VisitColumn(col);

            internal override SqlExpression VisitFunctionCall(SqlFunctionCall fc)
            {
                int num = 0;
                int count = fc.Arguments.Count;
                while (num < count)
                {
                    fc.Arguments[num] = this.VisitExpression(fc.Arguments[num]);
                    num++;
                }
                if ((fc.Arguments.Count > 0) && (fc.Arguments[0].SqlType != null))
                {
                    ProviderType type = this.typeProvider.ReturnTypeOfFunction(fc);
                    if (type != null)
                    {
                        fc.SetSqlType(type);
                    }
                }
                return fc;
            }

            internal override SqlExpression VisitLike(SqlLike like)
            {
                base.VisitLike(like);
                if ((!like.Expression.SqlType.IsUnicodeType && like.Pattern.SqlType.IsUnicodeType) && ((like.Pattern.NodeType == SqlNodeType.Value) || (like.Pattern.NodeType == SqlNodeType.ClientParameter)))
                {
                    SqlSimpleTypeExpression pattern = (SqlSimpleTypeExpression) like.Pattern;
                    pattern.SetSqlType(pattern.SqlType.GetNonUnicodeEquivalent());
                }
                return like;
            }

            internal override SqlExpression VisitScalarSubSelect(SqlSubSelect ss)
            {
                base.VisitScalarSubSelect(ss);
                ss.SetSqlType(ss.Select.Selection.SqlType);
                return ss;
            }

            internal override SqlExpression VisitSearchedCase(SqlSearchedCase c)
            {
                base.VisitSearchedCase(c);
                ProviderType type = c.Whens[0].Value.SqlType;
                for (int i = 1; i < c.Whens.Count; i++)
                {
                    ProviderType sqlType = c.Whens[i].Value.SqlType;
                    type = this.typeProvider.GetBestType(type, sqlType);
                }
                if (c.Else != null)
                {
                    ProviderType typeB = c.Else.SqlType;
                    type = this.typeProvider.GetBestType(type, typeB);
                }
                foreach (SqlWhen when in from w in c.Whens
                    where (w.Value.SqlType != type) && !w.Value.SqlType.IsRuntimeOnlyType
                    select w)
                {
                    when.Value = this.sql.UnaryConvert(when.Value.ClrType, type, when.Value, when.Value.SourceExpression);
                }
                if (((c.Else != null) && (c.Else.SqlType != type)) && !c.Else.SqlType.IsRuntimeOnlyType)
                {
                    c.Else = this.sql.UnaryConvert(c.Else.ClrType, type, c.Else, c.Else.SourceExpression);
                }
                return c;
            }

            internal override SqlExpression VisitSimpleCase(SqlSimpleCase c)
            {
                base.VisitSimpleCase(c);
                ProviderType type = c.Whens[0].Value.SqlType;
                for (int i = 1; i < c.Whens.Count; i++)
                {
                    ProviderType sqlType = c.Whens[i].Value.SqlType;
                    type = this.typeProvider.GetBestType(type, sqlType);
                }
                foreach (SqlWhen when in from w in c.Whens
                    where (w.Value.SqlType != type) && !w.Value.SqlType.IsRuntimeOnlyType
                    select w)
                {
                    when.Value = this.sql.UnaryConvert(when.Value.ClrType, type, when.Value, when.Value.SourceExpression);
                }
                return c;
            }

            internal override SqlExpression VisitUnaryOperator(SqlUnary uo)
            {
                base.VisitUnaryOperator(uo);
                if (((uo.NodeType != SqlNodeType.Convert) && (uo.Operand != null)) && (uo.Operand.SqlType != null))
                {
                    uo.SetSqlType(this.typeProvider.PredictTypeForUnary(uo.NodeType, uo.Operand.SqlType));
                }
                return uo;
            }
        }
    }
}

