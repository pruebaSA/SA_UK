﻿namespace System.Reflection.Emit
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct SignatureToken
    {
        public static readonly SignatureToken Empty;
        internal int m_signature;
        internal ModuleBuilder m_moduleBuilder;
        internal SignatureToken(int str, ModuleBuilder mod)
        {
            this.m_signature = str;
            this.m_moduleBuilder = mod;
        }

        public int Token =>
            this.m_signature;
        public override int GetHashCode() => 
            this.m_signature;

        public override bool Equals(object obj) => 
            ((obj is SignatureToken) && this.Equals((SignatureToken) obj));

        public bool Equals(SignatureToken obj) => 
            (obj.m_signature == this.m_signature);

        public static bool operator ==(SignatureToken a, SignatureToken b) => 
            a.Equals(b);

        public static bool operator !=(SignatureToken a, SignatureToken b) => 
            !(a == b);

        static SignatureToken()
        {
            Empty = new SignatureToken();
        }
    }
}

