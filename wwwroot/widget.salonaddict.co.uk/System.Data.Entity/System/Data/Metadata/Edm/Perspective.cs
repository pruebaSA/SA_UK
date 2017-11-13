namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Runtime.InteropServices;

    internal abstract class Perspective
    {
        private System.Data.Metadata.Edm.MetadataWorkspace m_metadataWorkspace;
        private DataSpace m_targetDataspace;

        internal Perspective(System.Data.Metadata.Edm.MetadataWorkspace metadataWorkspace, DataSpace targetDataspace)
        {
            EntityUtil.CheckArgumentNull<System.Data.Metadata.Edm.MetadataWorkspace>(metadataWorkspace, "metadataWorkspace");
            this.m_metadataWorkspace = metadataWorkspace;
            this.m_targetDataspace = targetDataspace;
        }

        internal virtual EntityContainer GetDefaultContainer() => 
            null;

        internal ReadOnlyCollection<EdmFunction> GetFunctions(string name, string namespaceName, bool ignoreCase)
        {
            EntityUtil.CheckStringArgument(name, "name");
            EntityUtil.CheckStringArgument(namespaceName, "namespaceName");
            string functionName = namespaceName + "." + name;
            ReadOnlyCollection<EdmFunction> functions = ((EdmItemCollection) this.m_metadataWorkspace.GetItemCollection(DataSpace.CSpace)).GetFunctions(functionName, ignoreCase);
            if ((functions != null) && (functions.Count != 0))
            {
                return functions;
            }
            StoreItemCollection itemCollection = (StoreItemCollection) this.m_metadataWorkspace.GetItemCollection(DataSpace.SSpace);
            return itemCollection.GetCTypeFunctions(functionName, ignoreCase);
        }

        internal virtual bool TryGetEntityContainer(string name, bool ignoreCase, out EntityContainer entityContainer) => 
            this.MetadataWorkspace.TryGetEntityContainer(name, ignoreCase, this.TargetDataspace, out entityContainer);

        internal bool TryGetExtent(EntityContainer entityContainer, string extentName, bool ignoreCase, out EntitySetBase outSet) => 
            entityContainer.BaseEntitySets.TryGetValue(extentName, ignoreCase, out outSet);

        internal virtual bool TryGetMappedPrimitiveType(PrimitiveTypeKind primitiveTypeKind, out PrimitiveType primitiveType)
        {
            primitiveType = this.m_metadataWorkspace.GetMappedPrimitiveType(primitiveTypeKind, DataSpace.CSpace);
            return (null != primitiveType);
        }

        internal virtual bool TryGetMember(StructuralType type, string memberName, bool ignoreCase, out EdmMember outMember)
        {
            EntityUtil.CheckArgumentNull<StructuralType>(type, "type");
            EntityUtil.CheckStringArgument(memberName, "memberName");
            outMember = null;
            return type.Members.TryGetValue(memberName, ignoreCase, out outMember);
        }

        internal abstract bool TryGetTypeByName(string fullName, bool ignoreCase, out TypeUsage typeUsage);

        internal System.Data.Metadata.Edm.MetadataWorkspace MetadataWorkspace =>
            this.m_metadataWorkspace;

        internal DataSpace TargetDataspace =>
            this.m_targetDataspace;
    }
}

