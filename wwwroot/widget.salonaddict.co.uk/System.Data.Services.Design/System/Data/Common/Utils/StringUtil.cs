namespace System.Data.Common.Utils
{
    using System;

    internal static class StringUtil
    {
        internal static bool IsNullOrEmptyOrWhiteSpace(string value) => 
            IsNullOrEmptyOrWhiteSpace(value, 0);

        internal static bool IsNullOrEmptyOrWhiteSpace(string value, int offset)
        {
            if (value != null)
            {
                for (int i = offset; i < value.Length; i++)
                {
                    if (!char.IsWhiteSpace(value[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal static bool IsNullOrEmptyOrWhiteSpace(string value, int offset, int length)
        {
            if (value != null)
            {
                length = Math.Min(value.Length, length);
                for (int i = offset; i < length; i++)
                {
                    if (!char.IsWhiteSpace(value[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}

