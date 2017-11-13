namespace System.Data.Services.Client
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public class DataServiceQuery<TElement> : DataServiceQuery, IQueryable<TElement>, IEnumerable<TElement>, IQueryable, IEnumerable
    {
        private System.Data.Services.Client.QueryComponents queryComponents;
        private readonly System.Linq.Expressions.Expression queryExpression;
        private readonly DataServiceQueryProvider queryProvider;

        private DataServiceQuery(System.Linq.Expressions.Expression expression, DataServiceQueryProvider provider)
        {
            this.queryExpression = expression;
            this.queryProvider = provider;
        }

        public DataServiceQuery<TElement> AddQueryOption(string name, object value)
        {
            Util.CheckArgumentNull<string>(name, "name");
            Util.CheckArgumentNull<object>(value, "value");
            MethodInfo method = typeof(DataServiceQuery<TElement>).GetMethod("AddQueryOption");
            return (DataServiceQuery<TElement>) this.Provider.CreateQuery<TElement>(System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.Convert(this.Expression, typeof(DataServiceOrderedQuery<TElement>)), method, new System.Linq.Expressions.Expression[] { System.Linq.Expressions.Expression.Constant(name), System.Linq.Expressions.Expression.Constant(value, typeof(object)) }));
        }

        public IAsyncResult BeginExecute(AsyncCallback callback, object state) => 
            base.BeginExecute(this, this.queryProvider.Context, callback, state);

        internal override IAsyncResult BeginExecuteInternal(AsyncCallback callback, object state) => 
            this.BeginExecute(callback, state);

        public IEnumerable<TElement> EndExecute(IAsyncResult asyncResult) => 
            DataServiceRequest.EndExecute<TElement>(this, this.queryProvider.Context, asyncResult);

        internal override IEnumerable EndExecuteInternal(IAsyncResult asyncResult) => 
            this.EndExecute(asyncResult);

        public IEnumerable<TElement> Execute() => 
            base.Execute<TElement>(this.queryProvider.Context, this.Translate());

        internal override IEnumerable ExecuteInternal() => 
            this.Execute();

        public DataServiceQuery<TElement> Expand(string path)
        {
            Util.CheckArgumentNull<string>(path, "path");
            Util.CheckArgumentNotEmpty(path, "path");
            MethodInfo method = typeof(DataServiceQuery<TElement>).GetMethod("Expand");
            return (DataServiceQuery<TElement>) this.Provider.CreateQuery<TElement>(System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.Convert(this.Expression, typeof(DataServiceOrderedQuery<TElement>)), method, new System.Linq.Expressions.Expression[] { System.Linq.Expressions.Expression.Constant(path) }));
        }

        public IEnumerator<TElement> GetEnumerator() => 
            this.Execute().GetEnumerator();

        public DataServiceQuery<TElement> IncludeTotalCount()
        {
            MethodInfo method = typeof(DataServiceQuery<TElement>).GetMethod("IncludeTotalCount");
            return (DataServiceQuery<TElement>) this.Provider.CreateQuery<TElement>(System.Linq.Expressions.Expression.Call(System.Linq.Expressions.Expression.Convert(this.Expression, typeof(DataServiceOrderedQuery<TElement>)), method));
        }

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        public override string ToString()
        {
            try
            {
                return base.ToString();
            }
            catch (NotSupportedException exception)
            {
                return System.Data.Services.Client.Strings.ALinq_TranslationError(exception.Message);
            }
        }

        private System.Data.Services.Client.QueryComponents Translate()
        {
            if (this.queryComponents == null)
            {
                this.queryComponents = this.queryProvider.Translate(this.queryExpression);
            }
            return this.queryComponents;
        }

        public override Type ElementType =>
            typeof(TElement);

        public override System.Linq.Expressions.Expression Expression =>
            this.queryExpression;

        internal override ProjectionPlan Plan =>
            null;

        public override IQueryProvider Provider =>
            this.queryProvider;

        internal override System.Data.Services.Client.QueryComponents QueryComponents =>
            this.Translate();

        public override Uri RequestUri =>
            this.Translate().Uri;

        internal class DataServiceOrderedQuery : DataServiceQuery<TElement>, IOrderedQueryable<TElement>, IQueryable<TElement>, IEnumerable<TElement>, IOrderedQueryable, IQueryable, IEnumerable
        {
            internal DataServiceOrderedQuery(Expression expression, DataServiceQueryProvider provider) : base(expression, provider)
            {
            }
        }
    }
}

