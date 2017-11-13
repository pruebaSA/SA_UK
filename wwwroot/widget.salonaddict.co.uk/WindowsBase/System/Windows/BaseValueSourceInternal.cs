namespace System.Windows
{
    using MS.Internal.WindowsBase;
    using System;

    [FriendAccessAllowed]
    internal enum BaseValueSourceInternal : short
    {
        Default = 1,
        ImplicitReference = 8,
        Inherited = 2,
        Local = 11,
        ParentTemplate = 9,
        ParentTemplateTrigger = 10,
        Style = 5,
        StyleTrigger = 7,
        TemplateTrigger = 6,
        ThemeStyle = 3,
        ThemeStyleTrigger = 4,
        Unknown = 0
    }
}

