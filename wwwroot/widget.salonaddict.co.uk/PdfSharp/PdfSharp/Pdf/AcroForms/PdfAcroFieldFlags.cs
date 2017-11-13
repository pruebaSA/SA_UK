namespace PdfSharp.Pdf.AcroForms
{
    using System;

    public enum PdfAcroFieldFlags
    {
        Combo = 0x20000,
        DoNotScroll = 0x800000,
        DoNotSpellCheckChoiseField = 0x400000,
        DoNotSpellCheckTextField = 0x400000,
        Edit = 0x40000,
        FileSelect = 0x100000,
        Multiline = 0x1000,
        MultiSelect = 0x200000,
        NoExport = 4,
        NoToggleToOff = 0x4000,
        Password = 0x2000,
        Pushbutton = 0x10000,
        Radio = 0x8000,
        ReadOnly = 1,
        Required = 2,
        Sort = 0x80000
    }
}

