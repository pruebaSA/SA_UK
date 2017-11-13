namespace SA.BAL
{
    using System;
    using System.Collections.Generic;

    public interface IUserManager
    {
        void DeleteSalonUser(SalonUserDB user);
        void DeleteWidgetApiKey(WidgetApiKeyDB keyId);
        SalonUserDB GetSalonUserById(Guid userId);
        SalonUserDB GetSalonUserByUsername(string username);
        List<SalonUserDB> GetSalonUsersBySalonId(Guid salonId);
        WidgetApiKeyDB GetWidgetApiKeyById(Guid keyId);
        List<WidgetApiKeyDB> GetWidgetApiKeyBySalonId(Guid salonId);
        WidgetApiKeyDB GetWidgetApiKeyByVerificationToken(string verificationToken);
        SalonUserDB InsertSalonUser(SalonUserDB user);
        WidgetApiKeyDB InsertWidgetApiKey(WidgetApiKeyDB apiKey);
        SalonUserDB UpdateSalonUser(SalonUserDB user);
        WidgetApiKeyDB UpdateWidgetApiKey(WidgetApiKeyDB apiKey);
    }
}

