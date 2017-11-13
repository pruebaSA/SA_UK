﻿namespace System.Data.Mapping.ViewGeneration.QueryRewriting
{
    using System;

    internal abstract class TileQueryProcessor<T_Query> where T_Query: ITileQuery
    {
        protected TileQueryProcessor()
        {
        }

        internal abstract T_Query CreateDerivedViewBySelectingConstantAttributes(T_Query query);
        internal abstract T_Query Difference(T_Query arg1, T_Query arg2);
        internal abstract T_Query Intersect(T_Query arg1, T_Query arg2);
        internal abstract bool IsSatisfiable(T_Query query);
        internal abstract T_Query Union(T_Query arg1, T_Query arg2);
    }
}

