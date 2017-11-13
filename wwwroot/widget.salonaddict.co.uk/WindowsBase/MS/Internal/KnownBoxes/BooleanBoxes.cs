namespace MS.Internal.KnownBoxes
{
    using MS.Internal.WindowsBase;
    using System;

    [FriendAccessAllowed]
    internal static class BooleanBoxes
    {
        internal static object FalseBox = false;
        internal static object TrueBox = true;

        internal static object Box(bool value)
        {
            if (value)
            {
                return TrueBox;
            }
            return FalseBox;
        }
    }
}

