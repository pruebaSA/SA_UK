namespace System.Runtime.Serialization
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.ServiceModel.Diagnostics;
    using System.Xml;
    using System.Xml.Schema;

    public class XsdDataContractImporter
    {
        private System.CodeDom.CodeCompileUnit codeCompileUnit;
        private System.Runtime.Serialization.DataContractSet dataContractSet;
        private static readonly XmlSchemaElement[] emptyElementArray = new XmlSchemaElement[0];
        private static readonly XmlQualifiedName[] emptyTypeNameArray = new XmlQualifiedName[0];
        private ImportOptions options;
        private XmlSchemaElement[] singleElementArray;
        private XmlQualifiedName[] singleTypeNameArray;

        public XsdDataContractImporter()
        {
        }

        public XsdDataContractImporter(System.CodeDom.CodeCompileUnit codeCompileUnit)
        {
            this.codeCompileUnit = codeCompileUnit;
        }

        public bool CanImport(XmlSchemaSet schemas)
        {
            if (schemas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
            }
            return this.InternalCanImport(schemas, null, null, null);
        }

        public bool CanImport(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames)
        {
            if (schemas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
            }
            if (typeNames == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeNames"));
            }
            return this.InternalCanImport(schemas, typeNames, emptyElementArray, emptyTypeNameArray);
        }

        public bool CanImport(XmlSchemaSet schemas, XmlSchemaElement element)
        {
            if (schemas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
            }
            if (element == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("element"));
            }
            this.SingleTypeNameArray[0] = null;
            this.SingleElementArray[0] = element;
            return this.InternalCanImport(schemas, emptyTypeNameArray, this.SingleElementArray, this.SingleTypeNameArray);
        }

        public bool CanImport(XmlSchemaSet schemas, XmlQualifiedName typeName)
        {
            if (schemas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
            }
            if (typeName == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));
            }
            return this.InternalCanImport(schemas, new XmlQualifiedName[] { typeName }, emptyElementArray, emptyTypeNameArray);
        }

        internal DataContract FindDataContract(XmlQualifiedName typeName)
        {
            if (typeName == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));
            }
            DataContract builtInDataContract = DataContract.GetBuiltInDataContract(typeName.Name, typeName.Namespace);
            if (builtInDataContract == null)
            {
                builtInDataContract = this.DataContractSet[typeName];
                if (builtInDataContract == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.Runtime.Serialization.SR.GetString("TypeHasNotBeenImported", new object[] { typeName.Name, typeName.Namespace })));
                }
            }
            return builtInDataContract;
        }

        private System.CodeDom.CodeCompileUnit GetCodeCompileUnit()
        {
            if (this.codeCompileUnit == null)
            {
                this.codeCompileUnit = new System.CodeDom.CodeCompileUnit();
            }
            return this.codeCompileUnit;
        }

        public CodeTypeReference GetCodeTypeReference(XmlQualifiedName typeName)
        {
            DataContract dataContract = this.FindDataContract(typeName);
            CodeExporter exporter = new CodeExporter(this.DataContractSet, this.Options, this.GetCodeCompileUnit());
            return exporter.GetCodeTypeReference(dataContract);
        }

        public CodeTypeReference GetCodeTypeReference(XmlQualifiedName typeName, XmlSchemaElement element)
        {
            if (element == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("element"));
            }
            if (typeName == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));
            }
            DataContract dataContract = this.FindDataContract(typeName);
            CodeExporter exporter = new CodeExporter(this.DataContractSet, this.Options, this.GetCodeCompileUnit());
            return exporter.GetElementTypeReference(dataContract, element.IsNillable);
        }

        public ICollection<CodeTypeReference> GetKnownTypeReferences(XmlQualifiedName typeName)
        {
            if (typeName == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));
            }
            DataContract builtInDataContract = DataContract.GetBuiltInDataContract(typeName.Name, typeName.Namespace);
            if (builtInDataContract == null)
            {
                builtInDataContract = this.DataContractSet[typeName];
                if (builtInDataContract == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.Runtime.Serialization.SR.GetString("TypeHasNotBeenImported", new object[] { typeName.Name, typeName.Namespace })));
                }
            }
            CodeExporter exporter = new CodeExporter(this.DataContractSet, this.Options, this.GetCodeCompileUnit());
            return exporter.GetKnownTypeReferences(builtInDataContract);
        }

        public void Import(XmlSchemaSet schemas)
        {
            if (schemas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
            }
            this.InternalImport(schemas, null, null, null);
        }

        public void Import(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames)
        {
            if (schemas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
            }
            if (typeNames == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeNames"));
            }
            this.InternalImport(schemas, typeNames, emptyElementArray, emptyTypeNameArray);
        }

        public XmlQualifiedName Import(XmlSchemaSet schemas, XmlSchemaElement element)
        {
            if (schemas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
            }
            if (element == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("element"));
            }
            this.SingleTypeNameArray[0] = null;
            this.SingleElementArray[0] = element;
            this.InternalImport(schemas, emptyTypeNameArray, this.SingleElementArray, this.SingleTypeNameArray);
            return this.SingleTypeNameArray[0];
        }

        public void Import(XmlSchemaSet schemas, XmlQualifiedName typeName)
        {
            if (schemas == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("schemas"));
            }
            if (typeName == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("typeName"));
            }
            this.SingleTypeNameArray[0] = typeName;
            this.InternalImport(schemas, this.SingleTypeNameArray, emptyElementArray, emptyTypeNameArray);
        }

        private bool InternalCanImport(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames, ICollection<XmlSchemaElement> elements, XmlQualifiedName[] elementTypeNames)
        {
            bool flag;
            System.Runtime.Serialization.DataContractSet set = (this.dataContractSet == null) ? null : new System.Runtime.Serialization.DataContractSet(this.dataContractSet);
            try
            {
                new SchemaImporter(schemas, typeNames, elements, elementTypeNames, this.DataContractSet, this.ImportXmlDataType).Import();
                flag = true;
            }
            catch (InvalidDataContractException)
            {
                this.dataContractSet = set;
                flag = false;
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                this.dataContractSet = set;
                if (DiagnosticUtility.ShouldTraceError)
                {
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Error, TraceCode.XsdImportError, System.Runtime.Serialization.SR.GetString("TraceCodeXsdImportError"), null, exception);
                }
                throw;
            }
            return flag;
        }

        private void InternalImport(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames, ICollection<XmlSchemaElement> elements, XmlQualifiedName[] elementTypeNames)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.XsdImportBegin, System.Runtime.Serialization.SR.GetString("TraceCodeXsdImportBegin"));
            }
            System.Runtime.Serialization.DataContractSet set = (this.dataContractSet == null) ? null : new System.Runtime.Serialization.DataContractSet(this.dataContractSet);
            try
            {
                new SchemaImporter(schemas, typeNames, elements, elementTypeNames, this.DataContractSet, this.ImportXmlDataType).Import();
                new CodeExporter(this.DataContractSet, this.Options, this.GetCodeCompileUnit()).Export();
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                this.dataContractSet = set;
                if (DiagnosticUtility.ShouldTraceError)
                {
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Error, TraceCode.XsdImportError, System.Runtime.Serialization.SR.GetString("TraceCodeXsdImportError"), null, exception);
                }
                throw;
            }
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.XsdImportEnd, System.Runtime.Serialization.SR.GetString("TraceCodeXsdImportEnd"));
            }
        }

        public System.CodeDom.CodeCompileUnit CodeCompileUnit =>
            this.GetCodeCompileUnit();

        private System.Runtime.Serialization.DataContractSet DataContractSet
        {
            get
            {
                if (this.dataContractSet == null)
                {
                    this.dataContractSet = (this.Options == null) ? new System.Runtime.Serialization.DataContractSet(null, null, null) : new System.Runtime.Serialization.DataContractSet(this.Options.DataContractSurrogate, this.Options.ReferencedTypes, this.Options.ReferencedCollectionTypes);
                }
                return this.dataContractSet;
            }
        }

        private bool ImportXmlDataType =>
            ((this.Options != null) && this.Options.ImportXmlType);

        public ImportOptions Options
        {
            get => 
                this.options;
            set
            {
                this.options = value;
            }
        }

        private XmlSchemaElement[] SingleElementArray
        {
            get
            {
                if (this.singleElementArray == null)
                {
                    this.singleElementArray = new XmlSchemaElement[1];
                }
                return this.singleElementArray;
            }
        }

        private XmlQualifiedName[] SingleTypeNameArray
        {
            get
            {
                if (this.singleTypeNameArray == null)
                {
                    this.singleTypeNameArray = new XmlQualifiedName[1];
                }
                return this.singleTypeNameArray;
            }
        }
    }
}

