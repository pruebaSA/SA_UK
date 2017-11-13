namespace System.Data.Common.Internal.Materialization
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Data.Objects.ELinq;
    using System.Data.Objects.Internal;
    using System.Data.Query.InternalTrees;
    using System.Globalization;
    using System.Text;

    internal class ColumnMapKeyBuilder : ColumnMapVisitor<int>
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly SpanIndex _spanIndex;

        private ColumnMapKeyBuilder(SpanIndex spanIndex)
        {
            this._spanIndex = spanIndex;
        }

        internal void Append(string value)
        {
            this._builder.Append(value);
        }

        private void Append(string prefix, IEnumerable<ColumnMap> elements)
        {
            this.Append(prefix);
            this.Append("{");
            if (elements != null)
            {
                string str = string.Empty;
                foreach (ColumnMap map in elements)
                {
                    this.Append(str, map);
                    str = ",";
                }
            }
            this.Append("}");
        }

        internal void Append(string prefix, EdmType type)
        {
            if (type != null)
            {
                this.Append(prefix, type.NamespaceName);
                this.Append(".", type.Name);
                if ((type.BuiltInTypeKind == BuiltInTypeKind.RowType) && (this._spanIndex != null))
                {
                    this.Append("<<");
                    Dictionary<int, AssociationEndMember> spanMap = this._spanIndex.GetSpanMap((RowType) type);
                    if (spanMap != null)
                    {
                        string str = string.Empty;
                        foreach (KeyValuePair<int, AssociationEndMember> pair in spanMap)
                        {
                            this.Append(str);
                            this.AppendValue("C", pair.Key);
                            this.Append(":", pair.Value.DeclaringType);
                            this.Append(".", pair.Value.Name);
                            str = ",";
                        }
                    }
                    this.Append(">>");
                }
            }
        }

        private void Append(string prefix, EntitySet entitySet)
        {
            if (entitySet != null)
            {
                this.Append(prefix, entitySet.EntityContainer.Name);
                this.Append(".", entitySet.Name);
            }
        }

        internal void Append(string prefix, TypeUsage type)
        {
            if (type != null)
            {
                InitializerMetadata metadata;
                if (InitializerMetadata.TryGetInitializerMetadata(type, out metadata))
                {
                    metadata.AppendColumnMapKey(this);
                }
                this.Append(prefix, type.EdmType);
            }
        }

        private void Append(string prefix, ColumnMap columnMap)
        {
            this.Append(prefix);
            this.Append("[");
            if (columnMap != null)
            {
                columnMap.Accept<int>(this, 0);
            }
            this.Append("]");
        }

        private void Append(string prefix, EntityIdentity entityIdentity)
        {
            this.Append(prefix);
            this.Append("[");
            this.Append(",K", entityIdentity.Keys);
            SimpleEntityIdentity identity = entityIdentity as SimpleEntityIdentity;
            if (identity != null)
            {
                this.Append(",", identity.EntitySet);
            }
            else
            {
                DiscriminatedEntityIdentity identity2 = (DiscriminatedEntityIdentity) entityIdentity;
                this.Append("CM", identity2.EntitySetColumnMap);
                foreach (EntitySet set in identity2.EntitySetMap)
                {
                    this.Append(",E", set);
                }
            }
            this.Append("]");
        }

        private void Append(string prefix, string value)
        {
            this.Append(prefix);
            this.Append("'");
            this.Append(value);
            this.Append("'");
        }

        internal void Append(string prefix, Type type)
        {
            this.Append(prefix, type.AssemblyQualifiedName);
        }

        private void AppendValue(string prefix, object value)
        {
            this.Append(prefix, string.Format(CultureInfo.InvariantCulture, "{0}", new object[] { value }));
        }

        internal static string GetColumnMapKey(ColumnMap columnMap, SpanIndex spanIndex)
        {
            ColumnMapKeyBuilder visitor = new ColumnMapKeyBuilder(spanIndex);
            columnMap.Accept<int>(visitor, 0);
            return visitor._builder.ToString();
        }

        internal override void Visit(ComplexTypeColumnMap columnMap, int dummy)
        {
            this.Append("C-", columnMap.Type);
            this.Append(",N", columnMap.NullSentinel);
            this.Append(",P", columnMap.Properties);
        }

        internal override void Visit(DiscriminatedCollectionColumnMap columnMap, int dummy)
        {
            this.Append("DC-D", columnMap.Discriminator);
            this.AppendValue(",DV", columnMap.DiscriminatorValue);
            this.Append(",FK", columnMap.ForeignKeys);
            this.Append(",K", columnMap.Keys);
            this.Append(",E", columnMap.Element);
        }

        internal override void Visit(EntityColumnMap columnMap, int dummy)
        {
            this.Append("E-", columnMap.Type);
            this.Append(",N", columnMap.NullSentinel);
            this.Append(",P", columnMap.Properties);
            this.Append(",I", columnMap.EntityIdentity);
        }

        internal override void Visit(MultipleDiscriminatorPolymorphicColumnMap columnMap, int dummy)
        {
            this.Append(string.Format(CultureInfo.InvariantCulture, "MD-{0}", new object[] { Guid.NewGuid() }));
        }

        internal override void Visit(RecordColumnMap columnMap, int dummy)
        {
            this.Append("R-", columnMap.Type);
            this.Append(",N", columnMap.NullSentinel);
            this.Append(",P", columnMap.Properties);
        }

        internal override void Visit(RefColumnMap columnMap, int dummy)
        {
            this.Append("Ref-", columnMap.EntityIdentity);
        }

        internal override void Visit(ScalarColumnMap columnMap, int dummy)
        {
            string str = string.Format(CultureInfo.InvariantCulture, "S({0}-{1}:{2})", new object[] { columnMap.CommandId, columnMap.ColumnPos, columnMap.Type.Identity });
            this.Append(str);
        }

        internal override void Visit(SimpleCollectionColumnMap columnMap, int dummy)
        {
            this.Append("DC-FK", columnMap.ForeignKeys);
            this.Append(",K", columnMap.Keys);
            this.Append(",E", columnMap.Element);
        }

        internal override void Visit(SimplePolymorphicColumnMap columnMap, int dummy)
        {
            this.Append("SP-", columnMap.Type);
            this.Append(",D", columnMap.TypeDiscriminator);
            this.Append(",N", columnMap.NullSentinel);
            this.Append(",P", columnMap.Properties);
            foreach (KeyValuePair<object, TypedColumnMap> pair in columnMap.TypeChoices)
            {
                this.AppendValue(",K", pair.Key);
                this.Append(":", pair.Value);
            }
        }

        internal override void Visit(VarRefColumnMap columnMap, int dummy)
        {
        }
    }
}

