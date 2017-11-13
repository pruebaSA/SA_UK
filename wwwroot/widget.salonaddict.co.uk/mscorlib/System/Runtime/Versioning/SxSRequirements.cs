namespace System.Runtime.Versioning
{
    using System;

    [Flags]
    internal enum SxSRequirements
    {
        AppDomainID = 1,
        AssemblyName = 4,
        None = 0,
        ProcessID = 2,
        TypeName = 8
    }
}

