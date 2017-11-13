namespace SA.Payments.Realex.RealAuth
{
    using System;

    public interface ISettingsProvider
    {
        string Account { get; }

        string MerchantID { get; }

        string SharedSecret { get; }
    }
}

