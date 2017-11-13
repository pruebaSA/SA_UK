namespace System.Windows
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [FriendAccessAllowed]
    internal delegate object GetReadOnlyValueCallback(DependencyObject d, out BaseValueSourceInternal source);
}

