namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.Utils;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;

    internal class MultipleDiscriminatorPolymorphicColumnMap : TypedColumnMap
    {
        private readonly Func<object[], EntityType> m_discriminate;
        private readonly Dictionary<EntityType, TypedColumnMap> m_typeChoices;
        private readonly SimpleColumnMap[] m_typeDiscriminators;

        internal MultipleDiscriminatorPolymorphicColumnMap(TypeUsage type, string name, ColumnMap[] baseTypeColumns, SimpleColumnMap[] typeDiscriminators, Dictionary<EntityType, TypedColumnMap> typeChoices, Func<object[], EntityType> discriminate) : base(type, name, baseTypeColumns)
        {
            this.m_typeDiscriminators = typeDiscriminators;
            this.m_typeChoices = typeChoices;
            this.m_discriminate = discriminate;
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
            builder.AppendFormat(CultureInfo.InvariantCulture, "P{{TypeId=<{0}>, ", new object[] { StringUtil.ToCommaSeparatedString(this.TypeDiscriminators) });
            foreach (KeyValuePair<EntityType, TypedColumnMap> pair in this.TypeChoices)
            {
                builder.AppendFormat(CultureInfo.InvariantCulture, "{0}(<{1}>,{2})", new object[] { str, pair.Key, pair.Value });
                str = ",";
            }
            builder.Append("}");
            return builder.ToString();
        }

        internal Func<object[], EntityType> Discriminate =>
            this.m_discriminate;

        internal Dictionary<EntityType, TypedColumnMap> TypeChoices =>
            this.m_typeChoices;

        internal SimpleColumnMap[] TypeDiscriminators =>
            this.m_typeDiscriminators;
    }
}

