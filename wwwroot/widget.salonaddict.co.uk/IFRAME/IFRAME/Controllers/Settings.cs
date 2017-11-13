namespace IFRAME.Controllers
{
    using System;
    using System.Configuration;
    using System.Text;

    public static class Settings
    {
        public static Guid IAPPLICATION_MANAGER_BOOKING_WIDGET =>
            new Guid(ConfigurationManager.AppSettings["IApplicationManager.BookingWidget"]);

        public static int IAPPOINTMENT_MANAGER_APPOINTMENT_REFRESH_RATE
        {
            get
            {
                int num;
                string s = ConfigurationManager.AppSettings["IAppointmentManager.Appointment.RefreshRate"];
                if (!int.TryParse(s, out num))
                {
                    num = 0xea60;
                }
                return num;
            }
        }

        public static bool IFRMCACHE_ISENABLED
        {
            get
            {
                string str = ConfigurationManager.AppSettings["IFRMCache.IsEnabled"];
                if (str == null)
                {
                    str = string.Empty;
                }
                str = str.Trim().ToUpperInvariant();
                if ((str != "1") && (str != "YES"))
                {
                    return (str == "TRUE");
                }
                return true;
            }
        }

        public static string IFRMCONTEXT_DEFAULT_WORKING_LANGUAGE
        {
            get
            {
                string str2 = ConfigurationManager.AppSettings["IFRMContext.DefaultWorkingLanguage"];
                if (str2 == null)
                {
                    str2 = string.Empty;
                }
                str2 = str2.Trim().ToLowerInvariant();
                return "en-GB";
            }
        }

        public static string IFRMCONTEXT_DEFAULT_WORKING_THEME
        {
            get
            {
                string str2 = ConfigurationManager.AppSettings["IFRMContext.DefaultWorkingTheme"];
                if (str2 == null)
                {
                    str2 = string.Empty;
                }
                str2 = str2.Trim().ToLowerInvariant();
                return "white";
            }
        }

        public static int IFRMMembership_MinimumPasswordLength
        {
            get
            {
                string s = ConfigurationManager.AppSettings["IFRMMembership.MinimumPasswordLength"];
                return int.Parse(s);
            }
        }

        public static int IFRMMembership_MinimumUsernameLength
        {
            get
            {
                string s = ConfigurationManager.AppSettings["IFRMMembership.MinimumUsernameLength"];
                return int.Parse(s);
            }
        }

        public static byte[] Security_Key_3DES
        {
            get
            {
                string s = ConfigurationManager.AppSettings["ISecurityManager.Key.3DESKey"];
                return Encoding.ASCII.GetBytes(s);
            }
        }

        public static byte[] Security_Key_HMACKey
        {
            get
            {
                string s = ConfigurationManager.AppSettings["ISecurityManager.Key.HMACKey"];
                return Encoding.ASCII.GetBytes(s);
            }
        }
    }
}

