namespace System.Data.Common.EntitySql
{
    using System;

    internal enum LiteralKind
    {
        Number,
        String,
        NonUnicodeString,
        UnicodeString,
        Boolean,
        Binary,
        DateTime,
        Time,
        DateTimeOffset,
        Guid,
        Null
    }
}

