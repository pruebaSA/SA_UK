namespace MS.Internal.IO.Packaging.CompoundFile
{
    using MS.Internal.WindowsBase;
    using System;

    [FriendAccessAllowed, Flags]
    internal enum ContainerFlags
    {
        ExecuteInstrumentation = 0x10,
        HostInBrowser = 1,
        Metro = 8,
        Writable = 2
    }
}

