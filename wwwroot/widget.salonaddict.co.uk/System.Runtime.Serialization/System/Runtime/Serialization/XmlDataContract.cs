namespace System.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    internal sealed class XmlDataContract : DataContract
    {
        [SecurityCritical]
        private XmlDataContractCriticalHelper helper;

        [SecurityTreatAsSafe, SecurityCritical]
        internal XmlDataContract() : base(new XmlDataContractCriticalHelper())
        {
            this.helper = base.Helper as XmlDataContractCriticalHelper;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal XmlDataContract(Type type) : base(new XmlDataContractCriticalHelper(type))
        {
            this.helper = base.Helper as XmlDataContractCriticalHelper;
        }

        internal override bool Equals(object other, Dictionary<DataContractPairKey, object> checkedContracts)
        {
            if (base.IsEqualOrChecked(other, checkedContracts))
            {
                return true;
            }
            XmlDataContract contract = other as XmlDataContract;
            if (contract == null)
            {
                return false;
            }
            if (this.HasRoot != contract.HasRoot)
            {
                return false;
            }
            if (this.IsAnonymous)
            {
                return contract.IsAnonymous;
            }
            return ((base.StableName.Name == contract.StableName.Name) && (base.StableName.Namespace == contract.StableName.Namespace));
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal System.Runtime.Serialization.CreateXmlSerializableDelegate GenerateCreateXmlSerializableDelegate()
        {
            Type underlyingType = base.UnderlyingType;
            CodeGenerator generator = new CodeGenerator();
            bool allowPrivateMemberAccess = this.RequiresMemberAccessForCreate(null);
            try
            {
                generator.BeginMethod("Create" + DataContract.GetClrTypeFullName(underlyingType), typeof(System.Runtime.Serialization.CreateXmlSerializableDelegate), allowPrivateMemberAccess);
            }
            catch (SecurityException exception)
            {
                if (!allowPrivateMemberAccess || !exception.PermissionType.Equals(typeof(ReflectionPermission)))
                {
                    throw;
                }
                this.RequiresMemberAccessForCreate(exception);
            }
            if (underlyingType.IsValueType)
            {
                LocalBuilder localBuilder = generator.DeclareLocal(underlyingType, underlyingType.Name + "Value");
                generator.Ldloca(localBuilder);
                generator.InitObj(underlyingType);
                generator.Ldloc(localBuilder);
            }
            else
            {
                generator.New(this.GetConstructor());
            }
            generator.ConvertValue(base.UnderlyingType, Globals.TypeOfIXmlSerializable);
            generator.Ret();
            return (System.Runtime.Serialization.CreateXmlSerializableDelegate) generator.EndMethod();
        }

        private ConstructorInfo GetConstructor()
        {
            Type underlyingType = base.UnderlyingType;
            if (underlyingType.IsValueType)
            {
                return null;
            }
            ConstructorInfo info = underlyingType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, Globals.EmptyTypeArray, null);
            if (info == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("IXmlSerializableMustHaveDefaultConstructor", new object[] { DataContract.GetClrTypeFullName(underlyingType) })));
            }
            return info;
        }

        public override int GetHashCode() => 
            base.GetHashCode();

        public override object ReadXmlValue(XmlReaderDelegator xmlReader, XmlObjectSerializerReadContext context)
        {
            object obj2;
            if (context == null)
            {
                obj2 = XmlObjectSerializerReadContext.ReadRootIXmlSerializable(xmlReader, this, true);
            }
            else
            {
                obj2 = context.ReadIXmlSerializable(xmlReader, this, true);
                context.AddNewObject(obj2);
            }
            xmlReader.ReadEndElement();
            return obj2;
        }

        private bool RequiresMemberAccessForCreate(SecurityException securityException)
        {
            if (!DataContract.IsTypeVisible(base.UnderlyingType))
            {
                if (securityException != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityException(System.Runtime.Serialization.SR.GetString("PartialTrustIXmlSerializableTypeNotPublic", new object[] { DataContract.GetClrTypeFullName(base.UnderlyingType) }), securityException));
                }
                return true;
            }
            if (!DataContract.ConstructorRequiresMemberAccess(this.GetConstructor()))
            {
                return false;
            }
            if (securityException != null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityException(System.Runtime.Serialization.SR.GetString("PartialTrustIXmlSerialzableNoPublicConstructor", new object[] { DataContract.GetClrTypeFullName(base.UnderlyingType) }), securityException));
            }
            return true;
        }

        [SecurityCritical]
        internal void SetTopLevelElementName(XmlQualifiedName elementName)
        {
            if (elementName != null)
            {
                XmlDictionary dictionary = new XmlDictionary();
                this.TopLevelElementName = dictionary.Add(elementName.Name);
                this.TopLevelElementNamespace = dictionary.Add(elementName.Namespace);
            }
        }

        public override void WriteXmlValue(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context)
        {
            if (context == null)
            {
                XmlObjectSerializerWriteContext.WriteRootIXmlSerializable(xmlWriter, obj);
            }
            else
            {
                context.WriteIXmlSerializable(xmlWriter, obj);
            }
        }

        internal override bool CanContainReferences =>
            false;

        internal System.Runtime.Serialization.CreateXmlSerializableDelegate CreateXmlSerializableDelegate
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (this.helper.CreateXmlSerializableDelegate == null)
                {
                    lock (this)
                    {
                        if (this.helper.CreateXmlSerializableDelegate == null)
                        {
                            System.Runtime.Serialization.CreateXmlSerializableDelegate delegate2 = this.GenerateCreateXmlSerializableDelegate();
                            Thread.MemoryBarrier();
                            this.helper.CreateXmlSerializableDelegate = delegate2;
                        }
                    }
                }
                return this.helper.CreateXmlSerializableDelegate;
            }
        }

        internal override bool HasRoot
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get => 
                this.helper.HasRoot;
            [SecurityCritical]
            set
            {
                this.helper.HasRoot = value;
            }
        }

        internal bool IsAnonymous =>
            this.helper.IsAnonymous;

        internal override bool IsBuiltInDataContract
        {
            get
            {
                if (base.UnderlyingType != Globals.TypeOfXmlElement)
                {
                    return (base.UnderlyingType == Globals.TypeOfXmlNodeArray);
                }
                return true;
            }
        }

        internal bool IsTopLevelElementNullable
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get => 
                this.helper.IsTopLevelElementNullable;
            [SecurityCritical]
            set
            {
                this.helper.IsTopLevelElementNullable = value;
            }
        }

        internal bool IsTypeDefinedOnImport
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get => 
                this.helper.IsTypeDefinedOnImport;
            [SecurityCritical]
            set
            {
                this.helper.IsTypeDefinedOnImport = value;
            }
        }

        internal override Dictionary<XmlQualifiedName, DataContract> KnownDataContracts
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get => 
                this.helper.KnownDataContracts;
            [SecurityCritical]
            set
            {
                this.helper.KnownDataContracts = value;
            }
        }

        internal override XmlDictionaryString TopLevelElementName
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get => 
                this.helper.TopLevelElementName;
            [SecurityCritical]
            set
            {
                this.helper.TopLevelElementName = value;
            }
        }

        internal override XmlDictionaryString TopLevelElementNamespace
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get => 
                this.helper.TopLevelElementNamespace;
            [SecurityCritical]
            set
            {
                this.helper.TopLevelElementNamespace = value;
            }
        }

        internal XmlSchemaType XsdType
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get => 
                this.helper.XsdType;
            [SecurityCritical]
            set
            {
                this.helper.XsdType = value;
            }
        }

        [SecurityCritical(SecurityCriticalScope.Everything)]
        private class XmlDataContractCriticalHelper : DataContract.DataContractCriticalHelper
        {
            private System.Runtime.Serialization.CreateXmlSerializableDelegate createXmlSerializable;
            private bool hasRoot;
            private bool isKnownTypeAttributeChecked;
            private bool isTopLevelElementNullable;
            private bool isTypeDefinedOnImport;
            private Dictionary<XmlQualifiedName, DataContract> knownDataContracts;
            private XmlDictionaryString topLevelElementName;
            private XmlDictionaryString topLevelElementNamespace;
            private XmlSchemaType xsdType;

            internal XmlDataContractCriticalHelper()
            {
            }

            internal XmlDataContractCriticalHelper(Type type) : base(type)
            {
                XmlSchemaType type2;
                bool flag;
                XmlQualifiedName name;
                if (type.IsDefined(Globals.TypeOfDataContractAttribute, false))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("IXmlSerializableCannotHaveDataContract", new object[] { DataContract.GetClrTypeFullName(type) })));
                }
                if (type.IsDefined(Globals.TypeOfCollectionDataContractAttribute, false))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("IXmlSerializableCannotHaveCollectionDataContract", new object[] { DataContract.GetClrTypeFullName(type) })));
                }
                SchemaExporter.GetXmlTypeInfo(type, out name, out type2, out flag);
                base.StableName = name;
                this.XsdType = type2;
                this.HasRoot = flag;
                XmlDictionary dictionary = new XmlDictionary();
                base.Name = dictionary.Add(base.StableName.Name);
                base.Namespace = dictionary.Add(base.StableName.Namespace);
                object[] customAttributes = base.UnderlyingType?.GetCustomAttributes(Globals.TypeOfXmlRootAttribute, false);
                if ((customAttributes == null) || (customAttributes.Length == 0))
                {
                    if (flag)
                    {
                        this.topLevelElementName = base.Name;
                        this.topLevelElementNamespace = (base.StableName.Namespace == "http://www.w3.org/2001/XMLSchema") ? DictionaryGlobals.EmptyString : base.Namespace;
                        this.isTopLevelElementNullable = true;
                    }
                }
                else
                {
                    if (!flag)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(System.Runtime.Serialization.SR.GetString("IsAnyCannotHaveXmlRoot", new object[] { DataContract.GetClrTypeFullName(base.UnderlyingType) })));
                    }
                    XmlRootAttribute attribute = (XmlRootAttribute) customAttributes[0];
                    this.isTopLevelElementNullable = attribute.IsNullable;
                    string elementName = attribute.ElementName;
                    this.topLevelElementName = ((elementName == null) || (elementName.Length == 0)) ? base.Name : dictionary.Add(DataContract.EncodeLocalName(elementName));
                    string str2 = attribute.Namespace;
                    this.topLevelElementNamespace = ((str2 == null) || (str2.Length == 0)) ? DictionaryGlobals.EmptyString : dictionary.Add(str2);
                }
            }

            internal System.Runtime.Serialization.CreateXmlSerializableDelegate CreateXmlSerializableDelegate
            {
                get => 
                    this.createXmlSerializable;
                set
                {
                    this.createXmlSerializable = value;
                }
            }

            internal override bool HasRoot
            {
                get => 
                    this.hasRoot;
                set
                {
                    this.hasRoot = value;
                }
            }

            internal bool IsAnonymous =>
                (this.xsdType != null);

            internal bool IsTopLevelElementNullable
            {
                get => 
                    this.isTopLevelElementNullable;
                set
                {
                    this.isTopLevelElementNullable = value;
                }
            }

            internal bool IsTypeDefinedOnImport
            {
                get => 
                    this.isTypeDefinedOnImport;
                set
                {
                    this.isTypeDefinedOnImport = value;
                }
            }

            internal override Dictionary<XmlQualifiedName, DataContract> KnownDataContracts
            {
                get
                {
                    if (!this.isKnownTypeAttributeChecked && (base.UnderlyingType != null))
                    {
                        lock (this)
                        {
                            if (!this.isKnownTypeAttributeChecked)
                            {
                                this.knownDataContracts = DataContract.ImportKnownTypeAttributes(base.UnderlyingType);
                                Thread.MemoryBarrier();
                                this.isKnownTypeAttributeChecked = true;
                            }
                        }
                    }
                    return this.knownDataContracts;
                }
                set
                {
                    this.knownDataContracts = value;
                }
            }

            internal override XmlDictionaryString TopLevelElementName
            {
                get => 
                    this.topLevelElementName;
                set
                {
                    this.topLevelElementName = value;
                }
            }

            internal override XmlDictionaryString TopLevelElementNamespace
            {
                get => 
                    this.topLevelElementNamespace;
                set
                {
                    this.topLevelElementNamespace = value;
                }
            }

            internal XmlSchemaType XsdType
            {
                get => 
                    this.xsdType;
                set
                {
                    this.xsdType = value;
                }
            }
        }
    }
}

