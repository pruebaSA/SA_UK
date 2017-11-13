namespace SA.Payments.Realex.RealVault
{
    using System;
    using System.Configuration;

    public sealed class SettingsProviderConfig : ISettingsProvider
    {
        public string Account
        {
            get
            {
                string str = ConfigurationManager.AppSettings["Realex.RealVault.Account"];
                if (string.IsNullOrEmpty(str))
                {
                    throw new RealVaultException("Application setting with key 'Realex.RealVault.Account' is not configured.", new ConfigurationErrorsException("Application setting with key 'Realex.RealVault.Account' is not configured."));
                }
                return str;
            }
        }

        public string MerchantID
        {
            get
            {
                string str = ConfigurationManager.AppSettings["Realex.RealVault.MerchantID"];
                if (string.IsNullOrEmpty(str))
                {
                    throw new RealVaultException("Application setting with key 'Realex.RealVault.MerchantID' is not configured.", new ConfigurationErrorsException("Application setting with key 'Realex.RealVault.MerchantID' is not configured."));
                }
                return str;
            }
        }

        public string SharedSecret
        {
            get
            {
                string str = ConfigurationManager.AppSettings["Realex.RealVault.SharedSecret"];
                if (string.IsNullOrEmpty(str))
                {
                    throw new RealVaultException("Application setting with key 'Realex.RealVault.SharedSecret' is not configured.", new ConfigurationErrorsException("Application setting with key 'Realex.RealVault.SharedSecret' is not configured."));
                }
                return str;
            }
        }
    }
}

