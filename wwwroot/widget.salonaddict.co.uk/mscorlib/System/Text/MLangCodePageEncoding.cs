﻿namespace System.Text
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable]
    internal sealed class MLangCodePageEncoding : ISerializable, IObjectReference
    {
        [NonSerialized]
        private DecoderFallback decoderFallback;
        [NonSerialized]
        private EncoderFallback encoderFallback;
        [NonSerialized]
        private int m_codePage;
        [NonSerialized]
        private bool m_deserializedFromEverett;
        [NonSerialized]
        private bool m_isReadOnly;
        [NonSerialized]
        private Encoding realEncoding;

        internal MLangCodePageEncoding(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            this.m_codePage = (int) info.GetValue("m_codePage", typeof(int));
            try
            {
                this.m_isReadOnly = (bool) info.GetValue("m_isReadOnly", typeof(bool));
                this.encoderFallback = (EncoderFallback) info.GetValue("encoderFallback", typeof(EncoderFallback));
                this.decoderFallback = (DecoderFallback) info.GetValue("decoderFallback", typeof(DecoderFallback));
            }
            catch (SerializationException)
            {
                this.m_deserializedFromEverett = true;
                this.m_isReadOnly = true;
            }
        }

        public object GetRealObject(StreamingContext context)
        {
            this.realEncoding = Encoding.GetEncoding(this.m_codePage);
            if (!this.m_deserializedFromEverett && !this.m_isReadOnly)
            {
                this.realEncoding = (Encoding) this.realEncoding.Clone();
                this.realEncoding.EncoderFallback = this.encoderFallback;
                this.realEncoding.DecoderFallback = this.decoderFallback;
            }
            return this.realEncoding;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new ArgumentException(Environment.GetResourceString("Arg_ExecutionEngineException"));
        }

        [Serializable]
        internal sealed class MLangDecoder : ISerializable, IObjectReference
        {
            [NonSerialized]
            private Encoding realEncoding;

            internal MLangDecoder(SerializationInfo info, StreamingContext context)
            {
                if (info == null)
                {
                    throw new ArgumentNullException("info");
                }
                this.realEncoding = (Encoding) info.GetValue("m_encoding", typeof(Encoding));
            }

            public object GetRealObject(StreamingContext context) => 
                this.realEncoding.GetDecoder();

            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_ExecutionEngineException"));
            }
        }

        [Serializable]
        internal sealed class MLangEncoder : ISerializable, IObjectReference
        {
            [NonSerialized]
            private Encoding realEncoding;

            internal MLangEncoder(SerializationInfo info, StreamingContext context)
            {
                if (info == null)
                {
                    throw new ArgumentNullException("info");
                }
                this.realEncoding = (Encoding) info.GetValue("m_encoding", typeof(Encoding));
            }

            public object GetRealObject(StreamingContext context) => 
                this.realEncoding.GetEncoder();

            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_ExecutionEngineException"));
            }
        }
    }
}

