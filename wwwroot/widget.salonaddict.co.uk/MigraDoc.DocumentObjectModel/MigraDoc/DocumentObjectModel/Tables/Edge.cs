namespace MigraDoc.DocumentObjectModel.Tables
{
    using System;

    [Flags]
    public enum Edge
    {
        Bottom = 4,
        Box = 15,
        Cross = 0xc0,
        DiagonalDown = 0x40,
        DiagonalUp = 0x80,
        Horizontal = 0x10,
        Interior = 0x30,
        Left = 2,
        Right = 8,
        Top = 1,
        Vertical = 0x20
    }
}

