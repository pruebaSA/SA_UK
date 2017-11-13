namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal class CustomAssemblyResolver : MetadataArtifactAssemblyResolver
    {
        private Func<AssemblyName, Assembly> _referenceResolver;
        private Func<IEnumerable<Assembly>> _wildcardAssemblyEnumerator;

        internal CustomAssemblyResolver(Func<IEnumerable<Assembly>> wildcardAssemblyEnumerator, Func<AssemblyName, Assembly> referenceResolver)
        {
            this._wildcardAssemblyEnumerator = wildcardAssemblyEnumerator;
            this._referenceResolver = referenceResolver;
        }

        internal override IEnumerable<Assembly> GetWildcardAssemblies()
        {
            IEnumerable<Assembly> enumerable = this._wildcardAssemblyEnumerator();
            if (enumerable == null)
            {
                throw EntityUtil.InvalidOperation(Strings.WildcardEnumeratorReturnedNull);
            }
            return enumerable;
        }

        internal override bool TryResolveAssemblyReference(AssemblyName refernceName, out Assembly assembly)
        {
            assembly = this._referenceResolver(refernceName);
            return (assembly != null);
        }
    }
}

