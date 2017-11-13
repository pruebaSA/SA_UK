namespace System.Web.Security
{
    using System;
    using System.Web;
    using System.Web.Configuration;

    internal class CookieProtectionHelper
    {
        internal static byte[] Decode(CookieProtection cookieProtection, string data)
        {
            byte[] buf = HttpServerUtility.UrlTokenDecode(data);
            if ((buf == null) || (cookieProtection == CookieProtection.None))
            {
                return buf;
            }
            if ((cookieProtection == CookieProtection.All) || (cookieProtection == CookieProtection.Encryption))
            {
                buf = MachineKeySection.EncryptOrDecryptData(false, buf, null, 0, buf.Length);
                if (buf == null)
                {
                    return null;
                }
            }
            if ((cookieProtection != CookieProtection.All) && (cookieProtection != CookieProtection.Validation))
            {
                return buf;
            }
            if (buf.Length <= 20)
            {
                return null;
            }
            byte[] dst = new byte[buf.Length - 20];
            Buffer.BlockCopy(buf, 0, dst, 0, dst.Length);
            byte[] buffer3 = MachineKeySection.HashData(dst, null, 0, dst.Length);
            if ((buffer3 == null) || (buffer3.Length != 20))
            {
                return null;
            }
            for (int i = 0; i < 20; i++)
            {
                if (buffer3[i] != buf[dst.Length + i])
                {
                    return null;
                }
            }
            return dst;
        }

        internal static string Encode(CookieProtection cookieProtection, byte[] buf, int count)
        {
            if ((cookieProtection == CookieProtection.All) || (cookieProtection == CookieProtection.Validation))
            {
                byte[] src = MachineKeySection.HashData(buf, null, 0, count);
                if ((src == null) || (src.Length != 20))
                {
                    return null;
                }
                if (buf.Length >= (count + 20))
                {
                    Buffer.BlockCopy(src, 0, buf, count, 20);
                }
                else
                {
                    byte[] buffer2 = buf;
                    buf = new byte[count + 20];
                    Buffer.BlockCopy(buffer2, 0, buf, 0, count);
                    Buffer.BlockCopy(src, 0, buf, count, 20);
                }
                count += 20;
            }
            if ((cookieProtection == CookieProtection.All) || (cookieProtection == CookieProtection.Encryption))
            {
                buf = MachineKeySection.EncryptOrDecryptData(true, buf, null, 0, count);
                count = buf.Length;
            }
            if (count < buf.Length)
            {
                byte[] buffer3 = buf;
                buf = new byte[count];
                Buffer.BlockCopy(buffer3, 0, buf, 0, count);
            }
            return HttpServerUtility.UrlTokenEncode(buf);
        }
    }
}

