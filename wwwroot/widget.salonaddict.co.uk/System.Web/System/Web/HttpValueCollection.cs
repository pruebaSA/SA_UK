﻿namespace System.Web
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Web.Util;

    [Serializable]
    internal class HttpValueCollection : NameValueCollection
    {
        internal HttpValueCollection() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        internal HttpValueCollection(int capacity) : base(capacity, (IEqualityComparer) StringComparer.OrdinalIgnoreCase)
        {
        }

        protected HttpValueCollection(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        internal HttpValueCollection(string str, bool readOnly, bool urlencoded, Encoding encoding) : base(StringComparer.OrdinalIgnoreCase)
        {
            if (!string.IsNullOrEmpty(str))
            {
                this.FillFromString(str, urlencoded, encoding);
            }
            base.IsReadOnly = readOnly;
        }

        internal void Add(HttpCookieCollection c)
        {
            int count = c.Count;
            for (int i = 0; i < count; i++)
            {
                this.ThrowIfMaxHttpCollectionKeysExceeded();
                HttpCookie cookie = c.Get(i);
                base.Add(cookie.Name, cookie.Value);
            }
        }

        internal void FillFromEncodedBytes(byte[] bytes, Encoding encoding)
        {
            int num = (bytes != null) ? bytes.Length : 0;
            for (int i = 0; i < num; i++)
            {
                string str;
                string str2;
                this.ThrowIfMaxHttpCollectionKeysExceeded();
                int offset = i;
                int num4 = -1;
                while (i < num)
                {
                    byte num5 = bytes[i];
                    if (num5 == 0x3d)
                    {
                        if (num4 < 0)
                        {
                            num4 = i;
                        }
                    }
                    else if (num5 == 0x26)
                    {
                        break;
                    }
                    i++;
                }
                if (num4 >= 0)
                {
                    str = HttpUtility.UrlDecode(bytes, offset, num4 - offset, encoding);
                    str2 = HttpUtility.UrlDecode(bytes, num4 + 1, (i - num4) - 1, encoding);
                }
                else
                {
                    str = null;
                    str2 = HttpUtility.UrlDecode(bytes, offset, i - offset, encoding);
                }
                base.Add(str, str2);
                if ((i == (num - 1)) && (bytes[i] == 0x26))
                {
                    base.Add(null, string.Empty);
                }
            }
        }

        internal void FillFromString(string s)
        {
            this.FillFromString(s, false, null);
        }

        internal void FillFromString(string s, bool urlencoded, Encoding encoding)
        {
            int num = (s != null) ? s.Length : 0;
            for (int i = 0; i < num; i++)
            {
                this.ThrowIfMaxHttpCollectionKeysExceeded();
                int startIndex = i;
                int num4 = -1;
                while (i < num)
                {
                    char ch = s[i];
                    if (ch == '=')
                    {
                        if (num4 < 0)
                        {
                            num4 = i;
                        }
                    }
                    else if (ch == '&')
                    {
                        break;
                    }
                    i++;
                }
                string str = null;
                string str2 = null;
                if (num4 >= 0)
                {
                    str = s.Substring(startIndex, num4 - startIndex);
                    str2 = s.Substring(num4 + 1, (i - num4) - 1);
                }
                else
                {
                    str2 = s.Substring(startIndex, i - startIndex);
                }
                if (urlencoded)
                {
                    base.Add(HttpUtility.UrlDecode(str, encoding), HttpUtility.UrlDecode(str2, encoding));
                }
                else
                {
                    base.Add(str, str2);
                }
                if ((i == (num - 1)) && (s[i] == '&'))
                {
                    base.Add(null, string.Empty);
                }
            }
        }

        internal void MakeReadOnly()
        {
            base.IsReadOnly = true;
        }

        internal void MakeReadWrite()
        {
            base.IsReadOnly = false;
        }

        internal void Reset()
        {
            base.Clear();
        }

        private void ThrowIfMaxHttpCollectionKeysExceeded()
        {
            if (this.Count >= AppSettings.MaxHttpCollectionKeys)
            {
                throw new InvalidOperationException();
            }
        }

        public override string ToString() => 
            this.ToString(true);

        internal virtual string ToString(bool urlencoded) => 
            this.ToString(urlencoded, null);

        internal virtual string ToString(bool urlencoded, IDictionary excludeKeys)
        {
            int count = this.Count;
            if (count == 0)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            bool flag = (excludeKeys != null) && (excludeKeys["__VIEWSTATE"] != null);
            for (int i = 0; i < count; i++)
            {
                string key = this.GetKey(i);
                if (((!flag || (key == null)) || !key.StartsWith("__VIEWSTATE", StringComparison.Ordinal)) && (((excludeKeys == null) || (key == null)) || (excludeKeys[key] == null)))
                {
                    string str3;
                    if (urlencoded)
                    {
                        key = HttpUtility.UrlEncodeUnicode(key);
                    }
                    string str2 = !string.IsNullOrEmpty(key) ? (key + "=") : string.Empty;
                    ArrayList list = (ArrayList) base.BaseGet(i);
                    int num3 = (list != null) ? list.Count : 0;
                    if (builder.Length > 0)
                    {
                        builder.Append('&');
                    }
                    if (num3 == 1)
                    {
                        builder.Append(str2);
                        str3 = (string) list[0];
                        if (urlencoded)
                        {
                            str3 = HttpUtility.UrlEncodeUnicode(str3);
                        }
                        builder.Append(str3);
                    }
                    else if (num3 == 0)
                    {
                        builder.Append(str2);
                    }
                    else
                    {
                        for (int j = 0; j < num3; j++)
                        {
                            if (j > 0)
                            {
                                builder.Append('&');
                            }
                            builder.Append(str2);
                            str3 = (string) list[j];
                            if (urlencoded)
                            {
                                str3 = HttpUtility.UrlEncodeUnicode(str3);
                            }
                            builder.Append(str3);
                        }
                    }
                }
            }
            return builder.ToString();
        }
    }
}

