namespace System.Data.OleDb
{
    using System;
    using System.Data.Common;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct ITransactionJoinWrapper : IDisposable
    {
        private object _unknown;
        private NativeMethods.ITransactionJoin _value;
        internal ITransactionJoinWrapper(object unknown)
        {
            this._unknown = unknown;
            this._value = unknown as NativeMethods.ITransactionJoin;
        }

        internal NativeMethods.ITransactionJoin Value =>
            this._value;
        public void Dispose()
        {
            object o = this._unknown;
            this._unknown = null;
            this._value = null;
            if (o != null)
            {
                Marshal.ReleaseComObject(o);
            }
        }
    }
}

