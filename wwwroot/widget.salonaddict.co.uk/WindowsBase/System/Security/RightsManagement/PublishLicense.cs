namespace System.Security.RightsManagement
{
    using MS.Internal.Security.RightsManagement;
    using MS.Internal.WindowsBase;
    using System;
    using System.Security;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    public class PublishLicense
    {
        private Guid _contentId;
        private string _referralInfoName;
        private Uri _referralInfoUri;
        private string _serializedPublishLicense;
        private Uri _useLicenseAcquisitionUriFromPublishLicense;

        public PublishLicense(string signedPublishLicense)
        {
            SecurityHelper.DemandRightsManagementPermission();
            if (signedPublishLicense == null)
            {
                throw new ArgumentNullException("signedPublishLicense");
            }
            this._serializedPublishLicense = signedPublishLicense;
            this._useLicenseAcquisitionUriFromPublishLicense = ClientSession.GetUseLicenseAcquisitionUriFromPublishLicense(this._serializedPublishLicense);
            if (this._useLicenseAcquisitionUriFromPublishLicense == null)
            {
                throw new RightsManagementException(RightsManagementFailureCode.InvalidLicense);
            }
            string contentIdFromPublishLicense = ClientSession.GetContentIdFromPublishLicense(this._serializedPublishLicense);
            if (contentIdFromPublishLicense == null)
            {
                throw new RightsManagementException(RightsManagementFailureCode.InvalidLicense);
            }
            this._contentId = new Guid(contentIdFromPublishLicense);
            ClientSession.GetReferralInfoFromPublishLicense(this._serializedPublishLicense, out this._referralInfoName, out this._referralInfoUri);
        }

        public UseLicense AcquireUseLicense(SecureEnvironment secureEnvironment)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return secureEnvironment?.ClientSession.AcquireUseLicense(this._serializedPublishLicense, false);
        }

        public UseLicense AcquireUseLicenseNoUI(SecureEnvironment secureEnvironment)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return secureEnvironment?.ClientSession.AcquireUseLicense(this._serializedPublishLicense, true);
        }

        public UnsignedPublishLicense DecryptUnsignedPublishLicense(CryptoProvider cryptoProvider)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return cryptoProvider?.DecryptPublishLicense(this._serializedPublishLicense);
        }

        public override string ToString()
        {
            SecurityHelper.DemandRightsManagementPermission();
            return this._serializedPublishLicense;
        }

        public Guid ContentId
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                return this._contentId;
            }
        }

        public string ReferralInfoName
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                return this._referralInfoName;
            }
        }

        public Uri ReferralInfoUri
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                return this._referralInfoUri;
            }
        }

        public Uri UseLicenseAcquisitionUrl
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                return this._useLicenseAcquisitionUriFromPublishLicense;
            }
        }
    }
}

