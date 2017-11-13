namespace System.Data.Mapping
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Text;

    internal class StorageScalarPropertyMapping : StoragePropertyMapping
    {
        private EdmProperty m_columnMember;

        internal StorageScalarPropertyMapping(EdmProperty member, EdmProperty columnMember) : base(member)
        {
            this.m_columnMember = columnMember;
        }

        internal override void Print(int index)
        {
            StorageEntityContainerMapping.GetPrettyPrintString(ref index);
            StringBuilder builder = new StringBuilder();
            builder.Append("ScalarPropertyMapping");
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
            }
            Console.WriteLine(builder.ToString());
        }

        internal EdmProperty ColumnProperty =>
            this.m_columnMember;
    }
}

