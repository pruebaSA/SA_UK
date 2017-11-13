namespace System.Data.Common
{
    using System;
    using System.Data;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct FieldMetadata
    {
        private readonly EdmMember _fieldType;
        private readonly int _ordinal;
        public FieldMetadata(int ordinal, EdmMember fieldType)
        {
            if (ordinal < 0)
            {
                throw EntityUtil.ArgumentOutOfRange("ordinal");
            }
            if (fieldType == null)
            {
                throw EntityUtil.ArgumentNull("fieldType");
            }
            this._fieldType = fieldType;
            this._ordinal = ordinal;
        }

        public EdmMember FieldType =>
            this._fieldType;
        public int Ordinal =>
            this._ordinal;
    }
}

