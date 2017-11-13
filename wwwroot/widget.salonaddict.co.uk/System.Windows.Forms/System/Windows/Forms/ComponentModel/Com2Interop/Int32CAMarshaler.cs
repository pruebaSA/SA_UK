namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    using System;
    using System.Windows.Forms;

    internal class Int32CAMarshaler : BaseCAMarshaler
    {
        public Int32CAMarshaler(NativeMethods.CA_STRUCT caStruct) : base(caStruct)
        {
        }

        protected override Array CreateArray() => 
            new int[base.Count];

        protected override object GetItemFromAddress(IntPtr addr) => 
            addr.ToInt32();

        public override System.Type ItemType =>
            typeof(int);
    }
}

