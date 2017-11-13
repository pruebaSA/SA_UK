namespace System.Data.Mapping.ViewGeneration.QueryRewriting
{
    using System;
    using System.Globalization;

    internal class TileBinaryOperator<T_Query> : Tile<T_Query> where T_Query: ITileQuery
    {
        private readonly Tile<T_Query> m_arg1;
        private readonly Tile<T_Query> m_arg2;

        public TileBinaryOperator(Tile<T_Query> arg1, Tile<T_Query> arg2, TileOpKind opKind, T_Query query) : base(opKind, query)
        {
            this.m_arg1 = arg1;
            this.m_arg2 = arg2;
        }

        internal override Tile<T_Query> Replace(Tile<T_Query> oldTile, Tile<T_Query> newTile)
        {
            Tile<T_Query> tile = this.Arg1.Replace(oldTile, newTile);
            Tile<T_Query> tile2 = this.Arg2.Replace(oldTile, newTile);
            if ((tile == this.Arg1) && (tile2 == this.Arg2))
            {
                return this;
            }
            return new TileBinaryOperator<T_Query>(tile, tile2, base.OpKind, base.Query);
        }

        public override Tile<T_Query> Arg1 =>
            this.m_arg1;

        public override Tile<T_Query> Arg2 =>
            this.m_arg2;

        public override string Description
        {
            get
            {
                string format = null;
                switch (base.OpKind)
                {
                    case TileOpKind.Union:
                        format = "({0} | {1})";
                        break;

                    case TileOpKind.Join:
                        format = "({0} & {1})";
                        break;

                    case TileOpKind.AntiSemiJoin:
                        format = "({0} - {1})";
                        break;
                }
                return string.Format(CultureInfo.InvariantCulture, format, new object[] { this.Arg1.Description, this.Arg2.Description });
            }
        }
    }
}

