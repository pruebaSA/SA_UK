namespace MS.Internal.IO.Packaging.CompoundFile
{
    using MS.Internal.Interop;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Security;
    using System.Windows;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    internal static class UnsafeNativeCompoundFileMethods
    {
        [SuppressUnmanagedCodeSecurity, DllImport("ole32.dll")]
        internal static extern int PropVariantClear(ref PROPVARIANT pvar);
        [SuppressUnmanagedCodeSecurity, DllImport("ole32.dll")]
        internal static extern int StgCreateDocfileOnILockBytes(UnsafeNativeILockBytes plkbyt, int grfMode, int reserved, out UnsafeNativeIStorage ppstgOpen);
        [SuppressUnmanagedCodeSecurity, DllImport("ole32.dll")]
        internal static extern int StgCreateStorageEx([In, MarshalAs(UnmanagedType.LPWStr)] string pwcsName, int grfMode, int stgfmt, int grfAttrs, IntPtr pStgOptions, IntPtr reserved2, ref Guid riid, out UnsafeNativeIStorage ppObjectOpen);
        [SuppressUnmanagedCodeSecurity, DllImport("ole32.dll")]
        internal static extern int StgOpenStorageEx([In, MarshalAs(UnmanagedType.LPWStr)] string pwcsName, int grfMode, int stgfmt, int grfAttrs, IntPtr pStgOptions, IntPtr reserved2, ref Guid riid, out UnsafeNativeIStorage ppObjectOpen);
        [SuppressUnmanagedCodeSecurity, DllImport("ole32.dll")]
        internal static extern int StgOpenStorageOnILockBytes(UnsafeNativeILockBytes plkbyt, UnsafeNativeIStorage pStgPriority, int grfMode, IntPtr snbExclude, int reserved, out UnsafeNativeIStorage ppstgOpen);

        internal class UnsafeLockBytesOnStream : UnsafeNativeCompoundFileMethods.UnsafeNativeILockBytes, IDisposable
        {
            private Stream _baseStream;

            internal UnsafeLockBytesOnStream(Stream underlyingStream)
            {
                if (!underlyingStream.CanSeek)
                {
                    throw new NotSupportedException(System.Windows.SR.Get("ILockBytesStreamMustSeek"));
                }
                this._baseStream = underlyingStream;
            }

            private void CheckDisposed()
            {
                if (this._baseStream == null)
                {
                    throw new ObjectDisposedException(null, System.Windows.SR.Get("StreamObjectDisposed"));
                }
            }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing && (this._baseStream != null))
                {
                    this._baseStream = null;
                }
            }

            void UnsafeNativeCompoundFileMethods.UnsafeNativeILockBytes.Flush()
            {
                this.CheckDisposed();
                this._baseStream.Flush();
            }

            void UnsafeNativeCompoundFileMethods.UnsafeNativeILockBytes.LockRegion(ulong libOffset, ulong cb, int dwLockType)
            {
                throw new NotSupportedException();
            }

            void UnsafeNativeCompoundFileMethods.UnsafeNativeILockBytes.ReadAt(ulong offset, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] byte[] pv, int cb, out int pcbRead)
            {
                this.CheckDisposed();
                this._baseStream.Seek((long) offset, SeekOrigin.Begin);
                pcbRead = this._baseStream.Read(pv, 0, cb);
            }

            void UnsafeNativeCompoundFileMethods.UnsafeNativeILockBytes.SetSize(ulong cb)
            {
                this.CheckDisposed();
                this._baseStream.SetLength((long) cb);
            }

            void UnsafeNativeCompoundFileMethods.UnsafeNativeILockBytes.Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
            {
                this.CheckDisposed();
                if ((grfStatFlag & -4) != 0)
                {
                    throw new ArgumentException(System.Windows.SR.Get("InvalidArgumentValue", new object[] { "grfStatFlag", grfStatFlag.ToString(CultureInfo.InvariantCulture) }));
                }
                System.Runtime.InteropServices.ComTypes.STATSTG statstg = new System.Runtime.InteropServices.ComTypes.STATSTG {
                    grfLocksSupported = 0,
                    cbSize = this._baseStream.Length,
                    type = 3
                };
                pstatstg = statstg;
            }

            void UnsafeNativeCompoundFileMethods.UnsafeNativeILockBytes.UnlockRegion(ulong libOffset, ulong cb, int dwLockType)
            {
                throw new NotSupportedException();
            }

            void UnsafeNativeCompoundFileMethods.UnsafeNativeILockBytes.WriteAt(ulong offset, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] byte[] pv, int cb, out int pcbWritten)
            {
                this.CheckDisposed();
                this._baseStream.Seek((long) offset, SeekOrigin.Begin);
                this._baseStream.Write(pv, 0, cb);
                pcbWritten = cb;
            }
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity, Guid("0000013B-0000-0000-C000-000000000046")]
        internal interface UnsafeNativeIEnumSTATPROPSETSTG
        {
            [PreserveSig]
            int Next(uint celt, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] STATPROPSETSTG rgelt, out uint pceltFetched);
            void Skip(uint celt);
            void Reset();
            void Clone(out UnsafeNativeCompoundFileMethods.UnsafeNativeIEnumSTATPROPSETSTG ppenum);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity, Guid("00000139-0000-0000-C000-000000000046")]
        internal interface UnsafeNativeIEnumSTATPROPSTG
        {
            [PreserveSig]
            int Next(uint celt, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] STATPROPSTG rgelt, out uint pceltFetched);
            void Skip(uint celt);
            void Reset();
            void Clone(out UnsafeNativeCompoundFileMethods.UnsafeNativeIEnumSTATPROPSTG ppenum);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity, Guid("0000000d-0000-0000-C000-000000000046")]
        internal interface UnsafeNativeIEnumSTATSTG
        {
            void Next(uint celt, out System.Runtime.InteropServices.ComTypes.STATSTG rgelt, out uint pceltFetched);
            void Skip(uint celt);
            void Reset();
            void Clone(out UnsafeNativeCompoundFileMethods.UnsafeNativeIEnumSTATSTG ppenum);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0000000a-0000-0000-C000-000000000046")]
        internal interface UnsafeNativeILockBytes
        {
            void ReadAt(ulong offset, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] byte[] pv, int cb, out int pcbRead);
            void WriteAt(ulong offset, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] byte[] pv, int cb, out int pcbWritten);
            void Flush();
            void SetSize(ulong cb);
            void LockRegion(ulong libOffset, ulong cb, int dwLockType);
            void UnlockRegion(ulong libOffset, ulong cb, int dwLockType);
            void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag);
        }

        [ComImport, SuppressUnmanagedCodeSecurity, Guid("0000013A-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface UnsafeNativeIPropertySetStorage
        {
            void Create(ref Guid rfmtid, ref Guid pclsid, uint grfFlags, uint grfMode, out UnsafeNativeCompoundFileMethods.UnsafeNativeIPropertyStorage ppprstg);
            [PreserveSig]
            int Open(ref Guid rfmtid, uint grfMode, out UnsafeNativeCompoundFileMethods.UnsafeNativeIPropertyStorage ppprstg);
            void Delete(ref Guid rfmtid);
            void Enum(out UnsafeNativeCompoundFileMethods.UnsafeNativeIEnumSTATPROPSETSTG ppenum);
        }

        [ComImport, Guid("00000138-0000-0000-C000-000000000046"), SuppressUnmanagedCodeSecurity, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface UnsafeNativeIPropertyStorage
        {
            [PreserveSig]
            int ReadMultiple(uint cpspec, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] PROPSPEC[] rgpspec, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] PROPVARIANT[] rgpropvar);
            void WriteMultiple(uint cpspec, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] PROPSPEC[] rgpspec, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] PROPVARIANT[] rgpropvar, uint propidNameFirst);
            void DeleteMultiple(uint cpspec, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] PROPSPEC[] rgpspec);
            void ReadPropertyNames(uint cpropid, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] uint[] rgpropid, [Out, MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.LPWStr, SizeParamIndex=0)] string[] rglpwstrName);
            void WritePropertyNames(uint cpropid, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] uint[] rgpropid, [In, MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.LPWStr, SizeParamIndex=0)] string[] rglpwstrName);
            void DeletePropertyNames(uint cpropid, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] uint[] rgpropid);
            void Commit(uint grfCommitFlags);
            void Revert();
            void Enum(out UnsafeNativeCompoundFileMethods.UnsafeNativeIEnumSTATPROPSTG ppenum);
            void SetTimes(ref System.Runtime.InteropServices.ComTypes.FILETIME pctime, ref System.Runtime.InteropServices.ComTypes.FILETIME patime, ref System.Runtime.InteropServices.ComTypes.FILETIME pmtime);
            void SetClass(ref Guid clsid);
            void Stat(out STATPROPSETSTG pstatpsstg);
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), SuppressUnmanagedCodeSecurity, Guid("0000000b-0000-0000-C000-000000000046")]
        internal interface UnsafeNativeIStorage
        {
            [PreserveSig]
            int CreateStream([In, MarshalAs(UnmanagedType.LPWStr)] string pwcsName, int grfMode, int reserved1, int reserved2, out UnsafeNativeCompoundFileMethods.UnsafeNativeIStream ppstm);
            [PreserveSig]
            int OpenStream([In, MarshalAs(UnmanagedType.LPWStr)] string pwcsName, int reserved1, int grfMode, int reserved2, out UnsafeNativeCompoundFileMethods.UnsafeNativeIStream ppstm);
            [PreserveSig]
            int CreateStorage([In, MarshalAs(UnmanagedType.LPWStr)] string pwcsName, int grfMode, int reserved1, int reserved2, out UnsafeNativeCompoundFileMethods.UnsafeNativeIStorage ppstg);
            [PreserveSig]
            int OpenStorage([In, MarshalAs(UnmanagedType.LPWStr)] string pwcsName, UnsafeNativeCompoundFileMethods.UnsafeNativeIStorage pstgPriority, int grfMode, IntPtr snbExclude, int reserved, out UnsafeNativeCompoundFileMethods.UnsafeNativeIStorage ppstg);
            void CopyTo(int ciidExclude, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] Guid[] rgiidExclude, IntPtr snbExclude, UnsafeNativeCompoundFileMethods.UnsafeNativeIStorage ppstg);
            void MoveElementTo([In, MarshalAs(UnmanagedType.LPWStr)] string pwcsName, UnsafeNativeCompoundFileMethods.UnsafeNativeIStorage pstgDest, [In, MarshalAs(UnmanagedType.LPWStr)] string pwcsNewName, int grfFlags);
            void Commit(int grfCommitFlags);
            void Revert();
            void EnumElements(int reserved1, IntPtr reserved2, int reserved3, out UnsafeNativeCompoundFileMethods.UnsafeNativeIEnumSTATSTG ppEnum);
            void DestroyElement([In, MarshalAs(UnmanagedType.LPWStr)] string pwcsName);
            void RenameElement([In, MarshalAs(UnmanagedType.LPWStr)] string pwcsOldName, [In, MarshalAs(UnmanagedType.LPWStr)] string pwcsNewName);
            void SetElementTimes([In, MarshalAs(UnmanagedType.LPWStr)] string pwcsName, System.Runtime.InteropServices.ComTypes.FILETIME pctime, System.Runtime.InteropServices.ComTypes.FILETIME patime, System.Runtime.InteropServices.ComTypes.FILETIME pmtime);
            void SetClass(ref Guid clsid);
            void SetStateBits(int grfStateBits, int grfMask);
            void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag);
        }

        [ComImport, SuppressUnmanagedCodeSecurity, Guid("0000000c-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface UnsafeNativeIStream
        {
            void Read([Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] byte[] pv, int cb, out int pcbRead);
            void Write([MarshalAs(UnmanagedType.LPArray, SizeParamIndex=1)] byte[] pv, int cb, out int pcbWritten);
            void Seek(long dlibMove, int dwOrigin, out long plibNewPosition);
            void SetSize(long libNewSize);
            void CopyTo(UnsafeNativeCompoundFileMethods.UnsafeNativeIStream pstm, long cb, out long pcbRead, out long pcbWritten);
            void Commit(int grfCommitFlags);
            void Revert();
            void LockRegion(long libOffset, long cb, int dwLockType);
            void UnlockRegion(long libOffset, long cb, int dwLockType);
            void Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag);
            void Clone(out UnsafeNativeCompoundFileMethods.UnsafeNativeIStream ppstm);
        }
    }
}

