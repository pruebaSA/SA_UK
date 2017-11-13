namespace MS.Internal.Security.RightsManagement
{
    using Microsoft.Win32;
    using MS.Internal;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.AccessControl;
    using System.Security.Permissions;
    using System.Security.RightsManagement;
    using System.Text;
    using System.Windows;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    internal class ClientSession : IDisposable
    {
        private CallbackHandler _callbackHandler;
        private List<CryptoProvider> _cryptoProviderList;
        private SafeRightsManagementHandle _defaultLibraryHandle;
        private const string _defaultUserName = "DefaultUser@DefaultDomain.DefaultCom";
        private const string _distributionPointLicenseAcquisitionType = "License-Acquisition-URL";
        private const string _distributionPointReferralInfoType = "Referral-Info";
        private SafeRightsManagementEnvironmentHandle _envHandle;
        private SafeRightsManagementSessionHandle _hSession;
        private const string _passportActivationRegistryFullKeyName = @"HKEY_LOCAL_MACHINE\Software\Microsoft\MSDRM\ServiceLocation\PassportActivation";
        private const string _passportActivationRegistryKeyName = @"Software\Microsoft\MSDRM\ServiceLocation\PassportActivation";
        private static ContentRight[] _rightEnums;
        private static string[] _rightNames;
        private ContentUser _user;
        private UserActivationMode _userActivationMode;

        static ClientSession()
        {
            ContentRight[] rightArray = new ContentRight[13];
            rightArray[1] = ContentRight.Edit;
            rightArray[2] = ContentRight.Print;
            rightArray[3] = ContentRight.Extract;
            rightArray[4] = ContentRight.ObjectModel;
            rightArray[5] = ContentRight.Owner;
            rightArray[6] = ContentRight.ViewRightsData;
            rightArray[7] = ContentRight.Forward;
            rightArray[8] = ContentRight.Reply;
            rightArray[9] = ContentRight.ReplyAll;
            rightArray[10] = ContentRight.Sign;
            rightArray[11] = ContentRight.DocumentEdit;
            rightArray[12] = ContentRight.Export;
            _rightEnums = rightArray;
            _rightNames = new string[] { "VIEW", "EDIT", "PRINT", "EXTRACT", "OBJMODEL", "OWNER", "VIEWRIGHTSDATA", "FORWARD", "REPLY", "REPLYALL", "SIGN", "DOCEDIT", "EXPORT" };
        }

        internal ClientSession(ContentUser user) : this(user, UserActivationMode.Permanent)
        {
        }

        internal ClientSession(ContentUser user, UserActivationMode userActivationMode)
        {
            Invariant.Assert(user != null);
            Invariant.Assert((userActivationMode == UserActivationMode.Permanent) || (userActivationMode == UserActivationMode.Temporary));
            this._user = user;
            this._userActivationMode = userActivationMode;
            this._callbackHandler = new CallbackHandler();
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMCreateClientSession(this._callbackHandler.CallbackDelegate, 1, this._user.AuthenticationProviderType, this._user.Name, out this._hSession));
            Invariant.Assert((this._hSession != null) && !this._hSession.IsInvalid);
        }

        internal void AcquireClientLicensorCertificate()
        {
            this.CheckDisposed();
            Uri clientLicensorUrl = this.GetClientLicensorUrl(this._user.AuthenticationType);
            string groupIdentityCert = this.GetGroupIdentityCert();
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMAcquireLicense(this._hSession, 0, groupIdentityCert, null, null, clientLicensorUrl.AbsoluteUri, IntPtr.Zero));
            this._callbackHandler.WaitForCompletion();
        }

        internal UseLicense AcquireUseLicense(string publishLicense, bool noUI)
        {
            this.CheckDisposed();
            Invariant.Assert(!this._envHandle.IsInvalid);
            SafeRightsManagementSessionHandle phLicenseStorageSession = null;
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMCreateLicenseStorageSession(this._envHandle, this._defaultLibraryHandle, this._hSession, 0, publishLicense, out phLicenseStorageSession));
            using (phLicenseStorageSession)
            {
                uint uFlags = 0;
                if (noUI)
                {
                    uFlags |= 0x10;
                }
                string groupIdentityCert = this.GetGroupIdentityCert();
                ArrayList oldList = EnumerateAllValuesOnSession(phLicenseStorageSession, EnumerateLicenseFlags.EulLid);
                Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMAcquireLicense(phLicenseStorageSession, uFlags, groupIdentityCert, null, null, null, IntPtr.Zero));
                this._callbackHandler.WaitForCompletion();
                ArrayList newList = EnumerateAllValuesOnSession(phLicenseStorageSession, EnumerateLicenseFlags.EulLid);
                int index = FindNewEntryIndex(oldList, newList);
                if (index < 0)
                {
                    throw new RightsManagementException(RightsManagementFailureCode.LicenseAcquisitionFailed);
                }
                return new UseLicense(GetLicenseOnSession(phLicenseStorageSession, EnumerateLicenseFlags.Eul, index));
            }
        }

        private string Activate(ActivationFlags activationFlags, Uri url)
        {
            ActivationServerInfo activationServerInfo = null;
            if (url != null)
            {
                activationServerInfo = new ActivationServerInfo {
                    PubKey = null,
                    Url = url.AbsoluteUri,
                    Version = 1
                };
            }
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMActivate(this._hSession, (uint) activationFlags, 0, activationServerInfo, IntPtr.Zero, IntPtr.Zero));
            this._callbackHandler.WaitForCompletion();
            return this._callbackHandler.CallbackData;
        }

        internal void ActivateMachine(AuthenticationType authentication)
        {
            this.CheckDisposed();
            ActivationFlags activationFlags = ActivationFlags.Machine | ActivationFlags.Silent;
            this.Activate(activationFlags, null);
        }

        internal ContentUser ActivateUser(AuthenticationType authentication, UserActivationMode userActivationMode)
        {
            this.CheckDisposed();
            ActivationFlags groupIdentity = ActivationFlags.GroupIdentity;
            if (this._user.AuthenticationType == AuthenticationType.Windows)
            {
                groupIdentity |= ActivationFlags.Silent;
            }
            if (userActivationMode == UserActivationMode.Temporary)
            {
                groupIdentity |= ActivationFlags.Temporary;
            }
            return ExtractUserFromCertificate(this.Activate(groupIdentity, this.GetCertificationUrl(authentication)));
        }

        private CryptoProvider BindUseLicense(string serializedUseLicense, List<RightNameExpirationInfoPair> unboundRightsList, BoundLicenseParams boundLicenseParams, out int theFirstHrFailureCode)
        {
            CryptoProvider provider2;
            List<SafeRightsManagementHandle> boundLicenseHandleList = new List<SafeRightsManagementHandle>(unboundRightsList.Count);
            List<RightNameExpirationInfoPair> rightsInfoList = new List<RightNameExpirationInfoPair>(unboundRightsList.Count);
            try
            {
                theFirstHrFailureCode = 0;
                foreach (RightNameExpirationInfoPair pair in unboundRightsList)
                {
                    boundLicenseParams.wszRightsRequested = pair.RightName;
                    SafeRightsManagementHandle boundLicenseHandle = null;
                    uint errorLogHandle = 0;
                    int num2 = MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMCreateBoundLicense(this._envHandle, boundLicenseParams, serializedUseLicense, out boundLicenseHandle, out errorLogHandle);
                    if ((boundLicenseHandle != null) && (num2 == 0))
                    {
                        boundLicenseHandleList.Add(boundLicenseHandle);
                        rightsInfoList.Add(pair);
                    }
                    if ((theFirstHrFailureCode == 0) && (num2 != 0))
                    {
                        theFirstHrFailureCode = num2;
                    }
                }
                if (boundLicenseHandleList.Count > 0)
                {
                    ContentUser owner = ExtractUserFromCertificateChain(boundLicenseParams.wszDefaultEnablingPrincipalCredentials);
                    CryptoProvider item = new CryptoProvider(boundLicenseHandleList, rightsInfoList, owner);
                    this.CryptoProviderList.Add(item);
                    return item;
                }
                provider2 = null;
            }
            catch
            {
                foreach (SafeRightsManagementHandle handle2 in boundLicenseHandleList)
                {
                    handle2.Dispose();
                }
                throw;
            }
            return provider2;
        }

        internal void BuildSecureEnvironment(string applicationManifest)
        {
            this.CheckDisposed();
            Invariant.Assert(this._envHandle == null);
            string securityProviderPath = GetSecurityProviderPath();
            string machineCert = this.GetMachineCert();
            this._defaultLibraryHandle = null;
            this._envHandle = null;
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMInitEnvironment(0, 1, securityProviderPath, applicationManifest, machineCert, out this._envHandle, out this._defaultLibraryHandle));
        }

        private void CheckDisposed()
        {
            if ((this._hSession == null) || this._hSession.IsInvalid)
            {
                throw new ObjectDisposedException("SecureEnvironment");
            }
        }

        internal static ClientSession DefaultUserClientSession(AuthenticationType authentication) => 
            new ClientSession(new ContentUser("DefaultUser@DefaultDomain.DefaultCom", authentication));

        internal void DeleteLicense(string licenseId)
        {
            this.CheckDisposed();
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMDeleteLicense(this._hSession, licenseId));
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    if (this._cryptoProviderList != null)
                    {
                        foreach (CryptoProvider provider in this._cryptoProviderList)
                        {
                            provider.Dispose();
                        }
                    }
                }
                finally
                {
                    this._cryptoProviderList = null;
                    try
                    {
                        if ((this._hSession != null) && !this._hSession.IsInvalid)
                        {
                            if (this._userActivationMode == UserActivationMode.Temporary)
                            {
                                this.RemoveUsersCertificates(EnumerateLicenseFlags.SpecifiedClientLicensor);
                                this.RemoveUsersCertificates(EnumerateLicenseFlags.SpecifiedGroupIdentity);
                            }
                            this._hSession.Dispose();
                        }
                    }
                    finally
                    {
                        this._hSession = null;
                        try
                        {
                            if ((this._defaultLibraryHandle != null) && !this._defaultLibraryHandle.IsInvalid)
                            {
                                this._defaultLibraryHandle.Dispose();
                            }
                        }
                        finally
                        {
                            this._defaultLibraryHandle = null;
                            try
                            {
                                if ((this._envHandle != null) && !this._envHandle.IsInvalid)
                                {
                                    this._envHandle.Dispose();
                                }
                            }
                            finally
                            {
                                this._envHandle = null;
                                try
                                {
                                    if (this._callbackHandler != null)
                                    {
                                        this._callbackHandler.Dispose();
                                    }
                                }
                                finally
                                {
                                    this._callbackHandler = null;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static ArrayList EnumerateAllValuesOnSession(SafeRightsManagementSessionHandle sessionHandle, EnumerateLicenseFlags enumerateLicenseFlags)
        {
            ArrayList list = new ArrayList(5);
            int index = 0;
            while (true)
            {
                string str = GetLicenseOnSession(sessionHandle, enumerateLicenseFlags, index);
                if (str == null)
                {
                    return list;
                }
                list.Add(str);
                index++;
            }
        }

        internal string EnumerateLicense(EnumerateLicenseFlags enumerateLicenseFlags, int index)
        {
            this.CheckDisposed();
            return GetLicenseOnSession(this._hSession, enumerateLicenseFlags, index);
        }

        internal List<string> EnumerateUsersCertificateIds(ContentUser user, EnumerateLicenseFlags certificateType)
        {
            this.CheckDisposed();
            if (((((certificateType != EnumerateLicenseFlags.Machine) && (certificateType != EnumerateLicenseFlags.GroupIdentity)) && ((certificateType != EnumerateLicenseFlags.GroupIdentityName) && (certificateType != EnumerateLicenseFlags.GroupIdentityLid))) && (((certificateType != EnumerateLicenseFlags.SpecifiedGroupIdentity) && (certificateType != EnumerateLicenseFlags.Eul)) && ((certificateType != EnumerateLicenseFlags.EulLid) && (certificateType != EnumerateLicenseFlags.ClientLicensor)))) && ((((certificateType != EnumerateLicenseFlags.ClientLicensorLid) && (certificateType != EnumerateLicenseFlags.SpecifiedClientLicensor)) && ((certificateType != EnumerateLicenseFlags.RevocationList) && (certificateType != EnumerateLicenseFlags.RevocationListLid))) && (certificateType != EnumerateLicenseFlags.Expired)))
            {
                throw new ArgumentOutOfRangeException("certificateType");
            }
            List<string> list = new List<string>();
            int index = 0;
            while (true)
            {
                string certificateChain = this.EnumerateLicense(certificateType, index);
                if (certificateChain == null)
                {
                    return list;
                }
                ContentUser userObj = ExtractUserFromCertificateChain(certificateChain);
                if (user.GenericEquals(userObj))
                {
                    list.Add(ExtractCertificateIdFromCertificateChain(certificateChain));
                }
                index++;
            }
        }

        internal static Dictionary<string, string> ExtractApplicationSpecificDataFromLicense(string useLicenseChain)
        {
            Invariant.Assert(useLicenseChain != null);
            Dictionary<string, string> dictionary = new Dictionary<string, string>(3, StringComparer.Ordinal);
            string elementFromCertificateChain = GetElementFromCertificateChain(useLicenseChain, 0);
            Invariant.Assert(elementFromCertificateChain != null);
            SafeRightsManagementQueryHandle queryRootHandle = null;
            int hr = MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMParseUnboundLicense(elementFromCertificateChain, out queryRootHandle);
            Errors.ThrowOnErrorCode(hr);
            using (queryRootHandle)
            {
                uint attributeIndex = 0;
                while (true)
                {
                    string key = GetUnboundLicenseStringAttribute(queryRootHandle, "appdata-name", attributeIndex);
                    if (key == null)
                    {
                        return dictionary;
                    }
                    Errors.ThrowOnErrorCode(hr);
                    string str3 = GetUnboundLicenseStringAttribute(queryRootHandle, "appdata-value", attributeIndex);
                    Errors.ThrowOnErrorCode(hr);
                    dictionary.Add(key, str3);
                    attributeIndex++;
                }
            }
        }

        internal static string ExtractCertificateIdFromCertificate(string certificate)
        {
            SafeRightsManagementQueryHandle queryRootHandle = null;
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMParseUnboundLicense(certificate, out queryRootHandle));
            using (queryRootHandle)
            {
                return GetUnboundLicenseStringAttribute(queryRootHandle, "id-value", 0);
            }
        }

        internal static string ExtractCertificateIdFromCertificateChain(string certificateChain)
        {
            Invariant.Assert(certificateChain != null);
            return ExtractCertificateIdFromCertificate(GetElementFromCertificateChain(certificateChain, 0));
        }

        private static DateTime ExtractIssuedTimeFromCertificate(string certificate, DateTime defaultValue)
        {
            SafeRightsManagementQueryHandle queryRootHandle = null;
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMParseUnboundLicense(certificate, out queryRootHandle));
            using (queryRootHandle)
            {
                return GetUnboundLicenseDateTimeAttribute(queryRootHandle, "issued-time", 0, defaultValue);
            }
        }

        private static DateTime ExtractIssuedTimeFromCertificateChain(string certificateChain, DateTime defaultValue)
        {
            Invariant.Assert(certificateChain != null);
            return ExtractIssuedTimeFromCertificate(GetElementFromCertificateChain(certificateChain, 0), defaultValue);
        }

        internal static ContentUser ExtractUserFromCertificate(string certificate)
        {
            SafeRightsManagementQueryHandle queryRootHandle = null;
            ContentUser user;
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMParseUnboundLicense(certificate, out queryRootHandle));
            using (queryRootHandle)
            {
                SafeRightsManagementQueryHandle subQueryHandle = null;
                Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMGetUnboundLicenseObject(queryRootHandle, "issued-principal", 0, out subQueryHandle));
                using (subQueryHandle)
                {
                    string name = GetUnboundLicenseStringAttribute(subQueryHandle, "name", 0);
                    string str2 = GetUnboundLicenseStringAttribute(subQueryHandle, "id-type", 0);
                    if (string.CompareOrdinal(AuthenticationType.Windows.ToString().ToUpper(CultureInfo.InvariantCulture), str2.ToUpper(CultureInfo.InvariantCulture)) == 0)
                    {
                        return new ContentUser(name, AuthenticationType.Windows);
                    }
                    user = new ContentUser(name, AuthenticationType.Passport);
                }
            }
            return user;
        }

        internal static ContentUser ExtractUserFromCertificateChain(string certificateChain)
        {
            Invariant.Assert(certificateChain != null);
            return ExtractUserFromCertificate(GetElementFromCertificateChain(certificateChain, 0));
        }

        ~ClientSession()
        {
            this.Dispose(false);
        }

        private static int FindNewEntryIndex(ArrayList oldList, ArrayList newList)
        {
            Invariant.Assert((oldList != null) && (newList != null));
            for (int i = 0; i < newList.Count; i++)
            {
                string strA = (string) newList[i];
                bool flag = false;
                foreach (string str2 in oldList)
                {
                    if (string.CompareOrdinal(strA, str2) == 0)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    return i;
                }
            }
            return -1;
        }

        private Uri GetCertificationUrl(AuthenticationType authentication)
        {
            Uri registryPassportCertificationUrl = null;
            if (authentication == AuthenticationType.Windows)
            {
                registryPassportCertificationUrl = this.GetServiceLocation(ServiceType.Certification, ServiceLocation.Enterprise, null);
                if (registryPassportCertificationUrl == null)
                {
                    registryPassportCertificationUrl = this.GetServiceLocation(ServiceType.Certification, ServiceLocation.Internet, null);
                }
                return registryPassportCertificationUrl;
            }
            registryPassportCertificationUrl = GetRegistryPassportCertificationUrl();
            if (registryPassportCertificationUrl == null)
            {
                registryPassportCertificationUrl = this.GetServiceLocation(ServiceType.Certification, ServiceLocation.Internet, null);
            }
            return registryPassportCertificationUrl;
        }

        private string GetClientLicensorCert() => 
            this.GetLatestCertificate(EnumerateLicenseFlags.SpecifiedClientLicensor);

        private Uri GetClientLicensorUrl(AuthenticationType authentication)
        {
            Uri uri = this.GetServiceLocation(ServiceType.ClientLicensor, ServiceLocation.Enterprise, null);
            if (uri == null)
            {
                uri = this.GetServiceLocation(ServiceType.ClientLicensor, ServiceLocation.Internet, null);
            }
            return uri;
        }

        internal static void GetContentIdFromLicense(string useLicenseChain, out string contentId, out string contentIdType)
        {
            Invariant.Assert(useLicenseChain != null);
            string elementFromCertificateChain = GetElementFromCertificateChain(useLicenseChain, 0);
            Invariant.Assert(elementFromCertificateChain != null);
            SafeRightsManagementQueryHandle queryRootHandle = null;
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMParseUnboundLicense(elementFromCertificateChain, out queryRootHandle));
            using (queryRootHandle)
            {
                SafeRightsManagementQueryHandle subQueryHandle = null;
                Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMGetUnboundLicenseObject(queryRootHandle, "work", 0, out subQueryHandle));
                using (subQueryHandle)
                {
                    contentIdType = GetUnboundLicenseStringAttribute(subQueryHandle, "id-type", 0);
                    contentId = GetUnboundLicenseStringAttribute(subQueryHandle, "id-value", 0);
                }
            }
        }

        internal static string GetContentIdFromPublishLicense(string publishLicense)
        {
            string str;
            Invariant.Assert(publishLicense != null);
            SafeRightsManagementQueryHandle queryRootHandle = null;
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMParseUnboundLicense(publishLicense, out queryRootHandle));
            using (queryRootHandle)
            {
                SafeRightsManagementQueryHandle subQueryHandle = null;
                Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMGetUnboundLicenseObject(queryRootHandle, "work", 0, out subQueryHandle));
                using (subQueryHandle)
                {
                    str = GetUnboundLicenseStringAttribute(subQueryHandle, "id-value", 0);
                }
            }
            return str;
        }

        private static void GetDistributionPointInfoFromPublishLicense(string publishLicense, string distributionPointType, out string nameAttributeValue, out string addressAttributeValue)
        {
            Invariant.Assert(publishLicense != null);
            nameAttributeValue = null;
            addressAttributeValue = null;
            SafeRightsManagementQueryHandle queryRootHandle = null;
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMParseUnboundLicense(publishLicense, out queryRootHandle));
            using (queryRootHandle)
            {
                uint index = 0;
                while (true)
                {
                    SafeRightsManagementQueryHandle subQueryHandle = null;
                    int hr = MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMGetUnboundLicenseObject(queryRootHandle, "distribution-point", index, out subQueryHandle);
                    if (hr == -2147168490)
                    {
                        return;
                    }
                    Errors.ThrowOnErrorCode(hr);
                    using (subQueryHandle)
                    {
                        if (string.CompareOrdinal(GetUnboundLicenseStringAttribute(subQueryHandle, "object-type", 0), distributionPointType) == 0)
                        {
                            nameAttributeValue = GetUnboundLicenseStringAttribute(subQueryHandle, "name", 0);
                            addressAttributeValue = GetUnboundLicenseStringAttribute(subQueryHandle, "address-value", 0);
                            return;
                        }
                    }
                    index++;
                }
            }
        }

        private static string GetElementFromCertificateChain(string certificateChain, int index)
        {
            Invariant.Assert(index >= 0);
            Invariant.Assert(certificateChain != null);
            uint certificateLength = 0;
            StringBuilder certificate = null;
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMDeconstructCertificateChain(certificateChain, (uint) index, ref certificateLength, null));
            certificate = new StringBuilder((int) certificateLength);
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMDeconstructCertificateChain(certificateChain, (uint) index, ref certificateLength, certificate));
            return certificate.ToString();
        }

        private string GetGroupIdentityCert() => 
            this.GetLatestCertificate(EnumerateLicenseFlags.SpecifiedGroupIdentity);

        private string GetLatestCertificate(EnumerateLicenseFlags enumerateLicenseFlags)
        {
            int index = 0;
            string certificateChain = this.EnumerateLicense(enumerateLicenseFlags, index);
            if (certificateChain == null)
            {
                return null;
            }
            DateTime time = ExtractIssuedTimeFromCertificateChain(certificateChain, DateTime.MinValue);
            while (certificateChain != null)
            {
                index++;
                string str2 = this.EnumerateLicense(enumerateLicenseFlags, index);
                if (str2 == null)
                {
                    return certificateChain;
                }
                DateTime time2 = ExtractIssuedTimeFromCertificateChain(str2, DateTime.MinValue);
                if (DateTime.Compare(time, time2) < 0)
                {
                    certificateChain = str2;
                    time = time2;
                }
            }
            return certificateChain;
        }

        internal static string GetLicenseOnSession(SafeRightsManagementSessionHandle sessionHandle, EnumerateLicenseFlags enumerateLicenseFlags, int index)
        {
            Invariant.Assert(index >= 0);
            if (((((enumerateLicenseFlags != EnumerateLicenseFlags.Machine) && (enumerateLicenseFlags != EnumerateLicenseFlags.GroupIdentity)) && ((enumerateLicenseFlags != EnumerateLicenseFlags.GroupIdentityName) && (enumerateLicenseFlags != EnumerateLicenseFlags.GroupIdentityLid))) && (((enumerateLicenseFlags != EnumerateLicenseFlags.SpecifiedGroupIdentity) && (enumerateLicenseFlags != EnumerateLicenseFlags.Eul)) && ((enumerateLicenseFlags != EnumerateLicenseFlags.EulLid) && (enumerateLicenseFlags != EnumerateLicenseFlags.ClientLicensor)))) && ((((enumerateLicenseFlags != EnumerateLicenseFlags.ClientLicensorLid) && (enumerateLicenseFlags != EnumerateLicenseFlags.SpecifiedClientLicensor)) && ((enumerateLicenseFlags != EnumerateLicenseFlags.RevocationList) && (enumerateLicenseFlags != EnumerateLicenseFlags.RevocationListLid))) && (enumerateLicenseFlags != EnumerateLicenseFlags.Expired)))
            {
                throw new ArgumentOutOfRangeException("enumerateLicenseFlags");
            }
            int hr = 0;
            bool pfSharedFlag = false;
            uint puCertDataLen = 0;
            StringBuilder wszCertificateData = null;
            hr = MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMEnumerateLicense(sessionHandle, (uint) enumerateLicenseFlags, (uint) index, ref pfSharedFlag, ref puCertDataLen, null);
            if (hr == -2147168461)
            {
                return null;
            }
            Errors.ThrowOnErrorCode(hr);
            if (puCertDataLen > 0x7fffffff)
            {
                return null;
            }
            wszCertificateData = new StringBuilder((int) puCertDataLen);
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMEnumerateLicense(sessionHandle, (uint) enumerateLicenseFlags, (uint) index, ref pfSharedFlag, ref puCertDataLen, wszCertificateData));
            return wszCertificateData.ToString();
        }

        internal string GetMachineCert()
        {
            this.CheckDisposed();
            return this.EnumerateLicense(EnumerateLicenseFlags.Machine, 0);
        }

        internal static string GetOwnerLicense(SafeRightsManagementPubHandle issuanceLicenseHandle)
        {
            Invariant.Assert(!issuanceLicenseHandle.IsInvalid);
            uint ownerLicenseLength = 0;
            StringBuilder ownerLicense = null;
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMGetOwnerLicense(issuanceLicenseHandle, ref ownerLicenseLength, null));
            ownerLicense = new StringBuilder((int) ownerLicenseLength);
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMGetOwnerLicense(issuanceLicenseHandle, ref ownerLicenseLength, ownerLicense));
            return ownerLicense.ToString();
        }

        internal static void GetReferralInfoFromPublishLicense(string publishLicense, out string referralInfoName, out Uri referralInfoUri)
        {
            string str;
            string str2;
            GetDistributionPointInfoFromPublishLicense(publishLicense, "Referral-Info", out str, out str2);
            referralInfoName = str;
            if (str2 != null)
            {
                referralInfoUri = new Uri(str2);
            }
            else
            {
                referralInfoUri = null;
            }
        }

        private static Uri GetRegistryPassportCertificationUrl()
        {
            Uri uri;
            new RegistryPermission(RegistryPermissionAccess.Read, AccessControlActions.View, @"HKEY_LOCAL_MACHINE\Software\Microsoft\MSDRM\ServiceLocation\PassportActivation").Assert();
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\MSDRM\ServiceLocation\PassportActivation");
                if (key == null)
                {
                    return null;
                }
                string uriString = key.GetValue(null) as string;
                if (uriString != null)
                {
                    return new Uri(uriString);
                }
                uri = null;
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
            return uri;
        }

        internal static ContentRight? GetRightFromString(string rightName)
        {
            rightName = rightName.ToString().ToUpper(CultureInfo.InvariantCulture);
            for (int i = 0; i < _rightEnums.Length; i++)
            {
                if (string.CompareOrdinal(_rightNames[i], rightName) == 0)
                {
                    return new ContentRight?(_rightEnums[i]);
                }
            }
            return null;
        }

        private static RightNameExpirationInfoPair GetRightInfoFromRightGroupQueryHandle(SafeRightsManagementQueryHandle rightGroupQueryHandle, uint rightIndex)
        {
            SafeRightsManagementQueryHandle subQueryHandle = null;
            RightNameExpirationInfoPair pair;
            int hr = MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMGetUnboundLicenseObject(rightGroupQueryHandle, "right", rightIndex, out subQueryHandle);
            switch (hr)
            {
                case -2147168461:
                case -2147168490:
                    return null;
            }
            Errors.ThrowOnErrorCode(hr);
            using (subQueryHandle)
            {
                string rightName = GetUnboundLicenseStringAttribute(subQueryHandle, "name", 0);
                DateTime minValue = DateTime.MinValue;
                DateTime maxValue = DateTime.MaxValue;
                SafeRightsManagementQueryHandle handle2 = null;
                if (MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMGetUnboundLicenseObject(subQueryHandle, "condition-list", 0, out handle2) >= 0)
                {
                    using (handle2)
                    {
                        SafeRightsManagementQueryHandle handle3 = null;
                        hr = MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMGetUnboundLicenseObject(handle2, "rangetime-condition", 0, out handle3);
                        switch (hr)
                        {
                            case -2147168461:
                            case -2147168490:
                                goto Label_00D0;
                        }
                        Errors.ThrowOnErrorCode(hr);
                        using (handle3)
                        {
                            minValue = GetUnboundLicenseDateTimeAttribute(handle3, "from-time", 0, DateTime.MinValue);
                            maxValue = GetUnboundLicenseDateTimeAttribute(handle3, "until-time", 0, DateTime.MaxValue);
                        }
                    }
                }
            Label_00D0:
                pair = new RightNameExpirationInfoPair(rightName, minValue, maxValue);
            }
            return pair;
        }

        private static List<RightNameExpirationInfoPair> GetRightsInfoFromUseLicense(string useLicenseChain, out string rightGroupName)
        {
            Invariant.Assert(useLicenseChain != null);
            string elementFromCertificateChain = GetElementFromCertificateChain(useLicenseChain, 0);
            Invariant.Assert(elementFromCertificateChain != null);
            List<RightNameExpirationInfoPair> list = new List<RightNameExpirationInfoPair>(10);
            SafeRightsManagementQueryHandle queryRootHandle = null;
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMParseUnboundLicense(elementFromCertificateChain, out queryRootHandle));
            using (queryRootHandle)
            {
                SafeRightsManagementQueryHandle subQueryHandle = null;
                Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMGetUnboundLicenseObject(queryRootHandle, "work", 0, out subQueryHandle));
                using (subQueryHandle)
                {
                    SafeRightsManagementQueryHandle handle3 = null;
                    Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMGetUnboundLicenseObject(subQueryHandle, "rights-group", 0, out handle3));
                    using (handle3)
                    {
                        rightGroupName = GetUnboundLicenseStringAttribute(handle3, "name", 0);
                        uint rightIndex = 0;
                        while (true)
                        {
                            RightNameExpirationInfoPair rightInfoFromRightGroupQueryHandle = GetRightInfoFromRightGroupQueryHandle(handle3, rightIndex);
                            if (rightInfoFromRightGroupQueryHandle == null)
                            {
                                return list;
                            }
                            list.Add(rightInfoFromRightGroupQueryHandle);
                            rightIndex++;
                        }
                    }
                }
            }
        }

        internal static string GetSecurityProviderPath()
        {
            uint typeLength = 0;
            StringBuilder type = null;
            uint pathLength = 0;
            StringBuilder path = null;
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMGetSecurityProvider(0, ref typeLength, null, ref pathLength, null));
            type = new StringBuilder((int) typeLength);
            path = new StringBuilder((int) pathLength);
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMGetSecurityProvider(0, ref typeLength, type, ref pathLength, path));
            return path.ToString();
        }

        private Uri GetServiceLocation(ServiceType serviceType, ServiceLocation serviceLocation, string issuanceLicense)
        {
            uint serviceUrlLength = 0;
            StringBuilder serviceUrl = null;
            int hr = MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMGetServiceLocation(this._hSession, (uint) serviceType, (uint) serviceLocation, issuanceLicense, ref serviceUrlLength, null);
            if (hr == -2147168439)
            {
                return null;
            }
            Errors.ThrowOnErrorCode(hr);
            serviceUrl = new StringBuilder((int) serviceUrlLength);
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMGetServiceLocation(this._hSession, (uint) serviceType, (uint) serviceLocation, issuanceLicense, ref serviceUrlLength, serviceUrl));
            return new Uri(serviceUrl.ToString());
        }

        internal static string GetStringFromRight(ContentRight right)
        {
            for (int i = 0; i < _rightEnums.Length; i++)
            {
                if (_rightEnums[i] == right)
                {
                    return _rightNames[i];
                }
            }
            throw new ArgumentOutOfRangeException("right");
        }

        private static DateTime GetUnboundLicenseDateTimeAttribute(SafeRightsManagementQueryHandle queryHandle, string attributeType, uint attributeIndex, DateTime defaultValue)
        {
            uint num2;
            uint size = SystemTime.Size;
            byte[] buffer = new byte[size];
            int hr = MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMGetUnboundLicenseAttribute(queryHandle, attributeType, attributeIndex, out num2, ref size, buffer);
            if (num2 != 3)
            {
                throw new RightsManagementException(RightsManagementFailureCode.InvalidLicense);
            }
            switch (hr)
            {
                case -2147168461:
                case -2147168490:
                    return defaultValue;
            }
            Errors.ThrowOnErrorCode(hr);
            SystemTime time = new SystemTime(buffer);
            return time.GetDateTime(defaultValue);
        }

        private static string GetUnboundLicenseStringAttribute(SafeRightsManagementQueryHandle queryHandle, string attributeType, uint attributeIndex)
        {
            uint num2;
            uint bufferSize = 0;
            byte[] buffer = null;
            int hr = MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMGetUnboundLicenseAttribute(queryHandle, attributeType, attributeIndex, out num2, ref bufferSize, null);
            if (hr == -2147168490)
            {
                return null;
            }
            Errors.ThrowOnErrorCode(hr);
            if (bufferSize < 2)
            {
                return null;
            }
            buffer = new byte[bufferSize];
            Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMGetUnboundLicenseAttribute(queryHandle, attributeType, attributeIndex, out num2, ref bufferSize, buffer));
            return Encoding.Unicode.GetString(buffer, 0, buffer.Length - 2);
        }

        internal static Uri GetUseLicenseAcquisitionUriFromPublishLicense(string publishLicense)
        {
            string str;
            string str2;
            GetDistributionPointInfoFromPublishLicense(publishLicense, "License-Acquisition-URL", out str, out str2);
            return new Uri(str2);
        }

        private bool IsActivated(ActivationFlags activateFlags)
        {
            this.CheckDisposed();
            return (0 == MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMIsActivated(this._hSession, (uint) activateFlags, null));
        }

        internal bool IsClientLicensorCertificatePresent()
        {
            this.CheckDisposed();
            return (this.GetClientLicensorCert() != null);
        }

        internal bool IsMachineActivated()
        {
            this.CheckDisposed();
            return (this.IsActivated(ActivationFlags.Machine) && (this.GetMachineCert() != null));
        }

        internal bool IsUserActivated()
        {
            this.CheckDisposed();
            if (!this.IsActivated(ActivationFlags.GroupIdentity))
            {
                return false;
            }
            string groupIdentityCert = this.GetGroupIdentityCert();
            if (groupIdentityCert == null)
            {
                return false;
            }
            ContentUser userObj = ExtractUserFromCertificateChain(groupIdentityCert);
            if (userObj == null)
            {
                return false;
            }
            return this._user.GenericEquals(userObj);
        }

        internal void RemoveUsersCertificates(EnumerateLicenseFlags certificateType)
        {
            this.CheckDisposed();
            Invariant.Assert((certificateType == EnumerateLicenseFlags.SpecifiedClientLicensor) || (certificateType == EnumerateLicenseFlags.SpecifiedGroupIdentity));
            foreach (string str in EnumerateAllValuesOnSession(this._hSession, certificateType))
            {
                this.DeleteLicense(ExtractCertificateIdFromCertificateChain(str));
            }
        }

        internal PublishLicense SignIssuanceLicense(IssuanceLicense issuanceLicense, out UseLicense authorUseLicense)
        {
            this.CheckDisposed();
            Invariant.Assert(issuanceLicense != null);
            Invariant.Assert(!this._envHandle.IsInvalid);
            using (CallbackHandler handler = new CallbackHandler())
            {
                string clientLicensorCert = this.GetClientLicensorCert();
                if (clientLicensorCert == null)
                {
                    throw new RightsManagementException(System.Windows.SR.Get("UserHasNoClientLicensorCert"));
                }
                clientLicensorCert = clientLicensorCert.Trim();
                if (clientLicensorCert.Length == 0)
                {
                    throw new RightsManagementException(System.Windows.SR.Get("UserHasNoClientLicensorCert"));
                }
                Errors.ThrowOnErrorCode(MS.Internal.Security.RightsManagement.SafeNativeMethods.DRMGetSignedIssuanceLicense(this._envHandle, issuanceLicense.Handle, 50, null, 0, "AES", clientLicensorCert, handler.CallbackDelegate, null, 0));
                handler.WaitForCompletion();
                PublishLicense license = new PublishLicense(handler.CallbackData);
                authorUseLicense = new UseLicense(GetOwnerLicense(issuanceLicense.Handle));
                return license;
            }
        }

        internal CryptoProvider TryBindUseLicenseToAllIdentites(string serializedUseLicense)
        {
            string str;
            string str2;
            string str3;
            this.CheckDisposed();
            Invariant.Assert(serializedUseLicense != null);
            int theFirstHrFailureCode = 0;
            int hr = 0;
            List<RightNameExpirationInfoPair> rightsInfoFromUseLicense = GetRightsInfoFromUseLicense(serializedUseLicense, out str);
            BoundLicenseParams boundLicenseParams = new BoundLicenseParams {
                uVersion = 0,
                hEnablingPrincipal = 0,
                hSecureStore = 0,
                wszRightsGroup = str
            };
            GetContentIdFromLicense(serializedUseLicense, out str2, out str3);
            boundLicenseParams.DRMIDuVersion = 0;
            boundLicenseParams.DRMIDIdType = str3;
            boundLicenseParams.DRMIDId = str2;
            boundLicenseParams.cAuthenticatorCount = 0;
            boundLicenseParams.rghAuthenticators = IntPtr.Zero;
            string groupIdentityCert = this.GetGroupIdentityCert();
            boundLicenseParams.wszDefaultEnablingPrincipalCredentials = groupIdentityCert;
            boundLicenseParams.dwFlags = 0;
            CryptoProvider provider = this.BindUseLicense(serializedUseLicense, rightsInfoFromUseLicense, boundLicenseParams, out theFirstHrFailureCode);
            if (provider != null)
            {
                return provider;
            }
            if ((hr == 0) && (theFirstHrFailureCode != 0))
            {
                hr = theFirstHrFailureCode;
            }
            int index = 0;
            while (true)
            {
                groupIdentityCert = this.EnumerateLicense(EnumerateLicenseFlags.GroupIdentity, index);
                if (groupIdentityCert == null)
                {
                    break;
                }
                index++;
                boundLicenseParams.wszDefaultEnablingPrincipalCredentials = groupIdentityCert;
                provider = this.BindUseLicense(serializedUseLicense, rightsInfoFromUseLicense, boundLicenseParams, out theFirstHrFailureCode);
                if (provider != null)
                {
                    return provider;
                }
                if ((hr == 0) && (theFirstHrFailureCode != 0))
                {
                    hr = theFirstHrFailureCode;
                }
            }
            Invariant.Assert(hr != 0);
            Errors.ThrowOnErrorCode(hr);
            return null;
        }

        private List<CryptoProvider> CryptoProviderList
        {
            get
            {
                if (this._cryptoProviderList == null)
                {
                    this._cryptoProviderList = new List<CryptoProvider>(5);
                }
                return this._cryptoProviderList;
            }
        }
    }
}

