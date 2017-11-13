namespace System.Runtime.Serialization.Formatters.Binary
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Security.Permissions;

    [ComVisible(true)]
    public sealed class BinaryFormatter : IRemotingFormatter, IFormatter
    {
        internal FormatterAssemblyStyle m_assemblyFormat;
        internal SerializationBinder m_binder;
        internal StreamingContext m_context;
        internal object[] m_crossAppDomainArray;
        internal TypeFilterLevel m_securityLevel;
        internal ISurrogateSelector m_surrogates;
        internal FormatterTypeStyle m_typeFormat;

        public BinaryFormatter()
        {
            this.m_typeFormat = FormatterTypeStyle.TypesAlways;
            this.m_securityLevel = TypeFilterLevel.Full;
            this.m_surrogates = null;
            this.m_context = new StreamingContext(StreamingContextStates.All);
        }

        public BinaryFormatter(ISurrogateSelector selector, StreamingContext context)
        {
            this.m_typeFormat = FormatterTypeStyle.TypesAlways;
            this.m_securityLevel = TypeFilterLevel.Full;
            this.m_surrogates = selector;
            this.m_context = context;
        }

        public object Deserialize(Stream serializationStream) => 
            this.Deserialize(serializationStream, null);

        public object Deserialize(Stream serializationStream, HeaderHandler handler) => 
            this.Deserialize(serializationStream, handler, true, null);

        internal object Deserialize(Stream serializationStream, HeaderHandler handler, bool fCheck) => 
            this.Deserialize(serializationStream, null, fCheck, null);

        internal object Deserialize(Stream serializationStream, HeaderHandler handler, bool fCheck, IMethodCallMessage methodCallMessage) => 
            this.Deserialize(serializationStream, handler, fCheck, false, methodCallMessage);

        internal object Deserialize(Stream serializationStream, HeaderHandler handler, bool fCheck, bool isCrossAppDomain, IMethodCallMessage methodCallMessage)
        {
            if (serializationStream == null)
            {
                throw new ArgumentNullException("serializationStream", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentNull_WithParamName"), new object[] { serializationStream }));
            }
            if (serializationStream.CanSeek && (serializationStream.Length == 0L))
            {
                throw new SerializationException(Environment.GetResourceString("Serialization_Stream"));
            }
            InternalFE formatterEnums = new InternalFE {
                FEtypeFormat = this.m_typeFormat,
                FEserializerTypeEnum = InternalSerializerTypeE.Binary,
                FEassemblyFormat = this.m_assemblyFormat,
                FEsecurityLevel = this.m_securityLevel
            };
            ObjectReader objectReader = new ObjectReader(serializationStream, this.m_surrogates, this.m_context, formatterEnums, this.m_binder) {
                crossAppDomainArray = this.m_crossAppDomainArray
            };
            return objectReader.Deserialize(handler, new __BinaryParser(serializationStream, objectReader), fCheck, isCrossAppDomain, methodCallMessage);
        }

        public object DeserializeMethodResponse(Stream serializationStream, HeaderHandler handler, IMethodCallMessage methodCallMessage) => 
            this.Deserialize(serializationStream, handler, true, methodCallMessage);

        public void Serialize(Stream serializationStream, object graph)
        {
            this.Serialize(serializationStream, graph, null);
        }

        public void Serialize(Stream serializationStream, object graph, Header[] headers)
        {
            this.Serialize(serializationStream, graph, headers, true);
        }

        internal void Serialize(Stream serializationStream, object graph, Header[] headers, bool fCheck)
        {
            if (serializationStream == null)
            {
                throw new ArgumentNullException("serializationStream", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentNull_WithParamName"), new object[] { serializationStream }));
            }
            InternalFE formatterEnums = new InternalFE {
                FEtypeFormat = this.m_typeFormat,
                FEserializerTypeEnum = InternalSerializerTypeE.Binary,
                FEassemblyFormat = this.m_assemblyFormat
            };
            ObjectWriter objectWriter = new ObjectWriter(this.m_surrogates, this.m_context, formatterEnums);
            __BinaryWriter serWriter = new __BinaryWriter(serializationStream, objectWriter, this.m_typeFormat);
            objectWriter.Serialize(graph, headers, serWriter, fCheck);
            this.m_crossAppDomainArray = objectWriter.crossAppDomainArray;
        }

        [ComVisible(false), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public object UnsafeDeserialize(Stream serializationStream, HeaderHandler handler) => 
            this.Deserialize(serializationStream, handler, false, null);

        [ComVisible(false), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public object UnsafeDeserializeMethodResponse(Stream serializationStream, HeaderHandler handler, IMethodCallMessage methodCallMessage) => 
            this.Deserialize(serializationStream, handler, false, methodCallMessage);

        public FormatterAssemblyStyle AssemblyFormat
        {
            get => 
                this.m_assemblyFormat;
            set
            {
                this.m_assemblyFormat = value;
            }
        }

        public SerializationBinder Binder
        {
            get => 
                this.m_binder;
            set
            {
                this.m_binder = value;
            }
        }

        public StreamingContext Context
        {
            get => 
                this.m_context;
            set
            {
                this.m_context = value;
            }
        }

        public TypeFilterLevel FilterLevel
        {
            get => 
                this.m_securityLevel;
            set
            {
                this.m_securityLevel = value;
            }
        }

        public ISurrogateSelector SurrogateSelector
        {
            get => 
                this.m_surrogates;
            set
            {
                this.m_surrogates = value;
            }
        }

        public FormatterTypeStyle TypeFormat
        {
            get => 
                this.m_typeFormat;
            set
            {
                this.m_typeFormat = value;
            }
        }
    }
}

