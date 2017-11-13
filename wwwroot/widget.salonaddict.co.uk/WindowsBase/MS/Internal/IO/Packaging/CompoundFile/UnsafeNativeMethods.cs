namespace MS.Internal.IO.Packaging.CompoundFile
{
    using System;
    using System.Runtime.InteropServices;

    internal static class UnsafeNativeMethods
    {
        internal static class ZLib
        {
            internal const string ZLibVersion = "1.1.4";

            [DllImport("PresentationNative_v0300.dll")]
            internal static extern ErrorCode ums_deflate([In, Out] ref UnsafeNativeMethods.ZStream stream, [In] int flush);
            [DllImport("PresentationNative_v0300.dll")]
            internal static extern ErrorCode ums_deflate_init([In, Out] ref UnsafeNativeMethods.ZStream stream, [In] int level, [In, MarshalAs(UnmanagedType.LPStr)] string version, [In] int zStreamStructSize);
            [DllImport("PresentationNative_v0300.dll")]
            internal static extern ErrorCode ums_inflate([In, Out] ref UnsafeNativeMethods.ZStream stream, [In] int flush);
            [DllImport("PresentationNative_v0300.dll")]
            internal static extern ErrorCode ums_inflate_init([In, Out] ref UnsafeNativeMethods.ZStream stream, [In, MarshalAs(UnmanagedType.LPStr)] string version, [In] int zStreamStructSize);

            internal enum ErrorCode
            {
                BufError = -5,
                DataError = -3,
                ErrorNo = -1,
                MemError = -4,
                NeedDictionary = 2,
                StreamEnd = 1,
                StreamError = -2,
                Success = 0,
                VersionError = -6
            }

            internal enum FlushCodes
            {
                Finish = 4,
                FullFlush = 3,
                NoFlush = 0,
                SyncFlush = 2
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct ZStream
        {
            internal IntPtr pInBuf;
            internal uint cbIn;
            internal uint cbTotalIn;
            internal IntPtr pOutBuf;
            internal uint cbOut;
            internal uint cbTotalOut;
            internal IntPtr pErrorMsgString;
            internal IntPtr pState;
            internal IntPtr pAlloc;
            internal IntPtr pFree;
            internal IntPtr pOpaque;
            internal int dataType;
            internal uint adler;
            internal uint reserved;
        }
    }
}

