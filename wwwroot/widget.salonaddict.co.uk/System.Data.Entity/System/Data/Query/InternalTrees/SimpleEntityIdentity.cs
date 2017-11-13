namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Data.Metadata.Edm;
    using System.Globalization;
    using System.Text;

    internal class SimpleEntityIdentity : EntityIdentity
    {
        private System.Data.Metadata.Edm.EntitySet m_entitySet;

        internal SimpleEntityIdentity(System.Data.Metadata.Edm.EntitySet entitySet, SimpleColumnMap[] keyColumns) : base(keyColumns)
        {
            this.m_entitySet = entitySet;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string str = string.Empty;
            builder.AppendFormat(CultureInfo.InvariantCulture, "[(ES={0}) (Keys={", new object[] { this.EntitySet.Name });
            foreach (SimpleColumnMap map in base.Keys)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, "{0}{1}", new object[] { str, map });
                str = ",";
            }
            builder.AppendFormat(CultureInfo.InvariantCulture, "})]", new object[0]);
            return builder.ToString();
        }

        internal System.Data.Metadata.Edm.EntitySet EntitySet =>
            this.m_entitySet;
    }
}

