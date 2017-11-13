namespace System.IdentityModel
{
    using System;
    using System.Security.Cryptography;
    using System.Security.Cryptography.Xml;

    internal static class CryptoHelper
    {
        private static byte[] emptyBuffer;
        private static System.Security.Cryptography.RandomNumberGenerator random;
        private static System.Security.Cryptography.Rijndael rijndael;
        private static System.Security.Cryptography.TripleDES tripleDES;

        internal static byte[] ComputeHash(byte[] buffer)
        {
            using (HashAlgorithm algorithm = NewSha1HashAlgorithm())
            {
                return algorithm.ComputeHash(buffer);
            }
        }

        internal static ICryptoTransform CreateDecryptor(byte[] key, byte[] iv, string algorithm)
        {
            switch (algorithm)
            {
                case "http://www.w3.org/2001/04/xmlenc#tripledes-cbc":
                    return TripleDES.CreateDecryptor(key, iv);

                case "http://www.w3.org/2001/04/xmlenc#aes128-cbc":
                case "http://www.w3.org/2001/04/xmlenc#aes192-cbc":
                case "http://www.w3.org/2001/04/xmlenc#aes256-cbc":
                    return Rijndael.CreateDecryptor(key, iv);
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(System.IdentityModel.SR.GetString("UnsupportedEncryptionAlgorithm", new object[] { algorithm })));
        }

        internal static ICryptoTransform CreateEncryptor(byte[] key, byte[] iv, string algorithm)
        {
            switch (algorithm)
            {
                case "http://www.w3.org/2001/04/xmlenc#tripledes-cbc":
                    return TripleDES.CreateEncryptor(key, iv);

                case "http://www.w3.org/2001/04/xmlenc#aes128-cbc":
                case "http://www.w3.org/2001/04/xmlenc#aes192-cbc":
                case "http://www.w3.org/2001/04/xmlenc#aes256-cbc":
                    return Rijndael.CreateEncryptor(key, iv);
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(System.IdentityModel.SR.GetString("UnsupportedEncryptionAlgorithm", new object[] { algorithm })));
        }

        internal static HashAlgorithm CreateHashAlgorithm(string algorithm)
        {
            switch (algorithm)
            {
                case "http://www.w3.org/2000/09/xmldsig#sha1":
                    return NewSha1HashAlgorithm();

                case "http://www.w3.org/2001/04/xmlenc#sha256":
                    return NewSha256HashAlgorithm();
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(System.IdentityModel.SR.GetString("UnsupportedCryptoAlgorithm", new object[] { algorithm })));
        }

        internal static KeyedHashAlgorithm CreateKeyedHashAlgorithm(byte[] key, string algorithm)
        {
            switch (algorithm)
            {
                case "http://www.w3.org/2000/09/xmldsig#hmac-sha1":
                    return NewHmacSha1KeyedHashAlgorithm(key);

                case "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256":
                    return NewHmacSha256KeyedHashAlgorithm(key);
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(System.IdentityModel.SR.GetString("UnsupportedEncryptionAlgorithm", new object[] { algorithm })));
        }

        internal static byte[] GenerateDerivedKey(byte[] key, string algorithm, byte[] label, byte[] nonce, int derivedKeySize, int position)
        {
            if ((algorithm != "http://schemas.xmlsoap.org/ws/2005/02/sc/dk/p_sha1") && (algorithm != "http://docs.oasis-open.org/ws-sx/ws-secureconversation/200512/dk/p_sha1"))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(System.IdentityModel.SR.GetString("UnsupportedKeyDerivationAlgorithm", new object[] { algorithm })));
            }
            return new Psha1DerivedKeyGenerator(key).GenerateDerivedKey(label, nonce, derivedKeySize, position);
        }

        internal static int GetIVSize(string algorithm)
        {
            switch (algorithm)
            {
                case "http://www.w3.org/2001/04/xmlenc#tripledes-cbc":
                    return TripleDES.BlockSize;

                case "http://www.w3.org/2001/04/xmlenc#aes128-cbc":
                case "http://www.w3.org/2001/04/xmlenc#aes192-cbc":
                case "http://www.w3.org/2001/04/xmlenc#aes256-cbc":
                    return Rijndael.BlockSize;
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(System.IdentityModel.SR.GetString("UnsupportedEncryptionAlgorithm", new object[] { algorithm })));
        }

        internal static SymmetricAlgorithm GetSymmetricAlgorithm(byte[] key, string algorithm)
        {
            SymmetricAlgorithm algorithm2;
            switch (algorithm)
            {
                case "http://www.w3.org/2001/04/xmlenc#tripledes-cbc":
                case "http://www.w3.org/2001/04/xmlenc#kw-tripledes":
                    algorithm2 = new TripleDESCryptoServiceProvider();
                    break;

                case "http://www.w3.org/2001/04/xmlenc#aes128-cbc":
                case "http://www.w3.org/2001/04/xmlenc#aes192-cbc":
                case "http://www.w3.org/2001/04/xmlenc#aes256-cbc":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes128":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes192":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes256":
                    algorithm2 = NewRijndaelSymmetricAlgorithm();
                    break;

                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(System.IdentityModel.SR.GetString("UnsupportedEncryptionAlgorithm", new object[] { algorithm })));
            }
            algorithm2.Key = key;
            return algorithm2;
        }

        internal static bool IsAsymmetricAlgorithm(string algorithm)
        {
            string str;
            if (((str = algorithm) == null) || (((str != "http://www.w3.org/2000/09/xmldsig#dsa-sha1") && (str != "http://www.w3.org/2000/09/xmldsig#rsa-sha1")) && (((str != "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256") && (str != "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p")) && (str != "http://www.w3.org/2001/04/xmlenc#rsa-1_5"))))
            {
                return false;
            }
            return true;
        }

        internal static bool IsEqual(byte[] a, byte[] b)
        {
            if (!object.ReferenceEquals(a, b))
            {
                if (((a == null) || (b == null)) || (a.Length != b.Length))
                {
                    return false;
                }
                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i] != b[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal static bool IsSymmetricAlgorithm(string algorithm)
        {
            switch (algorithm)
            {
                case "http://www.w3.org/2000/09/xmldsig#dsa-sha1":
                case "http://www.w3.org/2000/09/xmldsig#rsa-sha1":
                case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256":
                case "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p":
                case "http://www.w3.org/2001/04/xmlenc#rsa-1_5":
                    return false;

                case "http://www.w3.org/2000/09/xmldsig#hmac-sha1":
                case "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256":
                case "http://www.w3.org/2001/04/xmlenc#aes128-cbc":
                case "http://www.w3.org/2001/04/xmlenc#aes192-cbc":
                case "http://www.w3.org/2001/04/xmlenc#aes256-cbc":
                case "http://www.w3.org/2001/04/xmlenc#tripledes-cbc":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes128":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes192":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes256":
                case "http://www.w3.org/2001/04/xmlenc#kw-tripledes":
                case "http://schemas.xmlsoap.org/ws/2005/02/sc/dk/p_sha1":
                case "http://docs.oasis-open.org/ws-sx/ws-secureconversation/200512/dk/p_sha1":
                    return true;
            }
            return false;
        }

        internal static bool IsSymmetricAlgorithm(string algorithm, int keySize)
        {
            switch (algorithm)
            {
                case "http://www.w3.org/2000/09/xmldsig#dsa-sha1":
                case "http://www.w3.org/2000/09/xmldsig#rsa-sha1":
                case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256":
                case "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p":
                case "http://www.w3.org/2001/04/xmlenc#rsa-1_5":
                    return false;

                case "http://www.w3.org/2000/09/xmldsig#hmac-sha1":
                case "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256":
                case "http://schemas.xmlsoap.org/ws/2005/02/sc/dk/p_sha1":
                case "http://docs.oasis-open.org/ws-sx/ws-secureconversation/200512/dk/p_sha1":
                    return true;

                case "http://www.w3.org/2001/04/xmlenc#aes128-cbc":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes128":
                    return (keySize == 0x80);

                case "http://www.w3.org/2001/04/xmlenc#aes192-cbc":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes192":
                    return (keySize == 0xc0);

                case "http://www.w3.org/2001/04/xmlenc#aes256-cbc":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes256":
                    return (keySize == 0x100);

                case "http://www.w3.org/2001/04/xmlenc#tripledes-cbc":
                case "http://www.w3.org/2001/04/xmlenc#kw-tripledes":
                    return ((keySize == 0x80) || (keySize == 0xc0));
            }
            return false;
        }

        internal static KeyedHashAlgorithm NewHmacSha1KeyedHashAlgorithm(byte[] key) => 
            new HMACSHA1(key, !System.IdentityModel.SecurityUtils.RequiresFipsCompliance);

        internal static KeyedHashAlgorithm NewHmacSha256KeyedHashAlgorithm(byte[] key) => 
            new HMACSHA256(key);

        internal static System.Security.Cryptography.Rijndael NewRijndaelSymmetricAlgorithm()
        {
            if (!System.IdentityModel.SecurityUtils.RequiresFipsCompliance)
            {
                return new RijndaelManaged();
            }
            return new RijndaelCryptoServiceProvider();
        }

        internal static SHA1 NewSha1HashAlgorithm()
        {
            if (System.IdentityModel.SecurityUtils.RequiresFipsCompliance)
            {
                return new SHA1CryptoServiceProvider();
            }
            return new SHA1Managed();
        }

        internal static SHA256 NewSha256HashAlgorithm() => 
            new SHA256Managed();

        internal static byte[] UnwrapKey(byte[] wrappingKey, byte[] wrappedKey, string algorithm)
        {
            SymmetricAlgorithm algorithm2;
            switch (algorithm)
            {
                case "http://www.w3.org/2001/04/xmlenc#kw-tripledes":
                    algorithm2 = new TripleDESCryptoServiceProvider();
                    break;

                case "http://www.w3.org/2001/04/xmlenc#kw-aes128":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes192":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes256":
                    algorithm2 = NewRijndaelSymmetricAlgorithm();
                    break;

                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(System.IdentityModel.SR.GetString("UnsupportedKeyWrapAlgorithm", new object[] { algorithm })));
            }
            using (algorithm2)
            {
                algorithm2.Key = wrappingKey;
                return EncryptedXml.DecryptKey(wrappedKey, algorithm2);
            }
        }

        internal static void ValidateBufferBounds(Array buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("buffer"));
            }
            if ((count < 0) || (count > buffer.Length))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", System.IdentityModel.SR.GetString("ValueMustBeInRange", new object[] { 0, buffer.Length })));
            }
            if ((offset < 0) || (offset > (buffer.Length - count)))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", System.IdentityModel.SR.GetString("ValueMustBeInRange", new object[] { 0, buffer.Length - count })));
            }
        }

        internal static byte[] WrapKey(byte[] wrappingKey, byte[] keyToBeWrapped, string algorithm)
        {
            SymmetricAlgorithm algorithm2;
            switch (algorithm)
            {
                case "http://www.w3.org/2001/04/xmlenc#kw-tripledes":
                    algorithm2 = new TripleDESCryptoServiceProvider();
                    break;

                case "http://www.w3.org/2001/04/xmlenc#kw-aes128":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes192":
                case "http://www.w3.org/2001/04/xmlenc#kw-aes256":
                    algorithm2 = NewRijndaelSymmetricAlgorithm();
                    break;

                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(System.IdentityModel.SR.GetString("UnsupportedKeyWrapAlgorithm", new object[] { algorithm })));
            }
            using (algorithm2)
            {
                algorithm2.Key = wrappingKey;
                return EncryptedXml.EncryptKey(keyToBeWrapped, algorithm2);
            }
        }

        internal static byte[] EmptyBuffer
        {
            get
            {
                if (emptyBuffer == null)
                {
                    emptyBuffer = new byte[0];
                }
                return emptyBuffer;
            }
        }

        internal static System.Security.Cryptography.RandomNumberGenerator RandomNumberGenerator
        {
            get
            {
                if (random == null)
                {
                    random = new RNGCryptoServiceProvider();
                }
                return random;
            }
        }

        private static System.Security.Cryptography.Rijndael Rijndael
        {
            get
            {
                if (CryptoHelper.rijndael == null)
                {
                    System.Security.Cryptography.Rijndael rijndael = NewRijndaelSymmetricAlgorithm();
                    rijndael.Padding = PaddingMode.ISO10126;
                    CryptoHelper.rijndael = rijndael;
                }
                return CryptoHelper.rijndael;
            }
        }

        private static System.Security.Cryptography.TripleDES TripleDES
        {
            get
            {
                if (tripleDES == null)
                {
                    TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider {
                        Padding = PaddingMode.ISO10126
                    };
                    tripleDES = provider;
                }
                return tripleDES;
            }
        }
    }
}

