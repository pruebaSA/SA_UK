namespace System.Data.Objects.ELinq
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Objects;
    using System.Data.Objects.Internal;
    using System.Linq;
    using System.Linq.Expressions;

    internal sealed class ObjectQueryProvider : IQueryProvider
    {
        private readonly ObjectContext _context;

        internal ObjectQueryProvider(ObjectContext context)
        {
            this._context = context;
        }

        private ObjectQuery<S> CreateQuery<S>(Expression expression) => 
            new ObjectQuery<S>(new ELinqQueryState(typeof(S), this._context, expression));

        private ObjectQuery CreateQuery(Expression expression, Type ofType)
        {
            ObjectQueryState state = new ELinqQueryState(ofType, this._context, expression);
            return state.CreateQuery();
        }

        internal static TResult ExecuteSingle<TResult>(IEnumerable<TResult> query, Expression queryRoot) => 
            GetElementFunction<TResult>(queryRoot)(query);

        private static Func<IEnumerable<TResult>, TResult> GetElementFunction<TResult>(Expression queryRoot)
        {
            Func<IEnumerable<TResult>, TResult> func = null;
            SequenceMethod method;
            Func<IEnumerable<TResult>, TResult> func2 = null;
            Func<IEnumerable<TResult>, TResult> func3 = null;
            Func<IEnumerable<TResult>, TResult> func4 = null;
            if (ReflectionUtil.TryIdentifySequenceMethod(queryRoot, true, out method))
            {
                if ((SequenceMethod.First == method) || (SequenceMethod.FirstPredicate == method))
                {
                    if (func2 == null)
                    {
                        func2 = sequence => sequence.First<TResult>();
                    }
                    func = func2;
                }
                else if ((SequenceMethod.FirstOrDefault == method) || (SequenceMethod.FirstOrDefaultPredicate == method))
                {
                    if (func3 == null)
                    {
                        func3 = sequence => sequence.FirstOrDefault<TResult>();
                    }
                    func = func3;
                }
            }
            if (func != null)
            {
                return func;
            }
            if (func4 == null)
            {
                func4 = sequence => sequence.Single<TResult>();
            }
            return func4;
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            EntityUtil.CheckArgumentNull<Expression>(expression, "expression");
            if (!typeof(IQueryable).IsAssignableFrom(expression.Type))
            {
                throw EntityUtil.Argument(System.Data.Entity.Strings.ELinq_ExpressionMustBeIQueryable, "expression");
            }
            Type elementType = TypeSystem.GetElementType(expression.Type);
            return this.CreateQuery(expression, elementType);
        }

        IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
        {
            EntityUtil.CheckArgumentNull<Expression>(expression, "expression");
            if (!typeof(IQueryable<S>).IsAssignableFrom(expression.Type))
            {
                throw EntityUtil.Argument(System.Data.Entity.Strings.ELinq_ExpressionMustBeIQueryable, "expression");
            }
            return this.CreateQuery<S>(expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            EntityUtil.CheckArgumentNull<Expression>(expression, "expression");
            return ExecuteSingle<object>(this.CreateQuery(expression, expression.Type).Cast<object>(), expression);
        }

        S IQueryProvider.Execute<S>(Expression expression)
        {
            EntityUtil.CheckArgumentNull<Expression>(expression, "expression");
            return ExecuteSingle<S>(this.CreateQuery<S>(expression), expression);
        }
    }
}

