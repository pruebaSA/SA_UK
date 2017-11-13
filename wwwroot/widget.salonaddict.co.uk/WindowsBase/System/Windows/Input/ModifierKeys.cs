namespace System.Windows.Input
{
    using System;
    using System.ComponentModel;
    using System.Windows.Markup;

    [Flags, TypeConverter(typeof(ModifierKeysConverter)), ValueSerializer(typeof(ModifierKeysValueSerializer))]
    public enum ModifierKeys
    {
        Alt = 1,
        Control = 2,
        None = 0,
        Shift = 4,
        Windows = 8
    }
}

