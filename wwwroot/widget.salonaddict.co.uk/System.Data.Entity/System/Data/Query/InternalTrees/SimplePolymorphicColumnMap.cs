namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;

    internal class SimplePolymorphicColumnMap : TypedColumnMap
    {
        private Dictionary<object, TypedColumnMap> m_typedColumnMap;
        private SimpleColumnMap m_typeDiscriminator;

        internal SimplePolymorphicColumnMap(TypeUsage type, string name, ColumnMap[] baseTypeColumns, SimpleColumnMap typeDiscriminator, Dictionary<object, TypedColumnMap> typeChoices) : base(type, name, baseTypeColumns)
        {
            this.m_typedColumnMap = typeChoices;
            this.m_typeDiscriminator = typeDiscriminator;
        }

        [DebuggerNonUserCode]
        internal override void Accept<TArgType>(ColumnMapVisitor<TArgType> visitor, TArgType arg)
        {
            visitor.Visit(this, arg);
        }

        [DebuggerNonUserCode]
        internal override TResultType Accept<TResultType, TArgType>(ColumnMapVisitorWithResults<TResultType, TArgType> visitor, TArgType arg) => 
            visitor.Visit(this, arg);

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            string str = string.Empty;
            builder.AppendFormat(CultureInfo.InvariantCulture, "P{{TypeId={0}, ", new object[] { this.TypeDiscriminator });
            foreach (KeyValuePair<object, TypedColumnMap> pair in this.TypeChoices)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, "{0}({1},{2})", new object[] { str, pair.Key, pair.Value });
                str = ",";
            }
            builder.Append("}");
            return builder.ToString();
        }

        internal Dictionary<object, TypedColumnMap> TypeChoices =>
            this.m_typedColumnMap;

        internal SimpleColumnMap TypeDiscriminator =>
            this.m_typeDiscriminator;
    }
}

