namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data;
    using System.Runtime.InteropServices;

    internal class ModelPerspective : Perspective
    {
        internal ModelPerspective(MetadataWorkspace metadataWorkspace) : base(metadataWorkspace, DataSpace.CSpace)
        {
        }

        internal override bool TryGetTypeByName(string fullName, bool ignoreCase, out TypeUsage typeUsage)
        {
            EntityUtil.CheckStringArgument(fullName, "fullName");
            typeUsage = null;
            EdmType item = null;
            if (base.MetadataWorkspace.TryGetItem<EdmType>(fullName, ignoreCase, base.TargetDataspace, out item))
            {
                if (Helper.IsPrimitiveType(item))
                {
                    typeUsage = base.MetadataWorkspace.GetCanonicalModelTypeUsage(((PrimitiveType) item).PrimitiveTypeKind);
                }
                else
                {
                    typeUsage = TypeUsage.Create(item);
                }
            }
            return (typeUsage != null);
        }
    }
}

