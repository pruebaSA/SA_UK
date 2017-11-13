namespace System.Linq.Expressions
{
    using System;
    using System.Collections.ObjectModel;

    public sealed class Expression<TDelegate> : LambdaExpression
    {
        internal Expression(Expression body, ReadOnlyCollection<ParameterExpression> parameters) : base(body, typeof(TDelegate), parameters)
        {
        }

        public TDelegate Compile() => 
            ((TDelegate) base.Compile());
    }
}

