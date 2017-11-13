namespace System.Security.RightsManagement
{
    using MS.Internal.Security.RightsManagement;
    using MS.Internal.Utility;
    using MS.Internal.WindowsBase;
    using System;
    using System.Collections.Generic;
    using System.Security;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    public class UseLicense
    {
        private IDictionary<string, string> _applicationSpecificDataDictionary;
        private Guid _contentId;
        private ContentUser _owner;
        private string _serializedUseLicense;

        public UseLicense(string useLicense)
        {
            string str;
            string str2;
            SecurityHelper.DemandRightsManagementPermission();
            if (useLicense == null)
            {
                throw new ArgumentNullException("useLicense");
            }
            this._serializedUseLicense = useLicense;
            ClientSession.GetContentIdFromLicense(this._serializedUseLicense, out str, out str2);
            if (str == null)
            {
                throw new RightsManagementException(RightsManagementFailureCode.InvalidLicense);
            }
            this._contentId = new Guid(str);
            this._owner = ClientSession.ExtractUserFromCertificateChain(this._serializedUseLicense);
            this._applicationSpecificDataDictionary = new ReadOnlyDictionary<string, string>(ClientSession.ExtractApplicationSpecificDataFromLicense(this._serializedUseLicense));
        }

        public CryptoProvider Bind(SecureEnvironment secureEnvironment)
        {
            SecurityHelper.DemandRightsManagementPermission();
            return secureEnvironment?.ClientSession.TryBindUseLicenseToAllIdentites(this._serializedUseLicense);
        }

        public override bool Equals(object x)
        {
            SecurityHelper.DemandRightsManagementPermission();
            if (x == null)
            {
                return false;
            }
            if (x.GetType() != base.GetType())
            {
                return false;
            }
            UseLicense license = (UseLicense) x;
            return (string.CompareOrdinal(this._serializedUseLicense, license._serializedUseLicense) == 0);
        }

        public override int GetHashCode()
        {
            SecurityHelper.DemandRightsManagementPermission();
            return this._serializedUseLicense.GetHashCode();
        }

        public override string ToString()
        {
            SecurityHelper.DemandRightsManagementPermission();
            return this._serializedUseLicense;
        }

        public IDictionary<string, string> ApplicationData
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
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
        }

        public ContentUser Owner
        {
            get
            {
                SecurityHelper.DemandRightsManagementPermission();
                return this._owner;
            }
        }
    }
}

