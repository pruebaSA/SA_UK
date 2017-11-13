namespace MS.Utility
{
    using MS.Internal.WindowsBase;
    using System;

    [FriendAccessAllowed]
    internal static class EventType
    {
        internal const byte Checkpoint = 8;
        internal const byte EndEvent = 2;
        internal const byte Info = 0;
        internal const byte StartEvent = 1;
    }
}

