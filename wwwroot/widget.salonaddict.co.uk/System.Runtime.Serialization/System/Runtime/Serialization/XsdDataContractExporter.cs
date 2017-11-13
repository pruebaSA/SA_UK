namespace System.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Reflection;
    using System.ServiceModel.Diagnostics;
    using System.Xml;
    using System.Xml.Schema;

    public class XsdDataContractExporter
    {
        private System.Runtime.Serialization.DataContractSet dataContractSet;
        private ExportOptions options;
        private XmlSchemaSet schemas;

        public XsdDataContractExporter()
        {
        }

        public XsdDataContractExporter(XmlSchemaSet schemas)
        {
            this.schemas = schemas;
        }

        private void AddKnownTypes()
        {
            if (this.Options != null)
            {
                Collection<Type> knownTypes = this.Options.KnownTypes;
                if (knownTypes != null)
                {
                    for (int i = 0; i < knownTypes.Count; i++)
                    {
                        Type type = knownTypes[i];
                        if (type == null)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.Runtime.Serialization.SR.GetString("CannotExportNullKnownType")));
                        }
                        this.AddType(type);
                    }
                }
            }
        }

        private void AddType(Type type)
        {
            this.DataContractSet.Add(type);
        }

        public bool CanExport(ICollection<Assembly> assemblies)
        {
            bool flag;
            if (assemblies == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("assemblies"));
            }
            System.Runtime.Serialization.DataContractSet set = (this.dataContractSet == null) ? null : new System.Runtime.Serialization.DataContractSet(this.dataContractSet);
            try
            {
                foreach (Assembly assembly in assemblies)
                {
                    if (assembly == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.Runtime.Serialization.SR.GetString("CannotExportNullAssembly", new object[] { "assemblies" })));
                    }
                    Type[] types = assembly.GetTypes();
                    for (int i = 0; i < types.Length; i++)
                    {
                        this.CheckAndAddType(types[i]);
                    }
                }
                this.AddKnownTypes();
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
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Error, TraceCode.XsdExportError, System.Runtime.Serialization.SR.GetString("TraceCodeXsdExportError"), null, exception);
                }
                throw;
            }
            return flag;
        }

        public bool CanExport(ICollection<Type> types)
        {
            bool flag;
            if (types == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("types"));
            }
            System.Runtime.Serialization.DataContractSet set = (this.dataContractSet == null) ? null : new System.Runtime.Serialization.DataContractSet(this.dataContractSet);
            try
            {
                foreach (Type type in types)
                {
                    if (type == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.Runtime.Serialization.SR.GetString("CannotExportNullType", new object[] { "types" })));
                    }
                    this.AddType(type);
                }
                this.AddKnownTypes();
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
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Error, TraceCode.XsdExportError, System.Runtime.Serialization.SR.GetString("TraceCodeXsdExportError"), null, exception);
                }
                throw;
            }
            return flag;
        }

        public bool CanExport(Type type)
        {
            bool flag;
            if (type == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
            }
            System.Runtime.Serialization.DataContractSet set = (this.dataContractSet == null) ? null : new System.Runtime.Serialization.DataContractSet(this.dataContractSet);
            try
            {
                this.AddType(type);
                this.AddKnownTypes();
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
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Error, TraceCode.XsdExportError, System.Runtime.Serialization.SR.GetString("TraceCodeXsdExportError"), null, exception);
                }
                throw;
            }
            return flag;
        }

        private void CheckAndAddType(Type type)
        {
            type = this.GetSurrogatedType(type);
            if (!type.ContainsGenericParameters && DataContract.IsTypeSerializable(type))
            {
                this.AddType(type);
            }
        }

        private void Export()
        {
            this.AddKnownTypes();
            new SchemaExporter(this.GetSchemaSet(), this.DataContractSet).Export();
        }

        public void Export(ICollection<Assembly> assemblies)
        {
            if (assemblies == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("assemblies"));
            }
            this.TraceExportBegin();
            System.Runtime.Serialization.DataContractSet set = (this.dataContractSet == null) ? null : new System.Runtime.Serialization.DataContractSet(this.dataContractSet);
            try
            {
                foreach (Assembly assembly in assemblies)
                {
                    if (assembly == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.Runtime.Serialization.SR.GetString("CannotExportNullAssembly", new object[] { "assemblies" })));
                    }
                    Type[] types = assembly.GetTypes();
                    for (int i = 0; i < types.Length; i++)
                    {
                        this.CheckAndAddType(types[i]);
                    }
                }
                this.Export();
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
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Error, TraceCode.XsdExportError, System.Runtime.Serialization.SR.GetString("TraceCodeXsdExportError"), null, exception);
                }
                throw;
            }
            this.TraceExportEnd();
        }

        public void Export(ICollection<Type> types)
        {
            if (types == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("types"));
            }
            this.TraceExportBegin();
            System.Runtime.Serialization.DataContractSet set = (this.dataContractSet == null) ? null : new System.Runtime.Serialization.DataContractSet(this.dataContractSet);
            try
            {
                foreach (Type type in types)
                {
                    if (type == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.Runtime.Serialization.SR.GetString("CannotExportNullType", new object[] { "types" })));
                    }
                    this.AddType(type);
                }
                this.Export();
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
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Error, TraceCode.XsdExportError, System.Runtime.Serialization.SR.GetString("TraceCodeXsdExportError"), null, exception);
                }
                throw;
            }
            this.TraceExportEnd();
        }

        public void Export(Type type)
        {
            if (type == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
            }
            this.TraceExportBegin();
            System.Runtime.Serialization.DataContractSet set = (this.dataContractSet == null) ? null : new System.Runtime.Serialization.DataContractSet(this.dataContractSet);
            try
            {
                this.AddType(type);
                this.Export();
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
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Error, TraceCode.XsdExportError, System.Runtime.Serialization.SR.GetString("TraceCodeXsdExportError"), null, exception);
                }
                throw;
            }
            this.TraceExportEnd();
        }

        public XmlQualifiedName GetRootElementName(Type type)
        {
            if (type == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
            }
            type = this.GetSurrogatedType(type);
            DataContract dataContract = DataContract.GetDataContract(type);
            System.Runtime.Serialization.DataContractSet.EnsureTypeNotGeneric(dataContract.UnderlyingType);
            if (dataContract.HasRoot)
            {
                return new XmlQualifiedName(dataContract.TopLevelElementName.Value, dataContract.TopLevelElementNamespace.Value);
            }
            return null;
        }

        private XmlSchemaSet GetSchemaSet()
        {
            if (this.schemas == null)
            {
                this.schemas = new XmlSchemaSet();
                this.schemas.XmlResolver = null;
            }
            return this.schemas;
        }

        public XmlSchemaType GetSchemaType(Type type)
        {
            if (type == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
            }
            type = this.GetSurrogatedType(type);
            DataContract dataContract = DataContract.GetDataContract(type);
            System.Runtime.Serialization.DataContractSet.EnsureTypeNotGeneric(dataContract.UnderlyingType);
            XmlDataContract contract2 = dataContract as XmlDataContract;
            if ((contract2 != null) && contract2.IsAnonymous)
            {
                return contract2.XsdType;
            }
            return null;
        }

        public XmlQualifiedName GetSchemaTypeName(Type type)
        {
            if (type == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("type"));
            }
            type = this.GetSurrogatedType(type);
            DataContract dataContract = DataContract.GetDataContract(type);
            System.Runtime.Serialization.DataContractSet.EnsureTypeNotGeneric(dataContract.UnderlyingType);
            XmlDataContract contract2 = dataContract as XmlDataContract;
            if ((contract2 != null) && contract2.IsAnonymous)
            {
                return XmlQualifiedName.Empty;
            }
            return dataContract.StableName;
        }

        private Type GetSurrogatedType(Type type)
        {
            IDataContractSurrogate surrogate;
            if ((this.options != null) && ((surrogate = this.Options.GetSurrogate()) != null))
            {
                type = DataContractSurrogateCaller.GetDataContractType(surrogate, type);
            }
            return type;
        }

        private void TraceExportBegin()
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.XsdExportBegin, System.Runtime.Serialization.SR.GetString("TraceCodeXsdExportBegin"));
            }
        }

        private void TraceExportEnd()
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.XsdExportEnd, System.Runtime.Serialization.SR.GetString("TraceCodeXsdExportEnd"));
            }
        }

        private System.Runtime.Serialization.DataContractSet DataContractSet
        {
            get
            {
                if (this.dataContractSet == null)
                {
                    this.dataContractSet = new System.Runtime.Serialization.DataContractSet(this.Options?.GetSurrogate());
                }
                return this.dataContractSet;
            }
        }

        public ExportOptions Options
        {
            get => 
                this.options;
            set
            {
                this.options = value;
            }
        }

        public XmlSchemaSet Schemas
        {
            get
            {
                XmlSchemaSet schemaSet = this.GetSchemaSet();
                SchemaImporter.CompileSchemaSet(schemaSet);
                return schemaSet;
            }
        }
    }
}

