namespace System.ServiceModel.Activation
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    internal struct METADATA_RECORD
    {
        public uint dwMDIdentifier;
        public uint dwMDAttributes;
        public uint dwMDUserType;
        public uint dwMDDataType;
        public uint dwMDDataLen;
        public IntPtr pbMDData;
        public uint dwMDDataTag;
    }
}

