namespace System.Data.Objects.ELinq
{
    using System;
    using System.Data.Common.CommandTrees;
    using System.Linq.Expressions;

    internal sealed class Binding
    {
        internal readonly DbExpression CqtExpression;
        internal readonly Expression LinqExpression;

        internal Binding(Expression linqExpression, DbExpression cqtExpression)
        {
            this.LinqExpression = linqExpression;
            this.CqtExpression = cqtExpression;
        }
    }
}

