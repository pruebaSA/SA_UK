namespace System.Data.Common.EntitySql
{
    using System;

    internal class ErrorContext
    {
        internal string ErrorContextInfo;
        internal int InputPosition = -1;
        internal string QueryText;
        internal bool UseContextInfoAsResourceIdentifier = true;
    }
}

