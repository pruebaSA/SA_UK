namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Threading;

    public abstract class EdmType : GlobalItem
    {
        private EdmType _baseType;
        private CollectionType _collectionType;
        private string _identity;
        private string _name;
        private string _namespace;

        internal EdmType()
        {
        }

        internal EdmType(string name, string namespaceName, DataSpace dataSpace)
        {
            EntityUtil.GenericCheckArgumentNull<string>(name, "name");
            EntityUtil.GenericCheckArgumentNull<string>(namespaceName, "namespaceName");
            Initialize(this, name, namespaceName, dataSpace, false, null);
        }

        internal override void BuildIdentity(StringBuilder builder)
        {
            if (this.CacheIdentity != null)
            {
                builder.Append(this.CacheIdentity);
            }
            else
            {
                builder.Append(CreateEdmTypeIdentity(this.NamespaceName, this.Name));
            }
        }

        internal static string CreateEdmTypeIdentity(string namespaceName, string name)
        {
            string str = string.Empty;
            if (string.Empty != namespaceName)
            {
                str = namespaceName + ".";
            }
            return (str + name);
        }

        internal virtual IEnumerable<FacetDescription> GetAssociatedFacetDescriptions() => 
            MetadataItem.GetGeneralFacetDescriptions();

        public CollectionType GetCollectionType()
        {
            if (this._collectionType == null)
            {
                Interlocked.CompareExchange<CollectionType>(ref this._collectionType, new CollectionType(this), null);
            }
            return this._collectionType;
        }

        internal static void Initialize(EdmType edmType, string name, string namespaceName, DataSpace dataSpace, bool isAbstract, EdmType baseType)
        {
            edmType._baseType = baseType;
            edmType._name = name;
            edmType._namespace = namespaceName;
            edmType.DataSpace = dataSpace;
            edmType.Abstract = isAbstract;
        }

        internal virtual bool IsAssignableFrom(EdmType otherType) => 
            Helper.IsAssignableFrom(this, otherType);

        internal virtual bool IsBaseTypeOf(EdmType otherType) => 
            otherType?.IsSubtypeOf(this);

        internal virtual bool IsSubtypeOf(EdmType otherType) => 
            Helper.IsSubtypeOf(this, otherType);

        internal override void SetReadOnly()
        {
            if (!base.IsReadOnly)
            {
                base.SetReadOnly();
                EdmType baseType = this.BaseType;
                if (baseType != null)
                {
                    baseType.SetReadOnly();
                }
            }
        }

        public override string ToString() => 
            this.FullName;

        [MetadataProperty(PrimitiveTypeKind.Boolean, false)]
        public bool Abstract
        {
            get => 
                base.GetFlag(MetadataItem.MetadataFlags.IsAbstract);
            internal set
            {
                base.SetFlag(MetadataItem.MetadataFlags.IsAbstract, value);
            }
        }

        [MetadataProperty(BuiltInTypeKind.EdmType, false)]
        public EdmType BaseType
        {
            get => 
                this._baseType;
            internal set
            {
                Util.ThrowIfReadOnly(this);
                for (EdmType type = value; type != null; type = type.BaseType)
                {
                    if (type == this)
                    {
                        throw EntityUtil.InvalidBaseTypeLoop("value");
                    }
                }
                this._baseType = value;
            }
        }

        internal string CacheIdentity
        {
            get => 
                this._identity;
            private set
            {
                this._identity = value;
            }
        }

        internal virtual Type ClrType =>
            null;

        public virtual string FullName =>
            this.Identity;

        internal override string Identity
        {
            get
            {
                if (this.CacheIdentity == null)
                {
                    StringBuilder builder = new StringBuilder(50);
                    this.BuildIdentity(builder);
                    this.CacheIdentity = builder.ToString();
                }
                return this.CacheIdentity;
            }
        }

        [MetadataProperty(PrimitiveTypeKind.String, false)]
        public string Name
        {
            get => 
                this._name;
            internal set
            {
                this._name = value;
            }
        }

        [MetadataProperty(PrimitiveTypeKind.String, false)]
        public string NamespaceName
        {
            get => 
                this._namespace;
            internal set
            {
                this._namespace = value;
            }
        }
    }
}

