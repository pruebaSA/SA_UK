﻿namespace System.Runtime.Serialization.Formatters.Soap
{
    using System;
    using System.IO;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;

    public sealed class SoapFormatter : IRemotingFormatter, IFormatter
    {
        private Stream currentStream;
        private FormatterAssemblyStyle m_assemblyFormat;
        private SerializationBinder m_binder;
        private StreamingContext m_context;
        private TypeFilterLevel m_securityLevel;
        private ISurrogateSelector m_surrogates;
        private ISoapMessage m_topObject;
        private FormatterTypeStyle m_typeFormat;
        private SoapParser soapParser;

        public SoapFormatter()
        {
            this.m_assemblyFormat = FormatterAssemblyStyle.Full;
            this.m_securityLevel = TypeFilterLevel.Full;
            this.m_surrogates = null;
            this.m_context = new StreamingContext(StreamingContextStates.All);
        }

        public SoapFormatter(ISurrogateSelector selector, StreamingContext context)
        {
            this.m_assemblyFormat = FormatterAssemblyStyle.Full;
            this.m_securityLevel = TypeFilterLevel.Full;
            this.m_surrogates = selector;
            this.m_context = context;
        }

        public object Deserialize(Stream serializationStream) => 
            this.Deserialize(serializationStream, null);

        public object Deserialize(Stream serializationStream, HeaderHandler handler)
        {
            if (serializationStream == null)
            {
                throw new ArgumentNullException("serializationStream");
            }
            if (serializationStream.CanSeek && (serializationStream.Length == 0L))
            {
                throw new SerializationException(SoapUtil.GetResourceString("Serialization_Stream"));
            }
            InternalFE formatterEnums = new InternalFE {
                FEtypeFormat = this.m_typeFormat,
                FEtopObject = this.m_topObject,
                FEserializerTypeEnum = InternalSerializerTypeE.Soap,
                FEassemblyFormat = this.m_assemblyFormat,
                FEsecurityLevel = this.m_securityLevel
            };
            ObjectReader objectReader = new ObjectReader(serializationStream, this.m_surrogates, this.m_context, formatterEnums, this.m_binder);
            if ((this.soapParser == null) || (serializationStream != this.currentStream))
            {
                this.soapParser = new SoapParser(serializationStream);
                this.currentStream = serializationStream;
            }
            this.soapParser.Init(objectReader);
            return objectReader.Deserialize(handler, this.soapParser);
        }

        public void Serialize(Stream serializationStream, object graph)
        {
            this.Serialize(serializationStream, graph, null);
        }

        public void Serialize(Stream serializationStream, object graph, Header[] headers)
        {
            if (serializationStream == null)
            {
                throw new ArgumentNullException("serializationStream");
            }
            InternalFE formatterEnums = new InternalFE {
                FEtypeFormat = this.m_typeFormat,
                FEtopObject = this.m_topObject,
                FEserializerTypeEnum = InternalSerializerTypeE.Soap,
                FEassemblyFormat = this.m_assemblyFormat
            };
            new ObjectWriter(serializationStream, this.m_surrogates, this.m_context, formatterEnums).Serialize(graph, headers, new SoapWriter(serializationStream));
        }

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

        public ISoapMessage TopObject
        {
            get => 
                this.m_topObject;
            set
            {
                this.m_topObject = value;
            }
        }

        public FormatterTypeStyle TypeFormat
        {
            get => 
                this.m_typeFormat;
            set
            {
                if (value == FormatterTypeStyle.TypesWhenNeeded)
                {
                    this.m_typeFormat = FormatterTypeStyle.TypesWhenNeeded;
                }
                else
                {
                    this.m_typeFormat |= value;
                }
            }
        }
    }
}

