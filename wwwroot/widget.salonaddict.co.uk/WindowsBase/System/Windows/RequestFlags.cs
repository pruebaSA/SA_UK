namespace System.Windows
{
    using MS.Internal.WindowsBase;
    using System;

    [FriendAccessAllowed]
    internal enum RequestFlags
    {
        AnimationBaseValue = 1,
        CoercionBaseValue = 2,
        DeferredReferences = 4,
        FullyResolved = 0,
        RawEntry = 0x10,
        SkipDefault = 8
    }
}

