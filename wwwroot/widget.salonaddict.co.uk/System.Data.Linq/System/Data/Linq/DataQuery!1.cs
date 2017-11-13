namespace System.Data.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.Linq.Provider;
    using System.Data.Linq.SqlClient;
    using System.Linq;
    using System.Linq.Expressions;

    internal sealed class DataQuery<T> : IOrderedQueryable<T>, IQueryable<T>, IQueryProvider, IEnumerable<T>, IOrderedQueryable, IQueryable, IEnumerable, IListSource
    {
        private IBindingList cachedList;
        private DataContext context;
        private Expression queryExpression;

        public DataQuery(DataContext context, Expression expression)
        {
            this.context = context;
            this.queryExpression = expression;
        }

        internal IBindingList GetNewBindingList() => 
            BindingList.Create<T>(this.context, this);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => 
            ((IEnumerable<T>) this.context.Provider.Execute(this.queryExpression).ReturnValue).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            ((IEnumerable) this.context.Provider.Execute(this.queryExpression).ReturnValue).GetEnumerator();

        IList IListSource.GetList()
        {
            if (this.cachedList == null)
            {
                this.cachedList = this.GetNewBindingList();
            }
            return this.cachedList;
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            if (expression == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("expression");
            }
            Type elementType = TypeSystem.GetElementType(expression.Type);
            Type type2 = typeof(IQueryable<>).MakeGenericType(new Type[] { elementType });
            if (!type2.IsAssignableFrom(expression.Type))
            {
                throw System.Data.Linq.Error.ExpectedQueryableArgument("expression", type2);
            }
            return (IQueryable) Activator.CreateInstance(typeof(DataQuery<>).MakeGenericType(new Type[] { elementType }), new object[] { this.context, expression });
        }

        IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
        {
            if (expression == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("expression");
            }
            if (!typeof(IQueryable<S>).IsAssignableFrom(expression.Type))
            {
                throw System.Data.Linq.Error.ExpectedQueryableArgument("expression", typeof(IEnumerable<S>));
            }
            return new DataQuery<S>(this.context, expression);
        }

        object IQueryProvider.Execute(Expression expression) => 
            this.context.Provider.Execute(expression).ReturnValue;

        S IQueryProvider.Execute<S>(Expression expression) => 
            ((S) this.context.Provider.Execute(expression).ReturnValue);

        public override string ToString() => 
            this.context.Provider.GetQueryText(this.queryExpression);

        bool IListSource.ContainsListCollection =>
            false;

        Type IQueryable.ElementType =>
            typeof(T);

        Expression IQueryable.Expression =>
            this.queryExpression;

        IQueryProvider IQueryable.Provider =>
            this;
    }
}

