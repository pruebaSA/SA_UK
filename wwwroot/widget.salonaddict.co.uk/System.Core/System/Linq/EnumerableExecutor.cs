namespace System.Linq
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    internal abstract class EnumerableExecutor
    {
        protected EnumerableExecutor()
        {
        }

        internal static EnumerableExecutor Create(Expression expression) => 
            ((EnumerableExecutor) Activator.CreateInstance(typeof(EnumerableExecutor<>).MakeGenericType(new Type[] { expression.Type }), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new object[] { expression }, null));

        internal abstract object ExecuteBoxed();
    }
}

