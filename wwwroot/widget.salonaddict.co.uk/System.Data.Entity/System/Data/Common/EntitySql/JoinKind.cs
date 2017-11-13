namespace System.Data.Common.EntitySql
{
    using System;

    internal enum JoinKind
    {
        Cross,
        Inner,
        LeftOuter,
        FullOuter,
        RightOuter
    }
}

