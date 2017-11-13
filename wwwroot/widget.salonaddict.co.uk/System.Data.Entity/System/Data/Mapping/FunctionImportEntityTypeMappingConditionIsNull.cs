namespace System.Data.Mapping
{
    using System;

    internal sealed class FunctionImportEntityTypeMappingConditionIsNull : FunctionImportEntityTypeMappingCondition
    {
        internal readonly bool IsNull;

        internal FunctionImportEntityTypeMappingConditionIsNull(string columnName, bool isNull) : base(columnName)
        {
            this.IsNull = isNull;
        }

        internal override bool ColumnValueMatchesCondition(object columnValue)
        {
            bool flag = (columnValue == null) || Convert.IsDBNull(columnValue);
            return (flag == this.IsNull);
        }

        internal override ValueCondition ConditionValue
        {
            get
            {
                if (!this.IsNull)
                {
                    return ValueCondition.IsNotNull;
                }
                return ValueCondition.IsNull;
            }
        }
    }
}

