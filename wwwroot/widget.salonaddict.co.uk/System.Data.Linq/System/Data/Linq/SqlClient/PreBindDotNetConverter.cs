namespace System.Data.Linq.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq.Mapping;
    using System.Globalization;
    using System.Linq.Expressions;

    internal static class PreBindDotNetConverter
    {
        internal static bool CanConvert(SqlNode node)
        {
            SqlBinary bo = node as SqlBinary;
            if ((bo == null) || (!IsCompareToValue(bo) && !IsVbCompareStringEqualsValue(bo)))
            {
                SqlMember m = node as SqlMember;
                if ((m == null) || !IsSupportedMember(m))
                {
                    SqlMethodCall mc = node as SqlMethodCall;
                    if ((mc == null) || (!IsSupportedMethod(mc) && !IsSupportedVbHelperMethod(mc)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal static SqlNode Convert(SqlNode node, SqlFactory sql, MetaModel model) => 
            new Visitor(sql, model).Visit(node);

        private static bool IsCompareMethod(SqlMethodCall call) => 
            (((call.Method.IsStatic && (call.Method.Name == "Compare")) && (call.Arguments.Count > 1)) && (call.Method.ReturnType == typeof(int)));

        private static bool IsCompareToMethod(SqlMethodCall call) => 
            (((!call.Method.IsStatic && (call.Method.Name == "CompareTo")) && (call.Arguments.Count == 1)) && (call.Method.ReturnType == typeof(int)));

        private static bool IsCompareToValue(SqlBinary bo)
        {
            if ((!IsComparison(bo.NodeType) || (bo.Left.NodeType != SqlNodeType.MethodCall)) || (bo.Right.NodeType != SqlNodeType.Value))
            {
                return false;
            }
            SqlMethodCall left = (SqlMethodCall) bo.Left;
            if (!IsCompareToMethod(left))
            {
                return IsCompareMethod(left);
            }
            return true;
        }

        private static bool IsComparison(SqlNodeType nodeType)
        {
            switch (nodeType)
            {
                case SqlNodeType.EQ:
                case SqlNodeType.EQ2V:
                case SqlNodeType.LE:
                case SqlNodeType.LT:
                case SqlNodeType.GE:
                case SqlNodeType.GT:
                case SqlNodeType.NE:
                case SqlNodeType.NE2V:
                    return true;
            }
            return false;
        }

        private static bool IsNullableHasValue(SqlMember m) => 
            (TypeSystem.IsNullableType(m.Expression.ClrType) && (m.Member.Name == "HasValue"));

        private static bool IsNullableValue(SqlMember m) => 
            (TypeSystem.IsNullableType(m.Expression.ClrType) && (m.Member.Name == "Value"));

        private static bool IsSupportedMember(SqlMember m)
        {
            if (!IsNullableHasValue(m))
            {
                return IsNullableHasValue(m);
            }
            return true;
        }

        private static bool IsSupportedMethod(SqlMethodCall mc)
        {
            if (mc.Method.IsStatic)
            {
                switch (mc.Method.Name)
                {
                    case "op_Equality":
                    case "op_Inequality":
                    case "op_LessThan":
                    case "op_LessThanOrEqual":
                    case "op_GreaterThan":
                    case "op_GreaterThanOrEqual":
                    case "op_Multiply":
                    case "op_Division":
                    case "op_Subtraction":
                    case "op_Addition":
                    case "op_Modulus":
                    case "op_BitwiseAnd":
                    case "op_BitwiseOr":
                    case "op_ExclusiveOr":
                    case "op_UnaryNegation":
                    case "op_OnesComplement":
                    case "op_False":
                        return true;

                    case "Equals":
                        return (mc.Arguments.Count == 2);

                    case "Concat":
                        return (mc.Method.DeclaringType == typeof(string));
                }
                return false;
            }
            return (((mc.Method.Name == "Equals") && (mc.Arguments.Count == 1)) || ((mc.Method.Name == "GetType") && (mc.Arguments.Count == 0)));
        }

        private static bool IsSupportedVbHelperMethod(SqlMethodCall mc) => 
            IsVbIIF(mc);

        private static bool IsVbCompareString(SqlMethodCall call) => 
            ((call.Method.IsStatic && (call.Method.DeclaringType.FullName == "Microsoft.VisualBasic.CompilerServices.Operators")) && (call.Method.Name == "CompareString"));

        private static bool IsVbCompareStringEqualsValue(SqlBinary bo) => 
            (((IsComparison(bo.NodeType) && (bo.Left.NodeType == SqlNodeType.MethodCall)) && (bo.Right.NodeType == SqlNodeType.Value)) && IsVbCompareString((SqlMethodCall) bo.Left));

        private static bool IsVbIIF(SqlMethodCall mc) => 
            ((mc.Method.IsStatic && (mc.Method.DeclaringType.FullName == "Microsoft.VisualBasic.Interaction")) && (mc.Method.Name == "IIf"));

        private class Visitor : SqlVisitor
        {
            private MetaModel model;
            private SqlFactory sql;

            internal Visitor(SqlFactory sql, MetaModel model)
            {
                this.sql = sql;
                this.model = model;
            }

            private SqlExpression CreateComparison(SqlExpression a, SqlExpression b, Expression source)
            {
                SqlExpression match = this.sql.Binary(SqlNodeType.LT, a, b);
                SqlExpression expression2 = this.sql.Binary(SqlNodeType.EQ2V, a, b);
                return this.sql.SearchedCase(new SqlWhen[] { new SqlWhen(match, this.sql.ValueFromObject(-1, false, source)), new SqlWhen(expression2, this.sql.ValueFromObject(0, false, source)) }, this.sql.ValueFromObject(1, false, source), source);
            }

            private SqlBinary MakeCompareTo(SqlExpression left, SqlExpression right, SqlNodeType op, int iValue)
            {
                if (iValue == 0)
                {
                    return this.sql.Binary(op, left, right);
                }
                if ((op == SqlNodeType.EQ) || (op == SqlNodeType.EQ2V))
                {
                    switch (iValue)
                    {
                        case -1:
                            return this.sql.Binary(SqlNodeType.LT, left, right);

                        case 1:
                            return this.sql.Binary(SqlNodeType.GT, left, right);
                    }
                }
                return null;
            }

            private SqlExpression TranslateVbIIF(SqlMethodCall mc)
            {
                if (mc.Arguments[1].ClrType != mc.Arguments[2].ClrType)
                {
                    throw System.Data.Linq.SqlClient.Error.IifReturnTypesMustBeEqual(mc.Arguments[1].ClrType.Name, mc.Arguments[2].ClrType.Name);
                }
                List<SqlWhen> list = new List<SqlWhen>(1) {
                    new SqlWhen(mc.Arguments[0], mc.Arguments[1])
                };
                SqlExpression @else = mc.Arguments[2];
                while (@else.NodeType == SqlNodeType.SearchedCase)
                {
                    SqlSearchedCase @case = (SqlSearchedCase) @else;
                    list.AddRange(@case.Whens);
                    @else = @case.Else;
                }
                return this.sql.SearchedCase(list.ToArray(), @else, mc.SourceExpression);
            }

            internal override SqlExpression VisitBinaryOperator(SqlBinary bo)
            {
                if (PreBindDotNetConverter.IsCompareToValue(bo))
                {
                    SqlMethodCall left = (SqlMethodCall) bo.Left;
                    if (PreBindDotNetConverter.IsCompareToMethod(left))
                    {
                        int iValue = Convert.ToInt32(base.Eval(bo.Right), CultureInfo.InvariantCulture);
                        bo = this.MakeCompareTo(left.Object, left.Arguments[0], bo.NodeType, iValue) ?? bo;
                    }
                    else if (PreBindDotNetConverter.IsCompareMethod(left))
                    {
                        int num2 = Convert.ToInt32(base.Eval(bo.Right), CultureInfo.InvariantCulture);
                        bo = this.MakeCompareTo(left.Arguments[0], left.Arguments[1], bo.NodeType, num2) ?? bo;
                    }
                }
                else if (PreBindDotNetConverter.IsVbCompareStringEqualsValue(bo))
                {
                    SqlMethodCall call2 = (SqlMethodCall) bo.Left;
                    int num3 = Convert.ToInt32(base.Eval(bo.Right), CultureInfo.InvariantCulture);
                    SqlValue value2 = call2.Arguments[1] as SqlValue;
                    if ((value2 != null) && (value2.Value == null))
                    {
                        SqlValue right = new SqlValue(value2.ClrType, value2.SqlType, string.Empty, value2.IsClientSpecified, value2.SourceExpression);
                        bo = this.MakeCompareTo(call2.Arguments[0], right, bo.NodeType, num3) ?? bo;
                    }
                    else
                    {
                        bo = this.MakeCompareTo(call2.Arguments[0], call2.Arguments[1], bo.NodeType, num3) ?? bo;
                    }
                }
                return base.VisitBinaryOperator(bo);
            }

            internal override SqlNode VisitMember(SqlMember m)
            {
                m.Expression = this.VisitExpression(m.Expression);
                if (PreBindDotNetConverter.IsNullableValue(m))
                {
                    return this.sql.UnaryValueOf(m.Expression, m.SourceExpression);
                }
                if (PreBindDotNetConverter.IsNullableHasValue(m))
                {
                    return this.sql.Unary(SqlNodeType.IsNotNull, m.Expression, m.SourceExpression);
                }
                return m;
            }

            internal override SqlExpression VisitMethodCall(SqlMethodCall mc)
            {
                mc.Object = this.VisitExpression(mc.Object);
                int num = 0;
                int count = mc.Arguments.Count;
                while (num < count)
                {
                    mc.Arguments[num] = this.VisitExpression(mc.Arguments[num]);
                    num++;
                }
                if (mc.Method.IsStatic)
                {
                    if ((mc.Method.Name == "Equals") && (mc.Arguments.Count == 2))
                    {
                        return this.sql.Binary(SqlNodeType.EQ2V, mc.Arguments[0], mc.Arguments[1], mc.Method);
                    }
                    if ((mc.Method.DeclaringType == typeof(string)) && (mc.Method.Name == "Concat"))
                    {
                        SqlExpression expression;
                        SqlClientArray array = mc.Arguments[0] as SqlClientArray;
                        List<SqlExpression> expressions = null;
                        if (array != null)
                        {
                            expressions = array.Expressions;
                        }
                        else
                        {
                            expressions = mc.Arguments;
                        }
                        if (expressions.Count == 0)
                        {
                            return this.sql.ValueFromObject("", false, mc.SourceExpression);
                        }
                        if (expressions[0].SqlType.IsString || expressions[0].SqlType.IsChar)
                        {
                            expression = expressions[0];
                        }
                        else
                        {
                            expression = this.sql.ConvertTo(typeof(string), expressions[0]);
                        }
                        for (int i = 1; i < expressions.Count; i++)
                        {
                            if (expressions[i].SqlType.IsString || expressions[i].SqlType.IsChar)
                            {
                                expression = this.sql.Concat(new SqlExpression[] { expression, expressions[i] });
                            }
                            else
                            {
                                expression = this.sql.Concat(new SqlExpression[] { expression, this.sql.ConvertTo(typeof(string), expressions[i]) });
                            }
                        }
                        return expression;
                    }
                    if (PreBindDotNetConverter.IsVbIIF(mc))
                    {
                        return this.TranslateVbIIF(mc);
                    }
                    switch (mc.Method.Name)
                    {
                        case "op_Equality":
                            return this.sql.Binary(SqlNodeType.EQ, mc.Arguments[0], mc.Arguments[1], mc.Method, mc.ClrType);

                        case "op_Inequality":
                            return this.sql.Binary(SqlNodeType.NE, mc.Arguments[0], mc.Arguments[1], mc.Method, mc.ClrType);

                        case "op_LessThan":
                            return this.sql.Binary(SqlNodeType.LT, mc.Arguments[0], mc.Arguments[1], mc.Method, mc.ClrType);

                        case "op_LessThanOrEqual":
                            return this.sql.Binary(SqlNodeType.LE, mc.Arguments[0], mc.Arguments[1], mc.Method, mc.ClrType);

                        case "op_GreaterThan":
                            return this.sql.Binary(SqlNodeType.GT, mc.Arguments[0], mc.Arguments[1], mc.Method, mc.ClrType);

                        case "op_GreaterThanOrEqual":
                            return this.sql.Binary(SqlNodeType.GE, mc.Arguments[0], mc.Arguments[1], mc.Method, mc.ClrType);

                        case "op_Multiply":
                            return this.sql.Binary(SqlNodeType.Mul, mc.Arguments[0], mc.Arguments[1], mc.Method, mc.ClrType);

                        case "op_Division":
                            return this.sql.Binary(SqlNodeType.Div, mc.Arguments[0], mc.Arguments[1], mc.Method, mc.ClrType);

                        case "op_Subtraction":
                            return this.sql.Binary(SqlNodeType.Sub, mc.Arguments[0], mc.Arguments[1], mc.Method, mc.ClrType);

                        case "op_Addition":
                            return this.sql.Binary(SqlNodeType.Add, mc.Arguments[0], mc.Arguments[1], mc.Method, mc.ClrType);

                        case "op_Modulus":
                            return this.sql.Binary(SqlNodeType.Mod, mc.Arguments[0], mc.Arguments[1], mc.Method, mc.ClrType);

                        case "op_BitwiseAnd":
                            return this.sql.Binary(SqlNodeType.BitAnd, mc.Arguments[0], mc.Arguments[1], mc.Method, mc.ClrType);

                        case "op_BitwiseOr":
                            return this.sql.Binary(SqlNodeType.BitOr, mc.Arguments[0], mc.Arguments[1], mc.Method, mc.ClrType);

                        case "op_ExclusiveOr":
                            return this.sql.Binary(SqlNodeType.BitXor, mc.Arguments[0], mc.Arguments[1], mc.Method, mc.ClrType);

                        case "op_UnaryNegation":
                            return this.sql.Unary(SqlNodeType.Negate, mc.Arguments[0], mc.Method, mc.SourceExpression);

                        case "op_OnesComplement":
                            return this.sql.Unary(SqlNodeType.BitNot, mc.Arguments[0], mc.Method, mc.SourceExpression);

                        case "op_False":
                            return this.sql.Unary(SqlNodeType.Not, mc.Arguments[0], mc.Method, mc.SourceExpression);
                    }
                    return mc;
                }
                if ((mc.Method.Name == "Equals") && (mc.Arguments.Count == 1))
                {
                    return this.sql.Binary(SqlNodeType.EQ, mc.Object, mc.Arguments[0]);
                }
                if ((mc.Method.Name != "GetType") || (mc.Arguments.Count != 0))
                {
                    return mc;
                }
                MetaType sourceMetaType = TypeSource.GetSourceMetaType(mc.Object, this.model);
                if (sourceMetaType.HasInheritance)
                {
                    Type type = sourceMetaType.Discriminator.Type;
                    SqlDiscriminatorOf discriminator = new SqlDiscriminatorOf(mc.Object, type, this.sql.TypeProvider.From(type), mc.SourceExpression);
                    return this.VisitExpression(this.sql.DiscriminatedType(discriminator, sourceMetaType));
                }
                return this.VisitExpression(this.sql.StaticType(sourceMetaType, mc.SourceExpression));
            }

            internal override SqlExpression VisitTreat(SqlUnary t)
            {
                t.Operand = this.VisitExpression(t.Operand);
                Type clrType = t.ClrType;
                Type type = this.model.GetMetaType(t.Operand.ClrType).InheritanceRoot.Type;
                clrType = TypeSystem.GetNonNullableType(clrType);
                type = TypeSystem.GetNonNullableType(type);
                if (clrType == type)
                {
                    return t.Operand;
                }
                if (clrType.IsAssignableFrom(type))
                {
                    t.Operand.SetClrType(clrType);
                    return t.Operand;
                }
                if ((!clrType.IsAssignableFrom(type) && !type.IsAssignableFrom(clrType)) && (!clrType.IsInterface && !type.IsInterface))
                {
                    return this.sql.TypedLiteralNull(clrType, t.SourceExpression);
                }
                return t;
            }
        }
    }
}

