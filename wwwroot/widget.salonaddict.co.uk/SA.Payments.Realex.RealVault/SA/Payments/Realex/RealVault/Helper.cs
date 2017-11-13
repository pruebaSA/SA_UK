namespace SA.Payments.Realex.RealVault
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;

    internal static class Helper
    {
        internal static string GenerateSignatureSHA1(string sharedSecret, params string[] value)
        {
            if (sharedSecret == null)
            {
                return string.Empty;
            }
            if (value == null)
            {
                return string.Empty;
            }
            string s = string.Join(".", value);
            SHA1 sha = new SHA1Managed();
            string str2 = HexEncode(sha.ComputeHash(Encoding.UTF8.GetBytes(s)));
            return HexEncode(sha.ComputeHash(Encoding.UTF8.GetBytes(str2 + "." + sharedSecret)));
        }

        internal static string HexEncode(byte[] data)
        {
            StringBuilder hex = new StringBuilder();
            data.ToList<byte>().ForEach(delegate (byte b) {
                hex.Append(b.ToString("X2"));
            });
            return hex.ToString().ToLower();
        }

        internal static bool IsValidCardNumber(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            value = value.Replace("-", string.Empty);
            if (Regex.IsMatch(value, @"\D"))
            {
                return false;
            }
            List<int> source = Array.ConvertAll<char, int>(value.ToCharArray(), x => int.Parse(x.ToString())).ToList<int>();
            source.Reverse();
            return ((source.Select<int, int>(delegate (int digit, int index) {
                if ((index % 2) == 1)
                {
                    return (2 * digit);
                }
                return digit;
            }).ToList<int>().Sum<int>(delegate (int x) {
                if (x <= 9)
                {
                    return x;
                }
                return (1 + (x % 10));
            }) % 10) == 0);
        }
    }
}

