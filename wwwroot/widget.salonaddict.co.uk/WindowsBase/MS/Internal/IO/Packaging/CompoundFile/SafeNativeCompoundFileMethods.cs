namespace MS.Internal.IO.Packaging.CompoundFile
{
    using MS.Internal;
    using MS.Internal.Interop;
    using MS.Internal.WindowsBase;
    using MS.Win32;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Security;
    using System.Windows;

    [SecurityCritical(SecurityCriticalScope.Everything), SecurityTreatAsSafe]
    internal static class SafeNativeCompoundFileMethods
    {
        internal static int SafePropVariantClear(ref PROPVARIANT pvar)
        {
            SecurityHelper.DemandCompoundFileIOPermission();
            return UnsafeNativeCompoundFileMethods.PropVariantClear(ref pvar);
        }

        internal static int SafeStgCreateDocfileOnStream(Stream s, int grfMode, out IStorage ppstgOpen)
        {
            UnsafeNativeCompoundFileMethods.UnsafeNativeIStorage storage;
            SecurityHelper.DemandCompoundFileIOPermission();
            Invariant.Assert(s != null, "s cannot be null");
            UnsafeNativeCompoundFileMethods.UnsafeLockBytesOnStream plkbyt = new UnsafeNativeCompoundFileMethods.UnsafeLockBytesOnStream(s);
            int num = UnsafeNativeCompoundFileMethods.StgCreateDocfileOnILockBytes(plkbyt, grfMode, 0, out storage);
            if (num == 0)
            {
                ppstgOpen = new SafeIStorageImplementation(storage, plkbyt);
                return num;
            }
            ppstgOpen = null;
            plkbyt.Dispose();
            return num;
        }

        internal static int SafeStgCreateStorageEx(string pwcsName, int grfMode, int stgfmt, int grfAttrs, IntPtr pStgOptions, IntPtr reserved2, ref Guid riid, out IStorage ppObjectOpen)
        {
            UnsafeNativeCompoundFileMethods.UnsafeNativeIStorage storage;
            SecurityHelper.DemandCompoundFileIOPermission();
            int num = UnsafeNativeCompoundFileMethods.StgCreateStorageEx(pwcsName, grfMode, stgfmt, grfAttrs, pStgOptions, reserved2, ref riid, out storage);
            if (num == 0)
            {
                ppObjectOpen = new SafeIStorageImplementation(storage);
                return num;
            }
            ppObjectOpen = null;
            return num;
        }

        internal static int SafeStgOpenStorageEx(string pwcsName, int grfMode, int stgfmt, int grfAttrs, IntPtr pStgOptions, IntPtr reserved2, ref Guid riid, out IStorage ppObjectOpen)
        {
            UnsafeNativeCompoundFileMethods.UnsafeNativeIStorage storage;
            SecurityHelper.DemandCompoundFileIOPermission();
            int num = UnsafeNativeCompoundFileMethods.StgOpenStorageEx(pwcsName, grfMode, stgfmt, grfAttrs, pStgOptions, reserved2, ref riid, out storage);
            if (num == 0)
            {
                ppObjectOpen = new SafeIStorageImplementation(storage);
                return num;
            }
            ppObjectOpen = null;
            return num;
        }

        internal static int SafeStgOpenStorageOnStream(Stream s, int grfMode, out IStorage ppstgOpen)
        {
            UnsafeNativeCompoundFileMethods.UnsafeNativeIStorage storage;
            SecurityHelper.DemandCompoundFileIOPermission();
            Invariant.Assert(s != null, "s cannot be null");
            UnsafeNativeCompoundFileMethods.UnsafeLockBytesOnStream plkbyt = new UnsafeNativeCompoundFileMethods.UnsafeLockBytesOnStream(s);
            int num = UnsafeNativeCompoundFileMethods.StgOpenStorageOnILockBytes(plkbyt, null, grfMode, new IntPtr(0), 0, out storage);
            if (num == 0)
            {
                ppstgOpen = new SafeIStorageImplementation(storage);
                return num;
            }
            ppstgOpen = null;
            plkbyt.Dispose();
            return num;
        }

        internal static void UpdateModeFlagFromFileAccess(FileAccess access, ref int grfMode)
        {
            if (FileAccess.Write == access)
            {
                throw new NotSupportedException(System.Windows.SR.Get("WriteOnlyUnsupported"));
            }
            if ((FileAccess.ReadWrite == (access & FileAccess.ReadWrite)) || (FileAccess.ReadWrite == (access & FileAccess.ReadWrite)))
            {
                grfMode |= 2;
            }
            else if (FileAccess.Write == (access & FileAccess.Write))
            {
                grfMode |= 1;
            }
            else if (FileAccess.Read != (access & FileAccess.Read))
            {
                throw new ArgumentException(System.Windows.SR.Get("FileAccessInvalid"));
            }
        }

        private class SafeIEnumSTATPROPSETSTGImplementation : IEnumSTATPROPSETSTG, IDisposable
        {
            private UnsafeNativeCompoundFileMethods.UnsafeNativeIEnumSTATPROPSETSTG _unsafeEnumSTATPROPSETSTG;

            internal SafeIEnumSTATPROPSETSTGImplementation(UnsafeNativeCompoundFileMethods.UnsafeNativeIEnumSTATPROPSETSTG enumSTATPROPSETSTG)
            {
                this._unsafeEnumSTATPROPSETSTG = enumSTATPROPSETSTG;
            }

            public void Dispose()
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                try
                {
                    if (disposing && (this._unsafeEnumSTATPROPSETSTG != null))
                    {
                        MS.Win32.UnsafeNativeMethods.SafeReleaseComObject(this._unsafeEnumSTATPROPSETSTG);
                    }
                }
                finally
                {
                    this._unsafeEnumSTATPROPSETSTG = null;
                }
            }

            void IEnumSTATPROPSETSTG.Clone(out IEnumSTATPROPSETSTG ppenum)
            {
                UnsafeNativeCompoundFileMethods.UnsafeNativeIEnumSTATPROPSETSTG mstatpropsetstg;
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafeEnumSTATPROPSETSTG.Clone(out mstatpropsetstg);
                if (mstatpropsetstg != null)
                {
                    ppenum = new SafeNativeCompoundFileMethods.SafeIEnumSTATPROPSETSTGImplementation(mstatpropsetstg);
                }
                else
                {
                    ppenum = null;
                }
            }

            int IEnumSTATPROPSETSTG.Next(uint celt, STATPROPSETSTG rgelt, out uint pceltFetched)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                return this._unsafeEnumSTATPROPSETSTG.Next(celt, rgelt, out pceltFetched);
            }

            void IEnumSTATPROPSETSTG.Reset()
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafeEnumSTATPROPSETSTG.Reset();
            }

            void IEnumSTATPROPSETSTG.Skip(uint celt)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafeEnumSTATPROPSETSTG.Skip(celt);
            }
        }

        private class SafeIEnumSTATSTGImplementation : IEnumSTATSTG, IDisposable
        {
            private UnsafeNativeCompoundFileMethods.UnsafeNativeIEnumSTATSTG _unsafeEnumSTATSTG;

            internal SafeIEnumSTATSTGImplementation(UnsafeNativeCompoundFileMethods.UnsafeNativeIEnumSTATSTG enumSTATSTG)
            {
                this._unsafeEnumSTATSTG = enumSTATSTG;
            }

            public void Dispose()
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                try
                {
                    if (disposing && (this._unsafeEnumSTATSTG != null))
                    {
                        MS.Win32.UnsafeNativeMethods.SafeReleaseComObject(this._unsafeEnumSTATSTG);
                    }
                }
                finally
                {
                    this._unsafeEnumSTATSTG = null;
                }
            }

            void IEnumSTATSTG.Clone(out IEnumSTATSTG ppenum)
            {
                UnsafeNativeCompoundFileMethods.UnsafeNativeIEnumSTATSTG mstatstg;
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafeEnumSTATSTG.Clone(out mstatstg);
                if (mstatstg != null)
                {
                    ppenum = new SafeNativeCompoundFileMethods.SafeIEnumSTATSTGImplementation(mstatstg);
                }
                else
                {
                    ppenum = null;
                }
            }

            void IEnumSTATSTG.Next(uint celt, out System.Runtime.InteropServices.ComTypes.STATSTG rgelt, out uint pceltFetched)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                if (celt != 1)
                {
                    throw new ArgumentException(System.Windows.SR.Get("InvalidArgumentValue", new object[] { "celt", celt.ToString(CultureInfo.InvariantCulture) }));
                }
                this._unsafeEnumSTATSTG.Next(celt, out rgelt, out pceltFetched);
            }

            void IEnumSTATSTG.Reset()
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafeEnumSTATSTG.Reset();
            }

            void IEnumSTATSTG.Skip(uint celt)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafeEnumSTATSTG.Skip(celt);
            }
        }

        private class SafeIPropertyStorageImplementation : IPropertyStorage, IDisposable
        {
            private UnsafeNativeCompoundFileMethods.UnsafeNativeIPropertyStorage _unsafePropertyStorage;

            internal SafeIPropertyStorageImplementation(UnsafeNativeCompoundFileMethods.UnsafeNativeIPropertyStorage propertyStorage)
            {
                this._unsafePropertyStorage = propertyStorage;
            }

            public void Dispose()
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                try
                {
                    if (disposing && (this._unsafePropertyStorage != null))
                    {
                        MS.Win32.UnsafeNativeMethods.SafeReleaseComObject(this._unsafePropertyStorage);
                    }
                }
                finally
                {
                    this._unsafePropertyStorage = null;
                }
            }

            void IPropertyStorage.Commit(uint grfCommitFlags)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafePropertyStorage.Commit(grfCommitFlags);
            }

            void IPropertyStorage.DeleteMultiple(uint cpspec, PROPSPEC[] rgpspec)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafePropertyStorage.DeleteMultiple(cpspec, rgpspec);
            }

            void IPropertyStorage.DeletePropertyNames(uint cpropid, uint[] rgpropid)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafePropertyStorage.DeletePropertyNames(cpropid, rgpropid);
            }

            void IPropertyStorage.Enum(out IEnumSTATPROPSTG ppenum)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                ppenum = null;
            }

            int IPropertyStorage.ReadMultiple(uint cpspec, PROPSPEC[] rgpspec, PROPVARIANT[] rgpropvar)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                return this._unsafePropertyStorage.ReadMultiple(cpspec, rgpspec, rgpropvar);
            }

            void IPropertyStorage.ReadPropertyNames(uint cpropid, uint[] rgpropid, string[] rglpwstrName)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafePropertyStorage.ReadPropertyNames(cpropid, rgpropid, rglpwstrName);
            }

            void IPropertyStorage.Revert()
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafePropertyStorage.Revert();
            }

            void IPropertyStorage.SetClass(ref Guid clsid)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafePropertyStorage.SetClass(ref clsid);
            }

            void IPropertyStorage.SetTimes(ref System.Runtime.InteropServices.ComTypes.FILETIME pctime, ref System.Runtime.InteropServices.ComTypes.FILETIME patime, ref System.Runtime.InteropServices.ComTypes.FILETIME pmtime)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafePropertyStorage.SetTimes(ref pctime, ref patime, ref pmtime);
            }

            void IPropertyStorage.Stat(out STATPROPSETSTG pstatpsstg)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafePropertyStorage.Stat(out pstatpsstg);
            }

            void IPropertyStorage.WriteMultiple(uint cpspec, PROPSPEC[] rgpspec, PROPVARIANT[] rgpropvar, uint propidNameFirst)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafePropertyStorage.WriteMultiple(cpspec, rgpspec, rgpropvar, propidNameFirst);
            }

            void IPropertyStorage.WritePropertyNames(uint cpropid, uint[] rgpropid, string[] rglpwstrName)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafePropertyStorage.WritePropertyNames(cpropid, rgpropid, rglpwstrName);
            }
        }

        private class SafeIStorageImplementation : IStorage, IPropertySetStorage, IDisposable
        {
            private UnsafeNativeCompoundFileMethods.UnsafeLockBytesOnStream _unsafeLockByteStream;
            private UnsafeNativeCompoundFileMethods.UnsafeNativeIPropertySetStorage _unsafePropertySetStorage;
            private UnsafeNativeCompoundFileMethods.UnsafeNativeIStorage _unsafeStorage;

            internal SafeIStorageImplementation(UnsafeNativeCompoundFileMethods.UnsafeNativeIStorage storage) : this(storage, null)
            {
            }

            internal SafeIStorageImplementation(UnsafeNativeCompoundFileMethods.UnsafeNativeIStorage storage, UnsafeNativeCompoundFileMethods.UnsafeLockBytesOnStream lockBytesStream)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                if (storage == null)
                {
                    throw new ArgumentNullException("storage");
                }
                this._unsafeStorage = storage;
                this._unsafePropertySetStorage = (UnsafeNativeCompoundFileMethods.UnsafeNativeIPropertySetStorage) this._unsafeStorage;
                this._unsafeLockByteStream = lockBytesStream;
            }

            public void Dispose()
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                try
                {
                    if (disposing && (this._unsafeStorage != null))
                    {
                        MS.Win32.UnsafeNativeMethods.SafeReleaseComObject(this._unsafeStorage);
                        if (this._unsafeLockByteStream != null)
                        {
                            this._unsafeLockByteStream.Dispose();
                        }
                    }
                }
                finally
                {
                    this._unsafeStorage = null;
                    this._unsafePropertySetStorage = null;
                    this._unsafeLockByteStream = null;
                }
            }

            void IPropertySetStorage.Create(ref Guid rfmtid, ref Guid pclsid, uint grfFlags, uint grfMode, out IPropertyStorage ppprstg)
            {
                UnsafeNativeCompoundFileMethods.UnsafeNativeIPropertyStorage storage;
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafePropertySetStorage.Create(ref rfmtid, ref pclsid, grfFlags, grfMode, out storage);
                if (storage != null)
                {
                    ppprstg = new SafeNativeCompoundFileMethods.SafeIPropertyStorageImplementation(storage);
                }
                else
                {
                    ppprstg = null;
                }
            }

            void IPropertySetStorage.Delete(ref Guid rfmtid)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafePropertySetStorage.Delete(ref rfmtid);
            }

            void IPropertySetStorage.Enum(out IEnumSTATPROPSETSTG ppenum)
            {
                UnsafeNativeCompoundFileMethods.UnsafeNativeIEnumSTATPROPSETSTG mstatpropsetstg;
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafePropertySetStorage.Enum(out mstatpropsetstg);
                if (mstatpropsetstg != null)
                {
                    ppenum = new SafeNativeCompoundFileMethods.SafeIEnumSTATPROPSETSTGImplementation(mstatpropsetstg);
                }
                else
                {
                    ppenum = null;
                }
            }

            int IPropertySetStorage.Open(ref Guid rfmtid, uint grfMode, out IPropertyStorage ppprstg)
            {
                UnsafeNativeCompoundFileMethods.UnsafeNativeIPropertyStorage storage;
                SecurityHelper.DemandCompoundFileIOPermission();
                int num = this._unsafePropertySetStorage.Open(ref rfmtid, grfMode, out storage);
                if (storage != null)
                {
                    ppprstg = new SafeNativeCompoundFileMethods.SafeIPropertyStorageImplementation(storage);
                    return num;
                }
                ppprstg = null;
                return num;
            }

            void IStorage.Commit(int grfCommitFlags)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafeStorage.Commit(grfCommitFlags);
            }

            void IStorage.CopyTo(int ciidExclude, Guid[] rgiidExclude, IntPtr snbExclude, IStorage ppstg)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                Invariant.Assert(ppstg != null, "ppstg cannot be null");
                this._unsafeStorage.CopyTo(ciidExclude, rgiidExclude, snbExclude, ((SafeNativeCompoundFileMethods.SafeIStorageImplementation) ppstg)._unsafeStorage);
            }

            int IStorage.CreateStorage(string pwcsName, int grfMode, int reserved1, int reserved2, out IStorage ppstg)
            {
                UnsafeNativeCompoundFileMethods.UnsafeNativeIStorage storage;
                SecurityHelper.DemandCompoundFileIOPermission();
                int num = this._unsafeStorage.CreateStorage(pwcsName, grfMode, reserved1, reserved2, out storage);
                if (num == 0)
                {
                    ppstg = new SafeNativeCompoundFileMethods.SafeIStorageImplementation(storage);
                    return num;
                }
                ppstg = null;
                return num;
            }

            int IStorage.CreateStream(string pwcsName, int grfMode, int reserved1, int reserved2, out MS.Internal.IO.Packaging.CompoundFile.IStream ppstm)
            {
                UnsafeNativeCompoundFileMethods.UnsafeNativeIStream stream;
                SecurityHelper.DemandCompoundFileIOPermission();
                int num = this._unsafeStorage.CreateStream(pwcsName, grfMode, reserved1, reserved2, out stream);
                if (num == 0)
                {
                    ppstm = new SafeNativeCompoundFileMethods.SafeIStreamImplementation(stream);
                    return num;
                }
                ppstm = null;
                return num;
            }

            void IStorage.DestroyElement(string pwcsName)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafeStorage.DestroyElement(pwcsName);
            }

            void IStorage.EnumElements(int reserved1, IntPtr reserved2, int reserved3, out IEnumSTATSTG ppEnum)
            {
                UnsafeNativeCompoundFileMethods.UnsafeNativeIEnumSTATSTG mstatstg;
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafeStorage.EnumElements(reserved1, reserved2, reserved3, out mstatstg);
                if (mstatstg != null)
                {
                    ppEnum = new SafeNativeCompoundFileMethods.SafeIEnumSTATSTGImplementation(mstatstg);
                }
                else
                {
                    ppEnum = null;
                }
            }

            void IStorage.MoveElementTo(string pwcsName, IStorage pstgDest, string pwcsNewName, int grfFlags)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                Invariant.Assert(pstgDest != null, "pstgDest cannot be null");
                this._unsafeStorage.MoveElementTo(pwcsName, ((SafeNativeCompoundFileMethods.SafeIStorageImplementation) pstgDest)._unsafeStorage, pwcsNewName, grfFlags);
            }

            int IStorage.OpenStorage(string pwcsName, IStorage pstgPriority, int grfMode, IntPtr snbExclude, int reserved, out IStorage ppstg)
            {
                UnsafeNativeCompoundFileMethods.UnsafeNativeIStorage storage;
                SecurityHelper.DemandCompoundFileIOPermission();
                int num = this._unsafeStorage.OpenStorage(pwcsName, (pstgPriority == null) ? null : ((SafeNativeCompoundFileMethods.SafeIStorageImplementation) pstgPriority)._unsafeStorage, grfMode, snbExclude, reserved, out storage);
                if (num == 0)
                {
                    ppstg = new SafeNativeCompoundFileMethods.SafeIStorageImplementation(storage);
                    return num;
                }
                ppstg = null;
                return num;
            }

            int IStorage.OpenStream(string pwcsName, int reserved1, int grfMode, int reserved2, out MS.Internal.IO.Packaging.CompoundFile.IStream ppstm)
            {
                UnsafeNativeCompoundFileMethods.UnsafeNativeIStream stream;
                SecurityHelper.DemandCompoundFileIOPermission();
                int num = this._unsafeStorage.OpenStream(pwcsName, reserved1, grfMode, reserved2, out stream);
                if (num == 0)
                {
                    ppstm = new SafeNativeCompoundFileMethods.SafeIStreamImplementation(stream);
                    return num;
                }
                ppstm = null;
                return num;
            }

            void IStorage.RenameElement(string pwcsOldName, string pwcsNewName)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafeStorage.RenameElement(pwcsOldName, pwcsNewName);
            }

            void IStorage.Revert()
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafeStorage.Revert();
            }

            void IStorage.SetClass(ref Guid clsid)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafeStorage.SetClass(ref clsid);
            }

            void IStorage.SetElementTimes(string pwcsName, System.Runtime.InteropServices.ComTypes.FILETIME pctime, System.Runtime.InteropServices.ComTypes.FILETIME patime, System.Runtime.InteropServices.ComTypes.FILETIME pmtime)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafeStorage.SetElementTimes(pwcsName, pctime, patime, pmtime);
            }

            void IStorage.SetStateBits(int grfStateBits, int grfMask)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafeStorage.SetStateBits(grfStateBits, grfMask);
            }

            void IStorage.Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafeStorage.Stat(out pstatstg, grfStatFlag);
            }
        }

        private class SafeIStreamImplementation : MS.Internal.IO.Packaging.CompoundFile.IStream, IDisposable
        {
            private UnsafeNativeCompoundFileMethods.UnsafeNativeIStream _unsafeStream;

            internal SafeIStreamImplementation(UnsafeNativeCompoundFileMethods.UnsafeNativeIStream stream)
            {
                this._unsafeStream = stream;
            }

            public void Dispose()
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                try
                {
                    if (disposing && (this._unsafeStream != null))
                    {
                        MS.Win32.UnsafeNativeMethods.SafeReleaseComObject(this._unsafeStream);
                    }
                }
                finally
                {
                    this._unsafeStream = null;
                }
            }

            void MS.Internal.IO.Packaging.CompoundFile.IStream.Clone(out MS.Internal.IO.Packaging.CompoundFile.IStream ppstm)
            {
                UnsafeNativeCompoundFileMethods.UnsafeNativeIStream stream;
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafeStream.Clone(out stream);
                if (stream != null)
                {
                    ppstm = new SafeNativeCompoundFileMethods.SafeIStreamImplementation(stream);
                }
                else
                {
                    ppstm = null;
                }
            }

            void MS.Internal.IO.Packaging.CompoundFile.IStream.Commit(int grfCommitFlags)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafeStream.Commit(grfCommitFlags);
            }

            void MS.Internal.IO.Packaging.CompoundFile.IStream.CopyTo(MS.Internal.IO.Packaging.CompoundFile.IStream pstm, long cb, out long pcbRead, out long pcbWritten)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                Invariant.Assert(pstm != null, "pstm cannot be null");
                if (cb < 0L)
                {
                    throw new ArgumentException(System.Windows.SR.Get("InvalidArgumentValue", new object[] { "cb", cb.ToString(CultureInfo.InvariantCulture) }));
                }
                this._unsafeStream.CopyTo(((SafeNativeCompoundFileMethods.SafeIStreamImplementation) pstm)._unsafeStream, cb, out pcbRead, out pcbWritten);
            }

            void MS.Internal.IO.Packaging.CompoundFile.IStream.LockRegion(long libOffset, long cb, int dwLockType)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                if (libOffset < 0L)
                {
                    throw new ArgumentException(System.Windows.SR.Get("InvalidArgumentValue", new object[] { "libOffset", libOffset.ToString(CultureInfo.InvariantCulture) }));
                }
                if (cb < 0L)
                {
                    throw new ArgumentException(System.Windows.SR.Get("InvalidArgumentValue", new object[] { "cb", cb.ToString(CultureInfo.InvariantCulture) }));
                }
                this._unsafeStream.LockRegion(libOffset, cb, dwLockType);
            }

            void MS.Internal.IO.Packaging.CompoundFile.IStream.Read(byte[] pv, int cb, out int pcbRead)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                if (cb < 0)
                {
                    throw new ArgumentException(System.Windows.SR.Get("InvalidArgumentValue", new object[] { "cb", cb.ToString(CultureInfo.InvariantCulture) }));
                }
                this._unsafeStream.Read(pv, cb, out pcbRead);
            }

            void MS.Internal.IO.Packaging.CompoundFile.IStream.Revert()
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafeStream.Revert();
            }

            void MS.Internal.IO.Packaging.CompoundFile.IStream.Seek(long dlibMove, int dwOrigin, out long plibNewPosition)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                if (dwOrigin < 0)
                {
                    throw new ArgumentException(System.Windows.SR.Get("InvalidArgumentValue", new object[] { "dwOrigin", dwOrigin.ToString(CultureInfo.InvariantCulture) }));
                }
                if ((dlibMove < 0L) && (dwOrigin == 0))
                {
                    throw new ArgumentException(System.Windows.SR.Get("InvalidArgumentValue", new object[] { "dlibMove", dlibMove.ToString(CultureInfo.InvariantCulture) }));
                }
                this._unsafeStream.Seek(dlibMove, dwOrigin, out plibNewPosition);
            }

            void MS.Internal.IO.Packaging.CompoundFile.IStream.SetSize(long libNewSize)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                if (libNewSize < 0L)
                {
                    throw new ArgumentException(System.Windows.SR.Get("InvalidArgumentValue", new object[] { "libNewSize", libNewSize.ToString(CultureInfo.InvariantCulture) }));
                }
                this._unsafeStream.SetSize(libNewSize);
            }

            void MS.Internal.IO.Packaging.CompoundFile.IStream.Stat(out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, int grfStatFlag)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                this._unsafeStream.Stat(out pstatstg, grfStatFlag);
            }

            void MS.Internal.IO.Packaging.CompoundFile.IStream.UnlockRegion(long libOffset, long cb, int dwLockType)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                if (libOffset < 0L)
                {
                    throw new ArgumentException(System.Windows.SR.Get("InvalidArgumentValue", new object[] { "libOffset", libOffset.ToString(CultureInfo.InvariantCulture) }));
                }
                if (cb < 0L)
                {
                    throw new ArgumentException(System.Windows.SR.Get("InvalidArgumentValue", new object[] { "cb", cb.ToString(CultureInfo.InvariantCulture) }));
                }
                this._unsafeStream.UnlockRegion(libOffset, cb, dwLockType);
            }

            void MS.Internal.IO.Packaging.CompoundFile.IStream.Write(byte[] pv, int cb, out int pcbWritten)
            {
                SecurityHelper.DemandCompoundFileIOPermission();
                if (cb < 0)
                {
                    throw new ArgumentException(System.Windows.SR.Get("InvalidArgumentValue", new object[] { "cb", cb.ToString(CultureInfo.InvariantCulture) }));
                }
                this._unsafeStream.Write(pv, cb, out pcbWritten);
            }
        }
    }
}

