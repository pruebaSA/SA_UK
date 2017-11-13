namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Security.Cryptography;
    using System.Text;

    internal class StringHashBuilder
    {
        private byte[] _cachedBuffer;
        private MD5CryptoServiceProvider _md5HashProvider;
        private List<string> _strings;
        private int _totalLength;
        private const string NewLine = "\n";

        internal StringHashBuilder()
        {
            this._md5HashProvider = new MD5CryptoServiceProvider();
            this._strings = new List<string>();
        }

        internal StringHashBuilder(int startingBufferSize)
        {
            this._md5HashProvider = new MD5CryptoServiceProvider();
            this._strings = new List<string>();
            this._cachedBuffer = new byte[startingBufferSize];
        }

        internal virtual void Append(string s)
        {
            this.InternalAppend(s);
        }

        internal virtual void AppendLine(string s)
        {
            this.InternalAppend(s);
            this.InternalAppend("\n");
        }

        internal void Clear()
        {
            this._strings.Clear();
            this._totalLength = 0;
        }

        internal string ComputeHash()
        {
            int byteCount = this.GetByteCount();
            if (this._cachedBuffer == null)
            {
                this._cachedBuffer = new byte[byteCount];
            }
            else if (this._cachedBuffer.Length < byteCount)
            {
                int num2 = Math.Max(this._cachedBuffer.Length + (this._cachedBuffer.Length / 2), byteCount);
                this._cachedBuffer = new byte[num2];
            }
            int byteIndex = 0;
            foreach (string str in this._strings)
            {
                byteIndex += Encoding.Unicode.GetBytes(str, 0, str.Length, this._cachedBuffer, byteIndex);
            }
            return ConvertHashToString(this._md5HashProvider.ComputeHash(this._cachedBuffer, 0, byteCount));
        }

        public static string ComputeHash(string source)
        {
            StringHashBuilder builder = new StringHashBuilder();
            builder.Append(source);
            return builder.ComputeHash();
        }

        private static string ConvertHashToString(byte[] hash)
        {
            StringBuilder builder = new StringBuilder(hash.Length * 2);
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("x2", CultureInfo.InvariantCulture));
            }
            return builder.ToString();
        }

        private int GetByteCount()
        {
            int num = 0;
            foreach (string str in this._strings)
            {
                num += Encoding.Unicode.GetByteCount(str);
            }
            return num;
        }

        private void InternalAppend(string s)
        {
            if (s.Length != 0)
            {
                this._strings.Add(s);
                this._totalLength += s.Length;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            this._strings.ForEach(delegate (string s) {
                builder.Append(s);
            });
            return builder.ToString();
        }

        internal int CharCount =>
            this._totalLength;
    }
}

