namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    internal class OleStrCAMarshaler : BaseCAMarshaler
    {
        public OleStrCAMarshaler(NativeMethods.CA_STRUCT caAddr) : base(caAddr)
        {
        }

        protected override Array CreateArray() => 
            new string[base.Count];

        protected override object GetItemFromAddress(IntPtr addr)
        {
            string str = Marshal.PtrToStringUni(addr);
            Marshal.FreeCoTaskMem(addr);
            return str;
        }

        public override System.Type ItemType =>
            typeof(string);
    }
}

