﻿namespace System.Reflection.Emit
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, StructLayout(LayoutKind.Sequential), ComVisible(true)]
    public struct FieldToken
    {
        public static readonly FieldToken Empty;
        internal int m_fieldTok;
        internal object m_class;
        internal FieldToken(int field, Type fieldClass)
        {
            this.m_fieldTok = field;
            this.m_class = fieldClass;
        }

        public int Token =>
            this.m_fieldTok;
        public override int GetHashCode() => 
            this.m_fieldTok;

        public override bool Equals(object obj) => 
            ((obj is FieldToken) && this.Equals((FieldToken) obj));

        public bool Equals(FieldToken obj) => 
            ((obj.m_fieldTok == this.m_fieldTok) && (obj.m_class == this.m_class));

        public static bool operator ==(FieldToken a, FieldToken b) => 
            a.Equals(b);

        public static bool operator !=(FieldToken a, FieldToken b) => 
            !(a == b);

        static FieldToken()
        {
            Empty = new FieldToken();
        }
    }
}

