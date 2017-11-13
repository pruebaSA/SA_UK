namespace System.Data.EntityModel
{
    using Microsoft.CSharp;
    using Microsoft.VisualBasic;
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.EntityModel.Emitters;
    using System.Data.Metadata.Edm;
    using System.Data.Services.Common;
    using System.Data.Services.Design;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Threading;

    internal sealed class ClientApiGenerator : IDisposable
    {
        private System.Data.EntityModel.Emitters.AttributeEmitter _attributeEmitter;
        private CodeCompileUnit _compileUnit;
        private string _defaultContainerNamespace;
        private System.Data.Metadata.Edm.EdmItemCollection _edmItemCollection;
        private List<EdmSchemaError> _errors;
        private FixUpCollection _fixUps;
        private EntityClassGenerator _generator;
        private bool _isLanguageCaseSensitive = true;
        private Dictionary<string, string> _namespaceMap;
        private string _namespacePrefix;
        private EventHandler<PropertyGeneratedEventArgs> _onPropertyGenerated;
        private EventHandler<TypeGeneratedEventArgs> _onTypeGenerated;
        private string _sourceEdmNamespaceName;
        private System.Data.EntityModel.Emitters.TypeReference _typeReference = new System.Data.EntityModel.Emitters.TypeReference();

        public ClientApiGenerator(object sourceSchema, System.Data.Metadata.Edm.EdmItemCollection edmItemCollection, EntityClassGenerator generator, List<EdmSchemaError> errors, string namespacePrefix)
        {
            this._edmItemCollection = edmItemCollection;
            this._generator = generator;
            this._errors = errors;
            this._attributeEmitter = new System.Data.EntityModel.Emitters.AttributeEmitter(this._typeReference);
            this._namespacePrefix = namespacePrefix;
            this._namespaceMap = new Dictionary<string, string>();
            this._onTypeGenerated = new EventHandler<TypeGeneratedEventArgs>(this.TypeGeneratedEventHandler);
            this._onPropertyGenerated = new EventHandler<PropertyGeneratedEventArgs>(this.PropertyGeneratedEventHandler);
            this._generator.OnTypeGenerated += this._onTypeGenerated;
            this._generator.OnPropertyGenerated += this._onPropertyGenerated;
        }

        public void AddError(ModelBuilderErrorCode errorCode, EdmSchemaErrorSeverity severity, Exception ex)
        {
        }

        public void AddError(string message, ModelBuilderErrorCode errorCode, EdmSchemaErrorSeverity severity)
        {
        }

        internal void AddError(string message, ModelBuilderErrorCode errorCode, EdmSchemaErrorSeverity severity, Exception ex)
        {
        }

        public void Dispose()
        {
            this._generator.OnTypeGenerated -= this._onTypeGenerated;
            this._generator.OnPropertyGenerated -= this._onPropertyGenerated;
        }

        internal void GenerateCode(LazyTextWriterCreator target)
        {
            IndentedTextWriter writer = null;
            Stream stream = null;
            StreamReader reader = null;
            StreamWriter writer2 = null;
            TempFileCollection files = null;
            try
            {
                CodeDomProvider provider = null;
                switch (this.Language)
                {
                    case LanguageOption.GenerateCSharpCode:
                        provider = new CSharpCodeProvider();
                        break;

                    case LanguageOption.GenerateVBCode:
                        provider = new VBCodeProvider();
                        break;
                }
                this._isLanguageCaseSensitive = (provider.LanguageOptions & LanguageOptions.CaseInsensitive) == LanguageOptions.None;
                new NamespaceEmitter(this, this.NamespacePrefix, target.TargetFilePath).Emit();
                if (!this.RealErrorsExist)
                {
                    if ((this.FixUps.Count == 0) || !FixUpCollection.IsLanguageSupported(this.Language))
                    {
                        writer = new IndentedTextWriter(target.GetOrCreateTextWriter(), "\t");
                    }
                    else
                    {
                        files = new TempFileCollection(Path.GetTempPath());
                        string fileName = Path.Combine(files.TempDir, "EdmCodeGenFixup-" + Guid.NewGuid().ToString() + ".tmp");
                        files.AddFile(fileName, false);
                        stream = new FileStream(fileName, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
                        writer = new IndentedTextWriter(new StreamWriter(stream), "\t");
                    }
                    CodeGeneratorOptions options = new CodeGeneratorOptions {
                        BracingStyle = "C",
                        BlankLinesBetweenMembers = false,
                        VerbatimOrder = true
                    };
                    provider.GenerateCodeFromCompileUnit(this.CompileUnit, writer, options);
                    if (stream != null)
                    {
                        writer.Flush();
                        stream.Seek(0L, SeekOrigin.Begin);
                        reader = new StreamReader(stream);
                        this.FixUps.Do(reader, target.GetOrCreateTextWriter(), this.Language, !string.IsNullOrEmpty(this.SourceObjectNamespaceName));
                    }
                }
            }
            catch (UnauthorizedAccessException exception)
            {
                this.AddError(ModelBuilderErrorCode.SecurityError, EdmSchemaErrorSeverity.Error, exception);
            }
            catch (FileNotFoundException exception2)
            {
                this.AddError(ModelBuilderErrorCode.FileNotFound, EdmSchemaErrorSeverity.Error, exception2);
            }
            catch (SecurityException exception3)
            {
                this.AddError(ModelBuilderErrorCode.SecurityError, EdmSchemaErrorSeverity.Error, exception3);
            }
            catch (DirectoryNotFoundException exception4)
            {
                this.AddError(ModelBuilderErrorCode.DirectoryNotFound, EdmSchemaErrorSeverity.Error, exception4);
            }
            catch (IOException exception5)
            {
                this.AddError(ModelBuilderErrorCode.IOException, EdmSchemaErrorSeverity.Error, exception5);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
                if (files != null)
                {
                    files.Delete();
                    ((IDisposable) files).Dispose();
                }
                if (reader != null)
                {
                    reader.Close();
                }
                if (writer2 != null)
                {
                    writer2.Close();
                }
            }
        }

        public string GetClientTypeNamespace(string serviceTypeNamespace)
        {
            if (string.IsNullOrEmpty(this.NamespacePrefix))
            {
                return serviceTypeNamespace;
            }
            if (!string.IsNullOrEmpty(serviceTypeNamespace) && ((this.DefaultContainerNamespace == null) || (this.DefaultContainerNamespace != serviceTypeNamespace)))
            {
                return (this.NamespacePrefix + "." + serviceTypeNamespace);
            }
            return this.NamespacePrefix;
        }

        public string GetContainerNamespace(EntityContainer container)
        {
            if (container == null)
            {
                return null;
            }
            string namespaceName = null;
            EntitySetBase base2 = container.BaseEntitySets.FirstOrDefault<EntitySetBase>();
            if (base2 != null)
            {
                namespaceName = base2.ElementType.NamespaceName;
            }
            return namespaceName;
        }

        public CodeTypeReference GetLeastPossibleQualifiedTypeReference(EdmType type)
        {
            string name;
            string clientTypeNamespace = this.GetClientTypeNamespace(type.NamespaceName);
            if (clientTypeNamespace == this.SourceEdmNamespaceName)
            {
                name = type.Name;
            }
            else
            {
                name = this.GetObjectNamespace(clientTypeNamespace) + "." + type.Name;
            }
            return this.TypeReference.FromString(name);
        }

        private string GetObjectNamespace(string csdlNamespaceName)
        {
            string str;
            if (this._generator.EdmToObjectNamespaceMap.TryGetObjectNamespace(csdlNamespaceName, out str))
            {
                return str;
            }
            return csdlNamespaceName;
        }

        internal string GetRelationshipMultiplicityManyCollectionTypeName()
        {
            if (!this.UseDataServiceCollection)
            {
                return "System.Collections.ObjectModel.Collection";
            }
            return "System.Data.Services.Client.DataServiceCollection";
        }

        public IEnumerable<GlobalItem> GetSourceTypes()
        {
            foreach (EntityContainer iteratorVariable0 in this._edmItemCollection.GetItems<EntityContainer>())
            {
                BuiltInTypeKind builtInTypeKind = iteratorVariable0.BuiltInTypeKind;
                yield return iteratorVariable0;
            }
            foreach (EdmType iteratorVariable1 in this._edmItemCollection.GetItems<EdmType>())
            {
                switch (iteratorVariable1.BuiltInTypeKind)
                {
                    case BuiltInTypeKind.EntityType:
                    case BuiltInTypeKind.AssociationType:
                    case BuiltInTypeKind.ComplexType:
                        yield return iteratorVariable1;
                        break;

                    default:
                    {
                        continue;
                    }
                }
            }
        }

        private EntitySetBase GetUniqueEntitySetForType(EntityType entityType)
        {
            Func<EntitySetBase, bool> predicate = null;
            HashSet<EntitySetBase> source = new HashSet<EntitySetBase>(EqualityComparerEntitySet.Default);
            foreach (EntityContainer container in this.EdmItemCollection.GetItems<EntityContainer>())
            {
                bool flag = false;
                if (predicate == null)
                {
                    predicate = x => (x.BuiltInTypeKind == BuiltInTypeKind.EntitySet) && (x.ElementType == entityType);
                }
                foreach (EntitySetBase base2 in container.BaseEntitySets.Where<EntitySetBase>(predicate))
                {
                    if (flag)
                    {
                        return null;
                    }
                    flag = true;
                    source.Add(base2);
                }
            }
            if (source.Count == 1)
            {
                return source.Single<EntitySetBase>();
            }
            return null;
        }

        private static string Identity(EdmMember member) => 
            member.ToString();

        private static string Identity(EntitySetBase entitySet) => 
            entitySet.ToString();

        private static string Identity(MetadataItem item) => 
            item.ToString();

        private void PropertyGeneratedEventHandler(object sender, PropertyGeneratedEventArgs eventArgs)
        {
            if ((this.UseDataServiceCollection && ((eventArgs.PropertySource.BuiltInTypeKind == BuiltInTypeKind.EdmProperty) || (eventArgs.PropertySource.BuiltInTypeKind == BuiltInTypeKind.NavigationProperty))) && ((((EdmMember) eventArgs.PropertySource).DeclaringType.BuiltInTypeKind == BuiltInTypeKind.EntityType) || (((EdmMember) eventArgs.PropertySource).DeclaringType.BuiltInTypeKind == BuiltInTypeKind.ComplexType)))
            {
                string str = (eventArgs.PropertySource.BuiltInTypeKind == BuiltInTypeKind.EdmProperty) ? ((EdmProperty) eventArgs.PropertySource).Name : ((NavigationProperty) eventArgs.PropertySource).Name;
                eventArgs.AdditionalAfterSetStatements.Add(new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "OnPropertyChanged", new CodeExpression[] { new CodePrimitiveExpression(str) })));
            }
        }

        internal void RaisePropertyGeneratedEvent(PropertyGeneratedEventArgs eventArgs)
        {
            this._generator.RaisePropertyGeneratedEvent(eventArgs);
        }

        internal void RaiseTypeGeneratedEvent(TypeGeneratedEventArgs eventArgs)
        {
            this._generator.RaiseTypeGeneratedEvent(eventArgs);
        }

        private void TypeGeneratedEventHandler(object sender, TypeGeneratedEventArgs eventArgs)
        {
            Func<GlobalItem, bool> predicate = null;
            if (this.UseDataServiceCollection && ((eventArgs.TypeSource.BuiltInTypeKind == BuiltInTypeKind.EntityType) || (eventArgs.TypeSource.BuiltInTypeKind == BuiltInTypeKind.ComplexType)))
            {
                if (eventArgs.TypeSource.BuiltInTypeKind == BuiltInTypeKind.EntityType)
                {
                    EntitySetBase uniqueEntitySetForType = this.GetUniqueEntitySetForType((EntityType) eventArgs.TypeSource);
                    if (uniqueEntitySetForType != null)
                    {
                        List<CodeAttributeDeclaration> additionalAttributes = eventArgs.AdditionalAttributes;
                        CodeAttributeDeclaration item = new CodeAttributeDeclaration(new CodeTypeReference(typeof(EntitySetAttribute), CodeTypeReferenceOptions.GlobalReference), new CodeAttributeArgument[] { new CodeAttributeArgument(new CodePrimitiveExpression(uniqueEntitySetForType.Name)) });
                        additionalAttributes.Add(item);
                    }
                }
                if ((eventArgs.BaseType != null) && !string.IsNullOrEmpty(eventArgs.BaseType.BaseType))
                {
                    if (predicate == null)
                    {
                        predicate = x => ((EntityType) x).Name == eventArgs.BaseType.BaseType;
                    }
                    if ((from x in this.GetSourceTypes()
                        where x.BuiltInTypeKind == BuiltInTypeKind.EntityType
                        select x).Where<GlobalItem>(predicate).Count<GlobalItem>() != 0)
                    {
                        return;
                    }
                }
                eventArgs.AdditionalInterfaces.Add(typeof(INotifyPropertyChanged));
                CodeMemberEvent ctm = new CodeMemberEvent {
                    Type = new CodeTypeReference(typeof(PropertyChangedEventHandler), CodeTypeReferenceOptions.GlobalReference),
                    Name = "PropertyChanged",
                    Attributes = MemberAttributes.Public
                };
                ctm.ImplementationTypes.Add(typeof(INotifyPropertyChanged));
                System.Data.EntityModel.Emitters.AttributeEmitter.AddGeneratedCodeAttribute(ctm);
                eventArgs.AdditionalMembers.Add(ctm);
                CodeMemberMethod method = new CodeMemberMethod {
                    Name = "OnPropertyChanged"
                };
                method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(string), CodeTypeReferenceOptions.GlobalReference), "property"));
                method.ReturnType = new CodeTypeReference(typeof(void));
                System.Data.EntityModel.Emitters.AttributeEmitter.AddGeneratedCodeAttribute(method);
                method.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeEventReferenceExpression(new CodeThisReferenceExpression(), "PropertyChanged"), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)), new CodeStatement[] { new CodeExpressionStatement(new CodeDelegateInvokeExpression(new CodeEventReferenceExpression(new CodeThisReferenceExpression(), "PropertyChanged"), new CodeExpression[] { new CodeThisReferenceExpression(), new CodeObjectCreateExpression(new CodeTypeReference(typeof(PropertyChangedEventArgs), CodeTypeReferenceOptions.GlobalReference), new CodeExpression[] { new CodeArgumentReferenceExpression("property") }) })) }));
                method.Attributes = MemberAttributes.Family;
                eventArgs.AdditionalMembers.Add(method);
            }
        }

        internal void VerifyLanguageCaseSensitiveCompatibilityForEntitySet(EntityContainer item)
        {
            if (!this._isLanguageCaseSensitive)
            {
                HashSet<string> set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (EntitySetBase base2 in item.BaseEntitySets)
                {
                    if (System.Data.Metadata.Edm.Helper.IsEntitySet(base2))
                    {
                        EntitySet set2 = (EntitySet) base2;
                        if (set.Contains(Identity((EntitySetBase) set2)))
                        {
                            this.AddError(ModelBuilderErrorCode.IncompatibleSettingForCaseSensitiveOption, EdmSchemaErrorSeverity.Error, new InvalidOperationException(System.Data.Services.Design.Strings.EntitySetExistsWithDifferentCase(Identity((EntitySetBase) set2))));
                        }
                        else
                        {
                            set.Add(Identity((EntitySetBase) set2));
                        }
                    }
                }
            }
        }

        internal void VerifyLanguageCaseSensitiveCompatibilityForProperty(EdmMember item)
        {
            if (!this._isLanguageCaseSensitive)
            {
                ReadOnlyMetadataCollection<EdmMember> members = item.DeclaringType.Members;
                HashSet<string> set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (EdmMember member in members)
                {
                    if (set.Contains(Identity(member)) && string.Equals(Identity(item), Identity(member), StringComparison.OrdinalIgnoreCase))
                    {
                        this.AddError(ModelBuilderErrorCode.IncompatibleSettingForCaseSensitiveOption, EdmSchemaErrorSeverity.Error, new InvalidOperationException(System.Data.Services.Design.Strings.PropertyExistsWithDifferentCase(Identity(item))));
                    }
                    else
                    {
                        set.Add(Identity(member));
                    }
                }
            }
        }

        internal void VerifyLanguageCaseSensitiveCompatibilityForType(GlobalItem item)
        {
            if (!this._isLanguageCaseSensitive)
            {
                try
                {
                    this._edmItemCollection.GetItem<GlobalItem>(Identity(item), true);
                }
                catch (InvalidOperationException exception)
                {
                    this.AddError(ModelBuilderErrorCode.IncompatibleSettingForCaseSensitiveOption, EdmSchemaErrorSeverity.Error, exception);
                }
            }
        }

        internal System.Data.EntityModel.Emitters.AttributeEmitter AttributeEmitter =>
            this._attributeEmitter;

        internal CodeCompileUnit CompileUnit
        {
            get
            {
                if (this._compileUnit == null)
                {
                    this._compileUnit = new CodeCompileUnit();
                }
                return this._compileUnit;
            }
        }

        public string DefaultContainerNamespace
        {
            get => 
                this._defaultContainerNamespace;
            set
            {
                this._defaultContainerNamespace = value;
            }
        }

        internal System.Data.Metadata.Edm.EdmItemCollection EdmItemCollection =>
            this._edmItemCollection;

        internal FixUpCollection FixUps
        {
            get
            {
                if (this._fixUps == null)
                {
                    this._fixUps = new FixUpCollection();
                }
                return this._fixUps;
            }
        }

        internal bool IsLanguageCaseSensitive =>
            this._isLanguageCaseSensitive;

        internal LanguageOption Language =>
            this._generator.LanguageOption;

        internal StringComparison LanguageAppropriateStringComparer
        {
            get
            {
                if (this.IsLanguageCaseSensitive)
                {
                    return StringComparison.Ordinal;
                }
                return StringComparison.OrdinalIgnoreCase;
            }
        }

        public Dictionary<string, string> NamespaceMap =>
            this._namespaceMap;

        public string NamespacePrefix =>
            this._namespacePrefix;

        public bool RealErrorsExist
        {
            get
            {
                foreach (EdmSchemaError error in this._errors)
                {
                    if (error.Severity != EdmSchemaErrorSeverity.Warning)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public string SourceEdmNamespaceName
        {
            get
            {
                if (this._sourceEdmNamespaceName != null)
                {
                    return this._sourceEdmNamespaceName;
                }
                foreach (GlobalItem item in this.GetSourceTypes())
                {
                    EdmType type = item as EdmType;
                    if (type != null)
                    {
                        return type.NamespaceName;
                    }
                }
                return null;
            }
            set
            {
                this._sourceEdmNamespaceName = value;
            }
        }

        public string SourceObjectNamespaceName
        {
            get
            {
                string sourceEdmNamespaceName = this.SourceEdmNamespaceName;
                if (!string.IsNullOrEmpty(sourceEdmNamespaceName))
                {
                    return this.GetObjectNamespace(sourceEdmNamespaceName);
                }
                return null;
            }
        }

        internal System.Data.EntityModel.Emitters.TypeReference TypeReference =>
            this._typeReference;

        internal bool UseDataServiceCollection =>
            this._generator.UseDataServiceCollection;

        internal DataServiceCodeVersion Version =>
            this._generator.Version;


        public class EqualityComparerEntitySet : IEqualityComparer<EntitySetBase>
        {
            private static ClientApiGenerator.EqualityComparerEntitySet _comparer = new ClientApiGenerator.EqualityComparerEntitySet();

            public bool Equals(EntitySetBase x, EntitySetBase y) => 
                (((x == null) && (y == null)) || (((x != null) && (y != null)) && (x.Name == y.Name)));

            public int GetHashCode(EntitySetBase obj) => 
                obj.Name.GetHashCode();

            public static ClientApiGenerator.EqualityComparerEntitySet Default =>
                _comparer;
        }
    }
}

