namespace System.Xml.Xsl
{
    using System;

    [Flags]
    internal enum XslFlags
    {
        AnyType = 0x3f,
        Boolean = 4,
        Current = 0x100,
        FocusFilter = 0x700,
        FullFocus = 0x700,
        HasCalls = 0x1000,
        Last = 0x400,
        MayBeDefault = 0x2000,
        Node = 8,
        Nodeset = 0x10,
        None = 0,
        Number = 2,
        Position = 0x200,
        Rtf = 0x20,
        SideEffects = 0x4000,
        Stop = 0x8000,
        String = 1,
        TypeFilter = 0x3f
    }
}

