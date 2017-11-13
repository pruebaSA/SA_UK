namespace System.Data.Entity
{
    using System;

    internal static class Error
    {
        internal static Exception ArgumentNull(string paramName) => 
            new ArgumentNullException(paramName);

        internal static Exception ArgumentOutOfRange(string paramName) => 
            new ArgumentOutOfRangeException(paramName);

        internal static Exception NotImplemented() => 
            new NotImplementedException();

        internal static Exception NotSupported() => 
            new NotSupportedException();
    }
}

