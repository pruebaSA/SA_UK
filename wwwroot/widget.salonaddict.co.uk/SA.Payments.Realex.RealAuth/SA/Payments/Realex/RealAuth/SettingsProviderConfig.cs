namespace SA.Payments.Realex.RealAuth
{
    using System;
    using System.Configuration;

    public sealed class SettingsProviderConfig : ISettingsProvider
    {
        public string Account
        {
            get
            {
                string str = ConfigurationManager.AppSettings["Realex.RealAuth.Account"];
                if (string.IsNullOrEmpty(str))
                {
                    throw new RealAuthException("Application setting with key 'Realex.RealAuth.Account' is not configured.", new ConfigurationErrorsException("Application setting with key 'Realex.RealAuth.Account' is not configured."));
                }
                return str;
            }
        }

        public string MerchantID
        {
            get
            {
                string str = ConfigurationManager.AppSettings["Realex.RealAuth.MerchantID"];
                if (string.IsNullOrEmpty(str))
                {
                    throw new RealAuthException("Application setting with key 'Realex.RealAuth.MerchantID' is not configured.", new ConfigurationErrorsException("Application setting with key 'Realex.RealAuth.MerchantID' is not configured."));
                }
                return str;
            }
        }

        public string SharedSecret
        {
            get
            {
                string str = ConfigurationManager.AppSettings["Realex.RealAuth.SharedSecret"];
                if (string.IsNullOrEmpty(str))
                {
                    throw new RealAuthException("Application setting with key 'Realex.RealAuth.SharedSecret' is not configured.", new ConfigurationErrorsException("Application setting with key 'Realex.RealAuth.SharedSecret' is not configured."));
                }
                return str;
            }
        }
    }
}

