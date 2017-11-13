namespace System.Data.Mapping
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class StorageConditionPropertyMapping : StoragePropertyMapping
    {
        private EdmProperty m_columnMember;
        private bool? m_isNull;
        private object m_value;

        internal StorageConditionPropertyMapping(EdmProperty cdmMember, EdmProperty columnMember, object value, bool? isNull) : base(cdmMember)
        {
            this.m_columnMember = columnMember;
            this.m_value = value;
            this.m_isNull = isNull;
        }

        internal override void Print(int index)
        {
            StorageEntityContainerMapping.GetPrettyPrintString(ref index);
            StringBuilder builder = new StringBuilder();
            builder.Append("ConditionPropertyMapping");
            builder.Append("   ");
            if (this.EdmProperty != null)
            {
                builder.Append("Name:");
                builder.Append(this.EdmProperty.Name);
                builder.Append("   ");
            }
            if (this.ColumnProperty != null)
            {
                builder.Append("Column Name:");
                builder.Append(this.ColumnProperty.Name);
                builder.Append("   ");
            }
            if (this.Value != null)
            {
                builder.Append("Value:");
                builder.Append("'" + this.Value + "'");
                builder.Append("   ");
                builder.Append("Value CLR Type:");
                builder.Append("'" + this.Value.GetType() + "'");
                builder.Append("   ");
            }
            builder.Append("Value TypeMetadata:");
            EdmType edmType = this.ColumnProperty?.TypeUsage.EdmType;
            if (edmType != null)
            {
                builder.Append("'" + edmType.FullName + "'");
                builder.Append("   ");
            }
            if (this.IsNull.HasValue)
            {
                builder.Append("IsNull:");
                builder.Append(this.IsNull);
                builder.Append("   ");
            }
            Console.WriteLine(builder.ToString());
        }

        internal EdmProperty ColumnProperty =>
            this.m_columnMember;

        internal bool? IsNull =>
            this.m_isNull;

        internal object Value =>
            this.m_value;
    }
}

