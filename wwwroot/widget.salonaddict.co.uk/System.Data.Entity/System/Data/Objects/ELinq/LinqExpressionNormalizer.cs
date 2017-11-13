namespace System.Data.Objects.ELinq
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal class LinqExpressionNormalizer : System.Linq.Expressions.ExpressionVisitor
    {
        private readonly Dictionary<Expression, Pattern> _patterns = new Dictionary<Expression, Pattern>();
        private const bool LiftToNull = false;
        private static readonly MethodInfo s_relationalOperatorPlaceholderMethod = typeof(LinqExpressionNormalizer).GetMethod("RelationalOperatorPlaceholder", BindingFlags.NonPublic | BindingFlags.Static);

        private Expression CreateCompareExpression(Expression left, Expression right)
        {
            Expression expression = Expression.Condition(CreateRelationalOperator(ExpressionType.Equal, left, right), Expression.Constant(0), Expression.Condition(CreateRelationalOperator(ExpressionType.GreaterThan, left, right), Expression.Constant(1), Expression.Constant(-1)));
            this._patterns[expression] = new ComparePattern(left, right);
            return expression;
        }

        private static BinaryExpression CreateRelationalOperator(ExpressionType op, Expression left, Expression right)
        {
            BinaryExpression expression;
            TryCreateRelationalOperator(op, left, right, out expression);
            return expression;
        }

        private static bool HasPredicateArgument(MethodCallExpression callExpression, out int argumentOrdinal)
        {
            SequenceMethod method;
            argumentOrdinal = 0;
            bool flag = false;
            if ((2 <= callExpression.Arguments.Count) && ReflectionUtil.TryIdentifySequenceMethod(callExpression.Method, out method))
            {
                SequenceMethod method2 = method;
                if (method2 <= SequenceMethod.SkipWhileOrdinal)
                {
                    switch (method2)
                    {
                        case SequenceMethod.Where:
                        case SequenceMethod.WhereOrdinal:
                        case SequenceMethod.TakeWhile:
                        case SequenceMethod.TakeWhileOrdinal:
                        case SequenceMethod.SkipWhile:
                        case SequenceMethod.SkipWhileOrdinal:
                            goto Label_00B4;

                        case SequenceMethod.Skip:
                            return flag;
                    }
                    return flag;
                }
                switch (method2)
                {
                    case SequenceMethod.FirstPredicate:
                    case SequenceMethod.FirstOrDefaultPredicate:
                    case SequenceMethod.LastPredicate:
                    case SequenceMethod.LastOrDefaultPredicate:
                    case SequenceMethod.SinglePredicate:
                    case SequenceMethod.SingleOrDefaultPredicate:
                    case SequenceMethod.AnyPredicate:
                    case SequenceMethod.All:
                    case SequenceMethod.CountPredicate:
                    case SequenceMethod.LongCountPredicate:
                        goto Label_00B4;

                    case SequenceMethod.FirstOrDefault:
                    case SequenceMethod.Last:
                    case SequenceMethod.LastOrDefault:
                    case SequenceMethod.Single:
                    case SequenceMethod.SingleOrDefault:
                        return flag;

                    case SequenceMethod.Count:
                    case SequenceMethod.LongCount:
                        return flag;
                }
            }
            return flag;
        Label_00B4:
            argumentOrdinal = 1;
            return true;
        }

        private bool IsConstantZero(Expression expression) => 
            ((expression.NodeType == ExpressionType.Constant) && ((ConstantExpression) expression).Value.Equals(0));

        private static MethodCallExpression NormalizePredicateArgument(MethodCallExpression callExpression)
        {
            int num;
            Expression expression2;
            if (HasPredicateArgument(callExpression, out num) && TryMatchCoalescePattern(callExpression.Arguments[num], out expression2))
            {
                List<Expression> arguments = new List<Expression>(callExpression.Arguments) {
                    [num] = expression2
                };
                return Expression.Call(callExpression.Object, callExpression.Method, arguments);
            }
            return callExpression;
        }

        private static bool RelationalOperatorPlaceholder<TLeft, TRight>(TLeft left, TRight right) => 
            object.ReferenceEquals(left, right);

        private static bool TryCreateRelationalOperator(ExpressionType op, Expression left, Expression right, out BinaryExpression result)
        {
            MethodInfo method = s_relationalOperatorPlaceholderMethod.MakeGenericMethod(new Type[] { left.Type, right.Type });
            switch (op)
            {
                case ExpressionType.Equal:
                    result = Expression.Equal(left, right, false, method);
                    return true;

                case ExpressionType.GreaterThan:
                    result = Expression.GreaterThan(left, right, false, method);
                    return true;

                case ExpressionType.GreaterThanOrEqual:
                    result = Expression.GreaterThanOrEqual(left, right, false, method);
                    return true;

                case ExpressionType.LessThan:
                    result = Expression.LessThan(left, right, false, method);
                    return true;

                case ExpressionType.LessThanOrEqual:
                    result = Expression.LessThanOrEqual(left, right, false, method);
                    return true;

                case ExpressionType.NotEqual:
                    result = Expression.NotEqual(left, right, false, method);
                    return true;
            }
            result = null;
            return false;
        }

        private static bool TryMatchCoalescePattern(Expression expression, out Expression normalized)
        {
            normalized = null;
            bool flag = false;
            if (expression.NodeType == ExpressionType.Quote)
            {
                UnaryExpression expression2 = (UnaryExpression) expression;
                if (TryMatchCoalescePattern(expression2.Operand, out normalized))
                {
                    flag = true;
                    normalized = Expression.Quote(normalized);
                }
                return flag;
            }
            if (expression.NodeType == ExpressionType.Lambda)
            {
                LambdaExpression expression3 = (LambdaExpression) expression;
                if ((expression3.Body.NodeType == ExpressionType.Coalesce) && (expression3.Body.Type == typeof(bool)))
                {
                    BinaryExpression body = (BinaryExpression) expression3.Body;
                    if (body.Right.NodeType == ExpressionType.Constant)
                    {
                        bool flag2 = false;
                        if (flag2.Equals(((ConstantExpression) body.Right).Value))
                        {
                            normalized = Expression.Lambda(expression3.Type, Expression.Convert(body.Left, typeof(bool)), expression3.Parameters);
                            flag = true;
                        }
                    }
                }
            }
            return flag;
        }

        private static Expression UnwrapObjectConvert(Expression input)
        {
            if ((input.NodeType == ExpressionType.Constant) && (input.Type == typeof(object)))
            {
                ConstantExpression expression = (ConstantExpression) input;
                if ((expression.Value != null) && (expression.Value.GetType() != typeof(object)))
                {
                    return Expression.Constant(expression.Value, expression.Value.GetType());
                }
            }
            while ((ExpressionType.Convert == input.NodeType) && (typeof(object) == input.Type))
            {
                input = ((UnaryExpression) input).Operand;
            }
            return input;
        }

        internal override Expression VisitBinary(BinaryExpression b)
        {
            Pattern pattern;
            b = (BinaryExpression) base.VisitBinary(b);
            if (b.NodeType == ExpressionType.Equal)
            {
                Expression left = UnwrapObjectConvert(b.Left);
                Expression right = UnwrapObjectConvert(b.Right);
                if ((left != b.Left) || (right != b.Right))
                {
                    b = CreateRelationalOperator(ExpressionType.Equal, left, right);
                }
            }
            if ((this._patterns.TryGetValue(b.Left, out pattern) && (pattern.Kind == PatternKind.Compare)) && this.IsConstantZero(b.Right))
            {
                BinaryExpression expression3;
                ComparePattern pattern2 = (ComparePattern) pattern;
                if (TryCreateRelationalOperator(b.NodeType, pattern2.Left, pattern2.Right, out expression3))
                {
                    b = expression3;
                }
            }
            return b;
        }

        internal override Expression VisitMethodCall(MethodCallExpression m)
        {
            m = (MethodCallExpression) base.VisitMethodCall(m);
            if (m.Method.IsStatic && m.Method.Name.StartsWith("op_", StringComparison.Ordinal))
            {
                if (m.Arguments.Count == 2)
                {
                    switch (m.Method.Name)
                    {
                        case "op_Equality":
                            return Expression.Equal(m.Arguments[0], m.Arguments[1], false, m.Method);

                        case "op_Inequality":
                            return Expression.NotEqual(m.Arguments[0], m.Arguments[1], false, m.Method);

                        case "op_GreaterThan":
                            return Expression.GreaterThan(m.Arguments[0], m.Arguments[1], false, m.Method);

                        case "op_GreaterThanOrEqual":
                            return Expression.GreaterThanOrEqual(m.Arguments[0], m.Arguments[1], false, m.Method);

                        case "op_LessThan":
                            return Expression.LessThan(m.Arguments[0], m.Arguments[1], false, m.Method);

                        case "op_LessThanOrEqual":
                            return Expression.LessThanOrEqual(m.Arguments[0], m.Arguments[1], false, m.Method);

                        case "op_Multiply":
                            return Expression.Multiply(m.Arguments[0], m.Arguments[1], m.Method);

                        case "op_Subtraction":
                            return Expression.Subtract(m.Arguments[0], m.Arguments[1], m.Method);

                        case "op_Addition":
                            return Expression.Add(m.Arguments[0], m.Arguments[1], m.Method);

                        case "op_Division":
                            return Expression.Divide(m.Arguments[0], m.Arguments[1], m.Method);

                        case "op_Modulus":
                            return Expression.Modulo(m.Arguments[0], m.Arguments[1], m.Method);

                        case "op_BitwiseAnd":
                            return Expression.And(m.Arguments[0], m.Arguments[1], m.Method);

                        case "op_BitwiseOr":
                            return Expression.Or(m.Arguments[0], m.Arguments[1], m.Method);

                        case "op_ExclusiveOr":
                            return Expression.ExclusiveOr(m.Arguments[0], m.Arguments[1], m.Method);
                    }
                }
                if (m.Arguments.Count == 1)
                {
                    switch (m.Method.Name)
                    {
                        case "op_UnaryNegation":
                            return Expression.Negate(m.Arguments[0], m.Method);

                        case "op_UnaryPlus":
                            return Expression.UnaryPlus(m.Arguments[0], m.Method);

                        case "op_Explicit":
                        case "op_Implicit":
                            return Expression.Convert(m.Arguments[0], m.Type, m.Method);

                        case "op_OnesComplement":
                        case "op_False":
                            return Expression.Not(m.Arguments[0], m.Method);
                    }
                }
            }
            if ((m.Method.IsStatic && (m.Method.Name == "Equals")) && (m.Arguments.Count > 1))
            {
                return Expression.Equal(m.Arguments[0], m.Arguments[1], false, m.Method);
            }
            if ((!m.Method.IsStatic && (m.Method.Name == "Equals")) && (m.Arguments.Count > 0))
            {
                return CreateRelationalOperator(ExpressionType.Equal, m.Object, m.Arguments[0]);
            }
            if ((m.Method.IsStatic && (m.Method.Name == "CompareString")) && (m.Method.DeclaringType.FullName == "Microsoft.VisualBasic.CompilerServices.Operators"))
            {
                return this.CreateCompareExpression(m.Arguments[0], m.Arguments[1]);
            }
            if ((!m.Method.IsStatic && (m.Method.Name == "CompareTo")) && ((m.Arguments.Count == 1) && (m.Method.ReturnType == typeof(int))))
            {
                return this.CreateCompareExpression(m.Object, m.Arguments[0]);
            }
            if ((m.Method.IsStatic && (m.Method.Name == "Compare")) && ((m.Arguments.Count > 1) && (m.Method.ReturnType == typeof(int))))
            {
                return this.CreateCompareExpression(m.Arguments[0], m.Arguments[1]);
            }
            return NormalizePredicateArgument(m);
        }

        private sealed class ComparePattern : LinqExpressionNormalizer.Pattern
        {
            internal readonly Expression Left;
            internal readonly Expression Right;

            internal ComparePattern(Expression left, Expression right)
            {
                this.Left = left;
                this.Right = right;
            }

            internal override LinqExpressionNormalizer.PatternKind Kind =>
                LinqExpressionNormalizer.PatternKind.Compare;
        }

        private abstract class Pattern
        {
            protected Pattern()
            {
            }

            internal abstract LinqExpressionNormalizer.PatternKind Kind { get; }
        }

        private enum PatternKind
        {
            Compare
        }
    }
}

