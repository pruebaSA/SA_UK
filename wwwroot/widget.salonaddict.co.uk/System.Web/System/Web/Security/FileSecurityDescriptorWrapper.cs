namespace System.Web.Security
{
    using System;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Util;

    internal class FileSecurityDescriptorWrapper : IDisposable
    {
        internal bool _AnonymousAccess;
        internal bool _AnonymousAccessChecked;
        private string _FileName;
        private ReadWriteSpinLock _Lock = new ReadWriteSpinLock();
        private IntPtr _securityDescriptor;
        private bool _SecurityDescriptorBeingFreed;

        internal FileSecurityDescriptorWrapper(string strFile)
        {
            if (strFile.EndsWith(@"\", StringComparison.Ordinal))
            {
                this._FileName = strFile.Substring(0, strFile.Length - 1);
            }
            else
            {
                this._FileName = strFile;
            }
            this._securityDescriptor = UnsafeNativeMethods.GetFileSecurityDescriptor(this._FileName);
        }

        ~FileSecurityDescriptorWrapper()
        {
            this.FreeSecurityDescriptor();
        }

        internal void FreeSecurityDescriptor()
        {
            if (this.IsSecurityDescriptorValid())
            {
                this._SecurityDescriptorBeingFreed = true;
                this._Lock.AcquireWriterLock();
                try
                {
                    try
                    {
                        if (this.IsSecurityDescriptorValid())
                        {
                            IntPtr securityDesciptor = this._securityDescriptor;
                            this._securityDescriptor = UnsafeNativeMethods.INVALID_HANDLE_VALUE;
                            UnsafeNativeMethods.FreeFileSecurityDescriptor(securityDesciptor);
                        }
                    }
                    finally
                    {
                        this._Lock.ReleaseWriterLock();
                    }
                }
                catch
                {
                    throw;
                }
            }
        }

        internal bool IsAccessAllowed(IntPtr iToken, int iAccess)
        {
            if (iToken == IntPtr.Zero)
            {
                return true;
            }
            if (!this._SecurityDescriptorBeingFreed)
            {
                this._Lock.AcquireReaderLock();
                try
                {
                    try
                    {
                        if (!this._SecurityDescriptorBeingFreed)
                        {
                            if (this._securityDescriptor == IntPtr.Zero)
                            {
                                return true;
                            }
                            if (this._securityDescriptor == UnsafeNativeMethods.INVALID_HANDLE_VALUE)
                            {
                                return false;
                            }
                            return (UnsafeNativeMethods.IsAccessToFileAllowed(this._securityDescriptor, iToken, iAccess) != 0);
                        }
                    }
                    finally
                    {
                        this._Lock.ReleaseReaderLock();
                    }
                }
                catch
                {
                    throw;
                }
            }
            return this.IsAccessAllowedUsingNewSecurityDescriptor(iToken, iAccess);
        }

        private bool IsAccessAllowedUsingNewSecurityDescriptor(IntPtr iToken, int iAccess)
        {
            bool flag;
            if (iToken == IntPtr.Zero)
            {
                return true;
            }
            IntPtr fileSecurityDescriptor = UnsafeNativeMethods.GetFileSecurityDescriptor(this._FileName);
            if (fileSecurityDescriptor == IntPtr.Zero)
            {
                return true;
            }
            if (fileSecurityDescriptor == UnsafeNativeMethods.INVALID_HANDLE_VALUE)
            {
                return false;
            }
            try
            {
                try
                {
                    flag = UnsafeNativeMethods.IsAccessToFileAllowed(fileSecurityDescriptor, iToken, iAccess) != 0;
                }
                finally
                {
                    UnsafeNativeMethods.FreeFileSecurityDescriptor(fileSecurityDescriptor);
                }
            }
            catch
            {
                throw;
            }
            return flag;
        }

        internal bool IsSecurityDescriptorValid() => 
            ((this._securityDescriptor != UnsafeNativeMethods.INVALID_HANDLE_VALUE) && (this._securityDescriptor != IntPtr.Zero));

        internal void OnCacheItemRemoved(string key, object value, CacheItemRemovedReason reason)
        {
            this.FreeSecurityDescriptor();
        }

        void IDisposable.Dispose()
        {
            this.FreeSecurityDescriptor();
            GC.SuppressFinalize(this);
        }
    }
}

