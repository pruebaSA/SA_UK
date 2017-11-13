namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Globalization;
    using System.Text;

    internal class DiscriminatedEntityIdentity : EntityIdentity
    {
        private SimpleColumnMap m_entitySetColumn;
        private EntitySet[] m_entitySetMap;

        internal DiscriminatedEntityIdentity(SimpleColumnMap entitySetColumn, EntitySet[] entitySetMap, SimpleColumnMap[] keyColumns) : base(keyColumns)
        {
            this.m_entitySetColumn = entitySetColumn;
            this.m_entitySetMap = entitySetMap;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string str = string.Empty;
            builder.AppendFormat(CultureInfo.InvariantCulture, "[(Keys={", new object[0]);
            foreach (SimpleColumnMap map in base.Keys)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", new object[] { str, map });
                str = ",";
            }
            builder.AppendFormat(CultureInfo.InvariantCulture, "})]", new object[0]);
            return builder.ToString();
        }

        internal SimpleColumnMap EntitySetColumnMap =>
            this.m_entitySetColumn;

        internal EntitySet[] EntitySetMap =>
            this.m_entitySetMap;
    }
}

