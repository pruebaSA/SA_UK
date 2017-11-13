namespace System.Security.Cryptography
{
    using System;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public abstract class ECDsa : AsymmetricAlgorithm
    {
        protected ECDsa()
        {
        }

        public static ECDsa Create() => 
            Create(typeof(ECDsaCng).FullName);

        public static ECDsa Create(string algorithm)
        {
            if (algorithm == null)
            {
                throw new ArgumentNullException("algorithm");
            }
            return CoreCryptoConfig.CreateFromName<ECDsa>(algorithm);
        }

        public abstract byte[] SignHash(byte[] hash);
        public abstract bool VerifyHash(byte[] hash, byte[] signature);

        public override string KeyExchangeAlgorithm =>
            null;

        public override string SignatureAlgorithm =>
            "ECDsa";
    }
}

