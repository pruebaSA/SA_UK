namespace System.Data.Mapping.ViewGeneration.QueryRewriting
{
    using System;

    internal class TileNamed<T_Query> : Tile<T_Query> where T_Query: ITileQuery
    {
        public TileNamed(T_Query namedQuery) : base(TileOpKind.Named, namedQuery)
        {
        }

        internal override Tile<T_Query> Replace(Tile<T_Query> oldTile, Tile<T_Query> newTile)
        {
            if (this != oldTile)
            {
                return this;
            }
            return newTile;
        }

        public override string ToString() => 
            base.Query.ToString();

        public override Tile<T_Query> Arg1 =>
            null;

        public override Tile<T_Query> Arg2 =>
            null;

        public override string Description =>
            base.Query.Description;

        public T_Query NamedQuery =>
            base.Query;
    }
}

