namespace System.Data.Mapping
{
    using System;
    using System.Data;
    using System.Data.Metadata.Edm;
    using System.Globalization;

    internal sealed class StorageFunctionResultBinding
    {
        internal readonly string ColumnName;
        internal readonly EdmProperty Property;

        internal StorageFunctionResultBinding(string columnName, EdmProperty property)
        {
            this.ColumnName = EntityUtil.CheckArgumentNull<string>(columnName, "columnName");
            this.Property = EntityUtil.CheckArgumentNull<EdmProperty>(property, "property");
        }

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, "{0}->{1}", new object[] { this.ColumnName, this.Property });
    }
}

