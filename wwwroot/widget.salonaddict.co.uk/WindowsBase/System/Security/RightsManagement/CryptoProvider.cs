namespace System.Security.RightsManagement
{
    using MS.Internal;
    using MS.Internal.Security.RightsManagement;
    using MS.Internal.WindowsBase;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Security;
    using System.Windows;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    public class CryptoProvider : IDisposable
    {
        private int _blockSize;
        private ReadOnlyCollection<ContentGrant> _boundGrantReadOnlyCollection;
        private List<SafeRightsManagementHandle> _boundLicenseHandleList;
        private SafeRightsManagementHandle _boundLicenseOwnerViewRightsHandle = SafeRightsManagementHandle.InvalidHandle;
        private bool _boundLicenseOwnerViewRightsHandleCalculated;
        private List<RightNameExpirationInfoPair> _boundRightsInfoList;
        private SafeRightsManagementHandle _decryptorHandle = SafeRightsManagementHandle.InvalidHandle;
        private bool _decryptorHandleCalculated;
        private bool _disposed;
        private SafeRightsManagementHandle _encryptorHandle = SafeRightsManagementHandle.InvalidHandle;
        private bool _encryptorHandleCalculated;
        private ContentUser _owner;

        internal CryptoProvider(List<SafeRightsManagementHandle> boundLicenseHandleList, List<RightNameExpirationInfoPair> rightsInfoList, ContentUser owner)
        {
            Invariant.Assert(boundLicenseHandleList != null);
            Invariant.Assert(boundLicenseHandleList.Count > 0);
            Invariant.Assert(rightsInfoList != null);
            Invariant.Assert(rightsInfoList.Count > 0);
            Invariant.Assert(rightsInfoList.Count == boundLicenseHandleList.Count);
            Invariant.Assert(owner != null);
            this._boundLicenseHandleList = boundLicenseHandleList;
            this._boundRightsInfoList = rightsInfoList;
            this._owner = owner;
        }

        private void CheckDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(null, System.Windows.SR.Get("CryptoProviderDisposed"));
            }
        }

        public byte[] Decrypt(byte[] cryptoText)
        {
            SecurityHelper.DemandRightsManagementPermission();
            this.CheckDisposed();
            if (cryptoText == null)
            {
                throw new ArgumentNullException("cryptoText");
            }
            if (!this.CanDecrypt)
            {
                throw new RightsManagementException(RightsManagementFailureCode.RightNotGranted);
            }
            uint outputByteCount = 0;
            byte[] outputBuffer = null;
            outputByteCount = (uint) cryptoText.Length;
            outputBuffer = new byte[outputByteCount];
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMDecrypt(this.DecryptorHandle, 0, (uint) cryptoText.Length, cryptoText, ref outputByteCount, outputBuffer));
            return outputBuffer;
        }

        internal UnsignedPublishLicense DecryptPublishLicense(string serializedPublishLicense)
        {
            Invariant.Assert(serializedPublishLicense != null);
            if ((this.BoundLicenseOwnerViewRightsHandle == null) || this.BoundLicenseOwnerViewRightsHandle.IsInvalid)
            {
                throw new RightsManagementException(RightsManagementFailureCode.RightNotGranted);
            }
            return new UnsignedPublishLicense(this.BoundLicenseOwnerViewRightsHandle, serializedPublishLicense);
        }

        public void Dispose()
        {
            SecurityHelper.DemandRightsManagementPermission();
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if ((this._decryptorHandle != null) && !this._decryptorHandle.IsInvalid)
                    {
                        this._decryptorHandle.Close();
                    }
                    if ((this._encryptorHandle != null) && !this._encryptorHandle.IsInvalid)
                    {
                        this._encryptorHandle.Close();
                    }
                    if ((this._boundLicenseOwnerViewRightsHandle != null) && !this._boundLicenseOwnerViewRightsHandle.IsInvalid)
                    {
                        this._boundLicenseOwnerViewRightsHandle.Close();
                    }
                    if (this._boundLicenseHandleList != null)
                    {
                        foreach (SafeRightsManagementHandle handle in this._boundLicenseHandleList)
                        {
                            if ((handle != null) && !handle.IsInvalid)
                            {
                                handle.Close();
                            }
                        }
                    }
                }
            }
            finally
            {
                this._disposed = true;
                this._boundLicenseHandleList = null;
                this._boundLicenseOwnerViewRightsHandle = null;
                this._decryptorHandle = null;
                this._encryptorHandle = null;
            }
        }

        public byte[] Encrypt(byte[] clearText)
        {
            SecurityHelper.DemandRightsManagementPermission();
            this.CheckDisposed();
            if (clearText == null)
            {
                throw new ArgumentNullException("clearText");
            }
            if (!this.CanEncrypt)
            {
                throw new RightsManagementException(RightsManagementFailureCode.EncryptionNotPermitted);
            }
            uint outputByteCount = 0;
            byte[] outputBuffer = null;
            outputByteCount = (uint) clearText.Length;
            outputBuffer = new byte[outputByteCount];
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMEncrypt(this.EncryptorHandle, 0, (uint) clearText.Length, clearText, ref outputByteCount, outputBuffer));
            return outputBuffer;
        }

        ~CryptoProvider()
        {
            this.Dispose(false);
        }

        private int QueryBlockSize()
        {
            uint num2;
            uint outputByteCount = 0;
            byte[] outputBuffer = null;
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMGetInfo(this.DecryptorHandle, "block-size", out num2, ref outputByteCount, null));
            Invariant.Assert(outputByteCount == 4);
            outputBuffer = new byte[outputByteCount];
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMGetInfo(this.DecryptorHandle, "block-size", out num2, ref outputByteCount, outputBuffer));
            return BitConverter.ToInt32(outputBuffer, 0);
        }

        public int BlockSize
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                this.CheckDisposed();
                if (this._blockSize == 0)
                {
                    this._blockSize = this.QueryBlockSize();
                }
                return this._blockSize;
            }
        }

        public ReadOnlyCollection<ContentGrant> BoundGrants
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                this.CheckDisposed();
                if (this._boundGrantReadOnlyCollection == null)
                {
                    List<ContentGrant> list = new List<ContentGrant>(this._boundRightsInfoList.Count);
                    foreach (RightNameExpirationInfoPair pair in this._boundRightsInfoList)
                    {
                        ContentRight? rightFromString = ClientSession.GetRightFromString(pair.RightName);
                        if (rightFromString.HasValue)
                        {
                            list.Add(new ContentGrant(this._owner, rightFromString.Value, pair.ValidFrom, pair.ValidUntil));
                        }
                    }
                    this._boundGrantReadOnlyCollection = new ReadOnlyCollection<ContentGrant>(list);
                }
                return this._boundGrantReadOnlyCollection;
            }
        }

        private SafeRightsManagementHandle BoundLicenseOwnerViewRightsHandle
        {
            get
            {
                if (!this._boundLicenseOwnerViewRightsHandleCalculated)
                {
                    for (int i = 0; i < this._boundLicenseHandleList.Count; i++)
                    {
                        ContentRight? rightFromString = ClientSession.GetRightFromString(this._boundRightsInfoList[i].RightName);
                        if (rightFromString.HasValue && ((((ContentRight) rightFromString.Value) == ContentRight.Owner) || (((ContentRight) rightFromString.Value) == ContentRight.ViewRightsData)))
                        {
                            this._boundLicenseOwnerViewRightsHandle = this._boundLicenseHandleList[i];
                            this._boundLicenseOwnerViewRightsHandleCalculated = true;
                            return this._boundLicenseOwnerViewRightsHandle;
                        }
                    }
                    this._boundLicenseOwnerViewRightsHandleCalculated = true;
                }
                return this._boundLicenseOwnerViewRightsHandle;
            }
        }

        public bool CanDecrypt
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                this.CheckDisposed();
                return !this.DecryptorHandle.IsInvalid;
            }
        }

        public bool CanEncrypt
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                this.CheckDisposed();
                return !this.EncryptorHandle.IsInvalid;
            }
        }

        public bool CanMergeBlocks
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                this.CheckDisposed();
                return (this.BlockSize > 1);
            }
        }

        private SafeRightsManagementHandle DecryptorHandle
        {
            get
            {
                if (!this._decryptorHandleCalculated)
                {
                    for (int i = 0; i < this._boundLicenseHandleList.Count; i++)
                    {
                        SafeRightsManagementHandle decryptorHandle = null;
                        if (SafeNativeMethods.DRMCreateEnablingBitsDecryptor(this._boundLicenseHandleList[i], this._boundRightsInfoList[i].RightName, 0, null, out decryptorHandle) == 0)
                        {
                            this._decryptorHandle = decryptorHandle;
                            this._decryptorHandleCalculated = true;
                            return this._decryptorHandle;
                        }
                    }
                    this._decryptorHandleCalculated = true;
                }
                return this._decryptorHandle;
            }
        }

        private SafeRightsManagementHandle EncryptorHandle
        {
            get
            {
                if (!this._encryptorHandleCalculated)
                {
                    for (int i = 0; i < this._boundLicenseHandleList.Count; i++)
                    {
                        SafeRightsManagementHandle encryptorHandle = null;
                        if (SafeNativeMethods.DRMCreateEnablingBitsEncryptor(this._boundLicenseHandleList[i], this._boundRightsInfoList[i].RightName, 0, null, out encryptorHandle) == 0)
                        {
                            this._encryptorHandle = encryptorHandle;
                            this._encryptorHandleCalculated = true;
                            return this._encryptorHandle;
                        }
                    }
                    this._encryptorHandleCalculated = true;
                }
                return this._encryptorHandle;
            }
        }
    }
}

