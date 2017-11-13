namespace System.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;

    internal class ExpressionWriter : DataServiceExpressionVisitor
    {
        private readonly StringBuilder builder;
        private bool cantTranslateExpression;
        private readonly DataServiceContext context;
        private readonly Stack<Expression> expressionStack;
        private Expression parent;

        private ExpressionWriter(DataServiceContext context)
        {
            this.context = context;
            this.builder = new StringBuilder();
            this.expressionStack = new Stack<Expression>();
            this.expressionStack.Push(null);
        }

        internal static string ExpressionToString(DataServiceContext context, Expression e)
        {
            ExpressionWriter writer = new ExpressionWriter(context);
            string str = writer.Translate(e);
            if (writer.cantTranslateExpression)
            {
                throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CantTranslateExpression(e.ToString()));
            }
            return str;
        }

        private static bool IsInputReference(Expression exp) => 
            ((exp is InputReferenceExpression) || (exp is ParameterExpression));

        private string Translate(Expression e)
        {
            this.Visit(e);
            return this.builder.ToString();
        }

        private string TypeNameForUri(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            if (ClientConvert.IsKnownType(type))
            {
                if (!ClientConvert.IsSupportedPrimitiveTypeForUri(type))
                {
                    throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CantCastToUnsupportedPrimitive(type.Name));
                }
                return ClientConvert.ToTypeName(type);
            }
            return (this.context.ResolveNameFromType(type) ?? type.FullName);
        }

        internal override Expression Visit(Expression exp)
        {
            this.parent = this.expressionStack.Peek();
            this.expressionStack.Push(exp);
            Expression expression = base.Visit(exp);
            this.expressionStack.Pop();
            return expression;
        }

        internal override Expression VisitBinary(BinaryExpression b)
        {
            this.VisitOperand(b.Left);
            this.builder.Append(' ');
            switch (b.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    this.builder.Append("add");
                    break;

                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    this.builder.Append("and");
                    break;

                case ExpressionType.Divide:
                    this.builder.Append("div");
                    break;

                case ExpressionType.Equal:
                    this.builder.Append("eq");
                    break;

                case ExpressionType.GreaterThan:
                    this.builder.Append("gt");
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    this.builder.Append("ge");
                    break;

                case ExpressionType.LessThan:
                    this.builder.Append("lt");
                    break;

                case ExpressionType.LessThanOrEqual:
                    this.builder.Append("le");
                    break;

                case ExpressionType.Modulo:
                    this.builder.Append("mod");
                    break;

                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    this.builder.Append("mul");
                    break;

                case ExpressionType.NotEqual:
                    this.builder.Append("ne");
                    break;

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    this.builder.Append("or");
                    break;

                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    this.builder.Append("sub");
                    break;

                default:
                    this.cantTranslateExpression = true;
                    break;
            }
            this.builder.Append(' ');
            this.VisitOperand(b.Right);
            return b;
        }

        internal override Expression VisitConditional(ConditionalExpression c)
        {
            this.cantTranslateExpression = true;
            return c;
        }

        internal override Expression VisitConstant(ConstantExpression c)
        {
            string result = null;
            if (c.Value == null)
            {
                this.builder.Append("null");
                return c;
            }
            if (!ClientConvert.TryKeyPrimitiveToString(c.Value, out result))
            {
                throw new InvalidOperationException(System.Data.Services.Client.Strings.ALinq_CouldNotConvert(c.Value));
            }
            this.builder.Append(Uri.EscapeDataString(result));
            return c;
        }

        internal override Expression VisitInputReferenceExpression(InputReferenceExpression ire)
        {
            if ((this.parent == null) || (this.parent.NodeType != ExpressionType.MemberAccess))
            {
                string str = (this.parent != null) ? this.parent.ToString() : ire.ToString();
                throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CantTranslateExpression(str));
            }
            return ire;
        }

        internal override Expression VisitInvocation(InvocationExpression iv)
        {
            this.cantTranslateExpression = true;
            return iv;
        }

        internal override Expression VisitLambda(LambdaExpression lambda)
        {
            this.cantTranslateExpression = true;
            return lambda;
        }

        internal override Expression VisitListInit(ListInitExpression init)
        {
            this.cantTranslateExpression = true;
            return init;
        }

        internal override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Member is FieldInfo)
            {
                throw new NotSupportedException(System.Data.Services.Client.Strings.ALinq_CantReferToPublicField(m.Member.Name));
            }
            Expression exp = this.Visit(m.Expression);
            if (((m.Member.Name != "Value") || !m.Member.DeclaringType.IsGenericType) || (m.Member.DeclaringType.GetGenericTypeDefinition() != typeof(Nullable<>)))
            {
                if (!IsInputReference(exp))
                {
                    this.builder.Append('/');
                }
                this.builder.Append(m.Member.Name);
            }
            return m;
        }

        internal override Expression VisitMemberInit(MemberInitExpression init)
        {
            this.cantTranslateExpression = true;
            return init;
        }

        internal override Expression VisitMethodCall(MethodCallExpression m)
        {
            string str;
            if (TypeSystem.TryGetQueryOptionMethod(m.Method, out str))
            {
                this.builder.Append(str);
                this.builder.Append('(');
                if (str == "substringof")
                {
                    this.Visit(m.Arguments[0]);
                    this.builder.Append(',');
                    this.Visit(m.Object);
                }
                else
                {
                    if (m.Object != null)
                    {
                        this.Visit(m.Object);
                    }
                    if (m.Arguments.Count > 0)
                    {
                        if (m.Object != null)
                        {
                            this.builder.Append(',');
                        }
                        for (int i = 0; i < m.Arguments.Count; i++)
                        {
                            this.Visit(m.Arguments[i]);
                            if (i < (m.Arguments.Count - 1))
                            {
                                this.builder.Append(',');
                            }
                        }
                    }
                }
                this.builder.Append(')');
                return m;
            }
            this.cantTranslateExpression = true;
            return m;
        }

        internal override NewExpression VisitNew(NewExpression nex)
        {
            this.cantTranslateExpression = true;
            return nex;
        }

        internal override Expression VisitNewArray(NewArrayExpression na)
        {
            this.cantTranslateExpression = true;
            return na;
        }

        private void VisitOperand(Expression e)
        {
            if ((e is BinaryExpression) || (e is UnaryExpression))
            {
                this.builder.Append('(');
                this.Visit(e);
                this.builder.Append(')');
            }
            else
            {
                this.Visit(e);
            }
        }

        internal override Expression VisitParameter(ParameterExpression p) => 
            p;

        internal override Expression VisitTypeIs(TypeBinaryExpression b)
        {
            this.builder.Append("isof");
            this.builder.Append('(');
            if (!IsInputReference(b.Expression))
            {
                this.Visit(b.Expression);
                this.builder.Append(',');
                this.builder.Append(' ');
            }
            this.builder.Append('\'');
            this.builder.Append(this.TypeNameForUri(b.TypeOperand));
            this.builder.Append('\'');
            this.builder.Append(')');
            return b;
        }

        internal override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    if (u.Type == typeof(object))
                    {
                        if (!IsInputReference(u.Operand))
                        {
                            this.Visit(u.Operand);
                        }
                        return u;
                    }
                    this.builder.Append("cast");
                    this.builder.Append('(');
                    if (!IsInputReference(u.Operand))
                    {
                        this.Visit(u.Operand);
                        this.builder.Append(',');
                    }
                    this.builder.Append('\'');
                    this.builder.Append(this.TypeNameForUri(u.Type));
                    this.builder.Append('\'');
                    this.builder.Append(')');
                    return u;

                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    this.builder.Append(' ');
                    this.builder.Append("-");
                    this.VisitOperand(u.Operand);
                    return u;

                case ExpressionType.UnaryPlus:
                    return u;

                case ExpressionType.Not:
                    this.builder.Append("not");
                    this.builder.Append(' ');
                    this.VisitOperand(u.Operand);
                    return u;
            }
            this.cantTranslateExpression = true;
            return u;
        }
    }
}

