namespace SA.BAL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;

    public sealed class HashUtility
    {
        private HashAlgorithmType _algorithmType;
        private const HashAlgorithmType _DEFAULT_ALGORITHM = HashAlgorithmType.SHA1;
        private const int _DEFAULT_SALT_SIZE = 4;
        private byte[] _hash;
        private byte[] _salt;

        private HashUtility(byte[] hash, byte[] salt, HashAlgorithmType algorithmType)
        {
            this._hash = hash;
            this._salt = salt;
            this._algorithmType = algorithmType;
        }

        public static HashUtility CreateHash(byte[] value) => 
            CreateHash(value, 4);

        public static HashUtility CreateHash(byte[] value, HashAlgorithmType algorithmType) => 
            CreateHash(value, 4, algorithmType);

        public static HashUtility CreateHash(byte[] value, int saltSize) => 
            CreateHash(value, saltSize, HashAlgorithmType.SHA1);

        public static HashUtility CreateHash(byte[] value, int saltSize, HashAlgorithmType algorithmType)
        {
            byte[] data = new byte[saltSize];
            new RNGCryptoServiceProvider().GetNonZeroBytes(data);
            return CreateHash(value, data, algorithmType);
        }

        public static HashUtility CreateHash(byte[] value, byte[] salt, HashAlgorithmType algorithmType)
        {
            byte[] buffer;
            HashAlgorithm algorithm;
            switch (algorithmType)
            {
                case HashAlgorithmType.SHA1:
                    algorithm = new SHA1CryptoServiceProvider();
                    break;

                case HashAlgorithmType.SHA256:
                    algorithm = new SHA256CryptoServiceProvider();
                    break;

                case HashAlgorithmType.SHA384:
                    algorithm = new SHA384CryptoServiceProvider();
                    break;

                case HashAlgorithmType.SHA512:
                    algorithm = new SHA512CryptoServiceProvider();
                    break;

                case HashAlgorithmType.MD5:
                    algorithm = new MD5CryptoServiceProvider();
                    break;

                default:
                    throw new ArgumentException();
            }
            List<byte> list = new List<byte>();
            list.AddRange(value);
            list.AddRange(salt);
            try
            {
                buffer = algorithm.ComputeHash(list.ToArray());
            }
            catch
            {
                throw;
            }
            return new HashUtility(buffer, salt, algorithmType);
        }

        public override bool Equals(object value) => 
            ((value is HashUtility) && this.Hash.SequenceEqual<byte>(((HashUtility) value).Hash));

        public override int GetHashCode() => 
            this.Hash.GetHashCode();

        public HashAlgorithmType Algorithm =>
            this._algorithmType;

        public byte[] Hash =>
            this._hash;

        public byte[] Salt =>
            this._salt;

        public enum HashAlgorithmType
        {
            SHA1,
            SHA256,
            SHA384,
            SHA512,
            MD5
        }
    }
}

