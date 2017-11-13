﻿namespace System.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class EnumerableQuery<T> : EnumerableQuery, IOrderedQueryable<T>, IQueryable<T>, IOrderedQueryable, IQueryable, IQueryProvider, IEnumerable<T>, IEnumerable
    {
        private IEnumerable<T> enumerable;
        private System.Linq.Expressions.Expression expression;

        internal EnumerableQuery(IEnumerable<T> enumerable)
        {
            this.enumerable = enumerable;
            this.expression = System.Linq.Expressions.Expression.Constant(this);
        }

        internal EnumerableQuery(System.Linq.Expressions.Expression expression)
        {
            this.expression = expression;
        }

        private IEnumerator<T> GetEnumerator() => 
            this.enumerable?.GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => 
            this.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => 
            this.GetEnumerator();

        IQueryable IQueryProvider.CreateQuery(System.Linq.Expressions.Expression expression)
        {
            if (expression == null)
            {
                throw System.Linq.Error.ArgumentNull("expression");
            }
            Type type = TypeHelper.FindGenericType(typeof(IQueryable<>), expression.Type);
            if (type == null)
            {
                throw System.Linq.Error.ArgumentNotValid("expression");
            }
            return EnumerableQuery.Create(type.GetGenericArguments()[0], expression);
        }

        IQueryable<S> IQueryProvider.CreateQuery<S>(System.Linq.Expressions.Expression expression)
        {
            if (expression == null)
            {
                throw System.Linq.Error.ArgumentNull("expression");
            }
            if (!typeof(IQueryable<S>).IsAssignableFrom(expression.Type))
            {
                throw System.Linq.Error.ArgumentNotValid("expression");
            }
            return new EnumerableQuery<S>(expression);
        }

        object IQueryProvider.Execute(System.Linq.Expressions.Expression expression)
        {
            if (expression == null)
            {
                throw System.Linq.Error.ArgumentNull("expression");
            }
            typeof(EnumerableExecutor<>).MakeGenericType(new Type[] { expression.Type });
            return EnumerableExecutor.Create(expression).ExecuteBoxed();
        }

        S IQueryProvider.Execute<S>(System.Linq.Expressions.Expression expression)
        {
            if (expression == null)
            {
                throw System.Linq.Error.ArgumentNull("expression");
            }
            if (!typeof(S).IsAssignableFrom(expression.Type))
            {
                throw System.Linq.Error.ArgumentNotValid("expression");
            }
            return new EnumerableExecutor<S>(expression).Execute();
        }

        public override string ToString()
        {
            ConstantExpression expression = this.expression as ConstantExpression;
            if ((expression == null) || (expression.Value != this))
            {
                return this.expression.ToString();
            }
            if (this.enumerable != null)
            {
                return this.enumerable.ToString();
            }
            return "null";
        }

        internal override IEnumerable Enumerable =>
            this.enumerable;

        internal override System.Linq.Expressions.Expression Expression =>
            this.expression;

        Type IQueryable.ElementType =>
            typeof(T);

        System.Linq.Expressions.Expression IQueryable.Expression =>
            this.expression;

        IQueryProvider IQueryable.Provider =>
            this;
    }
}

