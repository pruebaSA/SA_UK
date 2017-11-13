namespace System.Windows
{
    using MS.Internal.WindowsBase;
    using System;

    [FriendAccessAllowed]
    internal enum FullValueSource : short
    {
        HasExpressionMarker = 0x100,
        IsAnimated = 0x20,
        IsCoerced = 0x40,
        IsExpression = 0x10,
        IsPotentiallyADeferredReference = 0x80,
        ModifiersMask = 0x70,
        ValueSourceMask = 15
    }
}

