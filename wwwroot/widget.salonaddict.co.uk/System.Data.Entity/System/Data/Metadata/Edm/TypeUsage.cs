namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;

    [DebuggerDisplay("EdmType={EdmType}, Facets.Count={Facets.Count}")]
    public sealed class TypeUsage : MetadataItem
    {
        private readonly System.Data.Metadata.Edm.EdmType _edmType;
        private ReadOnlyMetadataCollection<Facet> _facets;
        private string _identity;
        private TypeUsage _modelTypeUsage;
        internal static readonly byte? DefaultDateTimePrecisionFacetValue = null;
        internal static readonly bool DefaultFixedLengthFacetValue = false;
        internal static readonly EdmConstants.Unbounded DefaultMaxLengthFacetValue = EdmConstants.UnboundedValue;
        internal static readonly EdmConstants.Unbounded DefaultPrecisionFacetValue = EdmConstants.UnboundedValue;
        internal static readonly EdmConstants.Unbounded DefaultScaleFacetValue = EdmConstants.UnboundedValue;
        internal static readonly bool DefaultUnicodeFacetValue = true;
        private static readonly string[] s_identityFacets = new string[] { "DefaultValue", "FixedLength", "MaxLength", "Nullable", "Precision", "Scale", "Unicode" };

        private TypeUsage(System.Data.Metadata.Edm.EdmType edmType) : base(MetadataItem.MetadataFlags.Readonly)
        {
            EntityUtil.GenericCheckArgumentNull<System.Data.Metadata.Edm.EdmType>(edmType, "edmType");
            this._edmType = edmType;
        }

        private TypeUsage(System.Data.Metadata.Edm.EdmType edmType, IEnumerable<Facet> facets) : this(edmType)
        {
            MetadataCollection<Facet> metadatas = new MetadataCollection<Facet>(facets);
            metadatas.SetReadOnly();
            this._facets = metadatas.AsReadOnlyMetadataCollection();
        }

        internal override void BuildIdentity(StringBuilder builder)
        {
            if (this._identity != null)
            {
                builder.Append(this._identity);
            }
            else
            {
                builder.Append(this.EdmType.Identity);
                builder.Append("(");
                bool flag = true;
                for (int i = 0; i < this.Facets.Count; i++)
                {
                    Facet facet = this.Facets[i];
                    if (0 <= Array.BinarySearch<string>(s_identityFacets, facet.Name, StringComparer.Ordinal))
                    {
                        if (flag)
                        {
                            flag = false;
                        }
                        else
                        {
                            builder.Append(",");
                        }
                        builder.Append(facet.Name);
                        builder.Append("=");
                        builder.Append(facet.Value ?? string.Empty);
                    }
                }
                builder.Append(")");
            }
        }

        internal static TypeUsage Create(System.Data.Metadata.Edm.EdmType edmType) => 
            new TypeUsage(edmType);

        internal static TypeUsage Create(System.Data.Metadata.Edm.EdmType edmType, IEnumerable<Facet> facets) => 
            new TypeUsage(edmType, facets);

        internal static TypeUsage Create(System.Data.Metadata.Edm.EdmType edmType, FacetValues values) => 
            new TypeUsage(edmType, GetDefaultFacetDescriptionsAndOverrideFacetValues(edmType, values));

        public static TypeUsage CreateBinaryTypeUsage(PrimitiveType primitiveType, bool isFixedLength)
        {
            EntityUtil.CheckArgumentNull<PrimitiveType>(primitiveType, "primitiveType");
            if (primitiveType.PrimitiveTypeKind != PrimitiveTypeKind.Binary)
            {
                throw EntityUtil.NotBinaryTypeForTypeUsage();
            }
            FacetValues values = new FacetValues {
                MaxLength = DefaultMaxLengthFacetValue,
                FixedLength = new bool?(isFixedLength)
            };
            return Create(primitiveType, values);
        }

        public static TypeUsage CreateBinaryTypeUsage(PrimitiveType primitiveType, bool isFixedLength, int maxLength)
        {
            EntityUtil.CheckArgumentNull<PrimitiveType>(primitiveType, "primitiveType");
            if (primitiveType.PrimitiveTypeKind != PrimitiveTypeKind.Binary)
            {
                throw EntityUtil.NotBinaryTypeForTypeUsage();
            }
            ValidateMaxLength(maxLength);
            FacetValues values = new FacetValues {
                MaxLength = new int?(maxLength),
                FixedLength = new bool?(isFixedLength)
            };
            return Create(primitiveType, values);
        }

        public static TypeUsage CreateDateTimeOffsetTypeUsage(PrimitiveType primitiveType, byte? precision)
        {
            EntityUtil.CheckArgumentNull<PrimitiveType>(primitiveType, "primitiveType");
            if (primitiveType.PrimitiveTypeKind != PrimitiveTypeKind.DateTimeOffset)
            {
                throw EntityUtil.NotDateTimeOffsetTypeForTypeUsage();
            }
            FacetValues values = new FacetValues {
                Precision = precision
            };
            return Create(primitiveType, values);
        }

        public static TypeUsage CreateDateTimeTypeUsage(PrimitiveType primitiveType, byte? precision)
        {
            EntityUtil.CheckArgumentNull<PrimitiveType>(primitiveType, "primitiveType");
            if (primitiveType.PrimitiveTypeKind != PrimitiveTypeKind.DateTime)
            {
                throw EntityUtil.NotDateTimeTypeForTypeUsage();
            }
            FacetValues values = new FacetValues {
                Precision = precision
            };
            return Create(primitiveType, values);
        }

        public static TypeUsage CreateDecimalTypeUsage(PrimitiveType primitiveType)
        {
            EntityUtil.CheckArgumentNull<PrimitiveType>(primitiveType, "primitiveType");
            if (primitiveType.PrimitiveTypeKind != PrimitiveTypeKind.Decimal)
            {
                throw EntityUtil.NotDecimalTypeForTypeUsage();
            }
            FacetValues values = new FacetValues {
                Precision = DefaultPrecisionFacetValue,
                Scale = DefaultScaleFacetValue
            };
            return Create(primitiveType, values);
        }

        public static TypeUsage CreateDecimalTypeUsage(PrimitiveType primitiveType, byte precision, byte scale)
        {
            EntityUtil.CheckArgumentNull<PrimitiveType>(primitiveType, "primitiveType");
            if (primitiveType.PrimitiveTypeKind != PrimitiveTypeKind.Decimal)
            {
                throw EntityUtil.NotDecimalTypeForTypeUsage();
            }
            FacetValues values = new FacetValues {
                Precision = new byte?(precision),
                Scale = new byte?(scale)
            };
            return Create(primitiveType, values);
        }

        public static TypeUsage CreateDefaultTypeUsage(System.Data.Metadata.Edm.EdmType edmType)
        {
            EntityUtil.CheckArgumentNull<System.Data.Metadata.Edm.EdmType>(edmType, "edmType");
            return Create(edmType);
        }

        public static TypeUsage CreateStringTypeUsage(PrimitiveType primitiveType, bool isUnicode, bool isFixedLength)
        {
            EntityUtil.CheckArgumentNull<PrimitiveType>(primitiveType, "primitiveType");
            if (primitiveType.PrimitiveTypeKind != PrimitiveTypeKind.String)
            {
                throw EntityUtil.NotStringTypeForTypeUsage();
            }
            FacetValues values = new FacetValues {
                MaxLength = DefaultMaxLengthFacetValue,
                Unicode = new bool?(isUnicode),
                FixedLength = new bool?(isFixedLength)
            };
            return Create(primitiveType, values);
        }

        public static TypeUsage CreateStringTypeUsage(PrimitiveType primitiveType, bool isUnicode, bool isFixedLength, int maxLength)
        {
            EntityUtil.CheckArgumentNull<PrimitiveType>(primitiveType, "primitiveType");
            if (primitiveType.PrimitiveTypeKind != PrimitiveTypeKind.String)
            {
                throw EntityUtil.NotStringTypeForTypeUsage();
            }
            ValidateMaxLength(maxLength);
            FacetValues values = new FacetValues {
                MaxLength = new int?(maxLength),
                Unicode = new bool?(isUnicode),
                FixedLength = new bool?(isFixedLength)
            };
            return Create(primitiveType, values);
        }

        public static TypeUsage CreateTimeTypeUsage(PrimitiveType primitiveType, byte? precision)
        {
            EntityUtil.CheckArgumentNull<PrimitiveType>(primitiveType, "primitiveType");
            if (primitiveType.PrimitiveTypeKind != PrimitiveTypeKind.Time)
            {
                throw EntityUtil.NotTimeTypeForTypeUsage();
            }
            FacetValues values = new FacetValues {
                Precision = precision
            };
            return Create(primitiveType, values);
        }

        internal override bool EdmEquals(MetadataItem item)
        {
            if (!object.ReferenceEquals(this, item))
            {
                if ((item == null) || (System.Data.Metadata.Edm.BuiltInTypeKind.TypeUsage != item.BuiltInTypeKind))
                {
                    return false;
                }
                TypeUsage usage = (TypeUsage) item;
                if (!this.EdmType.EdmEquals(usage.EdmType))
                {
                    return false;
                }
                if ((this._facets != null) || (usage._facets != null))
                {
                    if (this.Facets.Count != usage.Facets.Count)
                    {
                        return false;
                    }
                    foreach (Facet facet in this.Facets)
                    {
                        Facet facet2;
                        if (!usage.Facets.TryGetValue(facet.Name, false, out facet2))
                        {
                            return false;
                        }
                        if (!object.Equals(facet.Value, facet2.Value))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private static IEnumerable<Facet> GetDefaultFacetDescriptionsAndOverrideFacetValues(System.Data.Metadata.Edm.EdmType type, FacetValues values) => 
            OverrideFacetValues<FacetDescription>(type.GetAssociatedFacetDescriptions(), fd => fd, fd => fd.DefaultValueFacet, values);

        private IEnumerable<Facet> GetFacets()
        {
            foreach (FacetDescription iteratorVariable0 in this._edmType.GetAssociatedFacetDescriptions())
            {
                yield return iteratorVariable0.DefaultValueFacet;
            }
        }

        internal TypeUsage GetModelTypeUsage()
        {
            if (this._modelTypeUsage == null)
            {
                TypeUsage usage;
                System.Data.Metadata.Edm.EdmType edmType = this.EdmType;
                if ((edmType.DataSpace == DataSpace.CSpace) || (edmType.DataSpace == DataSpace.OSpace))
                {
                    return this;
                }
                if (Helper.IsRowType(edmType))
                {
                    RowType type2 = (RowType) edmType;
                    EdmProperty[] properties = new EdmProperty[type2.Properties.Count];
                    for (int i = 0; i < properties.Length; i++)
                    {
                        EdmProperty property = type2.Properties[i];
                        TypeUsage modelTypeUsage = property.TypeUsage.GetModelTypeUsage();
                        properties[i] = new EdmProperty(property.Name, modelTypeUsage);
                    }
                    RowType type3 = new RowType(properties, type2.InitializerMetadata);
                    usage = Create(type3, this.Facets);
                }
                else if (Helper.IsCollectionType(edmType))
                {
                    CollectionType type4 = (CollectionType) edmType;
                    usage = Create(new CollectionType(type4.TypeUsage.GetModelTypeUsage()), this.Facets);
                }
                else if (Helper.IsRefType(edmType))
                {
                    usage = this;
                }
                else if (Helper.IsPrimitiveType(edmType))
                {
                    usage = ((PrimitiveType) edmType).ProviderManifest.GetEdmType(this);
                    if (usage == null)
                    {
                        throw EntityUtil.ProviderIncompatible(Strings.Mapping_ProviderReturnsNullType(this.ToString()));
                    }
                    if (!TypeSemantics.IsNullable(this))
                    {
                        FacetValues values = new FacetValues {
                            Nullable = 0
                        };
                        usage = Create(usage.EdmType, OverrideFacetValues(usage.Facets, values));
                    }
                }
                else
                {
                    if (!Helper.IsEntityTypeBase(edmType) && !Helper.IsComplexType(edmType))
                    {
                        return null;
                    }
                    usage = this;
                }
                Interlocked.CompareExchange<TypeUsage>(ref this._modelTypeUsage, usage, null);
            }
            return this._modelTypeUsage;
        }

        public bool IsSubtypeOf(TypeUsage typeUsage) => 
            (((this.EdmType != null) && (typeUsage != null)) && this.EdmType.IsSubtypeOf(typeUsage.EdmType));

        private static IEnumerable<Facet> OverrideFacetValues(IEnumerable<Facet> facets, FacetValues values) => 
            OverrideFacetValues<Facet>(facets, f => f.Description, f => f, values);

        private static IEnumerable<Facet> OverrideFacetValues<T>(IEnumerable<T> facetThings, Func<T, FacetDescription> getDescription, Func<T, Facet> getFacet, FacetValues values)
        {
            foreach (T iteratorVariable0 in facetThings)
            {
                Facet iteratorVariable2;
                FacetDescription description = getDescription(iteratorVariable0);
                if (!description.IsConstant && values.TryGetFacet(description, out iteratorVariable2))
                {
                    yield return iteratorVariable2;
                }
                else
                {
                    yield return getFacet(iteratorVariable0);
                }
            }
        }

        internal override void SetReadOnly()
        {
            base.SetReadOnly();
        }

        internal TypeUsage ShallowCopy(FacetValues facetValues) => 
            Create(this._edmType, OverrideFacetValues(this.Facets, facetValues));

        public override string ToString() => 
            this.EdmType.ToString();

        private static void ValidateMaxLength(int maxLength)
        {
            if (maxLength <= 0)
            {
                throw EntityUtil.ArgumentOutOfRange(Strings.InvalidMaxLengthSize, "maxLength");
            }
        }

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.TypeUsage;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.EdmType, false)]
        public System.Data.Metadata.Edm.EdmType EdmType =>
            this._edmType;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.Facet, true)]
        public ReadOnlyMetadataCollection<Facet> Facets
        {
            get
            {
                if (this._facets == null)
                {
                    MetadataCollection<Facet> metadatas = new MetadataCollection<Facet>(this.GetFacets());
                    metadatas.SetReadOnly();
                    Interlocked.CompareExchange<ReadOnlyMetadataCollection<Facet>>(ref this._facets, metadatas.AsReadOnlyMetadataCollection(), null);
                }
                return this._facets;
            }
        }

        internal override string Identity
        {
            get
            {
                if (this.Facets.Count == 0)
                {
                    return this.EdmType.Identity;
                }
                if (this._identity == null)
                {
                    StringBuilder builder = new StringBuilder(0x80);
                    this.BuildIdentity(builder);
                    string str = builder.ToString();
                    Interlocked.CompareExchange<string>(ref this._identity, str, null);
                }
                return this._identity;
            }
        }


        [CompilerGenerated]
        private sealed class <OverrideFacetValues>d__18<T> : IEnumerable<Facet>, IEnumerable, IEnumerator<Facet>, IEnumerator, IDisposable
        {
            private int <>1__state;
            private Facet <>2__current;
            public IEnumerable<T> <>3__facetThings;
            public Func<T, FacetDescription> <>3__getDescription;
            public Func<T, Facet> <>3__getFacet;
            public FacetValues <>3__values;
            public IEnumerator<T> <>7__wrap1c;
            private int <>l__initialThreadId;
            public FacetDescription <description>5__1a;
            public Facet <facet>5__1b;
            public T <thing>5__19;
            public IEnumerable<T> facetThings;
            public Func<T, FacetDescription> getDescription;
            public Func<T, Facet> getFacet;
            public FacetValues values;

            [DebuggerHidden]
            public <OverrideFacetValues>d__18(int <>1__state)
            {
                this.<>1__state = <>1__state;
                this.<>l__initialThreadId = Thread.CurrentThread.ManagedThreadId;
            }

            private void <>m__Finally1d()
            {
                this.<>1__state = -1;
                if (this.<>7__wrap1c != null)
                {
                    this.<>7__wrap1c.Dispose();
                }
            }

            private bool MoveNext()
            {
                bool flag;
                try
                {
                    switch (this.<>1__state)
                    {
                        case 0:
                            this.<>1__state = -1;
                            this.<>7__wrap1c = this.facetThings.GetEnumerator();
                            this.<>1__state = 1;
                            goto Label_00DD;

                        case 2:
                            this.<>1__state = 1;
                            goto Label_00DD;

                        case 3:
                            this.<>1__state = 1;
                            goto Label_00DD;

                        default:
                            goto Label_00F3;
                    }
                Label_0046:
                    this.<thing>5__19 = this.<>7__wrap1c.Current;
                    this.<description>5__1a = this.getDescription(this.<thing>5__19);
                    if (!this.<description>5__1a.IsConstant && this.values.TryGetFacet(this.<description>5__1a, out this.<facet>5__1b))
                    {
                        this.<>2__current = this.<facet>5__1b;
                        this.<>1__state = 2;
                        return true;
                    }
                    this.<>2__current = this.getFacet(this.<thing>5__19);
                    this.<>1__state = 3;
                    return true;
                Label_00DD:
                    if (this.<>7__wrap1c.MoveNext())
                    {
                        goto Label_0046;
                    }
                    this.<>m__Finally1d();
                Label_00F3:
                    flag = false;
                }
                fault
                {
                    this.System.IDisposable.Dispose();
                }
                return flag;
            }

            [DebuggerHidden]
            IEnumerator<Facet> IEnumerable<Facet>.GetEnumerator()
            {
                TypeUsage.<OverrideFacetValues>d__18<T> d__;
                if ((Thread.CurrentThread.ManagedThreadId == this.<>l__initialThreadId) && (this.<>1__state == -2))
                {
                    this.<>1__state = 0;
                    d__ = (TypeUsage.<OverrideFacetValues>d__18<T>) this;
                }
                else
                {
                    d__ = new TypeUsage.<OverrideFacetValues>d__18<T>(0);
                }
                d__.facetThings = this.<>3__facetThings;
                d__.getDescription = this.<>3__getDescription;
                d__.getFacet = this.<>3__getFacet;
                d__.values = this.<>3__values;
                return d__;
            }

            [DebuggerHidden]
            IEnumerator IEnumerable.GetEnumerator() => 
                this.System.Collections.Generic.IEnumerable<System.Data.Metadata.Edm.Facet>.GetEnumerator();

            [DebuggerHidden]
            void IEnumerator.Reset()
            {
                throw new NotSupportedException();
            }

            void IDisposable.Dispose()
            {
                switch (this.<>1__state)
                {
                    case 1:
                    case 2:
                    case 3:
                        try
                        {
                        }
                        finally
                        {
                            this.<>m__Finally1d();
                        }
                        return;
                }
            }

            Facet IEnumerator<Facet>.Current =>
                this.<>2__current;

            object IEnumerator.Current =>
                this.<>2__current;
        }
    }
}

