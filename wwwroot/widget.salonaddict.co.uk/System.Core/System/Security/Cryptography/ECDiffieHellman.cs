namespace System.Security.Cryptography
{
    using System;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public abstract class ECDiffieHellman : AsymmetricAlgorithm
    {
        protected ECDiffieHellman()
        {
        }

        public static ECDiffieHellman Create() => 
            Create(typeof(ECDiffieHellmanCng).FullName);

        public static ECDiffieHellman Create(string algorithm)
        {
            if (algorithm == null)
            {
                throw new ArgumentNullException("algorithm");
            }
            return CoreCryptoConfig.CreateFromName<ECDiffieHellman>(algorithm);
        }

        public abstract byte[] DeriveKeyMaterial(ECDiffieHellmanPublicKey otherPartyPublicKey);

        public override string KeyExchangeAlgorithm =>
            "ECDiffieHellman";

        public abstract ECDiffieHellmanPublicKey PublicKey { get; }

        public override string SignatureAlgorithm =>
            null;
    }
}

