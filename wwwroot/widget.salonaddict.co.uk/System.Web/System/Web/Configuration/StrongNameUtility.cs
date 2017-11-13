namespace System.Web.Configuration
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Web;

    internal class StrongNameUtility
    {
        private StrongNameUtility()
        {
        }

        internal static bool GenerateStrongNameFile(string filename)
        {
            IntPtr zero = IntPtr.Zero;
            long pcbKeyBlob = 0L;
            if (!StrongNameKeyGen(null, 0, out zero, out pcbKeyBlob) || (zero == IntPtr.Zero))
            {
                throw Marshal.GetExceptionForHR(StrongNameErrorInfo());
            }
            try
            {
                if ((pcbKeyBlob <= 0L) || (pcbKeyBlob > 0x7fffffffL))
                {
                    throw new InvalidOperationException(System.Web.SR.GetString("Browser_InvalidStrongNameKey"));
                }
                byte[] destination = new byte[pcbKeyBlob];
                Marshal.Copy(zero, destination, 0, (int) pcbKeyBlob);
                using (FileStream stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        writer.Write(destination);
                    }
                }
            }
            finally
            {
                if (zero != IntPtr.Zero)
                {
                    StrongNameFreeBuffer(zero);
                }
            }
            return true;
        }

        [DllImport("mscoree.dll")]
        internal static extern int StrongNameErrorInfo();
        [DllImport("mscoree.dll")]
        internal static extern void StrongNameFreeBuffer(IntPtr pbMemory);
        [DllImport("mscoree.dll")]
        internal static extern bool StrongNameKeyGen([MarshalAs(UnmanagedType.LPWStr)] string wszKeyContainer, uint dwFlags, out IntPtr ppbKeyBlob, out long pcbKeyBlob);
    }
}

