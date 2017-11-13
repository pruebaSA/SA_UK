namespace IFRAME.Controllers
{
    using System;

    public static class Constants
    {
        public static class Attributes
        {
            public const string APIKEY_SALON_MAPPING = "SALON";
            public const string APPOINTMENT_VIEWED_BY_SALON = "VIEWED_BY_SALON";
            public const string SALON_APPOINTMENT_COUNT = "APPOINTMENT_COUNT";
            public const string SALON_COMPANY_NAME = "COMPANY_NAME";
            public const string SALON_IDENTIFIER = "IDENTIFIER";
            public const string SALON_IS_PAYMENT_METHOD_SETUP = "PAYMENT_METHOD_SETUP";
            public const string SALON_REALEX_PAYER_REF = "REALEX_PAYER_REF";
            public const string SALON_VAT_NUMBER = "VAT_NUMBER";
            public const string SETTINGS_PRIVACY_OPENING_HOURS = "SETTINGS_PRIVACY_OPENING_HOURS";
            public const string SETTINGS_PRIVACY_WIDGET_STATUS = "SETTINGS_PRIVACY_WIDGET_STATUS";
        }

        public static class CacheKeys
        {
            public const string USER = "IFRM_USER_{0}";
        }

        public static class QueryStrings
        {
            public const string ALERT_ID = "aid";
            public const string API_KEY = "api_key";
            public const string APPOINTMENT_ID = "aid";
            public const string DATE = "date";
            public const string EMPLOYEE_ID = "eid";
            public const string EXPIRY = "exp";
            public const string FLAG = "flag";
            public const string HOLIDAY_ID = "hid";
            public const string INVOICE_TYPE = "type";
            public const string LANGUAGE = "lang";
            public const string LOG_ID = "lid";
            public const string MESSAGE_ID = "mid";
            public const string ORDER_BY = "sort";
            public const string ORDER_ID = "oid";
            public const string PAGE_INDEX = "page_index";
            public const string PAGE_SIZE = "page_size";
            public const string PLAN_TYPE = "pln";
            public const string REFERRING_PAGE = "ref";
            public const string RETURN_URL = "url";
            public const string SALON_ID = "sid";
            public const string SALON_INVOICE_ADJUSTMENT_ID = "aid";
            public const string SALON_INVOICE_ID = "inv";
            public const string SALON_PAYMENT_METHOD_ID = "pmid";
            public const string SERVICE_ID = "svid";
            public const string THEME = "theme";
            public const string TICKET_ID = "tid";
            public const string TIME = "time";
            public const string USER_ID = "uid";
            public const string WAPIKEY_ID = "wapi";
            public const string WSP_ID = "wspid";
            public const string WSP_NOTE_ID = "nid";
        }

        public static class Settings
        {
            public const string IAPPLICATION_MANAGER_BOOKING_WIDGET = "IApplicationManager.BookingWidget";
            public const string IAPPOINTMENT_MANAGER_APPOINTMENT_REFRESH_RATE = "IAppointmentManager.Appointment.RefreshRate";
            public const string IFRMCACHE_IS_ENABLED = "IFRMCache.IsEnabled";
            public const string IFRMCONTEXT_DEFAULT_WORKING_LANGUAGE = "IFRMContext.DefaultWorkingLanguage";
            public const string IFRMCONTEXT_DEFAULT_WORKING_THEME = "IFRMContext.DefaultWorkingTheme";
            public const string IFRMMEMBERSHIP_MINIMUM_PASSWORD_LENGTH = "IFRMMembership.MinimumPasswordLength";
            public const string IFRMMEMBERSHIP_MINIMUM_USERNAME_LENGTH = "IFRMMembership.MinimumUsernameLength";
            public const string SECURITY_KEY_3DES = "ISecurityManager.Key.3DESKey";
            public const string SECURITY_KEY_HMACKey = "ISecurityManager.Key.HMACKey";
        }
    }
}

