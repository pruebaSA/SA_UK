﻿namespace System.ServiceModel.Security
{
    using System;
    using System.ServiceModel;
    using System.Xml;

    internal sealed class EncryptedKey : EncryptedType
    {
        internal static readonly XmlDictionaryString CarriedKeyElementName = XD.XmlEncryptionDictionary.CarriedKeyName;
        private string carriedKeyName;
        internal static readonly XmlDictionaryString ElementName = XD.XmlEncryptionDictionary.EncryptedKey;
        private string recipient;
        internal static readonly XmlDictionaryString RecipientAttribute = XD.XmlEncryptionDictionary.Recipient;
        private System.ServiceModel.Security.ReferenceList referenceList;
        private byte[] wrappedKey;

        protected override void ForceEncryption()
        {
        }

        public byte[] GetWrappedKey()
        {
            if (base.State == EncryptedType.EncryptionState.New)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("BadEncryptionState")));
            }
            return this.wrappedKey;
        }

        protected override void ReadAdditionalAttributes(XmlDictionaryReader reader)
        {
            this.recipient = reader.GetAttribute(RecipientAttribute, null);
        }

        protected override void ReadAdditionalElements(XmlDictionaryReader reader)
        {
            if (reader.IsStartElement(System.ServiceModel.Security.ReferenceList.ElementName, EncryptedType.NamespaceUri))
            {
                this.referenceList = new System.ServiceModel.Security.ReferenceList();
                this.referenceList.ReadFrom(reader);
            }
            if (reader.IsStartElement(CarriedKeyElementName, EncryptedType.NamespaceUri))
            {
                reader.ReadStartElement(CarriedKeyElementName, EncryptedType.NamespaceUri);
                this.carriedKeyName = reader.ReadString();
                reader.ReadEndElement();
            }
        }

        protected override void ReadCipherData(XmlDictionaryReader reader)
        {
            this.wrappedKey = reader.ReadContentAsBase64();
        }

        protected override void ReadCipherData(XmlDictionaryReader reader, long maxBufferSize)
        {
            this.wrappedKey = System.ServiceModel.Security.SecurityUtils.ReadContentAsBase64(reader, maxBufferSize);
        }

        public void SetUpKeyWrap(byte[] wrappedKey)
        {
            if (base.State != EncryptedType.EncryptionState.New)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("BadEncryptionState")));
            }
            if (wrappedKey == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("wrappedKey");
            }
            this.wrappedKey = wrappedKey;
            base.State = EncryptedType.EncryptionState.Encrypted;
        }

        protected override void WriteAdditionalAttributes(XmlDictionaryWriter writer)
        {
            if (this.recipient != null)
            {
                writer.WriteAttributeString(RecipientAttribute, null, this.recipient);
            }
        }

        protected override void WriteAdditionalElements(XmlDictionaryWriter writer)
        {
            if (this.carriedKeyName != null)
            {
                writer.WriteStartElement(CarriedKeyElementName, EncryptedType.NamespaceUri);
                writer.WriteString(this.carriedKeyName);
                writer.WriteEndElement();
            }
            if (this.referenceList != null)
            {
                this.referenceList.WriteTo(writer, ServiceModelDictionaryManager.Instance);
            }
        }

        protected override void WriteCipherData(XmlDictionaryWriter writer)
        {
            writer.WriteBase64(this.wrappedKey, 0, this.wrappedKey.Length);
        }

        public string CarriedKeyName
        {
            get => 
                this.carriedKeyName;
            set
            {
                this.carriedKeyName = value;
            }
        }

        protected override XmlDictionaryString OpeningElementName =>
            ElementName;

        public string Recipient
        {
            get => 
                this.recipient;
            set
            {
                this.recipient = value;
            }
        }

        public System.ServiceModel.Security.ReferenceList ReferenceList
        {
            get => 
                this.referenceList;
            set
            {
                this.referenceList = value;
            }
        }
    }
}

