namespace System.Data.Common.CommandTrees
{
    using System;

    internal enum DbCommandTreeKind
    {
        Query,
        Update,
        Insert,
        Delete,
        Function
    }
}

