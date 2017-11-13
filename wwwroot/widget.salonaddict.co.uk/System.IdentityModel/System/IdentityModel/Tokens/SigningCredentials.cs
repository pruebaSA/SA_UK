namespace System.IdentityModel.Tokens
{
    using System;
    using System.IdentityModel;

    public class SigningCredentials
    {
        private string digestAlgorithm;
        private string signatureAlgorithm;
        private SecurityKey signingKey;
        private SecurityKeyIdentifier signingKeyIdentifier;

        public SigningCredentials(SecurityKey signingKey, string signatureAlgorithm, string digestAlgorithm) : this(signingKey, signatureAlgorithm, digestAlgorithm, null)
        {
        }

        public SigningCredentials(SecurityKey signingKey, string signatureAlgorithm, string digestAlgorithm, SecurityKeyIdentifier signingKeyIdentifier)
        {
            if (signingKey == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("signingKey"));
            }
            if (signatureAlgorithm == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("signatureAlgorithm"));
            }
            if (digestAlgorithm == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("digestAlgorithm"));
            }
            this.signingKey = signingKey;
            this.signatureAlgorithm = signatureAlgorithm;
            this.digestAlgorithm = digestAlgorithm;
            this.signingKeyIdentifier = signingKeyIdentifier;
        }

        public string DigestAlgorithm =>
            this.digestAlgorithm;

        public string SignatureAlgorithm =>
            this.signatureAlgorithm;

        public SecurityKey SigningKey =>
            this.signingKey;

        public SecurityKeyIdentifier SigningKeyIdentifier =>
            this.signingKeyIdentifier;
    }
}

