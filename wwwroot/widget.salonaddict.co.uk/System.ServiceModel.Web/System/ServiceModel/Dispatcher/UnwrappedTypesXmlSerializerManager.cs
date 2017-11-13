namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.Xml;
    using System.Xml.Serialization;

    internal class UnwrappedTypesXmlSerializerManager
    {
        private Dictionary<Type, XmlTypeMapping> allTypes = new Dictionary<Type, XmlTypeMapping>();
        private XmlReflectionImporter importer = new XmlReflectionImporter();
        private Dictionary<object, IList<Type>> operationTypes = new Dictionary<object, IList<Type>>();
        private bool serializersCreated;
        private Dictionary<Type, XmlSerializer> serializersMap = new Dictionary<Type, XmlSerializer>();
        private object thisLock = new object();

        private void BuildSerializers()
        {
            List<Type> list = new List<Type>();
            List<XmlMapping> list2 = new List<XmlMapping>();
            foreach (Type type in this.allTypes.Keys)
            {
                XmlTypeMapping item = this.allTypes[type];
                list.Add(type);
                list2.Add(item);
            }
            XmlSerializer[] serializerArray = XmlSerializer.FromMappings(list2.ToArray());
            for (int i = 0; i < list.Count; i++)
            {
                this.serializersMap.Add(list[i], serializerArray[i]);
            }
        }

        public TypeSerializerPair[] GetOperationSerializers(object key)
        {
            if (key == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("key");
            }
            lock (this.thisLock)
            {
                if (!this.serializersCreated)
                {
                    this.BuildSerializers();
                    this.serializersCreated = true;
                }
                List<TypeSerializerPair> list = new List<TypeSerializerPair>();
                IList<Type> list2 = this.operationTypes[key];
                for (int i = 0; i < list2.Count; i++)
                {
                    TypeSerializerPair item = new TypeSerializerPair {
                        Type = list2[i],
                        Serializer = new XmlSerializerXmlObjectSerializer(this.serializersMap[list2[i]])
                    };
                    list.Add(item);
                }
                return list.ToArray();
            }
        }

        public void RegisterType(object key, IList<Type> types)
        {
            if (key == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("key");
            }
            if (types == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("types");
            }
            lock (this.thisLock)
            {
                if (this.serializersCreated)
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.XmlSerializersCreatedBeforeRegistration, new object[0])));
                }
                for (int i = 0; i < types.Count; i++)
                {
                    if (!this.allTypes.ContainsKey(types[i]))
                    {
                        this.allTypes.Add(types[i], this.importer.ImportTypeMapping(types[i]));
                    }
                }
                this.operationTypes.Add(key, types);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TypeSerializerPair
        {
            public XmlObjectSerializer Serializer;
            public System.Type Type;
        }

        private class XmlSerializerXmlObjectSerializer : XmlObjectSerializer
        {
            private XmlSerializer serializer;

            public XmlSerializerXmlObjectSerializer(XmlSerializer serializer)
            {
                if (serializer == null)
                {
                    throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("serializer");
                }
                this.serializer = serializer;
            }

            public override bool IsStartObject(XmlDictionaryReader reader) => 
                this.serializer.CanDeserialize(reader);

            public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName) => 
                this.serializer.Deserialize(reader);

            public override void WriteEndObject(XmlDictionaryWriter writer)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }

            public override void WriteObject(XmlDictionaryWriter writer, object graph)
            {
                this.serializer.Serialize((XmlWriter) writer, graph);
            }

            public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }

            public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }
        }
    }
}

