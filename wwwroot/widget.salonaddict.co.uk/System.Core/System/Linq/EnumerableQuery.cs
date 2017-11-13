﻿namespace System.Linq
{
    using System;
    using System.Collections;
    using System.Linq.Expressions;
    using System.Reflection;

    internal abstract class EnumerableQuery
    {
        protected EnumerableQuery()
        {
        }

        internal static IQueryable Create(Type elementType, IEnumerable sequence) => 
            ((IQueryable) Activator.CreateInstance(typeof(EnumerableQuery<>).MakeGenericType(new Type[] { elementType }), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new object[] { sequence }, null));

        internal static IQueryable Create(Type elementType, System.Linq.Expressions.Expression expression) => 
            ((IQueryable) Activator.CreateInstance(typeof(EnumerableQuery<>).MakeGenericType(new Type[] { elementType }), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new object[] { expression }, null));

        internal abstract IEnumerable Enumerable { get; }

        internal abstract System.Linq.Expressions.Expression Expression { get; }
    }
}

