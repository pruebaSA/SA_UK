namespace MS.Internal.Security.RightsManagement
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    internal static class SafeNativeMethods
    {
        internal static int DRMAcquireLicense(SafeRightsManagementSessionHandle hSession, uint uFlags, string GroupIdentityCredential, string RequestedRights, string CustomData, string url, IntPtr context)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMAcquireLicense(hSession, uFlags, GroupIdentityCredential, RequestedRights, CustomData, url, context);
        }

        internal static int DRMActivate(SafeRightsManagementSessionHandle hSession, uint uFlags, uint uLangID, ActivationServerInfo activationServerInfo, IntPtr context, IntPtr parentWindowHandle)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMActivate(hSession, uFlags, uLangID, activationServerInfo, context, parentWindowHandle);
        }

        internal static int DRMAddRightWithUser(SafeRightsManagementPubHandle issuanceLicenseHandle, SafeRightsManagementPubHandle rightHandle, SafeRightsManagementPubHandle userHandle)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMAddRightWithUser(issuanceLicenseHandle, rightHandle, userHandle);
        }

        internal static int DRMCloseEnvironmentHandle(uint envHandle)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMCloseEnvironmentHandle(envHandle);
        }

        internal static int DRMCloseHandle(uint handle)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMCloseHandle(handle);
        }

        internal static int DRMClosePubHandle(uint pubHandle)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMClosePubHandle(pubHandle);
        }

        internal static int DRMCloseQueryHandle(uint queryHandle)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMCloseQueryHandle(queryHandle);
        }

        internal static int DRMCloseSession(uint sessionHandle)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMCloseSession(sessionHandle);
        }

        internal static int DRMCreateBoundLicense(SafeRightsManagementEnvironmentHandle environmentHandle, BoundLicenseParams boundLicenseParams, string licenseChain, out SafeRightsManagementHandle boundLicenseHandle, out uint errorLogHandle)
        {
            SecurityHelper.DemandRightsManagementPermission();
            int num = UnsafeNativeMethods.DRMCreateBoundLicense(environmentHandle, boundLicenseParams, licenseChain, out boundLicenseHandle, out errorLogHandle);
            if ((boundLicenseHandle != null) && boundLicenseHandle.IsInvalid)
            {
                boundLicenseHandle.Dispose();
                boundLicenseHandle = null;
            }
            return num;
        }

        internal static int DRMCreateClientSession(CallbackDelegate pfnCallback, uint uCallbackVersion, string GroupIDProviderType, string GroupID, out SafeRightsManagementSessionHandle phSession)
        {
            SecurityHelper.DemandRightsManagementPermission();
            int num = UnsafeNativeMethods.DRMCreateClientSession(pfnCallback, uCallbackVersion, GroupIDProviderType, GroupID, out phSession);
            if ((phSession != null) && phSession.IsInvalid)
            {
                phSession.Dispose();
                phSession = null;
            }
            return num;
        }

        internal static int DRMCreateEnablingBitsDecryptor(SafeRightsManagementHandle boundLicenseHandle, string right, uint auxLibrary, string auxPlugin, out SafeRightsManagementHandle decryptorHandle)
        {
            SecurityHelper.DemandRightsManagementPermission();
            int num = UnsafeNativeMethods.DRMCreateEnablingBitsDecryptor(boundLicenseHandle, right, auxLibrary, auxPlugin, out decryptorHandle);
            if ((decryptorHandle != null) && decryptorHandle.IsInvalid)
            {
                decryptorHandle.Dispose();
                decryptorHandle = null;
            }
            return num;
        }

        internal static int DRMCreateEnablingBitsEncryptor(SafeRightsManagementHandle boundLicenseHandle, string right, uint auxLibrary, string auxPlugin, out SafeRightsManagementHandle encryptorHandle)
        {
            SecurityHelper.DemandRightsManagementPermission();
            int num = UnsafeNativeMethods.DRMCreateEnablingBitsEncryptor(boundLicenseHandle, right, auxLibrary, auxPlugin, out encryptorHandle);
            if ((encryptorHandle != null) && encryptorHandle.IsInvalid)
            {
                encryptorHandle.Dispose();
                encryptorHandle = null;
            }
            return num;
        }

        internal static int DRMCreateIssuanceLicense(SystemTime timeFrom, SystemTime timeUntil, string referralInfoName, string referralInfoUrl, SafeRightsManagementPubHandle ownerUserHandle, string issuanceLicense, SafeRightsManagementHandle boundLicenseHandle, out SafeRightsManagementPubHandle issuanceLicenseHandle)
        {
            SecurityHelper.DemandRightsManagementPermission();
            int num = UnsafeNativeMethods.DRMCreateIssuanceLicense(timeFrom, timeUntil, referralInfoName, referralInfoUrl, ownerUserHandle, issuanceLicense, boundLicenseHandle, out issuanceLicenseHandle);
            if ((issuanceLicenseHandle != null) && issuanceLicenseHandle.IsInvalid)
            {
                issuanceLicenseHandle.Dispose();
                issuanceLicenseHandle = null;
            }
            return num;
        }

        internal static int DRMCreateLicenseStorageSession(SafeRightsManagementEnvironmentHandle hEnv, SafeRightsManagementHandle hDefLib, SafeRightsManagementSessionHandle hClientSession, uint uFlags, string IssuanceLicense, out SafeRightsManagementSessionHandle phLicenseStorageSession)
        {
            SecurityHelper.DemandRightsManagementPermission();
            int num = UnsafeNativeMethods.DRMCreateLicenseStorageSession(hEnv, hDefLib, hClientSession, uFlags, IssuanceLicense, out phLicenseStorageSession);
            if ((phLicenseStorageSession != null) && phLicenseStorageSession.IsInvalid)
            {
                phLicenseStorageSession.Dispose();
                phLicenseStorageSession = null;
            }
            return num;
        }

        internal static int DRMCreateRight(string rightName, SystemTime timeFrom, SystemTime timeUntil, uint countExtendedInfo, string[] extendedInfoNames, string[] extendedInfoValues, out SafeRightsManagementPubHandle rightHandle)
        {
            SecurityHelper.DemandRightsManagementPermission();
            int num = UnsafeNativeMethods.DRMCreateRight(rightName, timeFrom, timeUntil, countExtendedInfo, extendedInfoNames, extendedInfoValues, out rightHandle);
            if ((rightHandle != null) && rightHandle.IsInvalid)
            {
                rightHandle.Dispose();
                rightHandle = null;
            }
            return num;
        }

        internal static int DRMCreateUser(string userName, string userId, string userIdType, out SafeRightsManagementPubHandle userHandle)
        {
            SecurityHelper.DemandRightsManagementPermission();
            int num = UnsafeNativeMethods.DRMCreateUser(userName, userId, userIdType, out userHandle);
            if ((userHandle != null) && userHandle.IsInvalid)
            {
                userHandle.Dispose();
                userHandle = null;
            }
            return num;
        }

        internal static int DRMDeconstructCertificateChain(string chain, uint index, ref uint certificateLength, StringBuilder certificate)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMDeconstructCertificateChain(chain, index, ref certificateLength, certificate);
        }

        internal static int DRMDecrypt(SafeRightsManagementHandle cryptoProvHandle, uint position, uint inputByteCount, byte[] inputBuffer, ref uint outputByteCount, byte[] outputBuffer)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMDecrypt(cryptoProvHandle, position, inputByteCount, inputBuffer, ref outputByteCount, outputBuffer);
        }

        internal static int DRMDeleteLicense(SafeRightsManagementSessionHandle hSession, string wszLicenseId)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMDeleteLicense(hSession, wszLicenseId);
        }

        internal static int DRMEncrypt(SafeRightsManagementHandle cryptoProvHandle, uint position, uint inputByteCount, byte[] inputBuffer, ref uint outputByteCount, byte[] outputBuffer)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMEncrypt(cryptoProvHandle, position, inputByteCount, inputBuffer, ref outputByteCount, outputBuffer);
        }

        internal static int DRMEnumerateLicense(SafeRightsManagementSessionHandle hSession, uint uFlags, uint uIndex, ref bool pfSharedFlag, ref uint puCertDataLen, StringBuilder wszCertificateData)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMEnumerateLicense(hSession, uFlags, uIndex, ref pfSharedFlag, ref puCertDataLen, wszCertificateData);
        }

        internal static int DRMGetApplicationSpecificData(SafeRightsManagementPubHandle issuanceLicenseHandle, uint index, ref uint nameLength, StringBuilder name, ref uint valueLength, StringBuilder value)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMGetApplicationSpecificData(issuanceLicenseHandle, index, ref nameLength, name, ref valueLength, value);
        }

        internal static int DRMGetBoundLicenseAttribute(SafeRightsManagementHandle queryRootHandle, string attributeType, uint index, out uint encodingType, ref uint bufferSize, byte[] buffer)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMGetBoundLicenseAttribute(queryRootHandle, attributeType, index, out encodingType, ref bufferSize, buffer);
        }

        internal static int DRMGetBoundLicenseObject(SafeRightsManagementHandle queryRootHandle, string subObjectType, uint index, out SafeRightsManagementHandle subQueryHandle)
        {
            SecurityHelper.DemandRightsManagementPermission();
            int num = UnsafeNativeMethods.DRMGetBoundLicenseObject(queryRootHandle, subObjectType, index, out subQueryHandle);
            if ((subQueryHandle != null) && subQueryHandle.IsInvalid)
            {
                subQueryHandle.Dispose();
                subQueryHandle = null;
            }
            return num;
        }

        internal static int DRMGetInfo(SafeRightsManagementHandle handle, string attributeType, out uint encodingType, ref uint outputByteCount, byte[] outputBuffer)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMGetInfo(handle, attributeType, out encodingType, ref outputByteCount, outputBuffer);
        }

        internal static int DRMGetIntervalTime(SafeRightsManagementPubHandle issuanceLicenseHandle, ref uint days)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMGetIntervalTime(issuanceLicenseHandle, ref days);
        }

        internal static int DRMGetIssuanceLicenseInfo(SafeRightsManagementPubHandle issuanceLicenseHandle, SystemTime timeFrom, SystemTime timeUntil, uint flags, ref uint distributionPointNameLength, StringBuilder DistributionPointName, ref uint distributionPointUriLength, StringBuilder DistributionPointUri, out SafeRightsManagementPubHandle ownerHandle, out bool officialFlag)
        {
            SecurityHelper.DemandRightsManagementPermission();
            int num = UnsafeNativeMethods.DRMGetIssuanceLicenseInfo(issuanceLicenseHandle, timeFrom, timeUntil, flags, ref distributionPointNameLength, DistributionPointName, ref distributionPointUriLength, DistributionPointUri, out ownerHandle, out officialFlag);
            if ((ownerHandle != null) && ownerHandle.IsInvalid)
            {
                ownerHandle.Dispose();
                ownerHandle = null;
            }
            return num;
        }

        internal static int DRMGetIssuanceLicenseTemplate(SafeRightsManagementPubHandle issuanceLicenseHandle, ref uint issuanceLicenseTemplateLength, StringBuilder issuanceLicenseTemplate)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMGetIssuanceLicenseTemplate(issuanceLicenseHandle, ref issuanceLicenseTemplateLength, issuanceLicenseTemplate);
        }

        internal static int DRMGetNameAndDescription(SafeRightsManagementPubHandle issuanceLicenseHandle, uint uIndex, out uint localeId, ref uint nameLength, StringBuilder name, ref uint descriptionLength, StringBuilder description)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMGetNameAndDescription(issuanceLicenseHandle, uIndex, out localeId, ref nameLength, name, ref descriptionLength, description);
        }

        internal static int DRMGetOwnerLicense(SafeRightsManagementPubHandle issuanceLicenseHandle, ref uint ownerLicenseLength, StringBuilder ownerLicense)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMGetOwnerLicense(issuanceLicenseHandle, ref ownerLicenseLength, ownerLicense);
        }

        internal static int DRMGetRevocationPoint(SafeRightsManagementPubHandle issuanceLicenseHandle, ref uint idLength, StringBuilder id, ref uint idTypeLength, StringBuilder idType, ref uint urlLength, StringBuilder url, SystemTime frequency, ref uint nameLength, StringBuilder name, ref uint publicKeyLength, StringBuilder publicKey)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMGetRevocationPoint(issuanceLicenseHandle, ref idLength, id, ref idTypeLength, idType, ref urlLength, url, frequency, ref nameLength, name, ref publicKeyLength, publicKey);
        }

        internal static int DRMGetRightInfo(SafeRightsManagementPubHandle rightHandle, ref uint rightNameLength, StringBuilder rightName, SystemTime timeFrom, SystemTime timeUntil)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMGetRightInfo(rightHandle, ref rightNameLength, rightName, timeFrom, timeUntil);
        }

        internal static int DRMGetSecurityProvider(uint flags, ref uint typeLength, StringBuilder type, ref uint pathLength, StringBuilder path)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMGetSecurityProvider(flags, ref typeLength, type, ref pathLength, path);
        }

        internal static int DRMGetServiceLocation(SafeRightsManagementSessionHandle clientSessionHandle, uint serviceType, uint serviceLocation, string issuanceLicense, ref uint serviceUrlLength, StringBuilder serviceUrl)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMGetServiceLocation(clientSessionHandle, serviceType, serviceLocation, issuanceLicense, ref serviceUrlLength, serviceUrl);
        }

        internal static int DRMGetSignedIssuanceLicense(SafeRightsManagementEnvironmentHandle environmentHandle, SafeRightsManagementPubHandle issuanceLicenseHandle, uint flags, byte[] symmetricKey, uint symmetricKeyByteCount, string symmetricKeyType, string clientLicensorCertificate, CallbackDelegate pfnCallback, string Url, uint context)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMGetSignedIssuanceLicense(environmentHandle, issuanceLicenseHandle, flags, symmetricKey, symmetricKeyByteCount, symmetricKeyType, clientLicensorCertificate, pfnCallback, Url, context);
        }

        internal static int DRMGetUnboundLicenseAttribute(SafeRightsManagementQueryHandle queryRootHandle, string attributeType, uint index, out uint encodingType, ref uint bufferSize, byte[] buffer)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMGetUnboundLicenseAttribute(queryRootHandle, attributeType, index, out encodingType, ref bufferSize, buffer);
        }

        internal static int DRMGetUnboundLicenseObject(SafeRightsManagementQueryHandle queryRootHandle, string subObjectType, uint index, out SafeRightsManagementQueryHandle subQueryHandle)
        {
            SecurityHelper.DemandRightsManagementPermission();
            int num = UnsafeNativeMethods.DRMGetUnboundLicenseObject(queryRootHandle, subObjectType, index, out subQueryHandle);
            if ((subQueryHandle != null) && subQueryHandle.IsInvalid)
            {
                subQueryHandle.Dispose();
                subQueryHandle = null;
            }
            return num;
        }

        internal static int DRMGetUnboundLicenseObjectCount(SafeRightsManagementQueryHandle queryRootHandle, string subObjectType, out uint objectCount)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMGetUnboundLicenseObjectCount(queryRootHandle, subObjectType, out objectCount);
        }

        internal static int DRMGetUserInfo(SafeRightsManagementPubHandle userHandle, ref uint userNameLength, StringBuilder userName, ref uint userIdLength, StringBuilder userId, ref uint userIdTypeLength, StringBuilder userIdType)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMGetUserInfo(userHandle, ref userNameLength, userName, ref userIdLength, userId, ref userIdTypeLength, userIdType);
        }

        internal static int DRMGetUserRights(SafeRightsManagementPubHandle issuanceLicenseHandle, SafeRightsManagementPubHandle userHandle, uint index, out SafeRightsManagementPubHandle rightHandle)
        {
            SecurityHelper.DemandRightsManagementPermission();
            int num = UnsafeNativeMethods.DRMGetUserRights(issuanceLicenseHandle, userHandle, index, out rightHandle);
            if ((rightHandle != null) && rightHandle.IsInvalid)
            {
                rightHandle.Dispose();
                rightHandle = null;
            }
            return num;
        }

        internal static int DRMGetUsers(SafeRightsManagementPubHandle issuanceLicenseHandle, uint index, out SafeRightsManagementPubHandle userHandle)
        {
            SecurityHelper.DemandRightsManagementPermission();
            int num = UnsafeNativeMethods.DRMGetUsers(issuanceLicenseHandle, index, out userHandle);
            if ((userHandle != null) && userHandle.IsInvalid)
            {
                userHandle.Dispose();
                userHandle = null;
            }
            return num;
        }

        internal static int DRMInitEnvironment(uint eSecurityProviderType, uint eSpecification, string securityProvider, string manifestCredentials, string machineCredentials, out SafeRightsManagementEnvironmentHandle environmentHandle, out SafeRightsManagementHandle defaultLibrary)
        {
            SecurityHelper.DemandRightsManagementPermission();
            int num = UnsafeNativeMethods.DRMInitEnvironment(eSecurityProviderType, eSpecification, securityProvider, manifestCredentials, machineCredentials, out environmentHandle, out defaultLibrary);
            if ((environmentHandle != null) && environmentHandle.IsInvalid)
            {
                environmentHandle.Dispose();
                environmentHandle = null;
            }
            if ((defaultLibrary != null) && defaultLibrary.IsInvalid)
            {
                defaultLibrary.Dispose();
                defaultLibrary = null;
            }
            return num;
        }

        internal static int DRMIsActivated(SafeRightsManagementSessionHandle hSession, uint uFlags, ActivationServerInfo activationServerInfo)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMIsActivated(hSession, uFlags, activationServerInfo);
        }

        internal static int DRMParseUnboundLicense(string certificate, out SafeRightsManagementQueryHandle queryRootHandle)
        {
            SecurityHelper.DemandRightsManagementPermission();
            int num = UnsafeNativeMethods.DRMParseUnboundLicense(certificate, out queryRootHandle);
            if ((queryRootHandle != null) && queryRootHandle.IsInvalid)
            {
                queryRootHandle.Dispose();
                queryRootHandle = null;
            }
            return num;
        }

        internal static int DRMSetApplicationSpecificData(SafeRightsManagementPubHandle issuanceLicenseHandle, bool flagDelete, string name, string value)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMSetApplicationSpecificData(issuanceLicenseHandle, flagDelete, name, value);
        }

        internal static int DRMSetIntervalTime(SafeRightsManagementPubHandle issuanceLicenseHandle, uint days)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMSetIntervalTime(issuanceLicenseHandle, days);
        }

        internal static int DRMSetMetaData(SafeRightsManagementPubHandle issuanceLicenseHandle, string contentId, string contentIdType, string SkuId, string SkuIdType, string contentType, string contentName)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMSetMetaData(issuanceLicenseHandle, contentId, contentIdType, SkuId, SkuIdType, contentType, contentName);
        }

        internal static int DRMSetNameAndDescription(SafeRightsManagementPubHandle issuanceLicenseHandle, bool flagDelete, uint localeId, string name, string description)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMSetNameAndDescription(issuanceLicenseHandle, flagDelete, localeId, name, description);
        }

        internal static int DRMSetRevocationPoint(SafeRightsManagementPubHandle issuanceLicenseHandle, bool flagDelete, string id, string idType, string url, SystemTime frequency, string name, string publicKey)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return UnsafeNativeMethods.DRMSetRevocationPoint(issuanceLicenseHandle, flagDelete, id, idType, url, frequency, name, publicKey);
        }

        [SuppressUnmanagedCodeSecurity, SecurityCritical(SecurityCriticalScope.Everything)]
        private static class UnsafeNativeMethods
        {
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMAcquireLicense([In] SafeRightsManagementSessionHandle hSession, [In, MarshalAs(UnmanagedType.U4)] uint uFlags, [In, MarshalAs(UnmanagedType.LPWStr)] string GroupIdentityCredential, [In, MarshalAs(UnmanagedType.LPWStr)] string RequestedRights, [In, MarshalAs(UnmanagedType.LPWStr)] string CustomData, [In, MarshalAs(UnmanagedType.LPWStr)] string url, IntPtr context);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMActivate([In] SafeRightsManagementSessionHandle hSession, [In, MarshalAs(UnmanagedType.U4)] uint uFlags, [In, MarshalAs(UnmanagedType.U4)] uint uLangID, [In, MarshalAs(UnmanagedType.LPStruct)] ActivationServerInfo activationServerInfo, IntPtr context, IntPtr parentWindowHandle);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMAddRightWithUser([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [In] SafeRightsManagementPubHandle rightHandle, [In] SafeRightsManagementPubHandle userHandle);
            [DllImport("PresentationHostDll.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMCloseEnvironmentHandle([In, MarshalAs(UnmanagedType.U4)] uint envHandle);
            [DllImport("PresentationHostDll.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMCloseHandle([In, MarshalAs(UnmanagedType.U4)] uint handle);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMClosePubHandle([In, MarshalAs(UnmanagedType.U4)] uint pubHandle);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMCloseQueryHandle([In, MarshalAs(UnmanagedType.U4)] uint queryHandle);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMCloseSession([In, MarshalAs(UnmanagedType.U4)] uint sessionHandle);
            [DllImport("PresentationHostDll.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMCreateBoundLicense([In] SafeRightsManagementEnvironmentHandle environmentHandle, [In, MarshalAs(UnmanagedType.LPStruct)] BoundLicenseParams boundLicenseParams, [In, MarshalAs(UnmanagedType.LPWStr)] string licenseChain, out SafeRightsManagementHandle boundLicenseHandle, [MarshalAs(UnmanagedType.U4)] out uint errorLogHandle);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMCreateClientSession([In, MarshalAs(UnmanagedType.FunctionPtr)] CallbackDelegate pfnCallback, [In, MarshalAs(UnmanagedType.U4)] uint uCallbackVersion, [In, MarshalAs(UnmanagedType.LPWStr)] string GroupIDProviderType, [In, MarshalAs(UnmanagedType.LPWStr)] string GroupID, out SafeRightsManagementSessionHandle phSession);
            [DllImport("PresentationHostDll.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMCreateEnablingBitsDecryptor([In] SafeRightsManagementHandle boundLicenseHandle, [In, MarshalAs(UnmanagedType.LPWStr)] string right, [In, MarshalAs(UnmanagedType.U4)] uint auxLibrary, [In, MarshalAs(UnmanagedType.LPWStr)] string auxPlugin, out SafeRightsManagementHandle decryptorHandle);
            [DllImport("PresentationHostDll.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMCreateEnablingBitsEncryptor([In] SafeRightsManagementHandle boundLicenseHandle, [In, MarshalAs(UnmanagedType.LPWStr)] string right, [In, MarshalAs(UnmanagedType.U4)] uint auxLibrary, [In, MarshalAs(UnmanagedType.LPWStr)] string auxPlugin, out SafeRightsManagementHandle encryptorHandle);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMCreateIssuanceLicense([In, MarshalAs(UnmanagedType.LPStruct)] SystemTime timeFrom, [In, MarshalAs(UnmanagedType.LPStruct)] SystemTime timeUntil, [In, MarshalAs(UnmanagedType.LPWStr)] string referralInfoName, [In, MarshalAs(UnmanagedType.LPWStr)] string referralInfoUrl, [In] SafeRightsManagementPubHandle ownerUserHandle, [In, MarshalAs(UnmanagedType.LPWStr)] string issuanceLicense, [In] SafeRightsManagementHandle boundLicenseHandle, out SafeRightsManagementPubHandle issuanceLicenseHandle);
            [DllImport("PresentationHostDll.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMCreateLicenseStorageSession([In] SafeRightsManagementEnvironmentHandle envHandle, [In] SafeRightsManagementHandle hDefLib, [In] SafeRightsManagementSessionHandle hClientSession, [In, MarshalAs(UnmanagedType.U4)] uint uFlags, [In, MarshalAs(UnmanagedType.LPWStr)] string IssuanceLicense, out SafeRightsManagementSessionHandle phLicenseStorageSession);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMCreateRight([In, MarshalAs(UnmanagedType.LPWStr)] string rightName, [In, MarshalAs(UnmanagedType.LPStruct)] SystemTime timeFrom, [In, MarshalAs(UnmanagedType.LPStruct)] SystemTime timeUntil, [In, MarshalAs(UnmanagedType.U4)] uint countExtendedInfo, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=4)] string[] extendedInfoNames, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=4)] string[] extendedInfoValues, out SafeRightsManagementPubHandle rightHandle);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMCreateUser([In, MarshalAs(UnmanagedType.LPWStr)] string userName, [In, MarshalAs(UnmanagedType.LPWStr)] string userId, [In, MarshalAs(UnmanagedType.LPWStr)] string userIdType, out SafeRightsManagementPubHandle userHandle);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMDeconstructCertificateChain([In, MarshalAs(UnmanagedType.LPWStr)] string chain, [In, MarshalAs(UnmanagedType.U4)] uint index, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint certificateLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder certificate);
            [DllImport("PresentationHostDll.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMDecrypt([In] SafeRightsManagementHandle cryptoProvHandle, [In, MarshalAs(UnmanagedType.U4)] uint position, [In, MarshalAs(UnmanagedType.U4)] uint inputByteCount, byte[] inputBuffer, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint outputByteCount, byte[] outputBuffer);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMDeleteLicense([In] SafeRightsManagementSessionHandle hSession, [In, MarshalAs(UnmanagedType.LPWStr)] string wszLicenseId);
            [DllImport("PresentationHostDll.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMEncrypt([In] SafeRightsManagementHandle cryptoProvHandle, [In, MarshalAs(UnmanagedType.U4)] uint position, [In, MarshalAs(UnmanagedType.U4)] uint inputByteCount, byte[] inputBuffer, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint outputByteCount, byte[] outputBuffer);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMEnumerateLicense([In] SafeRightsManagementSessionHandle hSession, [In, MarshalAs(UnmanagedType.U4)] uint uFlags, [In, MarshalAs(UnmanagedType.U4)] uint uIndex, [In, Out, MarshalAs(UnmanagedType.Bool)] ref bool pfSharedFlag, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint puCertDataLen, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder wszCertificateData);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetApplicationSpecificData([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [In, MarshalAs(UnmanagedType.U4)] uint index, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint nameLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint valueLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder value);
            [DllImport("PresentationHostDll.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetBoundLicenseAttribute([In] SafeRightsManagementHandle queryRootHandle, [In, MarshalAs(UnmanagedType.LPWStr)] string attributeType, [In, MarshalAs(UnmanagedType.U4)] uint index, [MarshalAs(UnmanagedType.U4)] out uint encodingType, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint bufferSize, byte[] buffer);
            [DllImport("PresentationHostDll.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetBoundLicenseObject([In] SafeRightsManagementHandle queryRootHandle, [In, MarshalAs(UnmanagedType.LPWStr)] string subObjectType, [In, MarshalAs(UnmanagedType.U4)] uint index, out SafeRightsManagementHandle subQueryHandle);
            [DllImport("PresentationHostDll.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetInfo([In] SafeRightsManagementHandle handle, [In, MarshalAs(UnmanagedType.LPWStr)] string attributeType, [MarshalAs(UnmanagedType.U4)] out uint encodingType, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint outputByteCount, byte[] outputBuffer);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetIntervalTime([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint days);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetIssuanceLicenseInfo([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [MarshalAs(UnmanagedType.LPStruct)] SystemTime timeFrom, [MarshalAs(UnmanagedType.LPStruct)] SystemTime timeUntil, [In, MarshalAs(UnmanagedType.U4)] uint flags, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint distributionPointNameLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder DistributionPointName, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint distributionPointUriLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder DistributionPointUri, out SafeRightsManagementPubHandle ownerHandle, [MarshalAs(UnmanagedType.Bool)] out bool officialFlag);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetIssuanceLicenseTemplate([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint issuanceLicenseTemplateLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder issuanceLicenseTemplate);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetNameAndDescription([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [In, MarshalAs(UnmanagedType.U4)] uint uIndex, [MarshalAs(UnmanagedType.U4)] out uint localeId, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint nameLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint descriptionLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder description);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetOwnerLicense([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint ownerLicenseLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder ownerLicense);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetRevocationPoint([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint idLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder id, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint idTypeLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder idType, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint urlLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder url, [MarshalAs(UnmanagedType.LPStruct)] SystemTime frequency, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint nameLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder name, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint publicKeyLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder publicKey);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetRightInfo([In] SafeRightsManagementPubHandle rightHandle, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint rightNameLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder rightName, [MarshalAs(UnmanagedType.LPStruct)] SystemTime timeFrom, [MarshalAs(UnmanagedType.LPStruct)] SystemTime timeUntil);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetSecurityProvider([In, MarshalAs(UnmanagedType.U4)] uint flags, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint typeLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder type, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint pathLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder path);
            [DllImport("PresentationHostDll.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetServiceLocation([In] SafeRightsManagementSessionHandle clientSessionHandle, [In, MarshalAs(UnmanagedType.U4)] uint serviceType, [In, MarshalAs(UnmanagedType.U4)] uint serviceLocation, [In, MarshalAs(UnmanagedType.LPWStr)] string issuanceLicense, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint serviceUrlLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder serviceUrl);
            [DllImport("PresentationHostDll.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetSignedIssuanceLicense([In] SafeRightsManagementEnvironmentHandle environmentHandle, [In] SafeRightsManagementPubHandle issuanceLicenseHandle, [In, MarshalAs(UnmanagedType.U4)] uint flags, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=4)] byte[] symmetricKey, [In, MarshalAs(UnmanagedType.U4)] uint symmetricKeyByteCount, [In, MarshalAs(UnmanagedType.LPWStr)] string symmetricKeyType, [In, MarshalAs(UnmanagedType.LPWStr)] string clientLicensorCertificate, [In, MarshalAs(UnmanagedType.FunctionPtr)] CallbackDelegate pfnCallback, [In, MarshalAs(UnmanagedType.LPWStr)] string url, [In, MarshalAs(UnmanagedType.U4)] uint context);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetUnboundLicenseAttribute([In] SafeRightsManagementQueryHandle queryRootHandle, [In, MarshalAs(UnmanagedType.LPWStr)] string attributeType, [In, MarshalAs(UnmanagedType.U4)] uint index, [MarshalAs(UnmanagedType.U4)] out uint encodingType, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint bufferSize, byte[] buffer);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetUnboundLicenseObject([In] SafeRightsManagementQueryHandle queryRootHandle, [In, MarshalAs(UnmanagedType.LPWStr)] string subObjectType, [In, MarshalAs(UnmanagedType.U4)] uint index, out SafeRightsManagementQueryHandle subQueryHandle);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetUnboundLicenseObjectCount([In] SafeRightsManagementQueryHandle queryRootHandle, [In, MarshalAs(UnmanagedType.LPWStr)] string subObjectType, [MarshalAs(UnmanagedType.U4)] out uint objectCount);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetUserInfo([In] SafeRightsManagementPubHandle userHandle, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint userNameLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder userName, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint userIdLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder userId, [In, Out, MarshalAs(UnmanagedType.U4)] ref uint userIdTypeLength, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder userIdType);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetUserRights([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [In] SafeRightsManagementPubHandle userHandle, [In, MarshalAs(UnmanagedType.U4)] uint index, out SafeRightsManagementPubHandle rightHandle);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMGetUsers([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [In, MarshalAs(UnmanagedType.U4)] uint index, out SafeRightsManagementPubHandle userHandle);
            [DllImport("PresentationHostDll.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMInitEnvironment([In, MarshalAs(UnmanagedType.U4)] uint eSecurityProviderType, [In, MarshalAs(UnmanagedType.U4)] uint eSpecification, [In, MarshalAs(UnmanagedType.LPWStr)] string securityProvider, [In, MarshalAs(UnmanagedType.LPWStr)] string manifestCredentials, [In, MarshalAs(UnmanagedType.LPWStr)] string machineCredentials, out SafeRightsManagementEnvironmentHandle environmentHandle, out SafeRightsManagementHandle defaultLibrary);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMIsActivated([In] SafeRightsManagementSessionHandle hSession, [In, MarshalAs(UnmanagedType.U4)] uint uFlags, [In, MarshalAs(UnmanagedType.LPStruct)] ActivationServerInfo activationServerInfo);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMParseUnboundLicense([In, MarshalAs(UnmanagedType.LPWStr)] string certificate, out SafeRightsManagementQueryHandle queryRootHandle);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMSetApplicationSpecificData([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [In, MarshalAs(UnmanagedType.Bool)] bool flagDelete, [In, MarshalAs(UnmanagedType.LPWStr)] string name, [In, MarshalAs(UnmanagedType.LPWStr)] string value);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMSetIntervalTime([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [In, MarshalAs(UnmanagedType.U4)] uint days);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMSetMetaData([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [In, MarshalAs(UnmanagedType.LPWStr)] string contentId, [In, MarshalAs(UnmanagedType.LPWStr)] string contentIdType, [In, MarshalAs(UnmanagedType.LPWStr)] string SkuId, [In, MarshalAs(UnmanagedType.LPWStr)] string SkuIdType, [In, MarshalAs(UnmanagedType.LPWStr)] string contentType, [In, MarshalAs(UnmanagedType.LPWStr)] string contentName);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMSetNameAndDescription([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [In, MarshalAs(UnmanagedType.Bool)] bool flagDelete, [In, MarshalAs(UnmanagedType.U4)] uint localeId, [In, MarshalAs(UnmanagedType.LPWStr)] string name, [In, MarshalAs(UnmanagedType.LPWStr)] string description);
            [DllImport("msdrm.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Unicode)]
            internal static extern int DRMSetRevocationPoint([In] SafeRightsManagementPubHandle issuanceLicenseHandle, [In, MarshalAs(UnmanagedType.Bool)] bool flagDelete, [In, MarshalAs(UnmanagedType.LPWStr)] string id, [In, MarshalAs(UnmanagedType.LPWStr)] string idType, [In, MarshalAs(UnmanagedType.LPWStr)] string url, [In, MarshalAs(UnmanagedType.LPStruct)] SystemTime frequency, [In, MarshalAs(UnmanagedType.LPWStr)] string name, [In, MarshalAs(UnmanagedType.LPWStr)] string publicKey);
        }
    }
}

