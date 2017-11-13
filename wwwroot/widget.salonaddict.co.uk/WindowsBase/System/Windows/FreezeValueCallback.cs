namespace System.Windows
{
    using System;
    using System.Runtime.CompilerServices;

    internal delegate bool FreezeValueCallback(DependencyObject d, DependencyProperty dp, EntryIndex entryIndex, PropertyMetadata metadata, bool isChecking);
}

