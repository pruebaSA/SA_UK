﻿namespace System.Linq
{
    using System;

    internal sealed class SystemCore_EnumerableDebugViewEmptyException : Exception
    {
        public string Empty =>
            Strings.EmptyEnumerable;
    }
}

