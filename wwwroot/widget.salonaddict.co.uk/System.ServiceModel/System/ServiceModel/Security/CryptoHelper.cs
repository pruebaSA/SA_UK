namespace System.ServiceModel.Security
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.ServiceModel;

    internal static class CryptoHelper
    {
        private static byte[] emptyBuffer;
        private static readonly RandomNumberGenerator random = new RNGCryptoServiceProvider();

        internal static HashAlgorithm CreateHashAlgorithm(string digestMethod)
        {
            switch (digestMethod)
            {
                case "http://www.w3.org/2000/09/xmldsig#sha1":
                    return NewSha1HashAlgorithm();

                case "http://www.w3.org/2001/04/xmlenc#sha256":
                    return NewSha256HashAlgorithm();
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(System.ServiceModel.SR.GetString("UnsupportedCryptoAlgorithm", new object[] { digestMethod })));
        }

        internal static HashAlgorithm CreateHashForAsymmetricSignature(string signatureMethod)
        {
            switch (signatureMethod)
            {
                case "http://www.w3.org/2000/09/xmldsig#rsa-sha1":
                case "http://www.w3.org/2000/09/xmldsig#dsa-sha1":
                    return NewSha1HashAlgorithm();

                case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256":
                    return NewSha256HashAlgorithm();
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(System.ServiceModel.SR.GetString("UnsupportedCryptoAlgorithm", new object[] { signatureMethod })));
        }

        internal static byte[] ExtractIVAndDecrypt(SymmetricAlgorithm algorithm, byte[] cipherText, int offset, int count)
        {
            byte[] buffer2;
            if (cipherText == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("cipherText");
            }
            if ((count < 0) || (count > cipherText.Length))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", System.ServiceModel.SR.GetString("ValueMustBeInRange", new object[] { 0, cipherText.Length })));
            }
            if ((offset < 0) || (offset > (cipherText.Length - count)))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", System.ServiceModel.SR.GetString("ValueMustBeInRange", new object[] { 0, cipherText.Length - count })));
            }
            int num = algorithm.BlockSize / 8;
            byte[] dst = new byte[num];
            Buffer.BlockCopy(cipherText, offset, dst, 0, dst.Length);
            algorithm.Padding = PaddingMode.ISO10126;
            algorithm.Mode = CipherMode.CBC;
            try
            {
                using (ICryptoTransform transform = algorithm.CreateDecryptor(algorithm.Key, dst))
                {
                    buffer2 = transform.TransformFinalBlock(cipherText, offset + dst.Length, count - dst.Length);
                }
            }
            catch (CryptographicException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(System.ServiceModel.SR.GetString("DecryptionFailed"), exception));
            }
            return buffer2;
        }

        internal static void FillRandomBytes(byte[] buffer)
        {
            random.GetBytes(buffer);
        }

        internal static void GenerateIVAndEncrypt(SymmetricAlgorithm algorithm, ArraySegment<byte> plainText, out byte[] iv, out byte[] cipherText)
        {
            int num = algorithm.BlockSize / 8;
            iv = new byte[num];
            FillRandomBytes(iv);
            algorithm.Padding = PaddingMode.PKCS7;
            algorithm.Mode = CipherMode.CBC;
            using (ICryptoTransform transform = algorithm.CreateEncryptor(algorithm.Key, iv))
            {
                cipherText = transform.TransformFinalBlock(plainText.Array, plainText.Offset, plainText.Count);
            }
        }

        internal static byte[] GenerateIVAndEncrypt(SymmetricAlgorithm algorithm, byte[] plainText, int offset, int count)
        {
            byte[] buffer;
            byte[] buffer2;
            GenerateIVAndEncrypt(algorithm, new ArraySegment<byte>(plainText, offset, count), out buffer, out buffer2);
            byte[] dst = DiagnosticUtility.Utility.AllocateByteArray(buffer.Length + buffer2.Length);
            Buffer.BlockCopy(buffer, 0, dst, 0, buffer.Length);
            Buffer.BlockCopy(buffer2, 0, dst, buffer.Length, buffer2.Length);
            return dst;
        }

        private static CryptoAlgorithmType GetAlgorithmType(string algorithm)
        {
            switch (algorithm)
            {
                case "http://www.w3.org/2000/09/xmldsig#dsa-sha1":
                case "http://www.w3.org/2000/09/xmldsig#rsa-sha1":
                case "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256":
                case "http://www.w3.org/2001/04/xmlenc#rsa-oaep-mgf1p":
                case "http://www.w3.org/2001/04/xmlenc#rsa-1_5":
                    return CryptoAlgorithmType.Asymmetric;

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
                    return CryptoAlgorithmType.Symmetric;
            }
            return CryptoAlgorithmType.Unknown;
        }

        internal static bool IsEqual(byte[] a, byte[] b)
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
            return true;
        }

        internal static bool IsSymmetricAlgorithm(string algorithm) => 
            (GetAlgorithmType(algorithm) == CryptoAlgorithmType.Symmetric);

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

        internal static SHA1 NewSha1HashAlgorithm()
        {
            if (System.ServiceModel.Security.SecurityUtils.RequiresFipsCompliance)
            {
                return new SHA1CryptoServiceProvider();
            }
            return new SHA1Managed();
        }

        internal static SHA256 NewSha256HashAlgorithm() => 
            new SHA256Managed();

        internal static void ValidateBufferBounds(Array buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("buffer"));
            }
            if ((count < 0) || (count > buffer.Length))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", System.ServiceModel.SR.GetString("ValueMustBeInRange", new object[] { 0, buffer.Length })));
            }
            if ((offset < 0) || (offset > (buffer.Length - count)))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("offset", System.ServiceModel.SR.GetString("ValueMustBeInRange", new object[] { 0, buffer.Length - count })));
            }
        }

        internal static void ValidateSymmetricKeyLength(int keyLength, SecurityAlgorithmSuite algorithmSuite)
        {
            if (!algorithmSuite.IsSymmetricKeyLengthSupported(keyLength))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new ArgumentOutOfRangeException("algorithmSuite", System.ServiceModel.SR.GetString("UnsupportedKeyLength", new object[] { keyLength, algorithmSuite.ToString() })));
            }
            if ((keyLength % 8) != 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new ArgumentOutOfRangeException("algorithmSuite", System.ServiceModel.SR.GetString("KeyLengthMustBeMultipleOfEight", new object[] { keyLength })));
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

        private enum CryptoAlgorithmType
        {
            Unknown,
            Symmetric,
            Asymmetric
        }
    }
}

