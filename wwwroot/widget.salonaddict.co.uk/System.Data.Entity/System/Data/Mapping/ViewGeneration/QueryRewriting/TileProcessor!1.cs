﻿namespace System.Data.Mapping.ViewGeneration.QueryRewriting
{
    using System;

    internal abstract class TileProcessor<T_Tile>
    {
        protected TileProcessor()
        {
        }

        internal abstract T_Tile AntiSemiJoin(T_Tile a, T_Tile b);
        internal abstract T_Tile GetArg1(T_Tile tile);
        internal abstract T_Tile GetArg2(T_Tile tile);
        internal abstract TileOpKind GetOpKind(T_Tile tile);
        internal abstract bool IsEmpty(T_Tile tile);
        internal abstract T_Tile Join(T_Tile a, T_Tile b);
        internal abstract T_Tile Union(T_Tile a, T_Tile b);
    }
}

