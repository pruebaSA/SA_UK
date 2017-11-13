namespace System.DirectoryServices.Protocols
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Authentication;

    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
    public class SecurityPackageContextConnectionInformation
    {
        private SecurityProtocol securityProtocol;
        private CipherAlgorithmType identifier;
        private int strength;
        private HashAlgorithmType hashAlgorithm;
        private int hashStrength;
        private int keyExchangeAlgorithm;
        private int exchangeStrength;
        internal SecurityPackageContextConnectionInformation()
        {
        }

        public SecurityProtocol Protocol =>
            this.securityProtocol;
        public CipherAlgorithmType AlgorithmIdentifier =>
            this.identifier;
        public int CipherStrength =>
            this.strength;
        public HashAlgorithmType Hash =>
            this.hashAlgorithm;
        public int HashStrength =>
            this.hashStrength;
        public int KeyExchangeAlgorithm =>
            this.keyExchangeAlgorithm;
        public int ExchangeStrength =>
            this.exchangeStrength;
    }
}

