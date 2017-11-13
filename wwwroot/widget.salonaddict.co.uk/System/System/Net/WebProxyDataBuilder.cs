﻿namespace System.Net
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;

    internal abstract class WebProxyDataBuilder
    {
        private WebProxyData m_Result;
        private const string regexReserved = @"#$()+.?[\^{|";
        private static readonly char[] s_AddressListSplitChars = new char[] { ';', '=' };
        private static readonly char[] s_BypassListDelimiter = new char[] { ';' };

        protected WebProxyDataBuilder()
        {
        }

        public WebProxyData Build()
        {
            this.m_Result = new WebProxyData();
            this.BuildInternal();
            return this.m_Result;
        }

        protected abstract void BuildInternal();
        private static string BypassStringEscape(string rawString)
        {
            string str;
            string str2;
            string str3;
            Match match = new Regex("^(?<scheme>.*://)?(?<host>[^:]*)(?<port>:[0-9]{1,5})?$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase).Match(rawString);
            if (match.Success)
            {
                str = match.Groups["scheme"].Value;
                str2 = match.Groups["host"].Value;
                str3 = match.Groups["port"].Value;
            }
            else
            {
                str = string.Empty;
                str2 = rawString;
                str3 = string.Empty;
            }
            str = ConvertRegexReservedChars(str);
            str2 = ConvertRegexReservedChars(str2);
            str3 = ConvertRegexReservedChars(str3);
            if (str == string.Empty)
            {
                str = "(?:.*://)?";
            }
            if (str3 == string.Empty)
            {
                str3 = "(?::[0-9]{1,5})?";
            }
            return ("^" + str + str2 + str3 + "$");
        }

        private static string ConvertRegexReservedChars(string rawString)
        {
            if (rawString.Length == 0)
            {
                return rawString;
            }
            StringBuilder builder = new StringBuilder();
            foreach (char ch in rawString)
            {
                if (@"#$()+.?[\^{|".IndexOf(ch) != -1)
                {
                    builder.Append('\\');
                }
                else if (ch == '*')
                {
                    builder.Append('.');
                }
                builder.Append(ch);
            }
            return builder.ToString();
        }

        private static ArrayList ParseBypassList(string bypassListString, out bool bypassOnLocal)
        {
            string[] strArray = bypassListString.Split(s_BypassListDelimiter);
            bypassOnLocal = false;
            if (strArray.Length == 0)
            {
                return null;
            }
            ArrayList list = null;
            foreach (string str in strArray)
            {
                if (str != null)
                {
                    string strA = str.Trim();
                    if (strA.Length > 0)
                    {
                        if (string.Compare(strA, "<local>", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            bypassOnLocal = true;
                        }
                        else
                        {
                            strA = BypassStringEscape(strA);
                            if (list == null)
                            {
                                list = new ArrayList();
                            }
                            if (!list.Contains(strA))
                            {
                                list.Add(strA);
                            }
                        }
                    }
                }
            }
            return list;
        }

        private static Hashtable ParseProtocolProxies(string proxyListString)
        {
            if (proxyListString.Length == 0)
            {
                return null;
            }
            string[] strArray = proxyListString.Split(s_AddressListSplitChars);
            bool flag = true;
            string str = null;
            Hashtable hashtable = new Hashtable(CaseInsensitiveAscii.StaticInstance);
            foreach (string str2 in strArray)
            {
                string proxyString = str2.Trim().ToLower(CultureInfo.InvariantCulture);
                if (flag)
                {
                    str = proxyString;
                }
                else
                {
                    hashtable[str] = ParseProxyUri(proxyString, false);
                }
                flag = !flag;
            }
            if (hashtable.Count == 0)
            {
                return null;
            }
            return hashtable;
        }

        private static Uri ParseProxyUri(string proxyString, bool validate)
        {
            if (validate)
            {
                if (proxyString.Length == 0)
                {
                    return null;
                }
                if (proxyString.IndexOf('=') != -1)
                {
                    return null;
                }
            }
            if (proxyString.IndexOf("://") == -1)
            {
                proxyString = "http://" + proxyString;
            }
            try
            {
                return new Uri(proxyString);
            }
            catch (UriFormatException exception)
            {
                if (Logging.On)
                {
                    Logging.PrintError(Logging.Web, exception.Message);
                }
            }
            return null;
        }

        protected void SetAutoDetectSettings(bool value)
        {
            this.m_Result.automaticallyDetectSettings = value;
        }

        protected void SetAutoProxyUrl(string autoConfigUrl)
        {
            if (!string.IsNullOrEmpty(autoConfigUrl))
            {
                Uri result = null;
                if (Uri.TryCreate(autoConfigUrl, UriKind.Absolute, out result))
                {
                    this.m_Result.scriptLocation = result;
                }
            }
        }

        protected void SetProxyAndBypassList(string addressString, string bypassListString)
        {
            Uri uri = null;
            Hashtable hashtable = null;
            if (addressString != null)
            {
                uri = ParseProxyUri(addressString, true);
                if (uri == null)
                {
                    hashtable = ParseProtocolProxies(addressString);
                }
                if (((uri != null) || (hashtable != null)) && (bypassListString != null))
                {
                    bool bypassOnLocal = false;
                    this.m_Result.bypassList = ParseBypassList(bypassListString, out bypassOnLocal);
                    this.m_Result.bypassOnLocal = bypassOnLocal;
                }
            }
            if (hashtable != null)
            {
                uri = hashtable["http"] as Uri;
            }
            this.m_Result.proxyAddress = uri;
        }
    }
}

