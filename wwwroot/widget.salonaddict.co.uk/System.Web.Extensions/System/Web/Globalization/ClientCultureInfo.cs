namespace System.Web.Globalization
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Web.Script.Serialization;

    internal class ClientCultureInfo
    {
        private static Hashtable cultureScriptBlockCache = Hashtable.Synchronized(new Hashtable());
        public DateTimeFormatInfo dateTimeFormat;
        private static readonly CultureInfo enUS = CultureInfo.GetCultureInfo(0x409);
        public string name;
        public NumberFormatInfo numberFormat;

        private ClientCultureInfo(CultureInfo cultureInfo)
        {
            this.name = cultureInfo.Name;
            this.numberFormat = cultureInfo.NumberFormat;
            this.dateTimeFormat = cultureInfo.DateTimeFormat;
        }

        internal static string GetClientCultureScriptBlock() => 
            GetClientCultureScriptBlock(CultureInfo.CurrentCulture);

        internal static string GetClientCultureScriptBlock(CultureInfo cultureInfo)
        {
            if ((cultureInfo == null) || cultureInfo.Equals(enUS))
            {
                return null;
            }
            object obj2 = cultureScriptBlockCache[cultureInfo];
            if (obj2 == null)
            {
                ClientCultureInfo o = new ClientCultureInfo(cultureInfo);
                string str = JavaScriptSerializer.SerializeInternal(o);
                if (str.Length > 0)
                {
                    obj2 = "var __cultureInfo = '" + str + "';";
                }
                cultureScriptBlockCache[cultureInfo] = obj2;
            }
            return (string) obj2;
        }
    }
}

