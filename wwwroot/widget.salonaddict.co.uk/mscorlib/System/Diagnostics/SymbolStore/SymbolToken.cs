namespace System.Diagnostics.SymbolStore
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct SymbolToken
    {
        internal int m_token;
        public SymbolToken(int val)
        {
            this.m_token = val;
        }

        public int GetToken() => 
            this.m_token;

        public override int GetHashCode() => 
            this.m_token;

        public override bool Equals(object obj) => 
            ((obj is SymbolToken) && this.Equals((SymbolToken) obj));

        public bool Equals(SymbolToken obj) => 
            (obj.m_token == this.m_token);

        public static bool operator ==(SymbolToken a, SymbolToken b) => 
            a.Equals(b);

        public static bool operator !=(SymbolToken a, SymbolToken b) => 
            !(a == b);
    }
}

