﻿namespace System.Windows.Forms.Internal
{
    using System;

    [Flags]
    internal enum IntTextFormatFlags
    {
        Bottom = 8,
        CalculateRectangle = 0x400,
        Default = 0,
        EndEllipsis = 0x8000,
        ExpandTabs = 0x40,
        ExternalLeading = 0x200,
        HidePrefix = 0x100000,
        HorizontalCenter = 1,
        Internal = 0x1000,
        Left = 0,
        ModifyString = 0x10000,
        NoClipping = 0x100,
        NoFullWidthCharacterBreak = 0x80000,
        NoPrefix = 0x800,
        PathEllipsis = 0x4000,
        PrefixOnly = 0x200000,
        Right = 2,
        RightToLeft = 0x20000,
        SingleLine = 0x20,
        TabStop = 0x80,
        TextBoxControl = 0x2000,
        Top = 0,
        VerticalCenter = 4,
        WordBreak = 0x10,
        WordEllipsis = 0x40000
    }
}

