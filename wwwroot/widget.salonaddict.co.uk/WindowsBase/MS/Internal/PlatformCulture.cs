namespace MS.Internal
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Globalization;
    using System.Windows;

    [FriendAccessAllowed]
    internal static class PlatformCulture
    {
        public static CultureInfo Value
        {
            get
            {
                string str = System.Windows.SR.Get("WPF_UILanguage");
                Invariant.Assert(!string.IsNullOrEmpty(str), "No UILanguage was specified in stringtable.");
                return new CultureInfo(str);
            }
        }
    }
}

