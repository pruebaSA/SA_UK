namespace System.Linq
{
    using System;
    using System.Linq.Expressions;

    internal class EnumerableExecutor<T> : EnumerableExecutor
    {
        private Expression expression;
        private Func<T> func;

        internal EnumerableExecutor(Expression expression)
        {
            this.expression = expression;
        }

        internal T Execute() => 
            this.func?.Invoke();

        internal override object ExecuteBoxed() => 
            this.Execute();
    }
}

