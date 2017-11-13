namespace System.Windows.Markup
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [FriendAccessAllowed]
    internal delegate bool IsXmlNamespaceSupportedCallback(string xmlNamespace, out string newXmlNamespace);
}

