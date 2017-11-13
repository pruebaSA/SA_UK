namespace System.Messaging
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Serialization.Formatters.Binary;

    public class BinaryMessageFormatter : IMessageFormatter, ICloneable
    {
        private BinaryFormatter formatter;
        internal const short VT_BINARY_OBJECT = 0x300;

        public BinaryMessageFormatter()
        {
            this.formatter = new BinaryFormatter();
        }

        public BinaryMessageFormatter(FormatterAssemblyStyle topObjectFormat, FormatterTypeStyle typeFormat)
        {
            this.formatter = new BinaryFormatter();
            this.formatter.AssemblyFormat = topObjectFormat;
            this.formatter.TypeFormat = typeFormat;
        }

        public bool CanRead(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (message.BodyType != 0x300)
            {
                return false;
            }
            return true;
        }

        public object Clone() => 
            new BinaryMessageFormatter(this.TopObjectFormat, this.TypeFormat);

        public object Read(Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (message.BodyType != 0x300)
            {
                throw new InvalidOperationException(Res.GetString("InvalidTypeDeserialization"));
            }
            Stream bodyStream = message.BodyStream;
            return this.formatter.Deserialize(bodyStream);
        }

        public void Write(Message message, object obj)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            Stream serializationStream = new MemoryStream();
            this.formatter.Serialize(serializationStream, obj);
            message.BodyType = 0x300;
            message.BodyStream = serializationStream;
        }

        [DefaultValue(0), MessagingDescription("MsgTopObjectFormat")]
        public FormatterAssemblyStyle TopObjectFormat
        {
            get => 
                this.formatter.AssemblyFormat;
            set
            {
                this.formatter.AssemblyFormat = value;
            }
        }

        [MessagingDescription("MsgTypeFormat"), DefaultValue(0)]
        public FormatterTypeStyle TypeFormat
        {
            get => 
                this.formatter.TypeFormat;
            set
            {
                this.formatter.TypeFormat = value;
            }
        }
    }
}

