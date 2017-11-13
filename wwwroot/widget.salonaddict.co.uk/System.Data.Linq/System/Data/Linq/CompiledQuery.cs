namespace System.Data.Linq
{
    using System;
    using System.Data.Linq.Provider;
    using System.Linq.Expressions;

    public sealed class CompiledQuery
    {
        private ICompiledQuery compiled;
        private LambdaExpression query;

        private CompiledQuery(LambdaExpression query)
        {
            this.query = query;
        }

        public static Func<TArg0, TResult> Compile<TArg0, TResult>(Expression<Func<TArg0, TResult>> query) where TArg0: DataContext
        {
            if (query == null)
            {
                System.Data.Linq.Error.ArgumentNull("query");
            }
            if (UseExpressionCompile(query))
            {
                return query.Compile();
            }
            return new Func<TArg0, TResult>(new CompiledQuery(query).Invoke<TArg0, TResult>);
        }

        public static Func<TArg0, TArg1, TResult> Compile<TArg0, TArg1, TResult>(Expression<Func<TArg0, TArg1, TResult>> query) where TArg0: DataContext
        {
            if (query == null)
            {
                System.Data.Linq.Error.ArgumentNull("query");
            }
            if (UseExpressionCompile(query))
            {
                return query.Compile();
            }
            return new Func<TArg0, TArg1, TResult>(new CompiledQuery(query).Invoke<TArg0, TArg1, TResult>);
        }

        public static Func<TArg0, TArg1, TArg2, TResult> Compile<TArg0, TArg1, TArg2, TResult>(Expression<Func<TArg0, TArg1, TArg2, TResult>> query) where TArg0: DataContext
        {
            if (query == null)
            {
                System.Data.Linq.Error.ArgumentNull("query");
            }
            if (UseExpressionCompile(query))
            {
                return query.Compile();
            }
            return new Func<TArg0, TArg1, TArg2, TResult>(new CompiledQuery(query).Invoke<TArg0, TArg1, TArg2, TResult>);
        }

        public static Func<TArg0, TArg1, TArg2, TArg3, TResult> Compile<TArg0, TArg1, TArg2, TArg3, TResult>(Expression<Func<TArg0, TArg1, TArg2, TArg3, TResult>> query) where TArg0: DataContext
        {
            if (query == null)
            {
                System.Data.Linq.Error.ArgumentNull("query");
            }
            if (UseExpressionCompile(query))
            {
                return query.Compile();
            }
            return new Func<TArg0, TArg1, TArg2, TArg3, TResult>(new CompiledQuery(query).Invoke<TArg0, TArg1, TArg2, TArg3, TResult>);
        }

        private object ExecuteQuery(DataContext context, object[] args)
        {
            if (context == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("context");
            }
            return this.compiled?.Execute(context.Provider, args).ReturnValue;
        }

        private TResult Invoke<TArg0, TResult>(TArg0 arg0) where TArg0: DataContext => 
            ((TResult) this.ExecuteQuery(arg0, new object[] { arg0 }));

        private TResult Invoke<TArg0, TArg1, TResult>(TArg0 arg0, TArg1 arg1) where TArg0: DataContext => 
            ((TResult) this.ExecuteQuery(arg0, new object[] { arg0, arg1 }));

        private TResult Invoke<TArg0, TArg1, TArg2, TResult>(TArg0 arg0, TArg1 arg1, TArg2 arg2) where TArg0: DataContext => 
            ((TResult) this.ExecuteQuery(arg0, new object[] { arg0, arg1, arg2 }));

        private TResult Invoke<TArg0, TArg1, TArg2, TArg3, TResult>(TArg0 arg0, TArg1 arg1, TArg2 arg2, TArg3 arg3) where TArg0: DataContext => 
            ((TResult) this.ExecuteQuery(arg0, new object[] { arg0, arg1, arg2, arg3 }));

        private static bool UseExpressionCompile(LambdaExpression query) => 
            typeof(ITable).IsAssignableFrom(query.Body.Type);

        public LambdaExpression Expression =>
            this.query;
    }
}

