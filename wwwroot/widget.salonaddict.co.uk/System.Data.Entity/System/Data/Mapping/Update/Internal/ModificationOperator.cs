namespace System.Data.Mapping.Update.Internal
{
    using System;

    internal enum ModificationOperator : byte
    {
        Delete = 2,
        Insert = 1,
        Update = 0
    }
}

