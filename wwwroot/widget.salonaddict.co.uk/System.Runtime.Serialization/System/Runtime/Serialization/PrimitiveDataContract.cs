﻿namespace System.Runtime.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Security;
    using System.Xml;

    internal abstract class PrimitiveDataContract : DataContract
    {
        [SecurityCritical]
        private PrimitiveDataContractCriticalHelper helper;

        [SecurityCritical, SecurityTreatAsSafe]
        protected PrimitiveDataContract(Type type, XmlDictionaryString name, XmlDictionaryString ns) : base(new PrimitiveDataContractCriticalHelper(type, name, ns))
        {
            this.helper = base.Helper as PrimitiveDataContractCriticalHelper;
        }

        internal override bool Equals(object other, Dictionary<DataContractPairKey, object> checkedContracts)
        {
            if (!(other is PrimitiveDataContract))
            {
                return false;
            }
            Type c = base.GetType();
            Type type = other.GetType();
            if (!c.Equals(type) && !c.IsSubclassOf(type))
            {
                return type.IsSubclassOf(c);
            }
            return true;
        }

        internal static PrimitiveDataContract GetPrimitiveDataContract(Type type) => 
            (DataContract.GetBuiltInDataContract(type) as PrimitiveDataContract);

        internal static PrimitiveDataContract GetPrimitiveDataContract(string name, string ns) => 
            (DataContract.GetBuiltInDataContract(name, ns) as PrimitiveDataContract);

        protected object HandleReadValue(object obj, XmlObjectSerializerReadContext context)
        {
            context.AddNewObject(obj);
            return obj;
        }

        protected bool TryReadNullAtTopLevel(XmlReaderDelegator reader)
        {
            System.Runtime.Serialization.Attributes attributes = new System.Runtime.Serialization.Attributes();
            attributes.Read(reader);
            if (attributes.Ref != Globals.NewObjectId)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(System.Runtime.Serialization.SR.GetString("CannotDeserializeRefAtTopLevel", new object[] { attributes.Ref })));
            }
            if (attributes.XsiNil)
            {
                reader.Skip();
                return true;
            }
            return false;
        }

        public override void WriteXmlValue(XmlWriterDelegator xmlWriter, object obj, XmlObjectSerializerWriteContext context)
        {
            xmlWriter.WriteAnyType(obj);
        }

        internal override bool CanContainReferences =>
            false;

        internal override bool IsBuiltInDataContract =>
            true;

        internal override bool IsPrimitive =>
            true;

        internal abstract string ReadMethodName { get; }

        internal override XmlDictionaryString TopLevelElementNamespace
        {
            get => 
                DictionaryGlobals.SerializationNamespace;
            set
            {
            }
        }

        internal abstract string WriteMethodName { get; }

        internal MethodInfo XmlFormatContentWriterMethod
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (this.helper.XmlFormatContentWriterMethod == null)
                {
                    if (base.UnderlyingType.IsValueType)
                    {
                        this.helper.XmlFormatContentWriterMethod = typeof(XmlWriterDelegator).GetMethod(this.WriteMethodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, new Type[] { base.UnderlyingType }, null);
                    }
                    else
                    {
                        this.helper.XmlFormatContentWriterMethod = typeof(XmlObjectSerializerWriteContext).GetMethod(this.WriteMethodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, new Type[] { typeof(XmlWriterDelegator), base.UnderlyingType }, null);
                    }
                }
                return this.helper.XmlFormatContentWriterMethod;
            }
        }

        internal MethodInfo XmlFormatReaderMethod
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (this.helper.XmlFormatReaderMethod == null)
                {
                    this.helper.XmlFormatReaderMethod = typeof(XmlReaderDelegator).GetMethod(this.ReadMethodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                }
                return this.helper.XmlFormatReaderMethod;
            }
        }

        internal MethodInfo XmlFormatWriterMethod
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (this.helper.XmlFormatWriterMethod == null)
                {
                    if (base.UnderlyingType.IsValueType)
                    {
                        this.helper.XmlFormatWriterMethod = typeof(XmlWriterDelegator).GetMethod(this.WriteMethodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, new Type[] { base.UnderlyingType, typeof(XmlDictionaryString), typeof(XmlDictionaryString) }, null);
                    }
                    else
                    {
                        this.helper.XmlFormatWriterMethod = typeof(XmlObjectSerializerWriteContext).GetMethod(this.WriteMethodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance, null, new Type[] { typeof(XmlWriterDelegator), base.UnderlyingType, typeof(XmlDictionaryString), typeof(XmlDictionaryString) }, null);
                    }
                }
                return this.helper.XmlFormatWriterMethod;
            }
        }

        [SecurityCritical(SecurityCriticalScope.Everything)]
        private class PrimitiveDataContractCriticalHelper : DataContract.DataContractCriticalHelper
        {
            private MethodInfo xmlFormatContentWriterMethod;
            private MethodInfo xmlFormatReaderMethod;
            private MethodInfo xmlFormatWriterMethod;

            internal PrimitiveDataContractCriticalHelper(Type type, XmlDictionaryString name, XmlDictionaryString ns) : base(type)
            {
                base.SetDataContractName(name, ns);
            }

            internal MethodInfo XmlFormatContentWriterMethod
            {
                get => 
                    this.xmlFormatContentWriterMethod;
                set
                {
                    this.xmlFormatContentWriterMethod = value;
                }
            }

            internal MethodInfo XmlFormatReaderMethod
            {
                get => 
                    this.xmlFormatReaderMethod;
                set
                {
                    this.xmlFormatReaderMethod = value;
                }
            }

            internal MethodInfo XmlFormatWriterMethod
            {
                get => 
                    this.xmlFormatWriterMethod;
                set
                {
                    this.xmlFormatWriterMethod = value;
                }
            }
        }
    }
}

