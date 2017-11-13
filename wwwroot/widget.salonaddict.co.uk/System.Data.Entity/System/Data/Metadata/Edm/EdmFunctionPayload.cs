namespace System.Data.Metadata.Edm
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct EdmFunctionPayload
    {
        public string Name;
        public string NamespaceName;
        public string Schema;
        public string StoreFunctionName;
        public string CommandText;
        public System.Data.Metadata.Edm.EntitySet EntitySet;
        public bool? IsAggregate;
        public bool? IsBuiltIn;
        public bool? IsNiladic;
        public bool? IsComposable;
        public bool? IsFromProviderManifest;
        public bool? IsCachedStoreFunction;
        public FunctionParameter ReturnParameter;
        public System.Data.Metadata.Edm.ParameterTypeSemantics? ParameterTypeSemantics;
        public FunctionParameter[] Parameters;
        public System.Data.Metadata.Edm.DataSpace DataSpace;
    }
}

