namespace System.IO.Packaging
{
    using MS.Internal.IO.Packaging.CompoundFile;
    using System;
    using System.Collections.Generic;
    using System.Security.RightsManagement;

    public class RightsManagementInformation
    {
        private RightsManagementEncryptionTransform _rmet;

        internal RightsManagementInformation(RightsManagementEncryptionTransform rmet)
        {
            this._rmet = rmet;
        }

        public void DeleteUseLicense(ContentUser userKey)
        {
            this._rmet.DeleteUseLicense(userKey);
        }

        public IDictionary<ContentUser, UseLicense> GetEmbeddedUseLicenses() => 
            this._rmet.GetEmbeddedUseLicenses();

        public PublishLicense LoadPublishLicense() => 
            this._rmet.LoadPublishLicense();

        public UseLicense LoadUseLicense(ContentUser userKey) => 
            this._rmet.LoadUseLicense(userKey);

        public void SavePublishLicense(PublishLicense publishLicense)
        {
            this._rmet.SavePublishLicense(publishLicense);
        }

        public void SaveUseLicense(ContentUser userKey, UseLicense useLicense)
        {
            this._rmet.SaveUseLicense(userKey, useLicense);
        }

        public System.Security.RightsManagement.CryptoProvider CryptoProvider
        {
            get => 
                this._rmet.CryptoProvider;
            set
            {
                this._rmet.CryptoProvider = value;
            }
        }
    }
}

