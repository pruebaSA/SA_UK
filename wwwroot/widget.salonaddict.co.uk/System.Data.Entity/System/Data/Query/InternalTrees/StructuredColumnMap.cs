namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Globalization;
    using System.Text;

    internal abstract class StructuredColumnMap : ColumnMap
    {
        private readonly ColumnMap[] m_properties;

        internal StructuredColumnMap(TypeUsage type, string name, ColumnMap[] properties) : base(type, name)
        {
            this.m_properties = properties;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string str = string.Empty;
            builder.Append("{");
            foreach (ColumnMap map in this.Properties)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", new object[] { str, map });
                str = ",";
            }
            builder.Append("}");
            return builder.ToString();
        }

        internal virtual SimpleColumnMap NullSentinel =>
            null;

        internal ColumnMap[] Properties =>
            this.m_properties;
    }
}

