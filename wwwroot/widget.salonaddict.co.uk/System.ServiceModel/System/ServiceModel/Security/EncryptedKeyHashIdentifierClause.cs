namespace System.ServiceModel.Security
{
    using System;
    using System.Globalization;
    using System.IdentityModel.Tokens;

    internal sealed class EncryptedKeyHashIdentifierClause : BinaryKeyIdentifierClause
    {
        public EncryptedKeyHashIdentifierClause(byte[] encryptedKeyHash) : this(encryptedKeyHash, true)
        {
        }

        internal EncryptedKeyHashIdentifierClause(byte[] encryptedKeyHash, bool cloneBuffer) : this(encryptedKeyHash, cloneBuffer, null, 0)
        {
        }

        internal EncryptedKeyHashIdentifierClause(byte[] encryptedKeyHash, bool cloneBuffer, byte[] derivationNonce, int derivationLength) : base(null, encryptedKeyHash, cloneBuffer, derivationNonce, derivationLength)
        {
        }

        public byte[] GetEncryptedKeyHash() => 
            base.GetBuffer();

        public override string ToString() => 
            string.Format(CultureInfo.InvariantCulture, "EncryptedKeyHashIdentifierClause(Hash = {0})", new object[] { Convert.ToBase64String(base.GetRawBuffer()) });
    }
}

