namespace System.Security.Cryptography
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;

    internal sealed class CAPI : System.Security.Cryptography.CAPIMethods
    {
        private CAPI()
        {
        }

        internal static byte[] BlobToByteArray(IntPtr pBlob)
        {
            System.Security.Cryptography.CAPIBase.CRYPTOAPI_BLOB blob = (System.Security.Cryptography.CAPIBase.CRYPTOAPI_BLOB) Marshal.PtrToStructure(pBlob, typeof(System.Security.Cryptography.CAPIBase.CRYPTOAPI_BLOB));
            if (blob.cbData == 0)
            {
                return new byte[0];
            }
            return BlobToByteArray(blob);
        }

        internal static byte[] BlobToByteArray(System.Security.Cryptography.CAPIBase.CRYPTOAPI_BLOB blob)
        {
            if (blob.cbData == 0)
            {
                return new byte[0];
            }
            byte[] destination = new byte[blob.cbData];
            Marshal.Copy(blob.pbData, destination, 0, destination.Length);
            return destination;
        }

        internal static bool CertAddCertificateContextToStore([In] System.Security.Cryptography.SafeCertStoreHandle hCertStore, [In] System.Security.Cryptography.SafeCertContextHandle pCertContext, [In] uint dwAddDisposition, [In, Out] System.Security.Cryptography.SafeCertContextHandle ppStoreContext)
        {
            if (hCertStore == null)
            {
                throw new ArgumentNullException("hCertStore");
            }
            if (hCertStore.IsInvalid)
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_InvalidHandle"), "hCertStore");
            }
            if (pCertContext == null)
            {
                throw new ArgumentNullException("pCertContext");
            }
            if (pCertContext.IsInvalid)
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_InvalidHandle"), "pCertContext");
            }
            new StorePermission(StorePermissionFlags.AddToStore).Demand();
            return System.Security.Cryptography.CAPIUnsafe.CertAddCertificateContextToStore(hCertStore, pCertContext, dwAddDisposition, ppStoreContext);
        }

        internal static bool CertAddCertificateLinkToStore([In] System.Security.Cryptography.SafeCertStoreHandle hCertStore, [In] System.Security.Cryptography.SafeCertContextHandle pCertContext, [In] uint dwAddDisposition, [In, Out] System.Security.Cryptography.SafeCertContextHandle ppStoreContext)
        {
            if (hCertStore == null)
            {
                throw new ArgumentNullException("hCertStore");
            }
            if (hCertStore.IsInvalid)
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_InvalidHandle"), "hCertStore");
            }
            if (pCertContext == null)
            {
                throw new ArgumentNullException("pCertContext");
            }
            if (pCertContext.IsInvalid)
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_InvalidHandle"), "pCertContext");
            }
            new StorePermission(StorePermissionFlags.AddToStore).Demand();
            return System.Security.Cryptography.CAPIUnsafe.CertAddCertificateLinkToStore(hCertStore, pCertContext, dwAddDisposition, ppStoreContext);
        }

        internal static System.Security.Cryptography.SafeCertContextHandle CertDuplicateCertificateContext([In] IntPtr pCertContext)
        {
            if (pCertContext == IntPtr.Zero)
            {
                return System.Security.Cryptography.SafeCertContextHandle.InvalidHandle;
            }
            return System.Security.Cryptography.CAPISafe.CertDuplicateCertificateContext(pCertContext);
        }

        internal static IntPtr CertEnumCertificatesInStore([In] System.Security.Cryptography.SafeCertStoreHandle hCertStore, [In] IntPtr pPrevCertContext)
        {
            if (hCertStore == null)
            {
                throw new ArgumentNullException("hCertStore");
            }
            if (hCertStore.IsInvalid)
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_InvalidHandle"), "hCertStore");
            }
            if (pPrevCertContext == IntPtr.Zero)
            {
                new StorePermission(StorePermissionFlags.EnumerateCertificates).Demand();
            }
            IntPtr pCertContext = System.Security.Cryptography.CAPIUnsafe.CertEnumCertificatesInStore(hCertStore, pPrevCertContext);
            if (pCertContext == IntPtr.Zero)
            {
                int hr = Marshal.GetLastWin32Error();
                if (hr != -2146885628)
                {
                    System.Security.Cryptography.CAPISafe.CertFreeCertificateContext(pCertContext);
                    throw new CryptographicException(hr);
                }
            }
            return pCertContext;
        }

        internal static System.Security.Cryptography.SafeCertContextHandle CertFindCertificateInStore([In] System.Security.Cryptography.SafeCertStoreHandle hCertStore, [In] uint dwCertEncodingType, [In] uint dwFindFlags, [In] uint dwFindType, [In] IntPtr pvFindPara, [In] System.Security.Cryptography.SafeCertContextHandle pPrevCertContext)
        {
            if (hCertStore == null)
            {
                throw new ArgumentNullException("hCertStore");
            }
            if (hCertStore.IsInvalid)
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_InvalidHandle"), "hCertStore");
            }
            return System.Security.Cryptography.CAPIUnsafe.CertFindCertificateInStore(hCertStore, dwCertEncodingType, dwFindFlags, dwFindType, pvFindPara, pPrevCertContext);
        }

        internal static System.Security.Cryptography.SafeCertStoreHandle CertOpenStore([In] IntPtr lpszStoreProvider, [In] uint dwMsgAndCertEncodingType, [In] IntPtr hCryptProv, [In] uint dwFlags, [In] string pvPara)
        {
            if ((lpszStoreProvider != new IntPtr(2L)) && (lpszStoreProvider != new IntPtr(10L)))
            {
                throw new ArgumentException(SecurityResources.GetResourceString("Argument_InvalidValue"), "lpszStoreProvider");
            }
            if (((((dwFlags & 0x20000) == 0x20000) || ((dwFlags & 0x80000) == 0x80000)) || ((dwFlags & 0x90000) == 0x90000)) && ((pvPara != null) && pvPara.StartsWith(@"\\", StringComparison.Ordinal)))
            {
                new PermissionSet(PermissionState.Unrestricted).Demand();
            }
            if ((dwFlags & 0x10) == 0x10)
            {
                new StorePermission(StorePermissionFlags.DeleteStore).Demand();
            }
            else
            {
                new StorePermission(StorePermissionFlags.OpenStore).Demand();
            }
            if ((dwFlags & 0x2000) == 0x2000)
            {
                new StorePermission(StorePermissionFlags.CreateStore).Demand();
            }
            if ((dwFlags & 0x4000) == 0)
            {
                new StorePermission(StorePermissionFlags.CreateStore).Demand();
            }
            return System.Security.Cryptography.CAPIUnsafe.CertOpenStore(lpszStoreProvider, dwMsgAndCertEncodingType, hCryptProv, dwFlags | 4, pvPara);
        }

        internal static bool CryptAcquireContext(ref System.Security.Cryptography.SafeCryptProvHandle hCryptProv, IntPtr pwszContainer, IntPtr pwszProvider, uint dwProvType, uint dwFlags)
        {
            string str = null;
            if (pwszContainer != IntPtr.Zero)
            {
                str = Marshal.PtrToStringUni(pwszContainer);
            }
            string str2 = null;
            if (pwszProvider != IntPtr.Zero)
            {
                str2 = Marshal.PtrToStringUni(pwszProvider);
            }
            return CryptAcquireContext(ref hCryptProv, str, str2, dwProvType, dwFlags);
        }

        internal static bool CryptAcquireContext([In, Out] ref System.Security.Cryptography.SafeCryptProvHandle hCryptProv, [In, MarshalAs(UnmanagedType.LPStr)] string pwszContainer, [In, MarshalAs(UnmanagedType.LPStr)] string pwszProvider, [In] uint dwProvType, [In] uint dwFlags)
        {
            CspParameters parameters = new CspParameters {
                ProviderName = pwszProvider,
                KeyContainerName = pwszContainer,
                ProviderType = (int) dwProvType,
                KeyNumber = -1,
                Flags = ((dwFlags & 0x20) == 0x20) ? CspProviderFlags.UseMachineKeyStore : CspProviderFlags.NoFlags
            };
            KeyContainerPermission permission = new KeyContainerPermission(KeyContainerPermissionFlags.NoFlags);
            KeyContainerPermissionAccessEntry accessEntry = new KeyContainerPermissionAccessEntry(parameters, KeyContainerPermissionFlags.Open);
            permission.AccessEntries.Add(accessEntry);
            permission.Demand();
            bool flag = System.Security.Cryptography.CAPIUnsafe.CryptAcquireContext(ref hCryptProv, pwszContainer, pwszProvider, dwProvType, dwFlags);
            if (!flag && (Marshal.GetLastWin32Error() == -2146893802))
            {
                flag = System.Security.Cryptography.CAPIUnsafe.CryptAcquireContext(ref hCryptProv, pwszContainer, pwszProvider, dwProvType, dwFlags | 8);
            }
            return flag;
        }

        internal static System.Security.Cryptography.CAPIBase.CRYPT_OID_INFO CryptFindOIDInfo([In] uint dwKeyType, [In] IntPtr pvKey, [In] uint dwGroupId)
        {
            if (pvKey == IntPtr.Zero)
            {
                throw new ArgumentNullException("pvKey");
            }
            System.Security.Cryptography.CAPIBase.CRYPT_OID_INFO crypt_oid_info = new System.Security.Cryptography.CAPIBase.CRYPT_OID_INFO(Marshal.SizeOf(typeof(System.Security.Cryptography.CAPIBase.CRYPT_OID_INFO)));
            IntPtr ptr = System.Security.Cryptography.CAPISafe.CryptFindOIDInfo(dwKeyType, pvKey, dwGroupId);
            if (ptr != IntPtr.Zero)
            {
                crypt_oid_info = (System.Security.Cryptography.CAPIBase.CRYPT_OID_INFO) Marshal.PtrToStructure(ptr, typeof(System.Security.Cryptography.CAPIBase.CRYPT_OID_INFO));
            }
            return crypt_oid_info;
        }

        internal static System.Security.Cryptography.CAPIBase.CRYPT_OID_INFO CryptFindOIDInfo([In] uint dwKeyType, [In] System.Security.Cryptography.SafeLocalAllocHandle pvKey, [In] uint dwGroupId)
        {
            if (pvKey == null)
            {
                throw new ArgumentNullException("pvKey");
            }
            if (pvKey.IsInvalid)
            {
                throw new CryptographicException(SecurityResources.GetResourceString("Cryptography_InvalidHandle"), "pvKey");
            }
            System.Security.Cryptography.CAPIBase.CRYPT_OID_INFO crypt_oid_info = new System.Security.Cryptography.CAPIBase.CRYPT_OID_INFO(Marshal.SizeOf(typeof(System.Security.Cryptography.CAPIBase.CRYPT_OID_INFO)));
            IntPtr ptr = System.Security.Cryptography.CAPISafe.CryptFindOIDInfo(dwKeyType, pvKey, dwGroupId);
            if (ptr != IntPtr.Zero)
            {
                crypt_oid_info = (System.Security.Cryptography.CAPIBase.CRYPT_OID_INFO) Marshal.PtrToStructure(ptr, typeof(System.Security.Cryptography.CAPIBase.CRYPT_OID_INFO));
            }
            return crypt_oid_info;
        }

        internal static bool CryptMsgControl([In] System.Security.Cryptography.SafeCryptMsgHandle hCryptMsg, [In] uint dwFlags, [In] uint dwCtrlType, [In] IntPtr pvCtrlPara) => 
            System.Security.Cryptography.CAPIUnsafe.CryptMsgControl(hCryptMsg, dwFlags, dwCtrlType, pvCtrlPara);

        internal static bool CryptMsgCountersign([In] System.Security.Cryptography.SafeCryptMsgHandle hCryptMsg, [In] uint dwIndex, [In] uint cCountersigners, [In] IntPtr rgCountersigners) => 
            System.Security.Cryptography.CAPIUnsafe.CryptMsgCountersign(hCryptMsg, dwIndex, cCountersigners, rgCountersigners);

        internal static System.Security.Cryptography.SafeCryptMsgHandle CryptMsgOpenToEncode([In] uint dwMsgEncodingType, [In] uint dwFlags, [In] uint dwMsgType, [In] IntPtr pvMsgEncodeInfo, [In] IntPtr pszInnerContentObjID, [In] IntPtr pStreamInfo) => 
            System.Security.Cryptography.CAPIUnsafe.CryptMsgOpenToEncode(dwMsgEncodingType, dwFlags, dwMsgType, pvMsgEncodeInfo, pszInnerContentObjID, pStreamInfo);

        internal static System.Security.Cryptography.SafeCryptMsgHandle CryptMsgOpenToEncode([In] uint dwMsgEncodingType, [In] uint dwFlags, [In] uint dwMsgType, [In] IntPtr pvMsgEncodeInfo, [In] string pszInnerContentObjID, [In] IntPtr pStreamInfo) => 
            System.Security.Cryptography.CAPIUnsafe.CryptMsgOpenToEncode(dwMsgEncodingType, dwFlags, dwMsgType, pvMsgEncodeInfo, pszInnerContentObjID, pStreamInfo);

        internal static bool CryptProtectData([In] IntPtr pDataIn, [In] string szDataDescr, [In] IntPtr pOptionalEntropy, [In] IntPtr pvReserved, [In] IntPtr pPromptStruct, [In] uint dwFlags, [In, Out] IntPtr pDataBlob)
        {
            new DataProtectionPermission(DataProtectionPermissionFlags.ProtectData).Demand();
            return System.Security.Cryptography.CAPIUnsafe.CryptProtectData(pDataIn, szDataDescr, pOptionalEntropy, pvReserved, pPromptStruct, dwFlags, pDataBlob);
        }

        internal static System.Security.Cryptography.SafeCertContextHandle CryptUIDlgSelectCertificateW([In, Out, MarshalAs(UnmanagedType.LPStruct)] System.Security.Cryptography.CAPIBase.CRYPTUI_SELECTCERTIFICATE_STRUCTW csc)
        {
            if (!Environment.UserInteractive)
            {
                throw new InvalidOperationException(SecurityResources.GetResourceString("Environment_NotInteractive"));
            }
            new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
            return System.Security.Cryptography.CAPIUnsafe.CryptUIDlgSelectCertificateW(csc);
        }

        internal static bool CryptUIDlgViewCertificateW([In, MarshalAs(UnmanagedType.LPStruct)] System.Security.Cryptography.CAPIBase.CRYPTUI_VIEWCERTIFICATE_STRUCTW ViewInfo, [In, Out] IntPtr pfPropertiesChanged)
        {
            if (!Environment.UserInteractive)
            {
                throw new InvalidOperationException(SecurityResources.GetResourceString("Environment_NotInteractive"));
            }
            new UIPermission(UIPermissionWindow.SafeTopLevelWindows).Demand();
            return System.Security.Cryptography.CAPIUnsafe.CryptUIDlgViewCertificateW(ViewInfo, pfPropertiesChanged);
        }

        internal static bool CryptUnprotectData([In] IntPtr pDataIn, [In] IntPtr ppszDataDescr, [In] IntPtr pOptionalEntropy, [In] IntPtr pvReserved, [In] IntPtr pPromptStruct, [In] uint dwFlags, [In, Out] IntPtr pDataBlob)
        {
            new DataProtectionPermission(DataProtectionPermissionFlags.UnprotectData).Demand();
            return System.Security.Cryptography.CAPIUnsafe.CryptUnprotectData(pDataIn, ppszDataDescr, pOptionalEntropy, pvReserved, pPromptStruct, dwFlags, pDataBlob);
        }

        internal static unsafe bool DecodeObject(IntPtr pszStructType, byte[] pbEncoded, out System.Security.Cryptography.SafeLocalAllocHandle decodedValue, out uint cbDecodedValue)
        {
            decodedValue = System.Security.Cryptography.SafeLocalAllocHandle.InvalidHandle;
            cbDecodedValue = 0;
            uint num = 0;
            System.Security.Cryptography.SafeLocalAllocHandle invalidHandle = System.Security.Cryptography.SafeLocalAllocHandle.InvalidHandle;
            if (!System.Security.Cryptography.CAPISafe.CryptDecodeObject(0x10001, pszStructType, pbEncoded, (uint) pbEncoded.Length, 0, invalidHandle, new IntPtr((void*) &num)))
            {
                return false;
            }
            invalidHandle = LocalAlloc(0, new IntPtr((long) num));
            if (!System.Security.Cryptography.CAPISafe.CryptDecodeObject(0x10001, pszStructType, pbEncoded, (uint) pbEncoded.Length, 0, invalidHandle, new IntPtr((void*) &num)))
            {
                return false;
            }
            decodedValue = invalidHandle;
            cbDecodedValue = num;
            return true;
        }

        internal static unsafe bool DecodeObject(IntPtr pszStructType, IntPtr pbEncoded, uint cbEncoded, out System.Security.Cryptography.SafeLocalAllocHandle decodedValue, out uint cbDecodedValue)
        {
            decodedValue = System.Security.Cryptography.SafeLocalAllocHandle.InvalidHandle;
            cbDecodedValue = 0;
            uint num = 0;
            System.Security.Cryptography.SafeLocalAllocHandle invalidHandle = System.Security.Cryptography.SafeLocalAllocHandle.InvalidHandle;
            if (!System.Security.Cryptography.CAPISafe.CryptDecodeObject(0x10001, pszStructType, pbEncoded, cbEncoded, 0, invalidHandle, new IntPtr((void*) &num)))
            {
                return false;
            }
            invalidHandle = LocalAlloc(0, new IntPtr((long) num));
            if (!System.Security.Cryptography.CAPISafe.CryptDecodeObject(0x10001, pszStructType, pbEncoded, cbEncoded, 0, invalidHandle, new IntPtr((void*) &num)))
            {
                return false;
            }
            decodedValue = invalidHandle;
            cbDecodedValue = num;
            return true;
        }

        internal static unsafe bool EncodeObject(IntPtr lpszStructType, IntPtr pvStructInfo, out byte[] encodedData)
        {
            encodedData = new byte[0];
            uint num = 0;
            System.Security.Cryptography.SafeLocalAllocHandle invalidHandle = System.Security.Cryptography.SafeLocalAllocHandle.InvalidHandle;
            if (!System.Security.Cryptography.CAPISafe.CryptEncodeObject(0x10001, lpszStructType, pvStructInfo, invalidHandle, new IntPtr((void*) &num)))
            {
                return false;
            }
            invalidHandle = LocalAlloc(0, new IntPtr((long) num));
            if (!System.Security.Cryptography.CAPISafe.CryptEncodeObject(0x10001, lpszStructType, pvStructInfo, invalidHandle, new IntPtr((void*) &num)))
            {
                return false;
            }
            encodedData = new byte[num];
            Marshal.Copy(invalidHandle.DangerousGetHandle(), encodedData, 0, (int) num);
            invalidHandle.Dispose();
            return true;
        }

        internal static unsafe bool EncodeObject(string lpszStructType, IntPtr pvStructInfo, out byte[] encodedData)
        {
            encodedData = new byte[0];
            uint num = 0;
            System.Security.Cryptography.SafeLocalAllocHandle invalidHandle = System.Security.Cryptography.SafeLocalAllocHandle.InvalidHandle;
            if (!System.Security.Cryptography.CAPISafe.CryptEncodeObject(0x10001, lpszStructType, pvStructInfo, invalidHandle, new IntPtr((void*) &num)))
            {
                return false;
            }
            invalidHandle = LocalAlloc(0, new IntPtr((long) num));
            if (!System.Security.Cryptography.CAPISafe.CryptEncodeObject(0x10001, lpszStructType, pvStructInfo, invalidHandle, new IntPtr((void*) &num)))
            {
                return false;
            }
            encodedData = new byte[num];
            Marshal.Copy(invalidHandle.DangerousGetHandle(), encodedData, 0, (int) num);
            invalidHandle.Dispose();
            return true;
        }

        internal static System.Security.Cryptography.SafeLocalAllocHandle LocalAlloc(uint uFlags, IntPtr sizetdwBytes)
        {
            System.Security.Cryptography.SafeLocalAllocHandle handle = System.Security.Cryptography.CAPISafe.LocalAlloc(uFlags, sizetdwBytes);
            if ((handle == null) || handle.IsInvalid)
            {
                throw new OutOfMemoryException();
            }
            return handle;
        }

        internal static int SystemFunction040([In, Out] byte[] pDataIn, [In] uint cbDataIn, [In] uint dwFlags)
        {
            new DataProtectionPermission(DataProtectionPermissionFlags.ProtectMemory).Demand();
            return System.Security.Cryptography.CAPIUnsafe.SystemFunction040(pDataIn, cbDataIn, dwFlags);
        }

        internal static int SystemFunction041([In, Out] byte[] pDataIn, [In] uint cbDataIn, [In] uint dwFlags)
        {
            new DataProtectionPermission(DataProtectionPermissionFlags.UnprotectMemory).Demand();
            return System.Security.Cryptography.CAPIUnsafe.SystemFunction041(pDataIn, cbDataIn, dwFlags);
        }
    }
}

