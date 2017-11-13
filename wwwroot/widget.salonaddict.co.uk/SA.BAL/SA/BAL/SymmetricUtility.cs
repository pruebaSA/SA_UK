namespace SA.BAL
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public class SymmetricUtility
    {
        private AlgorithmType _algorithmType;
        private byte[] _key;

        public SymmetricUtility(byte[] key, AlgorithmType algorithmType)
        {
            this._key = key;
            this._algorithmType = algorithmType;
        }

        public string Decrypt(byte[] value)
        {
            SymmetricAlgorithm algorithm = SymmetricAlgorithm.Create(this._algorithmType.ToString());
            algorithm.Key = this._key;
            using (MemoryStream stream = new MemoryStream())
            {
                int offset = 0;
                byte[] destinationArray = new byte[algorithm.IV.Length];
                Array.Copy(value, destinationArray, destinationArray.Length);
                algorithm.IV = destinationArray;
                offset += algorithm.IV.Length;
                using (CryptoStream stream2 = new CryptoStream(stream, algorithm.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    stream2.Write(value, offset, value.Length - offset);
                    stream2.FlushFinalBlock();
                }
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public byte[] Encrypt(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            SymmetricAlgorithm algorithm = SymmetricAlgorithm.Create(this._algorithmType.ToString());
            algorithm.Key = this._key;
            using (MemoryStream stream = new MemoryStream())
            {
                algorithm.GenerateIV();
                stream.Write(algorithm.IV, 0, algorithm.IV.Length);
                using (CryptoStream stream2 = new CryptoStream(stream, algorithm.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    stream2.Write(bytes, 0, bytes.Length);
                    stream2.FlushFinalBlock();
                }
                return stream.ToArray();
            }
        }

        public enum AlgorithmType
        {
            DES,
            RC2,
            TripleDES,
            Rijndael
        }
    }
}

