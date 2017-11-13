namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data;
    using System.Runtime.CompilerServices;

    public sealed class FunctionParameter : MetadataItem
    {
        private readonly SafeLink<EdmFunction> _declaringFunction = new SafeLink<EdmFunction>();
        private readonly string _name;
        private readonly System.Data.Metadata.Edm.TypeUsage _typeUsage;
        internal static Func<FunctionParameter, SafeLink<EdmFunction>> DeclaringFunctionLinker = fp => fp._declaringFunction;

        internal FunctionParameter(string name, System.Data.Metadata.Edm.TypeUsage typeUsage, ParameterMode parameterMode)
        {
            EntityUtil.CheckStringArgument(name, "name");
            EntityUtil.GenericCheckArgumentNull<System.Data.Metadata.Edm.TypeUsage>(typeUsage, "typeUsage");
            this._name = name;
            this._typeUsage = typeUsage;
            base.SetParameterMode(parameterMode);
        }

        [CompilerGenerated]
        private static SafeLink<EdmFunction> <.cctor>b__0(FunctionParameter fp) => 
            fp._declaringFunction;

        internal override void SetReadOnly()
        {
            if (!base.IsReadOnly)
            {
                base.SetReadOnly();
            }
        }

        public override string ToString() => 
            this.Name;

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.FunctionParameter;

        public EdmFunction DeclaringFunction =>
            this._declaringFunction.Value;

        internal override string Identity =>
            this._name;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.ParameterMode, false)]
        public ParameterMode Mode =>
            base.GetParameterMode();

        [MetadataProperty(PrimitiveTypeKind.String, false)]
        public string Name =>
            this._name;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.TypeUsage, false)]
        public System.Data.Metadata.Edm.TypeUsage TypeUsage =>
            this._typeUsage;
    }
}

