namespace System.Security.Cryptography.Pkcs
{
    using System;
    using System.Security.Cryptography;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public abstract class RecipientInfo
    {
        private object m_cmsgRecipientInfo;
        private uint m_index;
        private System.Security.Cryptography.SafeLocalAllocHandle m_pCmsgRecipientInfo;
        private RecipientInfoType m_recipentInfoType;
        private RecipientSubType m_recipientSubType;

        internal RecipientInfo()
        {
        }

        internal RecipientInfo(RecipientInfoType recipientInfoType, RecipientSubType recipientSubType, System.Security.Cryptography.SafeLocalAllocHandle pCmsgRecipientInfo, object cmsgRecipientInfo, uint index)
        {
            if ((recipientInfoType < RecipientInfoType.Unknown) || (recipientInfoType > RecipientInfoType.KeyAgreement))
            {
                recipientInfoType = RecipientInfoType.Unknown;
            }
            if ((recipientSubType < RecipientSubType.Unknown) || (recipientSubType > RecipientSubType.PublicKeyAgreement))
            {
                recipientSubType = RecipientSubType.Unknown;
            }
            this.m_recipentInfoType = recipientInfoType;
            this.m_recipientSubType = recipientSubType;
            this.m_pCmsgRecipientInfo = pCmsgRecipientInfo;
            this.m_cmsgRecipientInfo = cmsgRecipientInfo;
            this.m_index = index;
        }

        internal object CmsgRecipientInfo =>
            this.m_cmsgRecipientInfo;

        public abstract byte[] EncryptedKey { get; }

        internal uint Index =>
            this.m_index;

        public abstract AlgorithmIdentifier KeyEncryptionAlgorithm { get; }

        internal System.Security.Cryptography.SafeLocalAllocHandle pCmsgRecipientInfo =>
            this.m_pCmsgRecipientInfo;

        public abstract SubjectIdentifier RecipientIdentifier { get; }

        internal RecipientSubType SubType =>
            this.m_recipientSubType;

        public RecipientInfoType Type =>
            this.m_recipentInfoType;

        public abstract int Version { get; }
    }
}

