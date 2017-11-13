namespace System.Runtime.CompilerServices
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection.Emit;

    public class ExecutionScope
    {
        public object[] Globals;
        private ExpressionCompiler.LambdaInfo Lambda;
        public object[] Locals;
        public ExecutionScope Parent;

        internal ExecutionScope(ExecutionScope parent, ExpressionCompiler.LambdaInfo lambda, object[] globals, object[] locals)
        {
            this.Parent = parent;
            this.Lambda = lambda;
            this.Globals = globals;
            this.Locals = locals;
        }

        public Delegate CreateDelegate(int indexLambda, object[] locals)
        {
            ExpressionCompiler.LambdaInfo lambda = this.Lambda.Lambdas[indexLambda];
            ExecutionScope target = new ExecutionScope(this, lambda, this.Globals, locals);
            return ((DynamicMethod) lambda.Method).CreateDelegate(lambda.Lambda.Type, target);
        }

        public object[] CreateHoistedLocals() => 
            new object[this.Lambda.HoistedLocals.Count];

        public Expression IsolateExpression(Expression expression, object[] locals)
        {
            ExpressionIsolator isolator = new ExpressionIsolator(this, locals);
            return isolator.Visit(expression);
        }

        private class ExpressionIsolator : ExpressionVisitor
        {
            private ExecutionScope top;
            private object[] toplocals;

            internal ExpressionIsolator(ExecutionScope top, object[] toplocals)
            {
                this.top = top;
                this.toplocals = toplocals;
            }

            internal override Expression VisitParameter(ParameterExpression p)
            {
                ExecutionScope top = this.top;
                object[] toplocals = this.toplocals;
                while (top != null)
                {
                    int num;
                    if (top.Lambda.HoistedLocals.TryGetValue(p, out num))
                    {
                        return Expression.Field(Expression.Convert(Expression.ArrayIndex(Expression.Constant(toplocals, typeof(object[])), Expression.Constant(num, typeof(int))), toplocals[num].GetType()), "Value");
                    }
                    toplocals = top.Locals;
                    top = top.Parent;
                }
                return p;
            }
        }
    }
}

