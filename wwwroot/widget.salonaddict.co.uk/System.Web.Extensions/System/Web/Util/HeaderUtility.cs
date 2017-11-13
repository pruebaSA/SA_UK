namespace System.Web.Util
{
    using System;

    internal static class HeaderUtility
    {
        public static bool IsEncodingInAcceptList(string acceptEncodingHeader, string expectedEncoding)
        {
            if (!string.IsNullOrEmpty(acceptEncodingHeader))
            {
                foreach (string str in acceptEncodingHeader.Split(new char[] { ',' }))
                {
                    if (string.Equals(str.Trim(), expectedEncoding, StringComparison.Ordinal))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

