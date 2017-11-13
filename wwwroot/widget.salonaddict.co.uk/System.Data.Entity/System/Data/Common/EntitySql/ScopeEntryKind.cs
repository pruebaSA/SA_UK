namespace System.Data.Common.EntitySql
{
    using System;

    internal enum ScopeEntryKind
    {
        WithSourceVar,
        SourceVar,
        JoinSourceVar,
        ApplySourceVar,
        ProjectList,
        DummyGroupKey
    }
}

