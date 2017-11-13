namespace System.ServiceModel.Security
{
    using System;

    internal enum ReceiveSecurityHeaderElementCategory
    {
        Signature,
        EncryptedData,
        EncryptedKey,
        SignatureConfirmation,
        ReferenceList,
        SecurityTokenReference,
        Timestamp,
        Token
    }
}

