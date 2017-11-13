namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;

    public abstract class MetadataItem
    {
        private static EdmType[] _builtInTypes = new EdmType[40];
        private static FacetDescription _collectionKindFacetDescription;
        private static FacetDescription _defaultValueFacetDescription;
        private System.Data.Metadata.Edm.Documentation _documentation;
        private MetadataFlags _flags;
        private object _flagsLock;
        private static readonly ReadOnlyCollection<FacetDescription> _generalFacetDescriptions;
        private MetadataCollection<MetadataProperty> _itemAttributes;
        private static FacetDescription _nullableFacetDescription;
        private static TypeUsage _nullTypeUsage;

        static MetadataItem()
        {
            _builtInTypes[0] = new ComplexType();
            _builtInTypes[2] = new ComplexType();
            _builtInTypes[1] = new ComplexType();
            _builtInTypes[3] = new ComplexType();
            _builtInTypes[3] = new ComplexType();
            _builtInTypes[7] = new EnumType();
            _builtInTypes[6] = new ComplexType();
            _builtInTypes[8] = new ComplexType();
            _builtInTypes[9] = new ComplexType();
            _builtInTypes[10] = new EnumType();
            _builtInTypes[11] = new ComplexType();
            _builtInTypes[12] = new ComplexType();
            _builtInTypes[13] = new ComplexType();
            _builtInTypes[14] = new ComplexType();
            _builtInTypes[4] = new ComplexType();
            _builtInTypes[5] = new ComplexType();
            _builtInTypes[15] = new ComplexType();
            _builtInTypes[0x10] = new ComplexType();
            _builtInTypes[0x11] = new ComplexType();
            _builtInTypes[0x12] = new ComplexType();
            _builtInTypes[0x13] = new ComplexType();
            _builtInTypes[20] = new ComplexType();
            _builtInTypes[0x15] = new ComplexType();
            _builtInTypes[0x16] = new ComplexType();
            _builtInTypes[0x17] = new ComplexType();
            _builtInTypes[0x18] = new ComplexType();
            _builtInTypes[0x19] = new EnumType();
            _builtInTypes[0x1a] = new ComplexType();
            _builtInTypes[0x1b] = new EnumType();
            _builtInTypes[0x1c] = new ComplexType();
            _builtInTypes[0x1d] = new ComplexType();
            _builtInTypes[30] = new ComplexType();
            _builtInTypes[0x1f] = new ComplexType();
            _builtInTypes[0x20] = new ComplexType();
            _builtInTypes[0x21] = new EnumType();
            _builtInTypes[0x22] = new ComplexType();
            _builtInTypes[0x23] = new ComplexType();
            _builtInTypes[0x24] = new ComplexType();
            _builtInTypes[0x25] = new ComplexType();
            _builtInTypes[0x26] = new ComplexType();
            _builtInTypes[0x27] = new ComplexType();
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.MetadataItem), "ItemType", false, null);
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.MetadataProperty), "MetadataProperty", true, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.MetadataItem));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.GlobalItem), "GlobalItem", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.MetadataItem));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.TypeUsage), "TypeUsage", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.MetadataItem));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmType), "EdmType", true, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.GlobalItem));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.SimpleType), "SimpleType", true, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmType));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EnumType), "EnumType", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.SimpleType));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.PrimitiveType), "PrimitiveType", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.SimpleType));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.CollectionType), "CollectionType", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmType));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.RefType), "RefType", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmType));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmMember), "EdmMember", true, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.MetadataItem));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmProperty), "EdmProperty", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmMember));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.NavigationProperty), "NavigationProperty", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmMember));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.ProviderManifest), "ProviderManifest", true, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.MetadataItem));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.RelationshipEndMember), "RelationshipEnd", true, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmMember));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.AssociationEndMember), "AssociationEnd", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.RelationshipEndMember));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EnumMember), "EnumMember", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.MetadataItem));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.ReferentialConstraint), "ReferentialConstraint", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.MetadataItem));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.StructuralType), "StructuralType", true, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmType));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.RowType), "RowType", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.StructuralType));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.ComplexType), "ComplexType", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.StructuralType));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EntityTypeBase), "ElementType", true, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.StructuralType));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EntityType), "EntityType", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EntityTypeBase));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.RelationshipType), "RelationshipType", true, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EntityTypeBase));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.AssociationType), "AssociationType", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.RelationshipType));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.Facet), "Facet", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.MetadataItem));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EntityContainer), "EntityContainerType", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.GlobalItem));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EntitySetBase), "BaseEntitySetType", true, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.MetadataItem));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EntitySet), "EntitySetType", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EntitySetBase));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.RelationshipSet), "RelationshipSet", true, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EntitySetBase));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.AssociationSet), "AssocationSetType", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.RelationshipSet));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.AssociationSetEnd), "AssociationSetEndType", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.MetadataItem));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.FunctionParameter), "FunctionParameter", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.MetadataItem));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmFunction), "EdmFunction", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmType));
            InitializeBuiltInTypes((ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.Documentation), "Documentation", false, (ComplexType) GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.MetadataItem));
            InitializeEnumType(System.Data.Metadata.Edm.BuiltInTypeKind.OperationAction, "DeleteAction", new string[] { "None", "Cascade", "Restrict" });
            InitializeEnumType(System.Data.Metadata.Edm.BuiltInTypeKind.RelationshipMultiplicity, "RelationshipMultiplicity", new string[] { "One", "ZeroToOne", "Many" });
            InitializeEnumType(System.Data.Metadata.Edm.BuiltInTypeKind.ParameterMode, "ParameterMode", new string[] { "In", "Out", "InOut" });
            InitializeEnumType(System.Data.Metadata.Edm.BuiltInTypeKind.CollectionKind, "CollectionKind", new string[] { "None", "List", "Bag" });
            InitializeEnumType(System.Data.Metadata.Edm.BuiltInTypeKind.PrimitiveTypeKind, "PrimitiveTypeKind", Enum.GetNames(typeof(PrimitiveTypeKind)));
            FacetDescription[] array = new FacetDescription[2];
            _nullableFacetDescription = new FacetDescription("Nullable", EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Boolean), null, null, true);
            array[0] = _nullableFacetDescription;
            _defaultValueFacetDescription = new FacetDescription("DefaultValue", GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmType), null, null, null);
            array[1] = _defaultValueFacetDescription;
            _generalFacetDescriptions = Array.AsReadOnly<FacetDescription>(array);
            _collectionKindFacetDescription = new FacetDescription("CollectionKind", GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EnumType), null, null, null);
            TypeUsage typeUsage = TypeUsage.Create(EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.String));
            TypeUsage usage2 = TypeUsage.Create(EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.Boolean));
            TypeUsage usage3 = TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmType));
            TypeUsage usage4 = TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.TypeUsage));
            TypeUsage usage5 = TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.ComplexType));
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.MetadataProperty, new EdmProperty[] { new EdmProperty("Name", typeUsage), new EdmProperty("TypeUsage", usage4), new EdmProperty("Value", usage5) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.MetadataItem, new EdmProperty[] { new EdmProperty("MetadataProperties", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.MetadataProperty).GetCollectionType())), new EdmProperty("Documentation", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.Documentation))) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.TypeUsage, new EdmProperty[] { new EdmProperty("EdmType", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmType))), new EdmProperty("Facets", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.Facet))) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.EdmType, new EdmProperty[] { new EdmProperty("Name", typeUsage), new EdmProperty("Namespace", typeUsage), new EdmProperty("Abstract", usage2), new EdmProperty("Sealed", usage2), new EdmProperty("BaseType", usage5) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.EnumType, new EdmProperty[] { new EdmProperty("EnumMembers", typeUsage) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.CollectionType, new EdmProperty[] { new EdmProperty("TypeUsage", usage4) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.RefType, new EdmProperty[] { new EdmProperty("EntityType", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EntityType))) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.EdmMember, new EdmProperty[] { new EdmProperty("Name", typeUsage), new EdmProperty("TypeUsage", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.TypeUsage))) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.EdmProperty, new EdmProperty[] { new EdmProperty("Nullable", typeUsage), new EdmProperty("DefaultValue", usage5) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.NavigationProperty, new EdmProperty[] { new EdmProperty("RelationshipTypeName", typeUsage), new EdmProperty("ToEndMemberName", typeUsage) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.RelationshipEndMember, new EdmProperty[] { new EdmProperty("OperationBehaviors", usage5), new EdmProperty("RelationshipMultiplicity", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EnumType))) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.EnumMember, new EdmProperty[] { new EdmProperty("Name", typeUsage) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.ReferentialConstraint, new EdmProperty[] { new EdmProperty("ToRole", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.RelationshipEndMember))), new EdmProperty("FromRole", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.RelationshipEndMember))), new EdmProperty("ToProperties", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmProperty).GetCollectionType())), new EdmProperty("FromProperties", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmProperty).GetCollectionType())) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.StructuralType, new EdmProperty[] { new EdmProperty("Members", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmMember))) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.EntityTypeBase, new EdmProperty[] { new EdmProperty("KeyMembers", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmMember))) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.Facet, new EdmProperty[] { new EdmProperty("Name", typeUsage), new EdmProperty("EdmType", usage3), new EdmProperty("Value", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EdmType))) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.EntityContainer, new EdmProperty[] { new EdmProperty("Name", typeUsage), new EdmProperty("EntitySets", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EntitySet))) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.EntitySetBase, new EdmProperty[] { new EdmProperty("Name", typeUsage), new EdmProperty("EntityType", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EntityType))), new EdmProperty("Schema", typeUsage), new EdmProperty("Table", typeUsage) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.AssociationSet, new EdmProperty[] { new EdmProperty("AssociationSetEnds", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.AssociationSetEnd).GetCollectionType())) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.AssociationSetEnd, new EdmProperty[] { new EdmProperty("Role", typeUsage), new EdmProperty("EntitySetType", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EntitySet))) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.FunctionParameter, new EdmProperty[] { new EdmProperty("Name", typeUsage), new EdmProperty("Mode", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.EnumType))), new EdmProperty("TypeUsage", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.TypeUsage))) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.EdmFunction, new EdmProperty[] { new EdmProperty("Name", typeUsage), new EdmProperty("Namespace", typeUsage), new EdmProperty("ReturnParameter", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.FunctionParameter))), new EdmProperty("Parameters", TypeUsage.Create(GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind.FunctionParameter).GetCollectionType())) });
            AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind.Documentation, new EdmProperty[] { new EdmProperty("Summary", typeUsage), new EdmProperty("LongDescription", typeUsage) });
            for (int i = 0; i < _builtInTypes.Length; i++)
            {
                _builtInTypes[i].SetReadOnly();
            }
            InitializeNullTypeUsage();
        }

        internal MetadataItem()
        {
            this._flagsLock = new object();
        }

        internal MetadataItem(MetadataFlags flags)
        {
            this._flagsLock = new object();
            this._flags = flags;
        }

        private static void AddBuiltInTypeProperties(System.Data.Metadata.Edm.BuiltInTypeKind builtInTypeKind, EdmProperty[] properties)
        {
            ComplexType builtInType = (ComplexType) GetBuiltInType(builtInTypeKind);
            if (properties != null)
            {
                for (int i = 0; i < properties.Length; i++)
                {
                    builtInType.AddMember(properties[i]);
                }
            }
        }

        internal void AddMetadataProperties(List<MetadataProperty> metadataProperties)
        {
            this.MetadataProperties.Source.AtomicAddRange(metadataProperties);
        }

        internal virtual void BuildIdentity(StringBuilder builder)
        {
            builder.Append(this.Identity);
        }

        private static MetadataFlags Convert(DataSpace space)
        {
            switch (space)
            {
                case DataSpace.OSpace:
                    return MetadataFlags.OSpace;

                case DataSpace.CSpace:
                    return MetadataFlags.CSpace;

                case DataSpace.SSpace:
                    return MetadataFlags.SSpace;

                case DataSpace.OCSpace:
                    return MetadataFlags.OCSpace;

                case DataSpace.CSSpace:
                    return MetadataFlags.CSSpace;
            }
            return MetadataFlags.None;
        }

        private static MetadataFlags Convert(ParameterMode mode)
        {
            switch (mode)
            {
                case ParameterMode.In:
                    return MetadataFlags.In;

                case ParameterMode.Out:
                    return MetadataFlags.Out;

                case ParameterMode.InOut:
                    return MetadataFlags.InOut;

                case ParameterMode.ReturnValue:
                    return MetadataFlags.ReturnValue;
            }
            return MetadataFlags.ParameterMode;
        }

        internal virtual bool EdmEquals(MetadataItem item)
        {
            if (item == null)
            {
                return false;
            }
            return ((this == item) || ((this.BuiltInTypeKind == item.BuiltInTypeKind) && (this.Identity == item.Identity)));
        }

        public static EdmType GetBuiltInType(System.Data.Metadata.Edm.BuiltInTypeKind builtInTypeKind) => 
            _builtInTypes[(int) builtInTypeKind];

        internal DataSpace GetDataSpace()
        {
            switch ((this._flags & MetadataFlags.DataSpace))
            {
                case MetadataFlags.CSpace:
                    return DataSpace.CSpace;

                case MetadataFlags.OSpace:
                    return DataSpace.OSpace;

                case MetadataFlags.OCSpace:
                    return DataSpace.OCSpace;

                case MetadataFlags.SSpace:
                    return DataSpace.SSpace;

                case MetadataFlags.CSSpace:
                    return DataSpace.CSSpace;
            }
            return ~DataSpace.OSpace;
        }

        internal bool GetFlag(MetadataFlags flag) => 
            (flag == (this._flags & flag));

        public static ReadOnlyCollection<FacetDescription> GetGeneralFacetDescriptions() => 
            _generalFacetDescriptions;

        internal ParameterMode GetParameterMode()
        {
            switch ((this._flags & MetadataFlags.ParameterMode))
            {
                case MetadataFlags.In:
                    return ParameterMode.In;

                case MetadataFlags.Out:
                    return ParameterMode.Out;

                case MetadataFlags.InOut:
                    return ParameterMode.InOut;

                case MetadataFlags.ReturnValue:
                    return ParameterMode.ReturnValue;
            }
            return ~ParameterMode.In;
        }

        private static void InitializeBuiltInTypes(ComplexType builtInType, string name, bool isAbstract, ComplexType baseType)
        {
            EdmType.Initialize(builtInType, name, "Edm", DataSpace.CSpace, isAbstract, baseType);
        }

        private static void InitializeEnumType(System.Data.Metadata.Edm.BuiltInTypeKind builtInTypeKind, string name, string[] enumMemberNames)
        {
            EnumType builtInType = (EnumType) GetBuiltInType(builtInTypeKind);
            EdmType.Initialize(builtInType, name, "Edm", DataSpace.CSpace, false, null);
            for (int i = 0; i < enumMemberNames.Length; i++)
            {
                builtInType.AddMember(new EnumMember(enumMemberNames[i]));
            }
        }

        private static void InitializeNullTypeUsage()
        {
            ComplexType edmType = new ComplexType("NullType", string.Empty, DataSpace.CSpace) {
                Abstract = true
            };
            _nullTypeUsage = TypeUsage.Create(edmType);
        }

        internal void SetDataSpace(DataSpace space)
        {
            this._flags = (this._flags & ~MetadataFlags.DataSpace) | (MetadataFlags.DataSpace & Convert(space));
        }

        internal void SetFlag(MetadataFlags flag, bool value)
        {
            MetadataFlags flags1 = flag & MetadataFlags.Readonly;
            lock (this._flagsLock)
            {
                if (!this.IsReadOnly || ((flag & MetadataFlags.Readonly) != MetadataFlags.Readonly))
                {
                    Util.ThrowIfReadOnly(this);
                    if (value)
                    {
                        this._flags |= flag;
                    }
                    else
                    {
                        this._flags &= ~flag;
                    }
                }
            }
        }

        internal void SetParameterMode(ParameterMode mode)
        {
            this._flags = (this._flags & ~MetadataFlags.ParameterMode) | (MetadataFlags.ParameterMode & Convert(mode));
        }

        internal virtual void SetReadOnly()
        {
            if (!this.IsReadOnly)
            {
                if (this._itemAttributes != null)
                {
                    this._itemAttributes.SetReadOnly();
                }
                this.SetFlag(MetadataFlags.Readonly, true);
            }
        }

        public abstract System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind { get; }

        internal static FacetDescription CollectionKindFacetDescription =>
            _collectionKindFacetDescription;

        internal static FacetDescription DefaultValueFacetDescription =>
            _defaultValueFacetDescription;

        public System.Data.Metadata.Edm.Documentation Documentation
        {
            get => 
                this._documentation;
            set
            {
                this._documentation = value;
            }
        }

        internal static System.Data.Metadata.Edm.EdmProviderManifest EdmProviderManifest =>
            System.Data.Metadata.Edm.EdmProviderManifest.Instance;

        internal abstract string Identity { get; }

        internal bool IsReadOnly =>
            this.GetFlag(MetadataFlags.Readonly);

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.MetadataProperty, true)]
        public ReadOnlyMetadataCollection<MetadataProperty> MetadataProperties =>
            this._itemAttributes?.AsReadOnlyMetadataCollection();

        internal static FacetDescription NullableFacetDescription =>
            _nullableFacetDescription;

        internal static TypeUsage NullType =>
            _nullTypeUsage;

        internal MetadataCollection<MetadataProperty> RawMetadataProperties =>
            this._itemAttributes;

        [Flags]
        internal enum MetadataFlags
        {
            CSpace = 1,
            CSSpace = 5,
            DataSpace = 7,
            In = 0x200,
            InOut = 0x600,
            IsAbstract = 0x10,
            None = 0,
            OCSpace = 3,
            OSpace = 2,
            Out = 0x400,
            ParameterMode = 0xe00,
            Readonly = 8,
            ReturnValue = 0x800,
            SSpace = 4
        }
    }
}

