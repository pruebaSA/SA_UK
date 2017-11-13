namespace System.Web.Util
{
    using System;
    using System.Globalization;
    using System.Text;

    internal static class HexParser
    {
        public static byte[] Parse(string token)
        {
            byte[] buffer = new byte[token.Length / 2];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = byte.Parse(token.Substring(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            return buffer;
        }

        public static string ToString(byte[] tokenBytes)
        {
            StringBuilder builder = new StringBuilder(tokenBytes.Length * 2);
            for (int i = 0; i < tokenBytes.Length; i++)
            {
                builder.Append(tokenBytes[i].ToString("x2", CultureInfo.InvariantCulture));
            }
            return builder.ToString();
        }
    }
}

