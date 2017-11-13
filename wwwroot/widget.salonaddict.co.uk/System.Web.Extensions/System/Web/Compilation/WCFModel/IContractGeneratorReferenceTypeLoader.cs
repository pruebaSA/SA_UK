namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.InteropServices;

    internal interface IContractGeneratorReferenceTypeLoader
    {
        void LoadAllAssemblies(out IEnumerable<Assembly> loadedAssemblies, out IEnumerable<Exception> loadingErrors);
        Assembly LoadAssembly(string assemblyName);
        Type LoadType(string typeName);
    }
}

