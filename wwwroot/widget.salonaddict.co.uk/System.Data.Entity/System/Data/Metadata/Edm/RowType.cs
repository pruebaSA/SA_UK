namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Objects.ELinq;
    using System.Text;
    using System.Threading;

    public sealed class RowType : StructuralType
    {
        private readonly System.Data.Objects.ELinq.InitializerMetadata _initializerMetadata;
        private ReadOnlyMetadataCollection<EdmProperty> _properties;

        internal RowType(IEnumerable<EdmProperty> properties) : this(properties, null)
        {
        }

        internal RowType(IEnumerable<EdmProperty> properties, System.Data.Objects.ELinq.InitializerMetadata initializerMetadata) : base(GetRowTypeIdentityFromProperties(CheckProperties(properties), initializerMetadata), "Transient", ~DataSpace.OSpace)
        {
            if (properties != null)
            {
                foreach (EdmProperty property in properties)
                {
                    this.AddProperty(property);
                }
            }
            this._initializerMetadata = initializerMetadata;
            this.SetReadOnly();
        }

        private void AddProperty(EdmProperty property)
        {
            EntityUtil.GenericCheckArgumentNull<EdmProperty>(property, "property");
            base.AddMember(property);
        }

        private static IEnumerable<EdmProperty> CheckProperties(IEnumerable<EdmProperty> properties)
        {
            if (properties != null)
            {
                int num = 0;
                using (IEnumerator<EdmProperty> enumerator = properties.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current == null)
                        {
                            throw EntityUtil.CollectionParameterElementIsNull("properties");
                        }
                        num++;
                    }
                }
            }
            return properties;
        }

        internal override bool EdmEquals(MetadataItem item)
        {
            if (!object.ReferenceEquals(this, item))
            {
                if ((item == null) || (System.Data.Metadata.Edm.BuiltInTypeKind.RowType != item.BuiltInTypeKind))
                {
                    return false;
                }
                RowType type = (RowType) item;
                if (base.Members.Count != type.Members.Count)
                {
                    return false;
                }
                for (int i = 0; i < base.Members.Count; i++)
                {
                    EdmMember member = base.Members[i];
                    EdmMember member2 = type.Members[i];
                    if (!member.EdmEquals(member2) || !member.TypeUsage.EdmEquals(member2.TypeUsage))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static string GetRowTypeIdentityFromProperties(IEnumerable<EdmProperty> properties, System.Data.Objects.ELinq.InitializerMetadata initializerMetadata)
        {
            StringBuilder builder = new StringBuilder("rowtype[");
            if (properties != null)
            {
                int num = 0;
                foreach (EdmProperty property in properties)
                {
                    if (num > 0)
                    {
                        builder.Append(",");
                    }
                    builder.Append("(");
                    builder.Append(property.Name);
                    builder.Append(",");
                    property.TypeUsage.BuildIdentity(builder);
                    builder.Append(")");
                    num++;
                }
            }
            builder.Append("]");
            if (initializerMetadata != null)
            {
                builder.Append(",").Append(initializerMetadata.Identity);
            }
            return builder.ToString();
        }

        internal override void ValidateMemberForAdd(EdmMember member)
        {
            if (!Helper.IsEdmProperty(member))
            {
                throw EntityUtil.RowTypeInvalidMembers();
            }
        }

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.RowType;

        internal System.Data.Objects.ELinq.InitializerMetadata InitializerMetadata =>
            this._initializerMetadata;

        public ReadOnlyMetadataCollection<EdmProperty> Properties
        {
            get
            {
                if (this._properties == null)
                {
                    Interlocked.CompareExchange<ReadOnlyMetadataCollection<EdmProperty>>(ref this._properties, new FilteredReadOnlyMetadataCollection<EdmProperty, EdmMember>(base.Members, new Predicate<EdmMember>(Helper.IsEdmProperty)), null);
                }
                return this._properties;
            }
        }
    }
}

