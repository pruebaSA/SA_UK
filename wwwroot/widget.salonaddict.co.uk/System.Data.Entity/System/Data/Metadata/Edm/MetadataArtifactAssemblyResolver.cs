namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal abstract class MetadataArtifactAssemblyResolver
    {
        protected MetadataArtifactAssemblyResolver()
        {
        }

        internal abstract IEnumerable<Assembly> GetWildcardAssemblies();
        internal abstract bool TryResolveAssemblyReference(AssemblyName refernceName, out Assembly assembly);
    }
}

