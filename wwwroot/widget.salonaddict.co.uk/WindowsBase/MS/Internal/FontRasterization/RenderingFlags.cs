namespace MS.Internal.FontRasterization
{
    using MS.Internal.WindowsBase;
    using System;

    [FriendAccessAllowed, Flags]
    internal enum RenderingFlags
    {
        BoldSimulation = 2,
        Hinting = 1,
        ItalicSimulation = 4,
        None = 0,
        SidewaysItalicSimulation = 8
    }
}

