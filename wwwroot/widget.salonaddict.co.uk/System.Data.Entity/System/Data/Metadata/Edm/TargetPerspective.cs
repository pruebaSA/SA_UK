namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data;
    using System.Runtime.InteropServices;

    internal class TargetPerspective : Perspective
    {
        private ModelPerspective _modelPerspective;

        internal TargetPerspective(MetadataWorkspace metadataWorkspace) : base(metadataWorkspace, DataSpace.SSpace)
        {
            this._modelPerspective = new ModelPerspective(metadataWorkspace);
        }

        internal override bool TryGetEntityContainer(string name, bool ignoreCase, out EntityContainer entityContainer)
        {
            if (!base.TryGetEntityContainer(name, ignoreCase, out entityContainer))
            {
                return this._modelPerspective.TryGetEntityContainer(name, ignoreCase, out entityContainer);
            }
            return true;
        }

        internal override bool TryGetTypeByName(string fullName, bool ignoreCase, out TypeUsage usage)
        {
            EntityUtil.CheckStringArgument(fullName, "fullName");
            EdmType item = null;
            if (base.MetadataWorkspace.TryGetItem<EdmType>(fullName, ignoreCase, base.TargetDataspace, out item))
            {
                usage = TypeUsage.Create(item);
                usage = Helper.GetModelTypeUsage(usage);
                return true;
            }
            return this._modelPerspective.TryGetTypeByName(fullName, ignoreCase, out usage);
        }
    }
}

