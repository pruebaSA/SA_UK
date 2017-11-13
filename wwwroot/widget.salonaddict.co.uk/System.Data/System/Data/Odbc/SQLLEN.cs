namespace System.Data.Odbc
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct SQLLEN
    {
        internal int _value;
        internal SQLLEN(int value)
        {
            this._value = value;
        }

        internal SQLLEN(long value)
        {
            this._value = (int) value;
        }

        internal SQLLEN(IntPtr value)
        {
            this._value = value.ToInt32();
        }

        public static implicit operator SQLLEN(int value) => 
            new SQLLEN(value);

        public static explicit operator SQLLEN(long value) => 
            new SQLLEN(value);

        public static implicit operator int(SQLLEN value) => 
            value._value;

        public static explicit operator long(SQLLEN value) => 
            ((long) value._value);

        public long ToInt64() => 
            ((long) this._value);
    }
}

