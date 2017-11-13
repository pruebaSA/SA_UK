namespace System.Security.RightsManagement
{
    using MS.Internal;
    using MS.Internal.Security.RightsManagement;
    using MS.Internal.WindowsBase;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;
    using System.Security;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    public class UnsignedPublishLicense
    {
        private IDictionary<string, string> _applicationSpecificDataDictionary;
        private Guid _contentId;
        private ICollection<ContentGrant> _grantCollection;
        private IDictionary<int, LocalizedNameDescriptionPair> _localizedNameDescriptionDictionary;
        private ContentUser _owner;
        private string _referralInfoName;
        private Uri _referralInfoUri;
        private MS.Internal.Security.RightsManagement.RevocationPoint _revocationPoint;
        private int _rightValidityIntervalDays;

        public UnsignedPublishLicense()
        {
            SecurityHelper.DemandRightsManagementPermission();
            this._grantCollection = new Collection<ContentGrant>();
            this._contentId = Guid.NewGuid();
        }

        public UnsignedPublishLicense(string publishLicenseTemplate) : this()
        {
            SecurityHelper.DemandRightsManagementPermission();
            if (publishLicenseTemplate == null)
            {
                throw new ArgumentNullException("publishLicenseTemplate");
            }
            using (IssuanceLicense license = new IssuanceLicense(DateTime.MinValue, DateTime.MaxValue, null, null, null, publishLicenseTemplate, SafeRightsManagementHandle.InvalidHandle, this._contentId, null, null, null, 0, null))
            {
                license.UpdateUnsignedPublishLicense(this);
            }
        }

        internal UnsignedPublishLicense(SafeRightsManagementHandle boundLicenseHandle, string publishLicenseTemplate) : this()
        {
            Invariant.Assert(!boundLicenseHandle.IsInvalid);
            Invariant.Assert(publishLicenseTemplate != null);
            using (IssuanceLicense license = new IssuanceLicense(DateTime.MinValue, DateTime.MaxValue, null, null, null, publishLicenseTemplate, boundLicenseHandle, this._contentId, null, null, null, 0, null))
            {
                license.UpdateUnsignedPublishLicense(this);
            }
        }

        public PublishLicense Sign(SecureEnvironment secureEnvironment, out UseLicense authorUseLicense)
        {
            ContentUser user;
            SecurityHelper.DemandRightsManagementPermission();
            if (secureEnvironment == null)
            {
                throw new ArgumentNullException("secureEnvironment");
            }
            if (this._owner != null)
            {
                user = this._owner;
            }
            else
            {
                user = secureEnvironment.User;
            }
            using (IssuanceLicense license = new IssuanceLicense(DateTime.MinValue, DateTime.MaxValue, this._referralInfoName, this._referralInfoUri, user, null, SafeRightsManagementHandle.InvalidHandle, this._contentId, this.Grants, this.LocalizedNameDescriptionDictionary, this.ApplicationSpecificDataDictionary, this._rightValidityIntervalDays, this._revocationPoint))
            {
                return secureEnvironment.ClientSession.SignIssuanceLicense(license, out authorUseLicense);
            }
        }

        public override string ToString()
        {
            SecurityHelper.DemandRightsManagementPermission();
            using (IssuanceLicense license = new IssuanceLicense(DateTime.MinValue, DateTime.MaxValue, this._referralInfoName, this._referralInfoUri, this._owner, null, SafeRightsManagementHandle.InvalidHandle, this._contentId, this.Grants, this.LocalizedNameDescriptionDictionary, this.ApplicationSpecificDataDictionary, this._rightValidityIntervalDays, this._revocationPoint))
            {
                return license.ToString();
            }
        }

        internal IDictionary<string, string> ApplicationSpecificDataDictionary
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                if (this._applicationSpecificDataDictionary == null)
                {
                    this._applicationSpecificDataDictionary = new Dictionary<string, string>(5);
                }
                return this._applicationSpecificDataDictionary;
            }
        }

        public Guid ContentId
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                return this._contentId;
            }
            set
            {
                SecurityHelper.DemandRightsManagementPermission();
                this._contentId = value;
            }
        }

        public ICollection<ContentGrant> Grants
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                return this._grantCollection;
            }
        }

        public IDictionary<int, LocalizedNameDescriptionPair> LocalizedNameDescriptionDictionary
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                if (this._localizedNameDescriptionDictionary == null)
                {
                    this._localizedNameDescriptionDictionary = new Dictionary<int, LocalizedNameDescriptionPair>(10);
                }
                return this._localizedNameDescriptionDictionary;
            }
        }

        public ContentUser Owner
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                return this._owner;
            }
            set
            {
                SecurityHelper.DemandRightsManagementPermission();
                this._owner = value;
            }
        }

        public string ReferralInfoName
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                return this._referralInfoName;
            }
            set
            {
                SecurityHelper.DemandRightsManagementPermission();
                this._referralInfoName = value;
            }
        }

        public Uri ReferralInfoUri
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                return this._referralInfoUri;
            }
            set
            {
                SecurityHelper.DemandRightsManagementPermission();
                this._referralInfoUri = value;
            }
        }

        internal MS.Internal.Security.RightsManagement.RevocationPoint RevocationPoint
        {
            get => 
                this._revocationPoint;
            set
            {
                this._revocationPoint = value;
            }
        }

        internal int RightValidityIntervalDays
        {
            get => 
                this._rightValidityIntervalDays;
            set
            {
                this._rightValidityIntervalDays = value;
            }
        }
    }
}

