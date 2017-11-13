namespace System.Data.Mapping
{
    using System;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Xml.XPath;

    internal sealed class FunctionImportEntityTypeMappingConditionValue : FunctionImportEntityTypeMappingCondition
    {
        private readonly Memoizer<Type, object> _convertedValues;
        private readonly XPathNavigator _xPathValue;

        internal FunctionImportEntityTypeMappingConditionValue(string columnName, XPathNavigator columnValue) : base(columnName)
        {
            this._xPathValue = EntityUtil.CheckArgumentNull<XPathNavigator>(columnValue, "columnValue");
            this._convertedValues = new Memoizer<Type, object>(new Func<Type, object>(this.GetConditionValue), null);
        }

        internal override bool ColumnValueMatchesCondition(object columnValue)
        {
            if ((columnValue == null) || Convert.IsDBNull(columnValue))
            {
                return false;
            }
            Type arg = columnValue.GetType();
            object y = this._convertedValues.Evaluate(arg);
            return CdpEqualityComparer.DefaultEqualityComparer.Equals(columnValue, y);
        }

        private object GetConditionValue(Type columnValueType)
        {
            object obj2;
            PrimitiveType type;
            if (!ClrProviderManifest.Instance.TryGetPrimitiveType(columnValueType, out type) || !StorageMappingItemLoader.IsTypeSupportedForCondition(type.PrimitiveTypeKind))
            {
                throw EntityUtil.CommandExecution(Strings.Mapping_FunctionImport_UnsupportedType(base.ColumnName, columnValueType.FullName));
            }
            try
            {
                obj2 = this._xPathValue.ValueAs(columnValueType);
            }
            catch (FormatException)
            {
                throw EntityUtil.CommandExecution(Strings.Mapping_FunctionImport_ConditionValueTypeMismatch("FunctionImportMapping", base.ColumnName, columnValueType.FullName));
            }
            return obj2;
        }

        internal override ValueCondition ConditionValue =>
            new ValueCondition(this._xPathValue.Value);
    }
}

