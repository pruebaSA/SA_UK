namespace SA.BAL
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    public sealed class HMACUtility
    {
        private byte[] _key;

        public HMACUtility(byte[] key)
        {
            this._key = key;
        }

        public string ComputeHash(string clearText)
        {
            StringBuilder builder = new StringBuilder();
            byte[] bytes = Encoding.ASCII.GetBytes(clearText);
            using (HMACSHA256 hmacsha = new HMACSHA256(this._key))
            {
                byte[] buffer2 = hmacsha.ComputeHash(bytes);
                for (int i = 0; i < buffer2.Length; i++)
                {
                    builder.Append(buffer2[i].ToString("X2"));
                }
            }
            return builder.ToString();
        }
    }
}

