namespace MS.Internal
{
    using MS.Internal.WindowsBase;
    using System;

    [Flags, FriendAccessAllowed]
    internal enum ShutDownEvents : ushort
    {
        All = 7,
        AppDomain = 3,
        DispatcherShutdown = 4,
        DomainUnload = 1,
        ProcessExit = 2
    }
}

