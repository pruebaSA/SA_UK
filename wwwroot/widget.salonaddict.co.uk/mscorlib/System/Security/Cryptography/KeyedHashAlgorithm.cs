namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;

    [ComVisible(true)]
    public abstract class KeyedHashAlgorithm : HashAlgorithm
    {
        protected byte[] KeyValue;

        protected KeyedHashAlgorithm()
        {
        }

        public static KeyedHashAlgorithm Create() => 
            Create("System.Security.Cryptography.KeyedHashAlgorithm");

        public static KeyedHashAlgorithm Create(string algName) => 
            ((KeyedHashAlgorithm) CryptoConfig.CreateFromName(algName));

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.KeyValue != null)
                {
                    Array.Clear(this.KeyValue, 0, this.KeyValue.Length);
                }
                this.KeyValue = null;
            }
            base.Dispose(disposing);
        }

        public virtual byte[] Key
        {
            get => 
                ((byte[]) this.KeyValue.Clone());
            set
            {
                if (base.State != 0)
                {
                    throw new CryptographicException(Environment.GetResourceString("Cryptography_HashKeySet"));
                }
                this.KeyValue = (byte[]) value.Clone();
            }
        }
    }
}

