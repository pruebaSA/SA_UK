namespace SA.Payments.Realex.RealAuth
{
    using System;

    public sealed class SettingsProvider : ISettingsProvider
    {
        private readonly string _account;
        private readonly string _merchantID;
        private readonly string _sharedShared;

        public SettingsProvider(string merchantID, string account, string sharedSecret)
        {
            this._account = account;
            this._merchantID = merchantID;
            this._sharedShared = sharedSecret;
        }

        public string Account =>
            this._account;

        public string MerchantID =>
            this._merchantID;

        public string SharedSecret =>
            this._sharedShared;
    }
}

