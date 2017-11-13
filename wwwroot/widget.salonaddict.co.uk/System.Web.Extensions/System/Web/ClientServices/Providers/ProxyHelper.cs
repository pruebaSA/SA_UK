namespace System.Web.ClientServices.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Web.Resources;
    using System.Web.Script.Serialization;

    internal static class ProxyHelper
    {
        private static void ChangeCookieAndStoreInDB(ref string cookieName, ref string cookieValue, string username, string connectionString, string connectionStringProvider)
        {
            string[] strArray = cookieValue.Split(new char[] { ';' });
            if (strArray.Length >= 1)
            {
                string str = strArray[0];
                bool flag = false;
                StringBuilder builder = new StringBuilder((connectionString == null) ? str : "Q", cookieValue.Length);
                for (int i = 1; i < strArray.Length; i++)
                {
                    if (string.Compare(strArray[i].Trim(), "HttpOnly", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        flag = true;
                    }
                    else
                    {
                        builder.Append(";" + strArray[i]);
                    }
                }
                if (flag)
                {
                    if (connectionString != null)
                    {
                        string str2 = SqlHelper.StoreCookieInDB(cookieName, str, username, connectionString, connectionStringProvider);
                        if (string.IsNullOrEmpty(str2))
                        {
                            return;
                        }
                        cookieName = str2;
                    }
                    cookieName = cookieName.Trim();
                    if (str.Length < 1)
                    {
                        cookieValue = ";" + builder.ToString().Substring((connectionString == null) ? 0 : 1);
                    }
                    else
                    {
                        cookieValue = builder.ToString().Trim();
                    }
                }
            }
        }

        internal static CookieContainer ConstructCookieContainer(string serverUri, string username, string connectionString, string connectionStringProvider)
        {
            if (username == null)
            {
                if (Thread.CurrentPrincipal != null)
                {
                    username = Thread.CurrentPrincipal.Identity.Name;
                }
                else
                {
                    username = string.Empty;
                }
            }
            string[] strArray = GetCookiesFromIECache(serverUri, username, connectionString, connectionStringProvider);
            if ((strArray == null) || (strArray.Length < 1))
            {
                return new CookieContainer();
            }
            CookieContainer container = new CookieContainer(strArray.Length + 10, strArray.Length + 10, 0x1000);
            Uri uri = new Uri(serverUri);
            for (int i = 0; i < strArray.Length; i++)
            {
                if (!string.IsNullOrEmpty(strArray[i]))
                {
                    string str;
                    string str2;
                    int index = strArray[i].IndexOf('=');
                    if (index < 0)
                    {
                        str = strArray[i];
                        str2 = string.Empty;
                    }
                    else
                    {
                        str = strArray[i].Substring(0, index);
                        str2 = strArray[i].Substring(index + 1);
                    }
                    str = str.Trim();
                    str2 = str2.Trim();
                    if ((str.Length != 0x20) || (str2 != "Q"))
                    {
                        container.Add(new Cookie(str, str2, "/", uri.Host));
                    }
                }
            }
            return container;
        }

        internal static object CreateWebRequestAndGetResponse(string serverUri, ref CookieContainer cookies, string username, string connectionString, string connectionStringProvider, string[] paramNames, object[] paramValues, Type returnType)
        {
            object obj2;
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(serverUri);
            request.UseDefaultCredentials = true;
            request.ContentType = "application/json; charset=utf-8";
            request.AllowAutoRedirect = true;
            request.Method = "POST";
            if (cookies == null)
            {
                cookies = ConstructCookieContainer(serverUri, username, connectionString, connectionStringProvider);
            }
            if (cookies != null)
            {
                request.CookieContainer = cookies;
            }
            if ((paramNames != null) && (paramNames.Length > 0))
            {
                byte[] serializedParameters = GetSerializedParameters(paramNames, paramValues);
                request.ContentLength = serializedParameters.Length;
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(serializedParameters, 0, serializedParameters.Length);
                    goto Label_0091;
                }
            }
            request.ContentLength = 0L;
        Label_0091:
            try
            {
                using (HttpWebResponse response = (HttpWebResponse) request.GetResponse())
                {
                    if (response == null)
                    {
                        throw new WebException(AtlasWeb.ClientService_BadJsonResponse);
                    }
                    GetCookiesFromResponse(response, cookies, serverUri, username, connectionString, connectionStringProvider);
                    if (returnType == null)
                    {
                        return null;
                    }
                    JavaScriptSerializer serializer = new JavaScriptSerializer(new SimpleTypeResolver());
                    string responseString = GetResponseString(response);
                    Dictionary<string, object> dictionary = serializer.DeserializeObject(responseString) as Dictionary<string, object>;
                    if ((dictionary == null) || !dictionary.ContainsKey("d"))
                    {
                        throw new WebException(AtlasWeb.ClientService_BadJsonResponse);
                    }
                    obj2 = ObjectConverter.ConvertObjectToType(dictionary["d"], returnType, serializer);
                }
            }
            catch (WebException exception)
            {
                HttpWebResponse response2 = (HttpWebResponse) exception.Response;
                if (response2 == null)
                {
                    throw;
                }
                throw new WebException(string.Format(CultureInfo.CurrentCulture, AtlasWeb.ProxyHelper_BadStatusCode, new object[] { response2.StatusCode.ToString(), GetResponseString(response2) }), exception);
            }
            return obj2;
        }

        internal static bool DoAnyCookiesExist(string serverUri, string username, string connectionString, string connectionStringProvider)
        {
            string[] strArray = GetCookiesFromIECache(serverUri, username, connectionString, connectionStringProvider);
            if ((strArray != null) && (strArray.Length >= 1))
            {
                foreach (string str in strArray)
                {
                    if ((str != null) && (str.Trim().Length > 0))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static string GetCookieFromDB(string cookieHeader, string username, string connectionString, string connectionStringProvider)
        {
            cookieHeader = cookieHeader.Trim();
            if (((cookieHeader.Length == 0x22) && (cookieHeader[0x21] == 'Q')) && (cookieHeader.IndexOf('=') == 0x20))
            {
                return SqlHelper.GetCookieFromDB(cookieHeader.Substring(0, 0x20), username, connectionString, connectionStringProvider);
            }
            return cookieHeader;
        }

        private static string[] GetCookiesFromIECache(string uri, string username, string connectionString, string connectionStringProvider)
        {
            int dwSize = 0;
            if ((UnsafeNativeMethods.InternetGetCookieW(uri, null, null, ref dwSize) == 0) || (dwSize < 1))
            {
                return null;
            }
            StringBuilder cookieValue = new StringBuilder(dwSize);
            if (UnsafeNativeMethods.InternetGetCookieW(uri, null, cookieValue, ref dwSize) == 0)
            {
                return null;
            }
            string[] strArray = cookieValue.ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (connectionString != null)
            {
                for (int i = 0; i < strArray.Length; i++)
                {
                    strArray[i] = GetCookieFromDB(strArray[i], username, connectionString, connectionStringProvider);
                }
            }
            return strArray;
        }

        private static void GetCookiesFromResponse(HttpWebResponse response, CookieContainer cookies, string serverUri, string username, string connectionString, string connectionStringProvider)
        {
            foreach (Cookie cookie in response.Cookies)
            {
                cookies.Add(cookie);
            }
            int count = response.Headers.Count;
            for (int i = 0; i < count; i++)
            {
                string key = response.Headers.GetKey(i);
                if ((key != null) && (key == "Set-Cookie"))
                {
                    string cookieHeaders = response.Headers.Get(i);
                    StoreCookie(serverUri, cookieHeaders, username, connectionString, connectionStringProvider);
                }
            }
        }

        private static string GetResponseString(HttpWebResponse response)
        {
            string str;
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    int capacity = 0x400;
                    if (stream.CanSeek && (stream.Length > capacity))
                    {
                        capacity = (int) stream.Length;
                    }
                    char[] buffer = new char[capacity];
                    StringBuilder builder = new StringBuilder(capacity);
                    for (int i = reader.Read(buffer, 0, capacity); i > 0; i = reader.Read(buffer, 0, capacity))
                    {
                        builder.Append(new string(buffer, 0, i));
                    }
                    str = builder.ToString();
                }
            }
            return str;
        }

        private static byte[] GetSerializedParameters(string[] paramNames, object[] paramValues)
        {
            int length = paramNames.Length;
            if (length != paramValues.Length)
            {
                throw new ArgumentException(null, "paramValues");
            }
            if (length < 1)
            {
                return new byte[0];
            }
            StringBuilder builder = new StringBuilder(40 * length);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            builder.Append("{" + serializer.Serialize(paramNames[0]) + ":" + serializer.Serialize(paramValues[0]));
            for (int i = 1; i < length; i++)
            {
                builder.Append("," + serializer.Serialize(paramNames[i]) + ":" + serializer.Serialize(paramValues[i]));
            }
            builder.Append("}");
            return Encoding.UTF8.GetBytes(builder.ToString());
        }

        private static void StoreCookie(string serverUri, string cookieHeaders, string username, string connectionString, string connectionStringProvider)
        {
            if (!string.IsNullOrEmpty(cookieHeaders))
            {
                string[] strArray = cookieHeaders.Split(new char[] { ',' });
                int index = 0;
                while (index < strArray.Length)
                {
                    StringBuilder builder = new StringBuilder(strArray[index++]);
                    while (index < strArray.Length)
                    {
                        int num2 = strArray[index].IndexOf('=');
                        int num3 = strArray[index].IndexOf(';');
                        if ((num2 > 0) && ((num3 < 0) || (num3 > num2)))
                        {
                            break;
                        }
                        builder.Append(",");
                        builder.Append(strArray[index++]);
                    }
                    string str = builder.ToString();
                    int length = str.IndexOf('=');
                    string cookieName = ((length < 0) ? str : str.Substring(0, length)).Trim();
                    string cookieValue = ((length < 0) ? string.Empty : str.Substring(length + 1)).Trim();
                    if (cookieValue.Length > 0)
                    {
                        ChangeCookieAndStoreInDB(ref cookieName, ref cookieValue, username, connectionString, connectionStringProvider);
                    }
                    UnsafeNativeMethods.InternetSetCookieW(serverUri, null, cookieName + " = " + cookieValue);
                }
            }
        }
    }
}

