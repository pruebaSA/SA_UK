namespace MS.Internal.FontRasterization
{
    using MS.Internal.WindowsBase;
    using System;

    [FriendAccessAllowed]
    internal enum OutlineTokenType
    {
        ClosePath = 5,
        CurveTo = 4,
        LineTo = 3,
        MoveTo = 2
    }
}

