namespace System.Net
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct HeaderVariantInfo
    {
        private string m_name;
        private CookieVariant m_variant;
        internal HeaderVariantInfo(string name, CookieVariant variant)
        {
            this.m_name = name;
            this.m_variant = variant;
        }

        internal string Name =>
            this.m_name;
        internal CookieVariant Variant =>
            this.m_variant;
    }
}

