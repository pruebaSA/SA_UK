namespace SA.Payments.Realex.RealVault
{
    using System;

    public interface ISettingsProvider
    {
        string Account { get; }

        string MerchantID { get; }

        string SharedSecret { get; }
    }
}

