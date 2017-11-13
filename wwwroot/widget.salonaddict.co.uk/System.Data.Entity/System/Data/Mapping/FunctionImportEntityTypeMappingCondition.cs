namespace System.Data.Mapping
{
    using System;
    using System.Data;

    internal abstract class FunctionImportEntityTypeMappingCondition
    {
        internal readonly string ColumnName;

        protected FunctionImportEntityTypeMappingCondition(string columnName)
        {
            this.ColumnName = EntityUtil.CheckArgumentNull<string>(columnName, "columnName");
        }

        internal abstract bool ColumnValueMatchesCondition(object columnValue);
        public override string ToString() => 
            this.ConditionValue.ToString();

        internal abstract ValueCondition ConditionValue { get; }
    }
}

