namespace MS.Internal.Security.RightsManagement
{
    using MS.Internal;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.RightsManagement;
    using System.Text;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    internal class IssuanceLicense : IDisposable
    {
        private SafeRightsManagementPubHandle _issuanceLicenseHandle;
        private List<SafeRightsManagementPubHandle> _pubHandlesList = new List<SafeRightsManagementPubHandle>(50);
        private const string DefaultContentType = "MS-GUID";
        private const string UnspecifiedAuthenticationType = "Unspecified";

        internal IssuanceLicense(DateTime validFrom, DateTime validUntil, string referralInfoName, Uri referralInfoUri, ContentUser owner, string issuanceLicense, SafeRightsManagementHandle boundLicenseHandle, Guid contentId, ICollection<ContentGrant> grantCollection, IDictionary<int, LocalizedNameDescriptionPair> localizedNameDescriptionDictionary, IDictionary<string, string> applicationSpecificDataDictionary, int rightValidityIntervalDays, RevocationPoint revocationPoint)
        {
            this.Initialize(validFrom, validUntil, referralInfoName, referralInfoUri, owner, issuanceLicense, boundLicenseHandle, contentId, grantCollection, localizedNameDescriptionDictionary, applicationSpecificDataDictionary, rightValidityIntervalDays, revocationPoint);
        }

        private void AddApplicationSpecificData(string name, string value)
        {
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMSetApplicationSpecificData(this._issuanceLicenseHandle, false, name, value));
        }

        private void AddGrant(ContentGrant grant)
        {
            Invariant.Assert(grant != null);
            Invariant.Assert(grant.User != null);
            SafeRightsManagementPubHandle rightHandle = this.GetRightHandle(grant);
            SafeRightsManagementPubHandle handleFromUser = this.GetHandleFromUser(grant.User);
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMAddRightWithUser(this._issuanceLicenseHandle, rightHandle, handleFromUser));
        }

        private void AddNameDescription(int localeId, LocalizedNameDescriptionPair nameDescription)
        {
            uint num = (uint) localeId;
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMSetNameAndDescription(this._issuanceLicenseHandle, false, num, nameDescription.Name, nameDescription.Description));
        }

        private void CheckDisposed()
        {
            Invariant.Assert((this._issuanceLicenseHandle != null) && !this._issuanceLicenseHandle.IsInvalid);
        }

        private string ConvertAuthenticationTypeToString(ContentUser user)
        {
            if (user.AuthenticationType == AuthenticationType.WindowsPassport)
            {
                return "Unspecified";
            }
            return user.AuthenticationType.ToString();
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
                    if (this._issuanceLicenseHandle != null)
                    {
                        this._issuanceLicenseHandle.Dispose();
                        foreach (SafeRightsManagementPubHandle handle in this._pubHandlesList)
                        {
                            handle.Dispose();
                        }
                    }
                }
                finally
                {
                    this._issuanceLicenseHandle = null;
                    this._pubHandlesList.Clear();
                }
            }
        }

        ~IssuanceLicense()
        {
            this.Dispose(false);
        }

        private KeyValuePair<string, string>? GetApplicationSpecificData(int index)
        {
            Invariant.Assert(index >= 0);
            uint nameLength = 0;
            uint valueLength = 0;
            int hr = SafeNativeMethods.DRMGetApplicationSpecificData(this._issuanceLicenseHandle, (uint) index, ref nameLength, null, ref valueLength, null);
            if (hr == -2147168461)
            {
                return null;
            }
            Errors.ThrowOnErrorCode(hr);
            StringBuilder name = null;
            if (nameLength > 0)
            {
                name = new StringBuilder((int) nameLength);
            }
            StringBuilder builder2 = null;
            if (valueLength > 0)
            {
                builder2 = new StringBuilder((int) valueLength);
            }
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMGetApplicationSpecificData(this._issuanceLicenseHandle, (uint) index, ref nameLength, name, ref valueLength, builder2));
            string key = name?.ToString();
            string str2 = builder2?.ToString();
            return new KeyValuePair<string, string>?(new KeyValuePair<string, string>(key, str2));
        }

        private SafeRightsManagementPubHandle GetHandleFromUser(ContentUser user)
        {
            SafeRightsManagementPubHandle userHandle = null;
            int num;
            if (user.GenericEquals(ContentUser.AnyoneUser) || user.GenericEquals(ContentUser.OwnerUser))
            {
                num = SafeNativeMethods.DRMCreateUser(user.Name, user.Name, this.ConvertAuthenticationTypeToString(user), out userHandle);
            }
            else
            {
                num = SafeNativeMethods.DRMCreateUser(user.Name, null, this.ConvertAuthenticationTypeToString(user), out userHandle);
            }
            Errors.ThrowOnErrorCode(num);
            this._pubHandlesList.Add(userHandle);
            return userHandle;
        }

        private void GetIssuanceLicenseInfo(out DateTime timeFrom, out DateTime timeUntil, DistributionPointInfo distributionPointInfo, out string distributionPointName, out string distributionPointUri, out ContentUser owner, out bool officialFlag)
        {
            uint distributionPointNameLength = 0;
            uint distributionPointUriLength = 0;
            bool flag = false;
            SafeRightsManagementPubHandle ownerHandle = null;
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMGetIssuanceLicenseInfo(this._issuanceLicenseHandle, null, null, (uint) distributionPointInfo, ref distributionPointNameLength, null, ref distributionPointUriLength, null, out ownerHandle, out flag));
            if (ownerHandle != null)
            {
                ownerHandle.Dispose();
                ownerHandle = null;
            }
            StringBuilder builder = null;
            if (distributionPointNameLength > 0)
            {
                builder = new StringBuilder((int) distributionPointNameLength);
            }
            StringBuilder builder2 = null;
            if (distributionPointUriLength > 0)
            {
                builder2 = new StringBuilder((int) distributionPointUriLength);
            }
            SystemTime time = new SystemTime(DateTime.Now);
            SystemTime time2 = new SystemTime(DateTime.Now);
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMGetIssuanceLicenseInfo(this._issuanceLicenseHandle, time, time2, (uint) distributionPointInfo, ref distributionPointNameLength, builder, ref distributionPointUriLength, builder2, out ownerHandle, out flag));
            timeFrom = time.GetDateTime(DateTime.MinValue);
            timeUntil = time2.GetDateTime(DateTime.MaxValue);
            if (builder != null)
            {
                distributionPointName = builder.ToString();
            }
            else
            {
                distributionPointName = null;
            }
            if (builder2 != null)
            {
                distributionPointUri = builder2.ToString();
            }
            else
            {
                distributionPointUri = null;
            }
            owner = null;
            if (ownerHandle != null)
            {
                this._pubHandlesList.Add(ownerHandle);
                if (!ownerHandle.IsInvalid)
                {
                    owner = GetUserFromHandle(ownerHandle);
                }
            }
            officialFlag = flag;
        }

        private ContentUser GetIssuanceLicenseUser(int index, out SafeRightsManagementPubHandle userHandle)
        {
            Invariant.Assert(index >= 0);
            int hr = SafeNativeMethods.DRMGetUsers(this._issuanceLicenseHandle, (uint) index, out userHandle);
            if (hr == -2147168461)
            {
                userHandle = null;
                return null;
            }
            Errors.ThrowOnErrorCode(hr);
            this._pubHandlesList.Add(userHandle);
            return GetUserFromHandle(userHandle);
        }

        private ContentRight? GetIssuanceLicenseUserRight(SafeRightsManagementPubHandle userHandle, int index, out SafeRightsManagementPubHandle rightHandle, out DateTime validFrom, out DateTime validUntil)
        {
            Invariant.Assert(index >= 0);
            int hr = SafeNativeMethods.DRMGetUserRights(this._issuanceLicenseHandle, userHandle, (uint) index, out rightHandle);
            if (hr == -2147168461)
            {
                rightHandle = null;
                validFrom = DateTime.MinValue;
                validUntil = DateTime.MaxValue;
                return null;
            }
            Errors.ThrowOnErrorCode(hr);
            this._pubHandlesList.Add(rightHandle);
            return GetRightFromHandle(rightHandle, out validFrom, out validUntil);
        }

        private LocalizedNameDescriptionPair GetLocalizedNameDescriptionPair(int index, out int localeId)
        {
            Invariant.Assert(index >= 0);
            uint num = 0;
            uint nameLength = 0;
            StringBuilder name = null;
            uint descriptionLength = 0;
            StringBuilder description = null;
            int hr = SafeNativeMethods.DRMGetNameAndDescription(this._issuanceLicenseHandle, (uint) index, out num, ref nameLength, name, ref descriptionLength, description);
            if (hr == -2147168461)
            {
                localeId = 0;
                return null;
            }
            Errors.ThrowOnErrorCode(hr);
            if (nameLength > 0)
            {
                name = new StringBuilder((int) nameLength);
            }
            if (descriptionLength > 0)
            {
                description = new StringBuilder((int) descriptionLength);
            }
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMGetNameAndDescription(this._issuanceLicenseHandle, (uint) index, out num, ref nameLength, name, ref descriptionLength, description));
            localeId = (int) num;
            return new LocalizedNameDescriptionPair(name?.ToString(), description?.ToString());
        }

        private RevocationPoint GetRevocationPoint()
        {
            uint idLength = 0;
            uint idTypeLength = 0;
            uint urlLength = 0;
            uint nameLength = 0;
            uint publicKeyLength = 0;
            SystemTime frequency = new SystemTime(DateTime.Now);
            int hr = SafeNativeMethods.DRMGetRevocationPoint(this._issuanceLicenseHandle, ref idLength, null, ref idTypeLength, null, ref urlLength, null, frequency, ref nameLength, null, ref publicKeyLength, null);
            if (hr == -2147168432)
            {
                return null;
            }
            Errors.ThrowOnErrorCode(hr);
            StringBuilder id = null;
            if (idLength > 0)
            {
                id = new StringBuilder((int) idLength);
            }
            StringBuilder idType = null;
            if (idTypeLength > 0)
            {
                idType = new StringBuilder((int) idTypeLength);
            }
            StringBuilder url = null;
            if (urlLength > 0)
            {
                url = new StringBuilder((int) urlLength);
            }
            StringBuilder name = null;
            if (nameLength > 0)
            {
                name = new StringBuilder((int) nameLength);
            }
            StringBuilder publicKey = null;
            if (publicKeyLength > 0)
            {
                publicKey = new StringBuilder((int) publicKeyLength);
            }
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMGetRevocationPoint(this._issuanceLicenseHandle, ref idLength, id, ref idTypeLength, idType, ref urlLength, url, frequency, ref nameLength, name, ref publicKeyLength, publicKey));
            return new RevocationPoint { 
                Id = id?.ToString(),
                IdType = idType?.ToString(),
                Url = (url == null) ? null : new Uri(url.ToString()),
                Name = name?.ToString(),
                PublicKey = publicKey?.ToString(),
                Frequency = frequency
            };
        }

        private static ContentRight? GetRightFromHandle(SafeRightsManagementPubHandle rightHandle, out DateTime validFrom, out DateTime validUntil)
        {
            uint rightNameLength = 0;
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMGetRightInfo(rightHandle, ref rightNameLength, null, null, null));
            StringBuilder rightName = new StringBuilder((int) rightNameLength);
            SystemTime timeFrom = new SystemTime(DateTime.Now);
            SystemTime timeUntil = new SystemTime(DateTime.Now);
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMGetRightInfo(rightHandle, ref rightNameLength, rightName, timeFrom, timeUntil));
            validFrom = timeFrom.GetDateTime(DateTime.MinValue);
            validUntil = timeUntil.GetDateTime(DateTime.MaxValue);
            return ClientSession.GetRightFromString(rightName.ToString());
        }

        private SafeRightsManagementPubHandle GetRightHandle(ContentGrant grant)
        {
            SafeRightsManagementPubHandle rightHandle = null;
            SystemTime timeFrom = null;
            SystemTime timeUntil = null;
            if ((grant.ValidFrom != DateTime.MinValue) || (grant.ValidUntil != DateTime.MaxValue))
            {
                timeFrom = new SystemTime(grant.ValidFrom);
                timeUntil = new SystemTime(grant.ValidUntil);
            }
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMCreateRight(ClientSession.GetStringFromRight(grant.Right), timeFrom, timeUntil, 0, null, null, out rightHandle));
            this._pubHandlesList.Add(rightHandle);
            return rightHandle;
        }

        private static ContentUser GetUserFromHandle(SafeRightsManagementPubHandle userHandle)
        {
            uint userNameLength = 0;
            StringBuilder userName = null;
            uint userIdLength = 0;
            StringBuilder userId = null;
            uint userIdTypeLength = 0;
            StringBuilder userIdType = null;
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMGetUserInfo(userHandle, ref userNameLength, null, ref userIdLength, null, ref userIdTypeLength, null));
            if (userNameLength > 0)
            {
                userName = new StringBuilder((int) userNameLength);
            }
            if (userIdLength > 0)
            {
                userId = new StringBuilder((int) userIdLength);
            }
            if (userIdTypeLength > 0)
            {
                userIdType = new StringBuilder((int) userIdTypeLength);
            }
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMGetUserInfo(userHandle, ref userNameLength, userName, ref userIdLength, userId, ref userIdTypeLength, userIdType));
            string name = null;
            if (userName != null)
            {
                name = userName.ToString();
            }
            string strA = null;
            if (userIdType != null)
            {
                strA = userIdType.ToString().ToUpperInvariant();
            }
            string str3 = null;
            if (userId != null)
            {
                str3 = userId.ToString().ToUpperInvariant();
            }
            if (string.CompareOrdinal(strA, AuthenticationType.Windows.ToString().ToUpperInvariant()) == 0)
            {
                return new ContentUser(name, AuthenticationType.Windows);
            }
            if (string.CompareOrdinal(strA, AuthenticationType.Passport.ToString().ToUpperInvariant()) == 0)
            {
                return new ContentUser(name, AuthenticationType.Passport);
            }
            if (string.CompareOrdinal(strA, AuthenticationType.Internal.ToString().ToUpperInvariant()) == 0)
            {
                if (ContentUser.CompareToAnyone(str3))
                {
                    return ContentUser.AnyoneUser;
                }
                if (ContentUser.CompareToOwner(str3))
                {
                    return ContentUser.OwnerUser;
                }
            }
            else if (string.CompareOrdinal(strA, "Unspecified".ToUpperInvariant()) == 0)
            {
                return new ContentUser(name, AuthenticationType.WindowsPassport);
            }
            throw new RightsManagementException(RightsManagementFailureCode.InvalidLicense);
        }

        private void Initialize(DateTime validFrom, DateTime validUntil, string referralInfoName, Uri referralInfoUri, ContentUser owner, string issuanceLicense, SafeRightsManagementHandle boundLicenseHandle, Guid contentId, ICollection<ContentGrant> grantCollection, IDictionary<int, LocalizedNameDescriptionPair> localizedNameDescriptionDictionary, IDictionary<string, string> applicationSpecificDataDictionary, int rightValidityIntervalDays, RevocationPoint revocationPoint)
        {
            SafeRightsManagementPubHandle handleFromUser;
            Invariant.Assert(boundLicenseHandle.IsInvalid || (issuanceLicense != null));
            SystemTime timeFrom = null;
            SystemTime timeUntil = null;
            if ((validFrom != DateTime.MinValue) || (validUntil != DateTime.MaxValue))
            {
                timeFrom = new SystemTime(validFrom);
                timeUntil = new SystemTime(validUntil);
            }
            string referralInfoUrl = null;
            if (referralInfoUri != null)
            {
                referralInfoUrl = referralInfoUri.ToString();
            }
            if (owner != null)
            {
                handleFromUser = this.GetHandleFromUser(owner);
            }
            else
            {
                handleFromUser = SafeRightsManagementPubHandle.InvalidHandle;
            }
            this._issuanceLicenseHandle = null;
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMCreateIssuanceLicense(timeFrom, timeUntil, referralInfoName, referralInfoUrl, handleFromUser, issuanceLicense, boundLicenseHandle, out this._issuanceLicenseHandle));
            Invariant.Assert((this._issuanceLicenseHandle != null) && !this._issuanceLicenseHandle.IsInvalid);
            if (rightValidityIntervalDays > 0)
            {
                SafeNativeMethods.DRMSetIntervalTime(this._issuanceLicenseHandle, (uint) rightValidityIntervalDays);
            }
            if (grantCollection != null)
            {
                foreach (ContentGrant grant in grantCollection)
                {
                    this.AddGrant(grant);
                }
            }
            if (localizedNameDescriptionDictionary != null)
            {
                foreach (KeyValuePair<int, LocalizedNameDescriptionPair> pair in localizedNameDescriptionDictionary)
                {
                    this.AddNameDescription(pair.Key, pair.Value);
                }
            }
            if (applicationSpecificDataDictionary != null)
            {
                foreach (KeyValuePair<string, string> pair2 in applicationSpecificDataDictionary)
                {
                    this.AddApplicationSpecificData(pair2.Key, pair2.Value);
                }
            }
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMSetMetaData(this._issuanceLicenseHandle, contentId.ToString("B"), "MS-GUID", null, null, null, null));
            if (revocationPoint != null)
            {
                this.SetRevocationPoint(revocationPoint);
            }
        }

        private void SetRevocationPoint(RevocationPoint revocationPoint)
        {
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMSetRevocationPoint(this._issuanceLicenseHandle, false, revocationPoint.Id, revocationPoint.IdType, revocationPoint.Url.AbsoluteUri, revocationPoint.Frequency, revocationPoint.Name, revocationPoint.PublicKey));
        }

        public override string ToString()
        {
            uint issuanceLicenseTemplateLength = 0;
            StringBuilder issuanceLicenseTemplate = null;
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMGetIssuanceLicenseTemplate(this._issuanceLicenseHandle, ref issuanceLicenseTemplateLength, null));
            issuanceLicenseTemplate = new StringBuilder((int) issuanceLicenseTemplateLength);
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMGetIssuanceLicenseTemplate(this._issuanceLicenseHandle, ref issuanceLicenseTemplateLength, issuanceLicenseTemplate));
            return issuanceLicenseTemplate.ToString();
        }

        internal void UpdateUnsignedPublishLicense(UnsignedPublishLicense unsignedPublishLicense)
        {
            DateTime time;
            DateTime time2;
            string str;
            string str2;
            ContentUser user;
            bool flag;
            Invariant.Assert(unsignedPublishLicense != null);
            DistributionPointInfo referralInfo = DistributionPointInfo.ReferralInfo;
            this.GetIssuanceLicenseInfo(out time, out time2, referralInfo, out str, out str2, out user, out flag);
            unsignedPublishLicense.ReferralInfoName = str;
            if (str2 != null)
            {
                unsignedPublishLicense.ReferralInfoUri = new Uri(str2);
            }
            else
            {
                unsignedPublishLicense.ReferralInfoUri = null;
            }
            unsignedPublishLicense.Owner = user;
            uint days = 0;
            Errors.ThrowOnErrorCode(SafeNativeMethods.DRMGetIntervalTime(this._issuanceLicenseHandle, ref days));
            unsignedPublishLicense.RightValidityIntervalDays = (int) days;
            int index = 0;
            while (true)
            {
                SafeRightsManagementPubHandle userHandle = null;
                ContentUser issuanceLicenseUser = this.GetIssuanceLicenseUser(index, out userHandle);
                if ((issuanceLicenseUser == null) || (userHandle == null))
                {
                    int num5 = 0;
                    while (true)
                    {
                        int num6;
                        LocalizedNameDescriptionPair localizedNameDescriptionPair = this.GetLocalizedNameDescriptionPair(num5, out num6);
                        if (localizedNameDescriptionPair == null)
                        {
                            break;
                        }
                        unsignedPublishLicense.LocalizedNameDescriptionDictionary.Add(num6, localizedNameDescriptionPair);
                        num5++;
                    }
                }
                int num4 = 0;
                while (true)
                {
                    DateTime time3;
                    DateTime time4;
                    SafeRightsManagementPubHandle rightHandle = null;
                    ContentRight? nullable = this.GetIssuanceLicenseUserRight(userHandle, num4, out rightHandle, out time3, out time4);
                    if (rightHandle == null)
                    {
                        break;
                    }
                    if (nullable.HasValue)
                    {
                        unsignedPublishLicense.Grants.Add(new ContentGrant(issuanceLicenseUser, nullable.Value, time3, time4));
                    }
                    num4++;
                }
                index++;
            }
            int num7 = 0;
            while (true)
            {
                KeyValuePair<string, string>? applicationSpecificData = this.GetApplicationSpecificData(num7);
                if (!applicationSpecificData.HasValue)
                {
                    break;
                }
                unsignedPublishLicense.ApplicationSpecificDataDictionary.Add(applicationSpecificData.Value.Key, applicationSpecificData.Value.Value);
                num7++;
            }
            unsignedPublishLicense.RevocationPoint = this.GetRevocationPoint();
        }

        internal SafeRightsManagementPubHandle Handle
        {
            get
            {
                this.CheckDisposed();
                return this._issuanceLicenseHandle;
            }
        }
    }
}

