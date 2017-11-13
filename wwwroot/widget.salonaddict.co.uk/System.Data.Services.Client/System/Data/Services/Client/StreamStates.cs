namespace System.Data.Services.Client
{
    using System;

    [Flags]
    internal enum StreamStates
    {
        NoStream,
        Added,
        Modified
    }
}

