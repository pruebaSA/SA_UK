namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data;
    using System.Data.Mapping;
    using System.Runtime.InteropServices;

    internal sealed class ClrPerspective : Perspective
    {
        private EntityContainer _defaultContainer;

        internal ClrPerspective(MetadataWorkspace metadataWorkspace) : base(metadataWorkspace, DataSpace.CSpace)
        {
        }

        internal override EntityContainer GetDefaultContainer() => 
            this._defaultContainer;

        private static TypeUsage GetMappedTypeUsage(Map map)
        {
            TypeUsage usage = null;
            if (map != null)
            {
                MetadataItem edmItem = map.EdmItem;
                EdmType edmType = edmItem as EdmType;
                if ((edmItem != null) && (edmType != null))
                {
                    usage = TypeUsage.Create(edmType);
                }
            }
            return usage;
        }

        internal void SetDefaultContainer(string defaultContainerName)
        {
            EntityContainer entityContainer = null;
            if (!string.IsNullOrEmpty(defaultContainerName) && !base.MetadataWorkspace.TryGetEntityContainer(defaultContainerName, DataSpace.CSpace, out entityContainer))
            {
                throw EntityUtil.InvalidDefaultContainerName("defaultContainerName", defaultContainerName);
            }
            this._defaultContainer = entityContainer;
        }

        internal override bool TryGetMember(StructuralType type, string memberName, bool ignoreCase, out EdmMember outMember)
        {
            outMember = null;
            Map map = null;
            if (base.MetadataWorkspace.TryGetMap(type, DataSpace.OCSpace, out map))
            {
                ObjectTypeMapping mapping = map as ObjectTypeMapping;
                if (mapping != null)
                {
                    ObjectMemberMapping memberMapForClrMember = mapping.GetMemberMapForClrMember(memberName, ignoreCase);
                    if (memberMapForClrMember != null)
                    {
                        outMember = memberMapForClrMember.EdmMember;
                        return true;
                    }
                }
            }
            return false;
        }

        internal bool TryGetType(Type clrType, out TypeUsage outTypeUsage) => 
            this.TryGetTypeByName(clrType.FullName, false, out outTypeUsage);

        internal override bool TryGetTypeByName(string fullName, bool ignoreCase, out TypeUsage typeUsage)
        {
            typeUsage = null;
            Map map = null;
            if (base.MetadataWorkspace.TryGetMap(fullName, DataSpace.OSpace, ignoreCase, DataSpace.OCSpace, out map))
            {
                if (map.EdmItem.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType)
                {
                    PrimitiveType mappedPrimitiveType = base.MetadataWorkspace.GetMappedPrimitiveType(((PrimitiveType) map.EdmItem).PrimitiveTypeKind, DataSpace.CSpace);
                    if (mappedPrimitiveType != null)
                    {
                        typeUsage = EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(mappedPrimitiveType.PrimitiveTypeKind);
                    }
                }
                else
                {
                    typeUsage = GetMappedTypeUsage(map);
                }
            }
            return (null != typeUsage);
        }
    }
}

