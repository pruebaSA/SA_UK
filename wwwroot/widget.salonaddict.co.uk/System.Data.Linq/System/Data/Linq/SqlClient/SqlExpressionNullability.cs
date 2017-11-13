namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;

    internal static class SqlExpressionNullability
    {
        private static bool? CanBeNull(IEnumerable<SqlExpression> exprs)
        {
            bool flag = false;
            foreach (SqlExpression expression in exprs)
            {
                bool? nullable = CanBeNull(expression);
                if (nullable == true)
                {
                    return true;
                }
                if (!nullable.HasValue)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                return null;
            }
            return false;
        }

        internal static bool? CanBeNull(SqlExpression expr)
        {
            switch (expr.NodeType)
            {
                case SqlNodeType.DiscriminatedType:
                case SqlNodeType.Exists:
                case SqlNodeType.IsNotNull:
                case SqlNodeType.IsNull:
                case SqlNodeType.Grouping:
                case SqlNodeType.Multiset:
                case SqlNodeType.New:
                case SqlNodeType.ObjectType:
                    return false;

                case SqlNodeType.Div:
                case SqlNodeType.BitAnd:
                case SqlNodeType.BitOr:
                case SqlNodeType.BitXor:
                case SqlNodeType.Add:
                case SqlNodeType.Concat:
                case SqlNodeType.Mod:
                case SqlNodeType.Mul:
                case SqlNodeType.Sub:
                {
                    bool? nullable5;
                    SqlBinary binary = (SqlBinary) expr;
                    bool? nullable = CanBeNull(binary.Left);
                    bool? nullable2 = CanBeNull(binary.Right);
                    if (nullable == false)
                    {
                        nullable5 = nullable2;
                    }
                    return new bool?(nullable5.GetValueOrDefault() || !nullable5.HasValue);
                }
                case SqlNodeType.ExprSet:
                {
                    SqlExprSet set = (SqlExprSet) expr;
                    return CanBeNull(set.Expressions);
                }
                case SqlNodeType.BitNot:
                case SqlNodeType.Negate:
                {
                    SqlUnary unary = (SqlUnary) expr;
                    return CanBeNull(unary.Operand);
                }
                case SqlNodeType.Column:
                {
                    SqlColumn column = (SqlColumn) expr;
                    if (column.MetaMember == null)
                    {
                        if (column.Expression != null)
                        {
                            return CanBeNull(column.Expression);
                        }
                        return null;
                    }
                    return new bool?(column.MetaMember.CanBeNull);
                }
                case SqlNodeType.ColumnRef:
                {
                    SqlColumnRef ref2 = (SqlColumnRef) expr;
                    return CanBeNull(ref2.Column);
                }
                case SqlNodeType.Lift:
                {
                    SqlLift lift = (SqlLift) expr;
                    return CanBeNull(lift.Expression);
                }
                case SqlNodeType.Value:
                    return new bool?(((SqlValue) expr).Value == null);

                case SqlNodeType.OuterJoinedValue:
                    return true;

                case SqlNodeType.SimpleCase:
                {
                    SqlSimpleCase @case = (SqlSimpleCase) expr;
                    return CanBeNull((IEnumerable<SqlExpression>) (from w in @case.Whens select w.Value));
                }
            }
            return null;
        }
    }
}

