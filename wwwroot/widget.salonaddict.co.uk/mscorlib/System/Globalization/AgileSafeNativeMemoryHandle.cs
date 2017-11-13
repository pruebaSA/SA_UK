namespace System.Globalization
{
    using Microsoft.Win32;
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    internal sealed class AgileSafeNativeMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private unsafe byte* bytes;
        private long fileSize;
        private bool mode;
        private const int PAGE_READONLY = 2;
        private const int SECTION_MAP_READ = 4;

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        internal AgileSafeNativeMemoryHandle() : base(true)
        {
        }

        internal AgileSafeNativeMemoryHandle(string fileName) : this(fileName, null)
        {
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        internal AgileSafeNativeMemoryHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
        {
            base.SetHandle(handle);
        }

        internal unsafe AgileSafeNativeMemoryHandle(string fileName, string fileMappingName) : base(true)
        {
            int num2;
            this.mode = true;
            SafeFileHandle hFile = Win32Native.UnsafeCreateFile(fileName, -2147483648, FileShare.Read, null, FileMode.Open, 0, IntPtr.Zero);
            int num = Marshal.GetLastWin32Error();
            if (hFile.IsInvalid)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("InvalidOperation_UnexpectedWin32Error"), new object[] { num }));
            }
            int fileSize = Win32Native.GetFileSize(hFile, out num2);
            if (fileSize == -1)
            {
                hFile.Close();
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("InvalidOperation_UnexpectedWin32Error"), new object[] { num }));
            }
            this.fileSize = (num2 << 0x20) | ((long) ((ulong) fileSize));
            if (this.fileSize == 0L)
            {
                hFile.Close();
            }
            else
            {
                SafeFileMappingHandle handle = Win32Native.CreateFileMapping(hFile, IntPtr.Zero, 2, 0, 0, fileMappingName);
                num = Marshal.GetLastWin32Error();
                hFile.Close();
                if (handle.IsInvalid)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("InvalidOperation_UnexpectedWin32Error"), new object[] { num }));
                }
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                }
                finally
                {
                    base.handle = Win32Native.MapViewOfFile(handle, 4, 0, 0, UIntPtr.Zero);
                }
                num = Marshal.GetLastWin32Error();
                if (base.handle == IntPtr.Zero)
                {
                    handle.Close();
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("InvalidOperation_UnexpectedWin32Error"), new object[] { num }));
                }
                this.bytes = (byte*) base.DangerousGetHandle();
                handle.Close();
            }
        }

        internal unsafe byte* GetBytePtr() => 
            this.bytes;

        protected override bool ReleaseHandle()
        {
            if (!this.IsInvalid)
            {
                if (!this.mode)
                {
                    Marshal.FreeHGlobal(base.handle);
                    base.handle = IntPtr.Zero;
                    return true;
                }
                if (Win32Native.UnmapViewOfFile(base.handle))
                {
                    base.handle = IntPtr.Zero;
                    return true;
                }
            }
            return false;
        }

        internal long FileSize =>
            this.fileSize;
    }
}

