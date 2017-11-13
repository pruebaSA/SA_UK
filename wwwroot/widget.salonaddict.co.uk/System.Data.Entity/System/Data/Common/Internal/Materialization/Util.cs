namespace System.Data.Common.Internal.Materialization
{
    using System;
    using System.Data;
    using System.Data.Mapping;
    using System.Data.Metadata.Edm;

    internal static class Util
    {
        internal static ObjectTypeMapping GetObjectMapping(EdmType type, MetadataWorkspace workspace)
        {
            ItemCollection items;
            EdmType mappedPrimitiveType;
            EdmType type3;
            if (workspace.TryGetItemCollection(DataSpace.CSpace, out items))
            {
                return (ObjectTypeMapping) workspace.GetMap(type, DataSpace.OCSpace);
            }
            if (type.DataSpace == DataSpace.CSpace)
            {
                if (Helper.IsPrimitiveType(type))
                {
                    mappedPrimitiveType = workspace.GetMappedPrimitiveType(((PrimitiveType) type).PrimitiveTypeKind, DataSpace.OSpace);
                }
                else
                {
                    mappedPrimitiveType = workspace.GetItem<EdmType>(type.FullName, DataSpace.OSpace);
                }
                type3 = type;
            }
            else
            {
                mappedPrimitiveType = type;
                type3 = type;
            }
            if ((!Helper.IsPrimitiveType(mappedPrimitiveType) && !Helper.IsEntityType(mappedPrimitiveType)) && !Helper.IsComplexType(mappedPrimitiveType))
            {
                throw EntityUtil.MaterializerUnsupportedType();
            }
            if (Helper.IsPrimitiveType(mappedPrimitiveType))
            {
                return new ObjectTypeMapping(mappedPrimitiveType, type3);
            }
            return DefaultObjectMappingItemCollection.LoadObjectMapping(type3, mappedPrimitiveType, null);
        }
    }
}

