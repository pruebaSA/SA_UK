namespace System.IdentityModel.Tokens
{
    using System;
    using System.IdentityModel;

    public abstract class SecurityKeyIdentifierClause
    {
        private readonly string clauseType;
        private int derivationLength;
        private byte[] derivationNonce;
        private string id;

        protected SecurityKeyIdentifierClause(string clauseType) : this(clauseType, null, 0)
        {
        }

        protected SecurityKeyIdentifierClause(string clauseType, byte[] nonce, int length)
        {
            this.clauseType = clauseType;
            this.derivationNonce = nonce;
            this.derivationLength = length;
        }

        public virtual SecurityKey CreateKey()
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.IdentityModel.SR.GetString("KeyIdentifierClauseDoesNotSupportKeyCreation")));
        }

        public byte[] GetDerivationNonce()
        {
            if (this.derivationNonce == null)
            {
                return null;
            }
            return (byte[]) this.derivationNonce.Clone();
        }

        public virtual bool Matches(SecurityKeyIdentifierClause keyIdentifierClause) => 
            object.ReferenceEquals(this, keyIdentifierClause);

        public virtual bool CanCreateKey =>
            false;

        public string ClauseType =>
            this.clauseType;

        public int DerivationLength =>
            this.derivationLength;

        public string Id
        {
            get => 
                this.id;
            set
            {
                this.id = value;
            }
        }
    }
}

