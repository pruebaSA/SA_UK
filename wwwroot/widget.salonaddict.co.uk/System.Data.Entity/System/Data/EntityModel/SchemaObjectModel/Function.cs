namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;

    internal class Function : SchemaType
    {
        private System.Data.Metadata.Edm.CollectionKind _collectionKind;
        private FunctionCommandText _commandText;
        private System.Data.EntityModel.SchemaObjectModel.EntityContainer _container;
        private EntityContainerEntitySet _entitySet;
        private string _functionStrongName;
        private bool _isAggregate;
        private bool _isBuiltIn;
        private bool _isComposable;
        private bool _isNiladicFunction;
        private SchemaElementLookUpTable<Parameter> _parameters;
        private System.Data.Metadata.Edm.ParameterTypeSemantics _parameterTypeSemantics;
        private System.Data.EntityModel.SchemaObjectModel.ReturnType _returnType;
        private string _schema;
        private string _storeFunctionName;
        private SchemaType _type;
        private string _unresolvedEntitySet;
        private string _unresolvedType;
        private static Regex s_typeParser = new Regex(@"^(?<modifier>((Collection)|(Ref)))\s*\(\s*(?<typeName>\S*)\s*\)$", RegexOptions.Compiled);

        protected Function(System.Data.EntityModel.SchemaObjectModel.EntityContainer container) : base(container.Schema)
        {
            this._isComposable = true;
            this._container = container;
            this._isComposable = false;
        }

        public Function(Schema parentElement) : base(parentElement)
        {
            this._isComposable = true;
        }

        internal override SchemaElement Clone(SchemaElement parentElement)
        {
            Function function = new FunctionImportElement((System.Data.EntityModel.SchemaObjectModel.EntityContainer) parentElement) {
                _isAggregate = this._isAggregate,
                _isBuiltIn = this._isBuiltIn,
                _isNiladicFunction = this._isNiladicFunction,
                _isComposable = this._isComposable,
                _entitySet = this._entitySet,
                _commandText = this._commandText,
                _storeFunctionName = this._storeFunctionName,
                _type = this._type,
                _returnType = this._returnType,
                _collectionKind = this._collectionKind,
                _parameterTypeSemantics = this._parameterTypeSemantics,
                _schema = this._schema,
                Name = this.Name
            };
            foreach (Parameter parameter in this.Parameters)
            {
                function.Parameters.TryAdd((Parameter) parameter.Clone(function));
            }
            return function;
        }

        private void HandleAggregateAttribute(XmlReader reader)
        {
            bool field = false;
            base.HandleBoolAttribute(reader, ref field);
            this.IsAggregate = field;
        }

        protected override bool HandleAttribute(XmlReader reader)
        {
            if (base.HandleAttribute(reader))
            {
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "ReturnType"))
            {
                this.HandleReturnTypeAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Aggregate"))
            {
                this.HandleAggregateAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "BuiltIn"))
            {
                this.HandleBuiltInAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "StoreFunctionName"))
            {
                this.HandleStoreFunctionNameAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "NiladicFunction"))
            {
                this.HandleNiladicFunctionAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "IsComposable"))
            {
                this.HandleIsComposableFunctionAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "ParameterTypeSemantics"))
            {
                this.HandleParameterTypeSemanticsAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Schema"))
            {
                this.HandleDbSchemaAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "EntitySet"))
            {
                this.HandleEntitySetAttribute(reader);
                return true;
            }
            return false;
        }

        private void HandleBuiltInAttribute(XmlReader reader)
        {
            bool field = false;
            base.HandleBoolAttribute(reader, ref field);
            this.IsBuiltIn = field;
        }

        private void HandleCommandTextFunctionElment(XmlReader reader)
        {
            FunctionCommandText text = new FunctionCommandText(this);
            text.Parse(reader);
            this._commandText = text;
        }

        private void HandleDbSchemaAttribute(XmlReader reader)
        {
            this._schema = reader.Value;
        }

        protected override bool HandleElement(XmlReader reader)
        {
            if (!base.HandleElement(reader))
            {
                if (base.CanHandleElement(reader, "CommandText"))
                {
                    this.HandleCommandTextFunctionElment(reader);
                    return true;
                }
                if (base.CanHandleElement(reader, "Parameter"))
                {
                    this.HandleParameterElement(reader);
                    return true;
                }
                if (!base.CanHandleElement(reader, "ReturnType"))
                {
                    return false;
                }
                if (base.ParentElement.Schema.DataModel == SchemaDataModelOption.ProviderManifestModel)
                {
                    this.HandleReturnTypeElement(reader);
                }
                else
                {
                    this.SkipThroughElement(reader);
                }
            }
            return true;
        }

        private void HandleEntitySetAttribute(XmlReader reader)
        {
            string str;
            if (Utils.GetString(base.Schema, reader, out str))
            {
                this._unresolvedEntitySet = str;
            }
        }

        private void HandleIsComposableFunctionAttribute(XmlReader reader)
        {
            bool field = true;
            base.HandleBoolAttribute(reader, ref field);
            this.IsComposable = field;
        }

        private void HandleNiladicFunctionAttribute(XmlReader reader)
        {
            bool field = false;
            base.HandleBoolAttribute(reader, ref field);
            this.IsNiladicFunction = field;
        }

        private void HandleParameterElement(XmlReader reader)
        {
            Parameter type = new Parameter(this);
            type.Parse(reader);
            this.Parameters.Add(type, true, new Func<object, string>(Strings.ParameterNameAlreadyDefinedDuplicate));
        }

        private void HandleParameterTypeSemanticsAttribute(XmlReader reader)
        {
            string str = reader.Value;
            if (!string.IsNullOrEmpty(str))
            {
                str = str.Trim();
                if (string.IsNullOrEmpty(str))
                {
                    return;
                }
                string str2 = str;
                if (str2 == null)
                {
                    goto Label_0065;
                }
                if (str2 != "ExactMatchOnly")
                {
                    if (str2 == "AllowImplicitPromotion")
                    {
                        this.ParameterTypeSemantics = System.Data.Metadata.Edm.ParameterTypeSemantics.AllowImplicitPromotion;
                        return;
                    }
                    if (str2 == "AllowImplicitConversion")
                    {
                        this.ParameterTypeSemantics = System.Data.Metadata.Edm.ParameterTypeSemantics.AllowImplicitConversion;
                        return;
                    }
                    goto Label_0065;
                }
                this.ParameterTypeSemantics = System.Data.Metadata.Edm.ParameterTypeSemantics.ExactMatchOnly;
            }
            return;
        Label_0065:
            base.AddError(ErrorCode.InvalidValueForParameterTypeSemantics, EdmSchemaErrorSeverity.Error, reader, Strings.InvalidValueForParameterTypeSemanticsAttribute(str));
        }

        private void HandleReturnTypeAttribute(XmlReader reader)
        {
            string str;
            if (Utils.GetString(base.Schema, reader, out str))
            {
                switch (RemoveTypeModifier(ref str))
                {
                    case TypeModifier.Array:
                        this.CollectionKind = System.Data.Metadata.Edm.CollectionKind.Bag;
                        break;
                }
                if (Utils.ValidateDottedName(base.Schema, reader, str))
                {
                    this.UnresolvedReturnType = str;
                }
            }
        }

        private void HandleReturnTypeElement(XmlReader reader)
        {
            System.Data.EntityModel.SchemaObjectModel.ReturnType type = new System.Data.EntityModel.SchemaObjectModel.ReturnType(this);
            type.Parse(reader);
            this._returnType = type;
        }

        private void HandleStoreFunctionNameAttribute(XmlReader reader)
        {
            string str = reader.Value.ToString();
            if (!string.IsNullOrEmpty(str))
            {
                str = str.Trim();
                this.StoreFunctionName = str;
            }
        }

        private bool MeetsFunctionImportReturnTypeRequirements(SchemaType type)
        {
            if ((this._type is ScalarType) && (this._collectionKind == System.Data.Metadata.Edm.CollectionKind.Bag))
            {
                return true;
            }
            if ((this._type is SchemaEntityType) && (this._collectionKind == System.Data.Metadata.Edm.CollectionKind.Bag))
            {
                return true;
            }
            if (base.Schema.EdmVersion == 1.1)
            {
                if ((this._type is ScalarType) && (this._collectionKind == System.Data.Metadata.Edm.CollectionKind.None))
                {
                    return true;
                }
                if ((this._type is SchemaEntityType) && (this._collectionKind == System.Data.Metadata.Edm.CollectionKind.None))
                {
                    return true;
                }
                if ((this._type is SchemaComplexType) && (this._collectionKind == System.Data.Metadata.Edm.CollectionKind.None))
                {
                    return true;
                }
                if ((this._type is SchemaComplexType) && (this._collectionKind == System.Data.Metadata.Edm.CollectionKind.Bag))
                {
                    return true;
                }
            }
            return false;
        }

        internal static TypeModifier RemoveTypeModifier(ref string type)
        {
            Match match = s_typeParser.Match(type);
            if (match.Success)
            {
                string str;
                type = match.Groups["typeName"].Value;
                if (((str = match.Groups["modifier"].Value) != null) && (str == "Collection"))
                {
                    return TypeModifier.Array;
                }
            }
            return TypeModifier.None;
        }

        internal override void ResolveTopLevelNames()
        {
            base.ResolveTopLevelNames();
            if (this.ReturnType != null)
            {
                this.ReturnType.ResolveTopLevelNames();
            }
            if ((this.UnresolvedReturnType != null) && base.Schema.ResolveTypeName(this, this.UnresolvedReturnType, out this._type))
            {
                if (this.IsFunctionImport)
                {
                    if (!this.MeetsFunctionImportReturnTypeRequirements(this._type))
                    {
                        base.AddError(ErrorCode.FunctionImportUnsupportedReturnType, EdmSchemaErrorSeverity.Error, this, (base.Schema.EdmVersion == 1.0) ? Strings.FunctionImportWithUnsupportedReturnTypeV1(this.Name) : Strings.FunctionImportWithUnsupportedReturnTypeV1_1(this.Name));
                    }
                }
                else if (!(this._type is ScalarType))
                {
                    if (base.Schema.DataModel != SchemaDataModelOption.ProviderManifestModel)
                    {
                        base.AddError(ErrorCode.FunctionWithNonScalarTypeNotSupported, EdmSchemaErrorSeverity.Error, this, Strings.FunctionWithNonScalarTypeNotSupported(this._type.FQName, this.FQName));
                    }
                    else
                    {
                        base.AddError(ErrorCode.FunctionWithNonScalarTypeNotSupported, EdmSchemaErrorSeverity.Error, this, Strings.FunctionWithNonEdmTypeNotSupported(this._type.FQName, this.FQName));
                    }
                }
            }
            if ((this.IsFunctionImport && (this._entitySet == null)) && (this._unresolvedEntitySet != null))
            {
                this._entitySet = this._container.FindEntitySet(this._unresolvedEntitySet);
                if (this._entitySet == null)
                {
                    base.AddError(ErrorCode.FunctionImportUnknownEntitySet, EdmSchemaErrorSeverity.Error, Strings.FunctionImportUnknownEntitySet(this._unresolvedEntitySet, this.Name));
                }
            }
            foreach (Parameter parameter in this.Parameters)
            {
                parameter.ResolveTopLevelNames();
            }
        }

        internal override void Validate()
        {
            base.Validate();
            if ((this._returnType != null) && (this._type != null))
            {
                base.AddError(ErrorCode.AmbiguousFunctionReturnType, EdmSchemaErrorSeverity.Error, Strings.AmbiguousFunctionReturnType(this.Name, "ReturnType"));
            }
            if (this._commandText != null)
            {
                this._commandText.Validate();
            }
            if (this.Type == null)
            {
                if (this.IsComposable)
                {
                    base.AddError(ErrorCode.ComposableFunctionWithoutReturnType, EdmSchemaErrorSeverity.Error, Strings.ComposableFunctionMustDeclareReturnType);
                }
            }
            else if (!this.IsComposable && !this.IsFunctionImport)
            {
                base.AddError(ErrorCode.NonComposableFunctionWithReturnType, EdmSchemaErrorSeverity.Error, Strings.NonComposableFunctionMustNotDeclareReturnType);
            }
            if (this.IsAggregate)
            {
                if (this.Parameters.Count != 1)
                {
                    base.AddError(ErrorCode.InvalidNumberOfParametersForAggregateFunction, EdmSchemaErrorSeverity.Error, this, Strings.InvalidNumberOfParametersForAggregateFunction(this.FQName));
                }
                else if (this.Parameters.GetElementAt(0).CollectionKind == System.Data.Metadata.Edm.CollectionKind.None)
                {
                    Parameter elementAt = this.Parameters.GetElementAt(0);
                    base.AddError(ErrorCode.InvalidParameterTypeForAggregateFunction, EdmSchemaErrorSeverity.Error, this, Strings.InvalidParameterTypeForAggregateFunction(elementAt.Name, this.FQName));
                }
            }
            if (!this.IsComposable && ((this.IsAggregate || this.IsNiladicFunction) || this.IsBuiltIn))
            {
                base.AddError(ErrorCode.NonComposableFunctionAttributesNotValid, EdmSchemaErrorSeverity.Error, Strings.NonComposableFunctionHasDisallowedAttribute);
            }
            if (this.CommandText != null)
            {
                if (this.IsComposable)
                {
                    base.AddError(ErrorCode.ComposableFunctionWithCommandText, EdmSchemaErrorSeverity.Error, Strings.CommandTextFunctionsNotComposable);
                }
                if (this.StoreFunctionName != null)
                {
                    base.AddError(ErrorCode.FunctionDeclaresCommandTextAndStoreFunctionName, EdmSchemaErrorSeverity.Error, Strings.CommandTextFunctionsCannotDeclareStoreFunctionName);
                }
            }
            if (this.IsFunctionImport)
            {
                SchemaEntityType type = this.Type as SchemaEntityType;
                if (type != null)
                {
                    if (this.EntitySet == null)
                    {
                        base.AddError(ErrorCode.FunctionImportReturnsEntitiesButDoesNotSpecifyEntitySet, EdmSchemaErrorSeverity.Error, Strings.FunctionImportReturnEntitiesButDoesNotSpecifyEntitySet(this.Name));
                    }
                    else if ((this.EntitySet.EntityType != null) && !type.IsOfType(this.EntitySet.EntityType))
                    {
                        base.AddError(ErrorCode.FunctionImportEntityTypeDoesNotMatchEntitySet, EdmSchemaErrorSeverity.Error, Strings.FunctionImportEntityTypeDoesNotMatchEntitySet(this.FQName, this.EntitySet.EntityType.FQName, this.EntitySet.Name, base.ParentElement.FQName));
                    }
                }
                if ((type == null) && (this.EntitySet != null))
                {
                    base.AddError(ErrorCode.FunctionImportSpecifiesEntitySetButDoesNotReturnEntityType, EdmSchemaErrorSeverity.Error, Strings.FunctionImportSpecifiesEntitySetButNotEntityType(this.Name, base.ParentElement.FQName));
                }
            }
        }

        public System.Data.Metadata.Edm.CollectionKind CollectionKind
        {
            get => 
                this._collectionKind;
            internal set
            {
                this._collectionKind = value;
            }
        }

        public string CommandText
        {
            get
            {
                if (this._commandText != null)
                {
                    return this._commandText.CommandText;
                }
                return null;
            }
        }

        public string DbSchema =>
            this._schema;

        public EntityContainerEntitySet EntitySet =>
            this._entitySet;

        public override string Identity
        {
            get
            {
                if (string.IsNullOrEmpty(this._functionStrongName))
                {
                    StringBuilder builder = new StringBuilder(this.FQName);
                    bool flag = true;
                    builder.Append('(');
                    foreach (Parameter parameter in this.Parameters)
                    {
                        if (!flag)
                        {
                            builder.Append(',');
                        }
                        else
                        {
                            flag = false;
                        }
                        builder.Append(Helper.ToString(parameter.ParameterDirection));
                        builder.Append(' ');
                        builder.Append(parameter.Type.FQName);
                    }
                    builder.Append(')');
                    this._functionStrongName = builder.ToString();
                }
                return this._functionStrongName;
            }
        }

        public bool IsAggregate
        {
            get => 
                this._isAggregate;
            internal set
            {
                this._isAggregate = value;
            }
        }

        public bool IsBuiltIn
        {
            get => 
                this._isBuiltIn;
            internal set
            {
                this._isBuiltIn = value;
            }
        }

        public bool IsComposable
        {
            get => 
                this._isComposable;
            internal set
            {
                this._isComposable = value;
            }
        }

        public virtual bool IsFunctionImport =>
            false;

        public bool IsNiladicFunction
        {
            get => 
                this._isNiladicFunction;
            internal set
            {
                this._isNiladicFunction = value;
            }
        }

        public SchemaElementLookUpTable<Parameter> Parameters
        {
            get
            {
                if (this._parameters == null)
                {
                    this._parameters = new SchemaElementLookUpTable<Parameter>();
                }
                return this._parameters;
            }
        }

        public System.Data.Metadata.Edm.ParameterTypeSemantics ParameterTypeSemantics
        {
            get => 
                this._parameterTypeSemantics;
            internal set
            {
                this._parameterTypeSemantics = value;
            }
        }

        public System.Data.EntityModel.SchemaObjectModel.ReturnType ReturnType =>
            this._returnType;

        public string StoreFunctionName
        {
            get => 
                this._storeFunctionName;
            internal set
            {
                this._storeFunctionName = value;
            }
        }

        public SchemaType Type
        {
            get
            {
                if (this.ReturnType != null)
                {
                    return this.ReturnType.Type;
                }
                return this._type;
            }
        }

        internal string UnresolvedReturnType
        {
            get => 
                this._unresolvedType;
            set
            {
                this._unresolvedType = value;
            }
        }
    }
}

