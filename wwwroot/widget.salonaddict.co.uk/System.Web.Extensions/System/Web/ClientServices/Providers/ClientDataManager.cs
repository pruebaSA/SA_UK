namespace System.Web.ClientServices.Providers
{
    using System;

    internal static class ClientDataManager
    {
        private static ClientData _applicationClientData;
        private static string _curUserName;
        private static ClientData _userClientData;

        internal static void DeleteAllCookies(string username, bool useIsolatedStore)
        {
            ClientData userClientData = GetUserClientData(username, useIsolatedStore);
            userClientData.CookieNames = new string[0];
            userClientData.CookieValues = new string[0];
        }

        internal static ClientData GetAppClientData(bool useIsolatedStore)
        {
            if (_applicationClientData == null)
            {
                _applicationClientData = ClientData.Load(null, useIsolatedStore);
            }
            return _applicationClientData;
        }

        internal static string GetCookie(string username, string cookieName, bool useIsolatedStore)
        {
            ClientData userClientData = GetUserClientData(username, useIsolatedStore);
            if (userClientData.CookieNames == null)
            {
                userClientData.CookieNames = new string[0];
                userClientData.CookieValues = new string[0];
                return null;
            }
            for (int i = 0; i < userClientData.CookieNames.Length; i++)
            {
                if (string.Compare(cookieName, userClientData.CookieNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return userClientData.CookieValues[i];
                }
            }
            return null;
        }

        internal static ClientData GetUserClientData(string username, bool useIsolatedStore)
        {
            if (username != _curUserName)
            {
                _curUserName = username;
                _userClientData = ClientData.Load(username, useIsolatedStore);
            }
            return _userClientData;
        }

        internal static string StoreCookie(string username, string cookieName, string cookieValue, bool useIsolatedStore)
        {
            ClientData userClientData = GetUserClientData(username, useIsolatedStore);
            if (userClientData.CookieNames == null)
            {
                userClientData.CookieNames = new string[0];
                userClientData.CookieValues = new string[0];
            }
            else
            {
                for (int i = 0; i < userClientData.CookieNames.Length; i++)
                {
                    if (userClientData.CookieValues[i].StartsWith(cookieName + "=", StringComparison.OrdinalIgnoreCase))
                    {
                        if (userClientData.CookieValues[i] != (cookieName + "=" + cookieValue))
                        {
                            userClientData.CookieValues[i] = cookieName + "=" + cookieValue;
                            userClientData.Save();
                        }
                        return userClientData.CookieNames[i];
                    }
                }
            }
            string str = Guid.NewGuid().ToString("N");
            string[] array = new string[userClientData.CookieNames.Length + 1];
            string[] strArray2 = new string[userClientData.CookieNames.Length + 1];
            userClientData.CookieNames.CopyTo(array, 0);
            userClientData.CookieValues.CopyTo(strArray2, 0);
            array[userClientData.CookieNames.Length] = str;
            strArray2[userClientData.CookieNames.Length] = cookieName + "=" + cookieValue;
            userClientData.CookieNames = array;
            userClientData.CookieValues = strArray2;
            userClientData.Save();
            return str;
        }
    }
}

