namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;

    public sealed class EdmFunction : EdmType
    {
        private readonly string _commandTextAttribute;
        private readonly System.Data.Metadata.Edm.EntitySet _entitySet;
        private readonly string _fullName;
        private readonly FunctionAttributes _functionAttributes;
        private readonly ReadOnlyMetadataCollection<FunctionParameter> _parameters;
        private readonly ParameterTypeSemantics _parameterTypeSemantics;
        private readonly FunctionParameter _returnParameter;
        private readonly string _schemaName;
        private readonly string _storeFunctionNameAttribute;

        internal EdmFunction(string name, string namespaceName, DataSpace dataSpace, EdmFunctionPayload payload) : base(name, namespaceName, dataSpace)
        {
            this._functionAttributes = FunctionAttributes.Default;
            this._schemaName = payload.Schema;
            this._fullName = base.NamespaceName + "." + base.Name;
            if (payload.ReturnParameter != null)
            {
                if (payload.ReturnParameter.Mode != ParameterMode.ReturnValue)
                {
                    throw EntityUtil.InvalidModeInReturnParameterInFunction("returnParameter");
                }
                this._returnParameter = SafeLink<EdmFunction>.BindChild<FunctionParameter>(this, FunctionParameter.DeclaringFunctionLinker, payload.ReturnParameter);
            }
            if (payload.IsAggregate.HasValue)
            {
                SetFunctionAttribute(ref this._functionAttributes, FunctionAttributes.Aggregate, payload.IsAggregate.Value);
            }
            if (payload.IsBuiltIn.HasValue)
            {
                SetFunctionAttribute(ref this._functionAttributes, FunctionAttributes.BuiltIn, payload.IsBuiltIn.Value);
            }
            if (payload.IsNiladic.HasValue)
            {
                SetFunctionAttribute(ref this._functionAttributes, FunctionAttributes.NiladicFunction, payload.IsNiladic.Value);
            }
            if (payload.IsComposable.HasValue)
            {
                SetFunctionAttribute(ref this._functionAttributes, FunctionAttributes.Default, payload.IsComposable.Value);
            }
            if (payload.IsFromProviderManifest.HasValue)
            {
                SetFunctionAttribute(ref this._functionAttributes, FunctionAttributes.IsFromProviderManifest, payload.IsFromProviderManifest.Value);
            }
            if (payload.IsCachedStoreFunction.HasValue)
            {
                SetFunctionAttribute(ref this._functionAttributes, FunctionAttributes.IsCachedStoreFunction, payload.IsCachedStoreFunction.Value);
            }
            if (payload.ParameterTypeSemantics.HasValue)
            {
                this._parameterTypeSemantics = payload.ParameterTypeSemantics.Value;
            }
            if (payload.StoreFunctionName != null)
            {
                this._storeFunctionNameAttribute = payload.StoreFunctionName;
            }
            if (payload.EntitySet != null)
            {
                this._entitySet = payload.EntitySet;
            }
            if (payload.CommandText != null)
            {
                this._commandTextAttribute = payload.CommandText;
            }
            if (payload.Parameters != null)
            {
                foreach (FunctionParameter parameter in payload.Parameters)
                {
                    if (parameter == null)
                    {
                        throw EntityUtil.CollectionParameterElementIsNull("parameters");
                    }
                    if (parameter.Mode == ParameterMode.ReturnValue)
                    {
                        throw EntityUtil.InvalidModeInParameterInFunction("parameters");
                    }
                }
                this._parameters = new SafeLinkCollection<EdmFunction, FunctionParameter>(this, FunctionParameter.DeclaringFunctionLinker, new MetadataCollection<FunctionParameter>(payload.Parameters));
            }
            else
            {
                this._parameters = new ReadOnlyMetadataCollection<FunctionParameter>(new MetadataCollection<FunctionParameter>());
            }
        }

        internal override void BuildIdentity(StringBuilder builder)
        {
            if (base.CacheIdentity != null)
            {
                builder.Append(base.CacheIdentity);
            }
            else
            {
                BuildIdentity(builder, this.FullName, this.Parameters);
            }
        }

        internal static string BuildIdentity(string functionName, IEnumerable<EdmType> functionParameters)
        {
            StringBuilder builder = new StringBuilder();
            BuildIdentity(builder, functionName, functionParameters);
            return builder.ToString();
        }

        private static void BuildIdentity(StringBuilder builder, string functionName, IEnumerable<EdmType> functionParameters)
        {
            builder.Append(functionName);
            builder.Append('(');
            bool flag = true;
            foreach (EdmType type in functionParameters)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    builder.Append(",");
                }
                builder.Append(ParameterMode.In);
                builder.Append(' ');
                TypeUsage.Create(type).BuildIdentity(builder);
            }
            builder.Append(')');
        }

        private static void BuildIdentity(StringBuilder builder, string functionName, IEnumerable<FunctionParameter> functionParameters)
        {
            builder.Append(functionName);
            builder.Append('(');
            bool flag = true;
            foreach (FunctionParameter parameter in functionParameters)
            {
                if (flag)
                {
                    flag = false;
                }
                else
                {
                    builder.Append(",");
                }
                builder.Append(Helper.ToString(parameter.Mode));
                builder.Append(' ');
                parameter.TypeUsage.BuildIdentity(builder);
            }
            builder.Append(')');
        }

        private bool GetFunctionAttribute(FunctionAttributes attribute) => 
            (attribute == ((byte) (attribute & this._functionAttributes)));

        private static void SetFunctionAttribute(ref FunctionAttributes field, FunctionAttributes attribute, bool isSet)
        {
            if (isSet)
            {
                field = (FunctionAttributes) ((byte) (field | attribute));
            }
            else
            {
                field = (FunctionAttributes) ((byte) (field ^ ((byte) (field & attribute))));
            }
        }

        internal override void SetReadOnly()
        {
            if (!base.IsReadOnly)
            {
                base.SetReadOnly();
                this.Parameters.Source.SetReadOnly();
                FunctionParameter returnParameter = this.ReturnParameter;
                if (returnParameter != null)
                {
                    returnParameter.SetReadOnly();
                }
            }
        }

        [MetadataProperty(PrimitiveTypeKind.Boolean, false)]
        internal bool AggregateAttribute =>
            this.GetFunctionAttribute(FunctionAttributes.Aggregate);

        [MetadataProperty(PrimitiveTypeKind.Boolean, false)]
        internal bool BuiltInAttribute =>
            this.GetFunctionAttribute(FunctionAttributes.BuiltIn);

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.EdmFunction;

        [MetadataProperty(PrimitiveTypeKind.String, false)]
        internal string CommandTextAttribute =>
            this._commandTextAttribute;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.EntitySet, false)]
        internal System.Data.Metadata.Edm.EntitySet EntitySet =>
            this._entitySet;

        public override string FullName =>
            this._fullName;

        internal bool IsCachedStoreFunction =>
            this.GetFunctionAttribute(FunctionAttributes.IsCachedStoreFunction);

        [MetadataProperty(PrimitiveTypeKind.Boolean, false)]
        internal bool IsComposableAttribute =>
            this.GetFunctionAttribute(FunctionAttributes.Default);

        [MetadataProperty(PrimitiveTypeKind.Boolean, false)]
        internal bool IsFromProviderManifest =>
            this.GetFunctionAttribute(FunctionAttributes.IsFromProviderManifest);

        [MetadataProperty(PrimitiveTypeKind.Boolean, false)]
        internal bool NiladicFunctionAttribute =>
            this.GetFunctionAttribute(FunctionAttributes.NiladicFunction);

        public ReadOnlyMetadataCollection<FunctionParameter> Parameters =>
            this._parameters;

        [MetadataProperty(typeof(ParameterTypeSemantics), false)]
        internal ParameterTypeSemantics ParameterTypeSemanticsAttribute =>
            this._parameterTypeSemantics;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.FunctionParameter, false)]
        public FunctionParameter ReturnParameter =>
            this._returnParameter;

        [MetadataProperty(PrimitiveTypeKind.String, false)]
        internal string Schema =>
            this._schemaName;

        [MetadataProperty(PrimitiveTypeKind.String, false)]
        internal string StoreFunctionNameAttribute =>
            this._storeFunctionNameAttribute;

        [Flags]
        private enum FunctionAttributes : byte
        {
            Aggregate = 1,
            BuiltIn = 2,
            Default = 8,
            IsCachedStoreFunction = 0x20,
            IsComposable = 8,
            IsFromProviderManifest = 0x10,
            NiladicFunction = 4,
            None = 0
        }
    }
}

