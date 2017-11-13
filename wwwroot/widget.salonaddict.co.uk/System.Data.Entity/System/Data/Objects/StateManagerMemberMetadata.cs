namespace System.Data.Objects
{
    using System;
    using System.Data;
    using System.Data.Mapping;
    using System.Data.Metadata.Edm;

    internal sealed class StateManagerMemberMetadata
    {
        private readonly EdmProperty _clrProperty;
        private readonly EdmProperty _edmProperty;
        private readonly bool _isComplexType;
        private readonly bool _isPartOfKey;

        internal StateManagerMemberMetadata(ObjectPropertyMapping memberMap, EdmProperty memberMetadata, bool isPartOfKey)
        {
            this._clrProperty = memberMap.ClrProperty;
            this._edmProperty = memberMetadata;
            this._isPartOfKey = isPartOfKey;
            this._isComplexType = Helper.IsEntityType(this._edmProperty.TypeUsage.EdmType) || Helper.IsComplexType(this._edmProperty.TypeUsage.EdmType);
        }

        public object GetValue(object userObject) => 
            LightweightCodeGenerator.GetValue(this._clrProperty, userObject);

        public void SetValue(object userObject, object value)
        {
            if (DBNull.Value == value)
            {
                value = null;
            }
            if (this.IsComplex && (value == null))
            {
                throw EntityUtil.NullableComplexTypesNotSupported(this.CLayerName);
            }
            LightweightCodeGenerator.SetValue(this._clrProperty, userObject, value);
        }

        internal EdmProperty CdmMetadata =>
            this._edmProperty;

        internal string CLayerName =>
            this._edmProperty.Name;

        internal EdmProperty ClrMetadata =>
            this._clrProperty;

        internal Type ClrType =>
            this._clrProperty.TypeUsage.EdmType.ClrType;

        internal bool IsComplex =>
            this._isComplexType;

        internal bool IsPartOfKey =>
            this._isPartOfKey;
    }
}

