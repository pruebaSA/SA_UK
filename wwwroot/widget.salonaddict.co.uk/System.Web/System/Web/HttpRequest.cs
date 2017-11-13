namespace System.Web
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Security.Authentication.ExtendedProtection;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Text;
    using System.Web.Configuration;
    using System.Web.Hosting;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class HttpRequest
    {
        private string[] _acceptTypes;
        internal string _AnonymousId;
        private HttpBrowserCapabilities _browsercaps;
        private VirtualPath _clientBaseDir;
        private HttpClientCertificate _clientCertificate;
        private VirtualPath _clientFilePath;
        private string _clientTarget;
        private bool _computePathInfo;
        private int _contentLength;
        private string _contentType;
        private HttpContext _context;
        private HttpCookieCollection _cookies;
        private VirtualPath _currentExecutionFilePath;
        private Encoding _encoding;
        private VirtualPath _filePath;
        private HttpFileCollection _files;
        private HttpInputStreamFilterSource _filterSource;
        private SimpleBitVector32 _flags;
        private HttpValueCollection _form;
        private HttpHeaderCollection _headers;
        private string _httpMethod;
        private System.Web.HttpVerb _httpVerb;
        private HttpInputStream _inputStream;
        private Stream _installedFilter;
        private WindowsIdentity _logonUserIdentity;
        private MultipartContentElement[] _multipartContentElements;
        private HttpValueCollection _params;
        private VirtualPath _path;
        private VirtualPath _pathInfo;
        private string _pathTranslated;
        private HttpValueCollection _queryString;
        private byte[] _queryStringBytes;
        private bool _queryStringOverriden;
        private string _queryStringText;
        private HttpRawUploadedContent _rawContent;
        private bool _readEntityBody;
        private Uri _referrer;
        private string _requestType;
        private string _rewrittenUrl;
        private HttpServerVarsCollection _serverVariables;
        private Uri _url;
        private string[] _userLanguages;
        private HttpWorkerRequest _wr;
        private const int contentEncodingResolved = 0x20;
        private const int needToValidateCookies = 4;
        private const int needToValidateForm = 2;
        private const int needToValidateHeaders = 8;
        private const int needToValidatePostedFiles = 0x40;
        private const int needToValidateQueryString = 1;
        private const int needToValidateRawUrl = 0x80;
        private const int needToValidateServerVariables = 0x10;
        internal static bool s_browserCapsEvaled = false;
        internal static object s_browserLock = new object();

        internal HttpRequest(HttpWorkerRequest wr, HttpContext context)
        {
            this._contentLength = -1;
            this._wr = wr;
            this._context = context;
        }

        internal HttpRequest(VirtualPath virtualPath, string queryString)
        {
            this._contentLength = -1;
            this._wr = null;
            this._pathTranslated = virtualPath.MapPath();
            this._httpMethod = "GET";
            this._url = new Uri("http://localhost" + virtualPath.VirtualPathString);
            this._path = virtualPath;
            this._queryStringText = queryString;
            this._queryStringOverriden = true;
            this._queryString = new HttpValueCollection(this._queryStringText, true, true, Encoding.Default);
            PerfCounters.IncrementCounter(AppPerfCounter.REQUESTS_EXECUTING);
        }

        public HttpRequest(string filename, string url, string queryString)
        {
            this._contentLength = -1;
            this._wr = null;
            this._pathTranslated = filename;
            this._httpMethod = "GET";
            this._url = new Uri(url);
            this._path = VirtualPath.CreateAbsolute(this._url.AbsolutePath);
            this._queryStringText = queryString;
            this._queryStringOverriden = true;
            this._queryString = new HttpValueCollection(this._queryStringText, true, true, Encoding.Default);
            PerfCounters.IncrementCounter(AppPerfCounter.REQUESTS_EXECUTING);
        }

        internal void AddResponseCookie(HttpCookie cookie)
        {
            if (this._cookies != null)
            {
                this._cookies.AddCookie(cookie, true);
            }
            if (this._params != null)
            {
                this._params.MakeReadWrite();
                this._params.Add(cookie.Name, cookie.Value);
                this._params.MakeReadOnly();
            }
        }

        private void AddServerVariableToCollection(string name)
        {
            this._serverVariables.AddStatic(name, this._wr.GetServerVariable(name));
        }

        private void AddServerVariableToCollection(string name, string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }
            this._serverVariables.AddStatic(name, value);
        }

        private void AddServerVariableToCollection(string name, DynamicServerVariable var)
        {
            this._serverVariables.AddDynamic(name, var);
        }

        internal void AppendToLogQueryString(string logData)
        {
            IIS7WorkerRequest request = this._wr as IIS7WorkerRequest;
            if ((request != null) && !string.IsNullOrEmpty(logData))
            {
                if (this._serverVariables == null)
                {
                    string serverVariable = request.GetServerVariable("LOG_QUERY_STRING");
                    if (string.IsNullOrEmpty(serverVariable))
                    {
                        request.SetServerVariable("LOG_QUERY_STRING", this.QueryStringText + logData);
                    }
                    else
                    {
                        request.SetServerVariable("LOG_QUERY_STRING", serverVariable + logData);
                    }
                }
                else
                {
                    string str2 = this._serverVariables.Get("LOG_QUERY_STRING");
                    if (string.IsNullOrEmpty(str2))
                    {
                        this._serverVariables.SetNoDemand("LOG_QUERY_STRING", this.QueryStringText + logData);
                    }
                    else
                    {
                        this._serverVariables.SetNoDemand("LOG_QUERY_STRING", str2 + logData);
                    }
                }
            }
        }

        public byte[] BinaryRead(int count)
        {
            if ((count < 0) || (count > this.TotalBytes))
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (count == 0)
            {
                return new byte[0];
            }
            byte[] buffer = new byte[count];
            int length = this.InputStream.Read(buffer, 0, count);
            if (length == count)
            {
                return buffer;
            }
            byte[] destinationArray = new byte[length];
            if (length > 0)
            {
                Array.Copy(buffer, destinationArray, length);
            }
            return destinationArray;
        }

        internal string CalcDynamicServerVariable(DynamicServerVariable var)
        {
            switch (var)
            {
                case DynamicServerVariable.AUTH_TYPE:
                    if ((this._context.User == null) || !this._context.User.Identity.IsAuthenticated)
                    {
                        return string.Empty;
                    }
                    return this._context.User.Identity.AuthenticationType;

                case DynamicServerVariable.AUTH_USER:
                    if ((this._context.User == null) || !this._context.User.Identity.IsAuthenticated)
                    {
                        return string.Empty;
                    }
                    return this._context.User.Identity.Name;

                case DynamicServerVariable.PATH_INFO:
                    return this.Path;

                case DynamicServerVariable.PATH_TRANSLATED:
                    return this.PhysicalPathInternal;

                case DynamicServerVariable.QUERY_STRING:
                    return this.QueryStringText;

                case DynamicServerVariable.SCRIPT_NAME:
                    return this.FilePath;
            }
            return null;
        }

        private string CombineAllHeaders(bool asRaw)
        {
            if (this._wr == null)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder(0x100);
            for (int i = 0; i < 40; i++)
            {
                string knownRequestHeader = this._wr.GetKnownRequestHeader(i);
                if (!string.IsNullOrEmpty(knownRequestHeader))
                {
                    string serverVariableNameFromKnownRequestHeaderIndex;
                    if (!asRaw)
                    {
                        serverVariableNameFromKnownRequestHeaderIndex = HttpWorkerRequest.GetServerVariableNameFromKnownRequestHeaderIndex(i);
                    }
                    else
                    {
                        serverVariableNameFromKnownRequestHeaderIndex = HttpWorkerRequest.GetKnownRequestHeaderName(i);
                    }
                    if (serverVariableNameFromKnownRequestHeaderIndex != null)
                    {
                        builder.Append(serverVariableNameFromKnownRequestHeaderIndex);
                        builder.Append(asRaw ? ": " : ":");
                        builder.Append(knownRequestHeader);
                        builder.Append("\r\n");
                    }
                }
            }
            string[][] unknownRequestHeaders = this._wr.GetUnknownRequestHeaders();
            if (unknownRequestHeaders != null)
            {
                for (int j = 0; j < unknownRequestHeaders.Length; j++)
                {
                    string header = unknownRequestHeaders[j][0];
                    if (!asRaw)
                    {
                        header = ServerVariableNameFromHeader(header);
                    }
                    builder.Append(header);
                    builder.Append(asRaw ? ": " : ":");
                    builder.Append(unknownRequestHeaders[j][1]);
                    builder.Append("\r\n");
                }
            }
            return builder.ToString();
        }

        internal static HttpCookie CreateCookieFromString(string s)
        {
            HttpCookie cookie = new HttpCookie();
            int num = (s != null) ? s.Length : 0;
            int startIndex = 0;
            bool flag = true;
            int num5 = 1;
            while (startIndex < num)
            {
                int num4;
                int index = s.IndexOf('&', startIndex);
                if (index < 0)
                {
                    index = num;
                }
                if (flag)
                {
                    num4 = s.IndexOf('=', startIndex);
                    if ((num4 >= 0) && (num4 < index))
                    {
                        cookie.Name = s.Substring(startIndex, num4 - startIndex);
                        startIndex = num4 + 1;
                    }
                    else if (index == num)
                    {
                        cookie.Name = s;
                        return cookie;
                    }
                    flag = false;
                }
                num4 = s.IndexOf('=', startIndex);
                if (((num4 < 0) && (index == num)) && (num5 == 0))
                {
                    cookie.Value = s.Substring(startIndex, num - startIndex);
                }
                else if ((num4 >= 0) && (num4 < index))
                {
                    cookie.Values.Add(s.Substring(startIndex, num4 - startIndex), s.Substring(num4 + 1, (index - num4) - 1));
                    num5++;
                }
                else
                {
                    cookie.Values.Add(null, s.Substring(startIndex, index - startIndex));
                    num5++;
                }
                startIndex = index + 1;
            }
            return cookie;
        }

        [PermissionSet(SecurityAction.Assert, Unrestricted=true)]
        private HttpClientCertificate CreateHttpClientCertificateWithAssert() => 
            new HttpClientCertificate(this._context);

        [PermissionSet(SecurityAction.Assert, Unrestricted=true)]
        private static WindowsIdentity CreateWindowsIdentityWithAssert(IntPtr token, string authType, WindowsAccountType accountType, bool isAuthenticated) => 
            new WindowsIdentity(token, authType, accountType, isAuthenticated);

        internal void Dispose()
        {
            if (this._serverVariables != null)
            {
                this._serverVariables.Dispose();
            }
            if (this._rawContent != null)
            {
                this._rawContent.Dispose();
            }
        }

        internal string FetchServerVariable(string variable) => 
            this._wr.GetServerVariable(variable);

        internal void FillInCookiesCollection(HttpCookieCollection cookieCollection, bool includeResponse)
        {
            if (this._wr != null)
            {
                string knownRequestHeader = this._wr.GetKnownRequestHeader(0x19);
                int num = (knownRequestHeader != null) ? knownRequestHeader.Length : 0;
                int startIndex = 0;
                HttpCookie cookie = null;
                while (startIndex < num)
                {
                    int num3 = startIndex;
                    while (num3 < num)
                    {
                        char ch = knownRequestHeader[num3];
                        if (ch == ';')
                        {
                            break;
                        }
                        num3++;
                    }
                    string s = knownRequestHeader.Substring(startIndex, num3 - startIndex).Trim();
                    startIndex = num3 + 1;
                    if (s.Length != 0)
                    {
                        HttpCookie cookie2 = CreateCookieFromString(s);
                        if (cookie != null)
                        {
                            string name = cookie2.Name;
                            if (((name != null) && (name.Length > 0)) && (name[0] == '$'))
                            {
                                if (StringUtil.EqualsIgnoreCase(name, "$Path"))
                                {
                                    cookie.Path = cookie2.Value;
                                }
                                else if (StringUtil.EqualsIgnoreCase(name, "$Domain"))
                                {
                                    cookie.Domain = cookie2.Value;
                                }
                                continue;
                            }
                        }
                        cookieCollection.AddCookie(cookie2, true);
                        cookie = cookie2;
                    }
                }
                if (includeResponse && (this.Response != null))
                {
                    HttpCookieCollection cookies = this.Response.Cookies;
                    if (cookies.Count > 0)
                    {
                        HttpCookie[] dest = new HttpCookie[cookies.Count];
                        cookies.CopyTo(dest, 0);
                        for (int i = 0; i < dest.Length; i++)
                        {
                            cookieCollection.AddCookie(dest[i], true);
                        }
                    }
                }
            }
        }

        private void FillInFilesCollection()
        {
            if ((this._wr != null) && StringUtil.StringStartsWithIgnoreCase(this.ContentType, "multipart/form-data"))
            {
                MultipartContentElement[] multipartContent = this.GetMultipartContent();
                if (multipartContent != null)
                {
                    bool flag = false;
                    if (this._flags[0x40])
                    {
                        this._flags.Clear(0x40);
                        flag = true;
                    }
                    for (int i = 0; i < multipartContent.Length; i++)
                    {
                        if (multipartContent[i].IsFile)
                        {
                            HttpPostedFile asPostedFile = multipartContent[i].GetAsPostedFile();
                            if (flag)
                            {
                                ValidateString(asPostedFile.FileName, "filename", "Request.Files");
                            }
                            this._files.AddFile(multipartContent[i].Name, asPostedFile);
                        }
                    }
                }
            }
        }

        private void FillInFormCollection()
        {
            if ((this._wr != null) && this._wr.HasEntityBody())
            {
                string contentType = this.ContentType;
                if (contentType != null)
                {
                    if (StringUtil.StringStartsWithIgnoreCase(contentType, "application/x-www-form-urlencoded"))
                    {
                        byte[] bytes = null;
                        HttpRawUploadedContent entireRawContent = this.GetEntireRawContent();
                        if (entireRawContent != null)
                        {
                            bytes = entireRawContent.GetAsByteArray();
                        }
                        if (bytes == null)
                        {
                            return;
                        }
                        try
                        {
                            this._form.FillFromEncodedBytes(bytes, this.ContentEncoding);
                            return;
                        }
                        catch (Exception exception)
                        {
                            throw new HttpException(System.Web.SR.GetString("Invalid_urlencoded_form_data"), exception);
                        }
                    }
                    if (StringUtil.StringStartsWithIgnoreCase(contentType, "multipart/form-data"))
                    {
                        MultipartContentElement[] multipartContent = this.GetMultipartContent();
                        if (multipartContent != null)
                        {
                            for (int i = 0; i < multipartContent.Length; i++)
                            {
                                if (this._form.Count >= AppSettings.MaxHttpCollectionKeys)
                                {
                                    throw new InvalidOperationException();
                                }
                                if (multipartContent[i].IsFormItem)
                                {
                                    this._form.Add(multipartContent[i].Name, multipartContent[i].GetAsString(this.ContentEncoding));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void FillInHeadersCollection()
        {
            if (this._wr != null)
            {
                for (int i = 0; i < 40; i++)
                {
                    string knownRequestHeader = this._wr.GetKnownRequestHeader(i);
                    if (!string.IsNullOrEmpty(knownRequestHeader))
                    {
                        string knownRequestHeaderName = HttpWorkerRequest.GetKnownRequestHeaderName(i);
                        this._headers.SynchronizeHeader(knownRequestHeaderName, knownRequestHeader);
                    }
                }
                string[][] unknownRequestHeaders = this._wr.GetUnknownRequestHeaders();
                if (unknownRequestHeaders != null)
                {
                    for (int j = 0; j < unknownRequestHeaders.Length; j++)
                    {
                        this._headers.SynchronizeHeader(unknownRequestHeaders[j][0], unknownRequestHeaders[j][1]);
                    }
                }
            }
        }

        private void FillInParamsCollection()
        {
            this._params.Add(this.QueryString);
            this._params.Add(this.Form);
            this._params.Add(this.Cookies);
            this._params.Add(this.ServerVariables);
        }

        private void FillInQueryStringCollection()
        {
            byte[] queryStringBytes = this.QueryStringBytes;
            if (queryStringBytes != null)
            {
                if (queryStringBytes.Length != 0)
                {
                    this._queryString.FillFromEncodedBytes(queryStringBytes, this.QueryStringEncoding);
                }
            }
            else if (!string.IsNullOrEmpty(this.QueryStringText))
            {
                this._queryString.FillFromString(this.QueryStringText, true, this.QueryStringEncoding);
            }
        }

        internal void FillInServerVariablesCollection()
        {
            if (this._wr != null)
            {
                this.AddServerVariableToCollection("ALL_HTTP", this.CombineAllHeaders(false));
                this.AddServerVariableToCollection("ALL_RAW", this.CombineAllHeaders(true));
                this.AddServerVariableToCollection("APPL_MD_PATH");
                this.AddServerVariableToCollection("APPL_PHYSICAL_PATH", this._wr.GetAppPathTranslated());
                this.AddServerVariableToCollection("AUTH_TYPE", DynamicServerVariable.AUTH_TYPE);
                this.AddServerVariableToCollection("AUTH_USER", DynamicServerVariable.AUTH_USER);
                this.AddServerVariableToCollection("AUTH_PASSWORD");
                this.AddServerVariableToCollection("LOGON_USER");
                this.AddServerVariableToCollection("REMOTE_USER", DynamicServerVariable.AUTH_USER);
                this.AddServerVariableToCollection("CERT_COOKIE");
                this.AddServerVariableToCollection("CERT_FLAGS");
                this.AddServerVariableToCollection("CERT_ISSUER");
                this.AddServerVariableToCollection("CERT_KEYSIZE");
                this.AddServerVariableToCollection("CERT_SECRETKEYSIZE");
                this.AddServerVariableToCollection("CERT_SERIALNUMBER");
                this.AddServerVariableToCollection("CERT_SERVER_ISSUER");
                this.AddServerVariableToCollection("CERT_SERVER_SUBJECT");
                this.AddServerVariableToCollection("CERT_SUBJECT");
                string knownRequestHeader = this._wr.GetKnownRequestHeader(11);
                this.AddServerVariableToCollection("CONTENT_LENGTH", (knownRequestHeader != null) ? knownRequestHeader : "0");
                this.AddServerVariableToCollection("CONTENT_TYPE", this.ContentType);
                this.AddServerVariableToCollection("GATEWAY_INTERFACE");
                this.AddServerVariableToCollection("HTTPS");
                this.AddServerVariableToCollection("HTTPS_KEYSIZE");
                this.AddServerVariableToCollection("HTTPS_SECRETKEYSIZE");
                this.AddServerVariableToCollection("HTTPS_SERVER_ISSUER");
                this.AddServerVariableToCollection("HTTPS_SERVER_SUBJECT");
                this.AddServerVariableToCollection("INSTANCE_ID");
                this.AddServerVariableToCollection("INSTANCE_META_PATH");
                this.AddServerVariableToCollection("LOCAL_ADDR", this._wr.GetLocalAddress());
                this.AddServerVariableToCollection("PATH_INFO", DynamicServerVariable.PATH_INFO);
                this.AddServerVariableToCollection("PATH_TRANSLATED", DynamicServerVariable.PATH_TRANSLATED);
                this.AddServerVariableToCollection("QUERY_STRING", DynamicServerVariable.QUERY_STRING);
                this.AddServerVariableToCollection("REMOTE_ADDR", this.UserHostAddress);
                this.AddServerVariableToCollection("REMOTE_HOST", this.UserHostName);
                this.AddServerVariableToCollection("REMOTE_PORT");
                this.AddServerVariableToCollection("REQUEST_METHOD", this.HttpMethod);
                this.AddServerVariableToCollection("SCRIPT_NAME", DynamicServerVariable.SCRIPT_NAME);
                this.AddServerVariableToCollection("SERVER_NAME", this._wr.GetServerName());
                this.AddServerVariableToCollection("SERVER_PORT", this._wr.GetLocalPortAsString());
                this.AddServerVariableToCollection("SERVER_PORT_SECURE", this._wr.IsSecure() ? "1" : "0");
                this.AddServerVariableToCollection("SERVER_PROTOCOL", this._wr.GetHttpVersion());
                this.AddServerVariableToCollection("SERVER_SOFTWARE");
                this.AddServerVariableToCollection("URL", DynamicServerVariable.SCRIPT_NAME);
                for (int i = 0; i < 40; i++)
                {
                    string str2 = this._wr.GetKnownRequestHeader(i);
                    if (!string.IsNullOrEmpty(str2))
                    {
                        this.AddServerVariableToCollection(HttpWorkerRequest.GetServerVariableNameFromKnownRequestHeaderIndex(i), str2);
                    }
                }
                string[][] unknownRequestHeaders = this._wr.GetUnknownRequestHeaders();
                if (unknownRequestHeaders != null)
                {
                    for (int j = 0; j < unknownRequestHeaders.Length; j++)
                    {
                        this.AddServerVariableToCollection(ServerVariableNameFromHeader(unknownRequestHeaders[j][0]), unknownRequestHeaders[j][1]);
                    }
                }
            }
        }

        private static string GetAttributeFromHeader(string headerValue, string attrName)
        {
            int index;
            if (headerValue == null)
            {
                return null;
            }
            int length = headerValue.Length;
            int num2 = attrName.Length;
            int startIndex = 1;
            while (startIndex < length)
            {
                startIndex = CultureInfo.InvariantCulture.CompareInfo.IndexOf(headerValue, attrName, startIndex, CompareOptions.IgnoreCase);
                if ((startIndex < 0) || ((startIndex + num2) >= length))
                {
                    break;
                }
                char c = headerValue[startIndex - 1];
                char ch2 = headerValue[startIndex + num2];
                if ((((c == ';') || (c == ',')) || char.IsWhiteSpace(c)) && ((ch2 == '=') || char.IsWhiteSpace(ch2)))
                {
                    break;
                }
                startIndex += num2;
            }
            if ((startIndex < 0) || (startIndex >= length))
            {
                return null;
            }
            startIndex += num2;
            while ((startIndex < length) && char.IsWhiteSpace(headerValue[startIndex]))
            {
                startIndex++;
            }
            if ((startIndex >= length) || (headerValue[startIndex] != '='))
            {
                return null;
            }
            startIndex++;
            while ((startIndex < length) && char.IsWhiteSpace(headerValue[startIndex]))
            {
                startIndex++;
            }
            if (startIndex >= length)
            {
                return null;
            }
            if ((startIndex < length) && (headerValue[startIndex] == '"'))
            {
                if (startIndex == (length - 1))
                {
                    return null;
                }
                index = headerValue.IndexOf('"', startIndex + 1);
                if ((index < 0) || (index == (startIndex + 1)))
                {
                    return null;
                }
                return headerValue.Substring(startIndex + 1, (index - startIndex) - 1).Trim();
            }
            index = startIndex;
            while (index < length)
            {
                if ((headerValue[index] == ' ') || (headerValue[index] == ','))
                {
                    break;
                }
                index++;
            }
            if (index == startIndex)
            {
                return null;
            }
            return headerValue.Substring(startIndex, index - startIndex).Trim();
        }

        private Encoding GetEncodingFromHeaders()
        {
            if ((this.UserAgent != null) && CultureInfo.InvariantCulture.CompareInfo.IsPrefix(this.UserAgent, "UP"))
            {
                string str = this.Headers["x-up-devcap-post-charset"];
                if (!string.IsNullOrEmpty(str))
                {
                    try
                    {
                        return Encoding.GetEncoding(str);
                    }
                    catch
                    {
                    }
                }
            }
            if (!this._wr.HasEntityBody())
            {
                return null;
            }
            string contentType = this.ContentType;
            if (contentType == null)
            {
                return null;
            }
            string attributeFromHeader = GetAttributeFromHeader(contentType, "charset");
            if (attributeFromHeader == null)
            {
                return null;
            }
            Encoding encoding = null;
            try
            {
                encoding = Encoding.GetEncoding(attributeFromHeader);
            }
            catch
            {
            }
            return encoding;
        }

        private HttpRawUploadedContent GetEntireRawContent()
        {
            if (this._wr == null)
            {
                return null;
            }
            if (this._rawContent == null)
            {
                HttpRuntimeSection httpRuntime = RuntimeConfig.GetConfig(this._context).HttpRuntime;
                int maxRequestLengthBytes = httpRuntime.MaxRequestLengthBytes;
                if (this.ContentLength > maxRequestLengthBytes)
                {
                    if (!(this._wr is IIS7WorkerRequest))
                    {
                        this.Response.CloseConnectionAfterError();
                    }
                    throw new HttpException(System.Web.SR.GetString("Max_request_length_exceeded"), null, 0xbbc);
                }
                int requestLengthDiskThresholdBytes = httpRuntime.RequestLengthDiskThresholdBytes;
                HttpRawUploadedContent data = new HttpRawUploadedContent(requestLengthDiskThresholdBytes, this.ContentLength);
                byte[] preloadedEntityBody = this._wr.GetPreloadedEntityBody();
                if (preloadedEntityBody != null)
                {
                    this._wr.UpdateRequestCounters(preloadedEntityBody.Length);
                    data.AddBytes(preloadedEntityBody, 0, preloadedEntityBody.Length);
                }
                if (!this._wr.IsEntireEntityBodyIsPreloaded())
                {
                    int num3 = (this.ContentLength > 0) ? (this.ContentLength - data.Length) : 0x7fffffff;
                    HttpApplication applicationInstance = this._context.ApplicationInstance;
                    byte[] buffer = (applicationInstance != null) ? applicationInstance.EntityBuffer : new byte[0x2000];
                    int length = data.Length;
                    IIS7WorkerRequest request = this._wr as IIS7WorkerRequest;
                    while (num3 > 0)
                    {
                        int num6;
                        int size = buffer.Length;
                        if (size > num3)
                        {
                            size = num3;
                        }
                        if (request != null)
                        {
                            num6 = request.ReadEntityBodyWithTimeout(buffer, size, this._context.TimeLeft);
                        }
                        else
                        {
                            num6 = this._wr.ReadEntityBody(buffer, size);
                        }
                        if (num6 <= 0)
                        {
                            break;
                        }
                        this._wr.UpdateRequestCounters(num6);
                        this._readEntityBody = true;
                        data.AddBytes(buffer, 0, num6);
                        num3 -= num6;
                        length += num6;
                        if (length > maxRequestLengthBytes)
                        {
                            throw new HttpException(System.Web.SR.GetString("Max_request_length_exceeded"), null, 0xbbc);
                        }
                        if ((num3 > 0) && (this._context.TimeLeft <= 0L))
                        {
                            throw new HttpException(System.Web.SR.GetString("Request_timed_out"));
                        }
                    }
                }
                data.DoneAddingBytes();
                if ((this._installedFilter != null) && (data.Length > 0))
                {
                    try
                    {
                        try
                        {
                            this._filterSource.SetContent(data);
                            HttpRawUploadedContent content2 = new HttpRawUploadedContent(requestLengthDiskThresholdBytes, data.Length);
                            HttpApplication application2 = this._context.ApplicationInstance;
                            byte[] buffer3 = (application2 != null) ? application2.EntityBuffer : new byte[0x2000];
                            while (true)
                            {
                                int num7 = this._installedFilter.Read(buffer3, 0, buffer3.Length);
                                if (num7 == 0)
                                {
                                    break;
                                }
                                content2.AddBytes(buffer3, 0, num7);
                            }
                            content2.DoneAddingBytes();
                            data = content2;
                        }
                        finally
                        {
                            this._filterSource.SetContent(null);
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
                this._rawContent = data;
            }
            return this._rawContent;
        }

        private byte[] GetMultipartBoundary()
        {
            string attributeFromHeader = GetAttributeFromHeader(this.ContentType, "boundary");
            if (attributeFromHeader == null)
            {
                return null;
            }
            attributeFromHeader = "--" + attributeFromHeader;
            return Encoding.ASCII.GetBytes(attributeFromHeader.ToCharArray());
        }

        private MultipartContentElement[] GetMultipartContent()
        {
            if (this._multipartContentElements == null)
            {
                byte[] multipartBoundary = this.GetMultipartBoundary();
                if (multipartBoundary == null)
                {
                    return new MultipartContentElement[0];
                }
                HttpRawUploadedContent entireRawContent = this.GetEntireRawContent();
                if (entireRawContent == null)
                {
                    return new MultipartContentElement[0];
                }
                this._multipartContentElements = HttpMultipartContentTemplateParser.Parse(entireRawContent, entireRawContent.Length, multipartBoundary, this.ContentEncoding);
            }
            return this._multipartContentElements;
        }

        private NameValueCollection GetParams()
        {
            if (this._params == null)
            {
                this._params = new HttpValueCollection(0x40);
                this.FillInParamsCollection();
                this._params.MakeReadOnly();
            }
            return this._params;
        }

        [AspNetHostingPermission(SecurityAction.Demand, Level=AspNetHostingPermissionLevel.Low)]
        private NameValueCollection GetParamsWithDemand() => 
            this.GetParams();

        private NameValueCollection GetServerVars()
        {
            if (this._serverVariables == null)
            {
                this._serverVariables = new HttpServerVarsCollection(this._wr, this);
                if (!(this._wr is IIS7WorkerRequest))
                {
                    this._serverVariables.MakeReadOnly();
                }
            }
            return this._serverVariables;
        }

        [AspNetHostingPermission(SecurityAction.Demand, Level=AspNetHostingPermissionLevel.Low)]
        private NameValueCollection GetServerVarsWithDemand() => 
            this.GetServerVars();

        internal void InternalRewritePath(VirtualPath newPath, string newQueryString, bool rebaseClientPath)
        {
            this._pathTranslated = null;
            this._pathInfo = null;
            this._filePath = null;
            this._url = null;
            if (this._wr != null)
            {
                this._wr.GetRawUrl();
            }
            this._path = newPath;
            if (rebaseClientPath)
            {
                this._clientBaseDir = null;
                this._clientFilePath = newPath;
            }
            this._computePathInfo = true;
            if (newQueryString != null)
            {
                this.QueryStringText = newQueryString;
            }
            this._rewrittenUrl = this._path.VirtualPathString;
            string queryStringText = this.QueryStringText;
            if (!string.IsNullOrEmpty(queryStringText))
            {
                this._rewrittenUrl = this._rewrittenUrl + "?" + queryStringText;
            }
            IIS7WorkerRequest request = this._wr as IIS7WorkerRequest;
            if (request != null)
            {
                request.RewriteNotifyPipeline(this._path.VirtualPathString, newQueryString, rebaseClientPath);
            }
        }

        internal void InternalRewritePath(VirtualPath newFilePath, VirtualPath newPathInfo, string newQueryString, bool setClientFilePath)
        {
            this._pathTranslated = (this._wr != null) ? newFilePath.MapPathInternal() : null;
            this._pathInfo = newPathInfo;
            this._filePath = newFilePath;
            this._url = null;
            if (this._wr != null)
            {
                this._wr.GetRawUrl();
            }
            if (newPathInfo == null)
            {
                this._path = newFilePath;
            }
            else
            {
                string virtualPath = newFilePath.VirtualPathStringWhicheverAvailable + "/" + newPathInfo.VirtualPathString;
                this._path = VirtualPath.Create(virtualPath);
            }
            if (newQueryString != null)
            {
                this.QueryStringText = newQueryString;
            }
            this._rewrittenUrl = this._path.VirtualPathString;
            string queryStringText = this.QueryStringText;
            if (!string.IsNullOrEmpty(queryStringText))
            {
                this._rewrittenUrl = this._rewrittenUrl + "?" + queryStringText;
            }
            this._computePathInfo = false;
            if (setClientFilePath)
            {
                this._clientFilePath = newFilePath;
            }
            IIS7WorkerRequest request = this._wr as IIS7WorkerRequest;
            if (request != null)
            {
                string newPath = ((this._path != null) && (this._path.VirtualPathString != null)) ? this._path.VirtualPathString : string.Empty;
                request.RewriteNotifyPipeline(newPath, newQueryString, setClientFilePath);
            }
        }

        internal void InvalidateParams()
        {
            this._params = null;
        }

        public int[] MapImageCoordinates(string imageFieldName)
        {
            NameValueCollection queryString = null;
            switch (this.HttpVerb)
            {
                case System.Web.HttpVerb.GET:
                case System.Web.HttpVerb.HEAD:
                    queryString = this.QueryString;
                    break;

                case System.Web.HttpVerb.POST:
                    queryString = this.Form;
                    break;

                default:
                    return null;
            }
            int[] numArray = null;
            try
            {
                string s = queryString[imageFieldName + ".x"];
                string str2 = queryString[imageFieldName + ".y"];
                if ((s != null) && (str2 != null))
                {
                    numArray = new int[] { int.Parse(s, CultureInfo.InvariantCulture), int.Parse(str2, CultureInfo.InvariantCulture) };
                }
            }
            catch
            {
            }
            return numArray;
        }

        public string MapPath(string virtualPath) => 
            this.MapPath(VirtualPath.CreateAllowNull(virtualPath));

        internal string MapPath(VirtualPath virtualPath)
        {
            if (this._wr != null)
            {
                return this.MapPath(virtualPath, this.FilePathObject, true);
            }
            return virtualPath.MapPath();
        }

        public string MapPath(string virtualPath, string baseVirtualDir, bool allowCrossAppMapping)
        {
            VirtualPath filePathObject;
            if (string.IsNullOrEmpty(baseVirtualDir))
            {
                filePathObject = this.FilePathObject;
            }
            else
            {
                filePathObject = VirtualPath.CreateTrailingSlash(baseVirtualDir);
            }
            return this.MapPath(VirtualPath.CreateAllowNull(virtualPath), filePathObject, allowCrossAppMapping);
        }

        internal string MapPath(VirtualPath virtualPath, VirtualPath baseVirtualDir, bool allowCrossAppMapping)
        {
            if (this._wr == null)
            {
                throw new HttpException(System.Web.SR.GetString("Cannot_map_path_without_context"));
            }
            if (virtualPath == null)
            {
                virtualPath = VirtualPath.Create(".");
            }
            VirtualPath path = virtualPath;
            if (baseVirtualDir != null)
            {
                virtualPath = baseVirtualDir.Combine(virtualPath);
            }
            if (!allowCrossAppMapping)
            {
                virtualPath.FailIfNotWithinAppRoot();
            }
            string str = virtualPath.MapPathInternal();
            if (((virtualPath.VirtualPathString == "/") && (path.VirtualPathString != "/")) && (!path.HasTrailingSlash && UrlPath.PathEndsWithExtraSlash(str)))
            {
                str = str.Substring(0, str.Length - 1);
            }
            InternalSecurityPermissions.PathDiscovery(str).Demand();
            return str;
        }

        private static string[] ParseMultivalueHeader(string s)
        {
            int num = (s != null) ? s.Length : 0;
            if (num == 0)
            {
                return null;
            }
            ArrayList list = new ArrayList();
            int startIndex = 0;
            while (startIndex < num)
            {
                int index = s.IndexOf(',', startIndex);
                if (index < 0)
                {
                    index = num;
                }
                list.Add(s.Substring(startIndex, index - startIndex));
                startIndex = index + 1;
                if ((startIndex < num) && (s[startIndex] == ' '))
                {
                    startIndex++;
                }
            }
            int count = list.Count;
            if (count == 0)
            {
                return null;
            }
            string[] array = new string[count];
            list.CopyTo(0, array, 0, count);
            return array;
        }

        private static string RemoveNullCharacters(string s)
        {
            if (s == null)
            {
                return null;
            }
            if (s.IndexOf('\0') > -1)
            {
                return s.Replace("\0", string.Empty);
            }
            return s;
        }

        internal void ResetCookies()
        {
            if (this._cookies != null)
            {
                this._cookies.Reset();
                this.FillInCookiesCollection(this._cookies, true);
            }
            if (this._params != null)
            {
                this._params.MakeReadWrite();
                this._params.Reset();
                this.FillInParamsCollection();
                this._params.MakeReadOnly();
            }
        }

        public void SaveAs(string filename, bool includeHeaders)
        {
            if (!System.IO.Path.IsPathRooted(filename) && RuntimeConfig.GetConfig(this._context).HttpRuntime.RequireRootedSaveAsPath)
            {
                throw new HttpException(System.Web.SR.GetString("SaveAs_requires_rooted_path", new object[] { filename }));
            }
            FileStream stream = new FileStream(filename, FileMode.Create);
            try
            {
                if (includeHeaders)
                {
                    TextWriter writer = new StreamWriter(stream);
                    writer.Write(this.HttpMethod + " " + this.Path);
                    string queryStringText = this.QueryStringText;
                    if (!string.IsNullOrEmpty(queryStringText))
                    {
                        writer.Write("?" + queryStringText);
                    }
                    if (this._wr != null)
                    {
                        writer.Write(" " + this._wr.GetHttpVersion() + "\r\n");
                        writer.Write(this.CombineAllHeaders(true));
                    }
                    else
                    {
                        writer.Write("\r\n");
                    }
                    writer.Write("\r\n");
                    writer.Flush();
                }
                ((HttpInputStream) this.InputStream).WriteTo(stream);
                stream.Flush();
            }
            finally
            {
                stream.Close();
            }
        }

        private static string ServerVariableNameFromHeader(string header) => 
            ("HTTP_" + header.ToUpper(CultureInfo.InvariantCulture).Replace('-', '_'));

        internal void SetDynamicCompression(bool enable)
        {
            IIS7WorkerRequest request = this._wr as IIS7WorkerRequest;
            if (request != null)
            {
                if (this._serverVariables == null)
                {
                    request.SetServerVariable("IIS_EnableDynamicCompression", enable ? null : "0");
                }
                else
                {
                    this._serverVariables.SetNoDemand("IIS_EnableDynamicCompression", enable ? null : "0");
                }
            }
        }

        internal void SetSkipAuthorization(bool value)
        {
            IIS7WorkerRequest request = this._wr as IIS7WorkerRequest;
            if (request != null)
            {
                if (this._serverVariables == null)
                {
                    request.SetServerVariable("IS_LOGIN_PAGE", value ? "1" : null);
                }
                else
                {
                    this._serverVariables.SetNoDemand("IS_LOGIN_PAGE", value ? "1" : null);
                }
            }
        }

        internal VirtualPath SwitchCurrentExecutionFilePath(VirtualPath path)
        {
            VirtualPath path2 = this._currentExecutionFilePath;
            this._currentExecutionFilePath = path;
            return path2;
        }

        internal HttpValueCollection SwitchForm(HttpValueCollection form)
        {
            HttpValueCollection values = this._form;
            this._form = form;
            return values;
        }

        internal void SynchronizeHeader(string name, string value)
        {
            HttpHeaderCollection headers = this.Headers as HttpHeaderCollection;
            if (headers != null)
            {
                headers.SynchronizeHeader(name, value);
            }
            HttpServerVarsCollection serverVariables = this.ServerVariables as HttpServerVarsCollection;
            if (serverVariables != null)
            {
                string str = "HTTP_" + name.ToUpper(CultureInfo.InvariantCulture).Replace('-', '_');
                serverVariables.SynchronizeServerVariable(str, value);
            }
        }

        internal void SynchronizeServerVariable(string name, string value)
        {
            if (name == "IS_LOGIN_PAGE")
            {
                bool flag = (value != null) && (value != "0");
                this._context.SetSkipAuthorizationNoDemand(flag, true);
            }
            HttpServerVarsCollection serverVariables = this.ServerVariables as HttpServerVarsCollection;
            if (serverVariables != null)
            {
                serverVariables.SynchronizeServerVariable(name, value);
            }
        }

        private static void ValidateCookieCollection(HttpCookieCollection cc)
        {
            int count = cc.Count;
            for (int i = 0; i < count; i++)
            {
                string key = cc.GetKey(i);
                string str2 = cc.Get(i).Value;
                if (!string.IsNullOrEmpty(str2))
                {
                    ValidateString(str2, key, "Request.Cookies");
                }
            }
        }

        public void ValidateInput()
        {
            this._flags.Set(1);
            this._flags.Set(2);
            this._flags.Set(4);
            this._flags.Set(0x40);
            this._flags.Set(0x80);
        }

        private static void ValidateNameValueCollection(NameValueCollection nvc, string collectionName)
        {
            int count = nvc.Count;
            for (int i = 0; i < count; i++)
            {
                string key = nvc.GetKey(i);
                if ((key == null) || !key.StartsWith("__", StringComparison.Ordinal))
                {
                    string str2 = nvc.Get(i);
                    if (!string.IsNullOrEmpty(str2))
                    {
                        ValidateString(str2, key, collectionName);
                    }
                }
            }
        }

        internal string ValidateRawUrl() => 
            this.RawUrl;

        private static void ValidateString(string s, string valueName, string collectionName)
        {
            s = RemoveNullCharacters(s);
            int matchIndex = 0;
            if (CrossSiteScriptingValidation.IsDangerousString(s, out matchIndex))
            {
                string str = valueName + "=\"";
                int startIndex = matchIndex - 10;
                if (startIndex <= 0)
                {
                    startIndex = 0;
                }
                else
                {
                    str = str + "...";
                }
                int length = matchIndex + 20;
                if (length >= s.Length)
                {
                    length = s.Length;
                    str = str + s.Substring(startIndex, length - startIndex) + "\"";
                }
                else
                {
                    str = str + s.Substring(startIndex, length - startIndex) + "...\"";
                }
                throw new HttpRequestValidationException(System.Web.SR.GetString("Dangerous_input_detected", new object[] { collectionName, str }));
            }
        }

        public string[] AcceptTypes
        {
            get
            {
                if ((this._acceptTypes == null) && (this._wr != null))
                {
                    this._acceptTypes = ParseMultivalueHeader(this._wr.GetKnownRequestHeader(20));
                }
                return this._acceptTypes;
            }
        }

        public string AnonymousID =>
            this._AnonymousId;

        public string ApplicationPath =>
            HttpRuntime.AppDomainAppVirtualPath;

        internal VirtualPath ApplicationPathObject =>
            HttpRuntime.AppDomainAppVirtualPathObject;

        public string AppRelativeCurrentExecutionFilePath =>
            UrlPath.MakeVirtualPathAppRelative(this.CurrentExecutionFilePath);

        public HttpBrowserCapabilities Browser
        {
            get
            {
                if (this._browsercaps == null)
                {
                    if (!s_browserCapsEvaled)
                    {
                        lock (s_browserLock)
                        {
                            if (!s_browserCapsEvaled)
                            {
                                HttpCapabilitiesBase.GetBrowserCapabilities(this);
                            }
                            s_browserCapsEvaled = true;
                        }
                    }
                    this._browsercaps = HttpCapabilitiesBase.GetBrowserCapabilities(this);
                }
                return this._browsercaps;
            }
            set
            {
                this._browsercaps = value;
            }
        }

        internal VirtualPath ClientBaseDir
        {
            get
            {
                if (this._clientBaseDir == null)
                {
                    if (this.ClientFilePath.HasTrailingSlash)
                    {
                        this._clientBaseDir = this.ClientFilePath;
                    }
                    else
                    {
                        this._clientBaseDir = this.ClientFilePath.Parent;
                    }
                }
                return this._clientBaseDir;
            }
        }

        public HttpClientCertificate ClientCertificate
        {
            [AspNetHostingPermission(SecurityAction.Demand, Level=AspNetHostingPermissionLevel.Low)]
            get
            {
                if (this._clientCertificate == null)
                {
                    this._clientCertificate = this.CreateHttpClientCertificateWithAssert();
                }
                return this._clientCertificate;
            }
        }

        internal VirtualPath ClientFilePath
        {
            get
            {
                if (this._clientFilePath == null)
                {
                    if (this._wr != null)
                    {
                        if (this._wr.IsRewriteModuleEnabled)
                        {
                            string rawUrl = this.RawUrl;
                            int index = rawUrl.IndexOf('?');
                            if (index > -1)
                            {
                                rawUrl = rawUrl.Substring(0, index);
                            }
                            this._clientFilePath = VirtualPath.Create(rawUrl, VirtualPathOptions.AllowAbsolutePath);
                        }
                        else
                        {
                            this._clientFilePath = this._wr.GetFilePathObject();
                        }
                    }
                    else
                    {
                        this._clientFilePath = this.PathObject;
                    }
                }
                return this._clientFilePath;
            }
            set
            {
                this._clientFilePath = value;
            }
        }

        internal string ClientTarget
        {
            get
            {
                if (this._clientTarget != null)
                {
                    return this._clientTarget;
                }
                return string.Empty;
            }
            set
            {
                this._clientTarget = value;
                this._browsercaps = null;
            }
        }

        public Encoding ContentEncoding
        {
            get
            {
                if (!this._flags[0x20] || (this._encoding == null))
                {
                    this._encoding = this.GetEncodingFromHeaders();
                    if (this._encoding == null)
                    {
                        GlobalizationSection globalization = RuntimeConfig.GetLKGConfig(this._context).Globalization;
                        this._encoding = globalization.RequestEncoding;
                    }
                    this._flags.Set(0x20);
                }
                return this._encoding;
            }
            set
            {
                this._encoding = value;
                this._flags.Set(0x20);
            }
        }

        public int ContentLength
        {
            get
            {
                if ((this._contentLength == -1) && (this._wr != null))
                {
                    string knownRequestHeader = this._wr.GetKnownRequestHeader(11);
                    if (knownRequestHeader != null)
                    {
                        try
                        {
                            this._contentLength = int.Parse(knownRequestHeader, CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                        }
                    }
                    else if (this._wr.IsEntireEntityBodyIsPreloaded())
                    {
                        byte[] preloadedEntityBody = this._wr.GetPreloadedEntityBody();
                        if (preloadedEntityBody != null)
                        {
                            this._contentLength = preloadedEntityBody.Length;
                        }
                    }
                }
                if (this._contentLength < 0)
                {
                    return 0;
                }
                return this._contentLength;
            }
        }

        public string ContentType
        {
            get
            {
                if (this._contentType == null)
                {
                    if (this._wr != null)
                    {
                        this._contentType = this._wr.GetKnownRequestHeader(12);
                    }
                    if (this._contentType == null)
                    {
                        this._contentType = string.Empty;
                    }
                }
                return this._contentType;
            }
            set
            {
                this._contentType = value;
            }
        }

        internal HttpContext Context
        {
            get => 
                this._context;
            set
            {
                this._context = value;
            }
        }

        public HttpCookieCollection Cookies
        {
            get
            {
                if (this._cookies == null)
                {
                    this._cookies = new HttpCookieCollection(null, false);
                    if (this._wr != null)
                    {
                        this.FillInCookiesCollection(this._cookies, true);
                    }
                }
                if (this._flags[4])
                {
                    this._flags.Clear(4);
                    ValidateCookieCollection(this._cookies);
                }
                return this._cookies;
            }
        }

        public string CurrentExecutionFilePath =>
            this.CurrentExecutionFilePathObject.VirtualPathString;

        internal VirtualPath CurrentExecutionFilePathObject
        {
            get
            {
                if (this._currentExecutionFilePath != null)
                {
                    return this._currentExecutionFilePath;
                }
                return this.FilePathObject;
            }
        }

        internal byte[] EntityBody
        {
            get
            {
                if (!this._readEntityBody)
                {
                    return null;
                }
                return this._rawContent.GetAsByteArray();
            }
        }

        public string FilePath =>
            VirtualPath.GetVirtualPathString(this.FilePathObject);

        internal VirtualPath FilePathObject
        {
            get
            {
                if (this._filePath == null)
                {
                    if (!this._computePathInfo)
                    {
                        if (this._wr != null)
                        {
                            this._filePath = this._wr.GetFilePathObject();
                            if (!this._wr.IsRewriteModuleEnabled)
                            {
                                this._clientFilePath = this._filePath;
                            }
                        }
                        else
                        {
                            this._filePath = this.PathObject;
                        }
                    }
                    else if (this._context != null)
                    {
                        this._filePath = this.PathObject;
                        int length = this._context.GetFilePathData().Path.VirtualPathStringNoTrailingSlash.Length;
                        if (this.Path.Length == length)
                        {
                            this._filePath = this.PathObject;
                        }
                        else
                        {
                            this._filePath = VirtualPath.CreateAbsolute(this.Path.Substring(0, length));
                        }
                    }
                }
                return this._filePath;
            }
        }

        public HttpFileCollection Files
        {
            get
            {
                if (this._files == null)
                {
                    this._files = new HttpFileCollection();
                    if (this._wr != null)
                    {
                        this.FillInFilesCollection();
                    }
                }
                return this._files;
            }
        }

        public Stream Filter
        {
            get
            {
                if (this._installedFilter != null)
                {
                    return this._installedFilter;
                }
                if (this._filterSource == null)
                {
                    this._filterSource = new HttpInputStreamFilterSource();
                }
                return this._filterSource;
            }
            set
            {
                if (this._filterSource == null)
                {
                    throw new HttpException(System.Web.SR.GetString("Invalid_request_filter"));
                }
                this._installedFilter = value;
            }
        }

        public NameValueCollection Form
        {
            get
            {
                if (this._form == null)
                {
                    this._form = new HttpValueCollection();
                    if (this._wr != null)
                    {
                        this.FillInFormCollection();
                    }
                    this._form.MakeReadOnly();
                }
                if (this._flags[2])
                {
                    this._flags.Clear(2);
                    ValidateNameValueCollection(this._form, "Request.Form");
                }
                return this._form;
            }
        }

        internal bool HasForm
        {
            get
            {
                if (this._form != null)
                {
                    return (this._form.Count > 0);
                }
                if ((this._wr != null) && !this._wr.HasEntityBody())
                {
                    return false;
                }
                return (this.Form.Count > 0);
            }
        }

        internal bool HasQueryString
        {
            get
            {
                if (this._queryString != null)
                {
                    return (this._queryString.Count > 0);
                }
                byte[] queryStringBytes = this.QueryStringBytes;
                if (queryStringBytes != null)
                {
                    return (queryStringBytes.Length > 0);
                }
                return (this.QueryStringText.Length > 0);
            }
        }

        public NameValueCollection Headers
        {
            get
            {
                if (this._headers == null)
                {
                    this._headers = new HttpHeaderCollection(this._wr, this, 8);
                    if (this._wr != null)
                    {
                        this.FillInHeadersCollection();
                    }
                    if (!(this._wr is IIS7WorkerRequest))
                    {
                        this._headers.MakeReadOnly();
                    }
                }
                return this._headers;
            }
        }

        public ChannelBinding HttpChannelBinding
        {
            [PermissionSet(SecurityAction.Demand, Unrestricted=true)]
            get
            {
                if (this._wr is IIS7WorkerRequest)
                {
                    return ((IIS7WorkerRequest) this._wr).HttpChannelBindingToken;
                }
                if (!(this._wr is ISAPIWorkerRequestInProc))
                {
                    throw new PlatformNotSupportedException();
                }
                return ((ISAPIWorkerRequestInProc) this._wr).HttpChannelBindingToken;
            }
        }

        public string HttpMethod
        {
            get
            {
                if (this._httpMethod == null)
                {
                    this._httpMethod = this._wr.GetHttpVerbName();
                }
                return this._httpMethod;
            }
        }

        internal System.Web.HttpVerb HttpVerb
        {
            get
            {
                if (this._httpVerb == System.Web.HttpVerb.Unparsed)
                {
                    this._httpVerb = System.Web.HttpVerb.Unknown;
                    string httpMethod = this.HttpMethod;
                    if (httpMethod != null)
                    {
                        switch (httpMethod.Length)
                        {
                            case 3:
                                if (httpMethod != "GET")
                                {
                                    if (httpMethod == "PUT")
                                    {
                                        this._httpVerb = System.Web.HttpVerb.PUT;
                                    }
                                    break;
                                }
                                this._httpVerb = System.Web.HttpVerb.GET;
                                break;

                            case 4:
                                if (httpMethod != "POST")
                                {
                                    if (httpMethod == "HEAD")
                                    {
                                        this._httpVerb = System.Web.HttpVerb.HEAD;
                                    }
                                    break;
                                }
                                this._httpVerb = System.Web.HttpVerb.POST;
                                break;

                            case 5:
                                if (httpMethod == "DEBUG")
                                {
                                    this._httpVerb = System.Web.HttpVerb.DEBUG;
                                }
                                break;

                            case 6:
                                if (httpMethod == "DELETE")
                                {
                                    this._httpVerb = System.Web.HttpVerb.DELETE;
                                }
                                break;
                        }
                    }
                }
                return this._httpVerb;
            }
        }

        internal string IfModifiedSince =>
            this._wr?.GetKnownRequestHeader(30);

        internal string IfNoneMatch =>
            this._wr?.GetKnownRequestHeader(0x1f);

        public Stream InputStream
        {
            get
            {
                if (this._inputStream == null)
                {
                    HttpRawUploadedContent data = null;
                    if (this._wr != null)
                    {
                        data = this.GetEntireRawContent();
                    }
                    if (data != null)
                    {
                        this._inputStream = new HttpInputStream(data, 0, data.Length);
                    }
                    else
                    {
                        this._inputStream = new HttpInputStream(null, 0, 0);
                    }
                }
                return this._inputStream;
            }
        }

        public bool IsAuthenticated =>
            (((this._context.User != null) && (this._context.User.Identity != null)) && this._context.User.Identity.IsAuthenticated);

        internal bool IsDebuggingRequest =>
            (this.HttpVerb == System.Web.HttpVerb.DEBUG);

        public bool IsLocal
        {
            get
            {
                string userHostAddress = this.UserHostAddress;
                if (string.IsNullOrEmpty(userHostAddress))
                {
                    return false;
                }
                return (((userHostAddress == "127.0.0.1") || (userHostAddress == "::1")) || (userHostAddress == this.LocalAddress));
            }
        }

        public bool IsSecureConnection =>
            ((this._wr != null) && this._wr.IsSecure());

        public string this[string key]
        {
            get
            {
                string str = this.QueryString[key];
                if (str != null)
                {
                    return str;
                }
                str = this.Form[key];
                if (str != null)
                {
                    return str;
                }
                HttpCookie cookie = this.Cookies[key];
                if (cookie != null)
                {
                    return cookie.Value;
                }
                str = this.ServerVariables[key];
                if (str != null)
                {
                    return str;
                }
                return null;
            }
        }

        internal string LocalAddress
        {
            get
            {
                if (this._wr != null)
                {
                    return this._wr.GetLocalAddress();
                }
                return null;
            }
        }

        public WindowsIdentity LogonUserIdentity
        {
            [AspNetHostingPermission(SecurityAction.Demand, Level=AspNetHostingPermissionLevel.Medium)]
            get
            {
                if ((this._logonUserIdentity == null) && (this._wr != null))
                {
                    if ((this._wr is IIS7WorkerRequest) && (((this._context.NotificationContext.CurrentNotification == RequestNotification.AuthenticateRequest) && !this._context.NotificationContext.IsPostNotification) || (this._context.NotificationContext.CurrentNotification < RequestNotification.AuthenticateRequest)))
                    {
                        throw new InvalidOperationException(System.Web.SR.GetString("Invalid_before_authentication"));
                    }
                    IntPtr userToken = this._wr.GetUserToken();
                    if (userToken != IntPtr.Zero)
                    {
                        string serverVariable = this._wr.GetServerVariable("LOGON_USER");
                        string str2 = this._wr.GetServerVariable("AUTH_TYPE");
                        bool isAuthenticated = !string.IsNullOrEmpty(serverVariable) || (!string.IsNullOrEmpty(str2) && !StringUtil.EqualsIgnoreCase(str2, "basic"));
                        this._logonUserIdentity = CreateWindowsIdentityWithAssert(userToken, (str2 == null) ? "" : str2, WindowsAccountType.Normal, isAuthenticated);
                    }
                }
                return this._logonUserIdentity;
            }
        }

        public NameValueCollection Params
        {
            get
            {
                if (HttpRuntime.HasAspNetHostingPermission(AspNetHostingPermissionLevel.Low))
                {
                    return this.GetParams();
                }
                return this.GetParamsWithDemand();
            }
        }

        public string Path =>
            this.PathObject.VirtualPathString;

        public string PathInfo
        {
            get
            {
                if (this.PathInfoObject != null)
                {
                    return this.PathInfoObject.VirtualPathString;
                }
                return string.Empty;
            }
        }

        internal VirtualPath PathInfoObject
        {
            get
            {
                if (this._pathInfo == null)
                {
                    if (!this._computePathInfo && (this._wr != null))
                    {
                        this._pathInfo = VirtualPath.CreateAbsoluteAllowNull(this._wr.GetPathInfo());
                    }
                    if ((this._pathInfo == null) && (this._context != null))
                    {
                        VirtualPath pathObject = this.PathObject;
                        int length = pathObject.VirtualPathString.Length;
                        VirtualPath filePathObject = this.FilePathObject;
                        int startIndex = filePathObject.VirtualPathString.Length;
                        if (filePathObject == null)
                        {
                            this._pathInfo = pathObject;
                        }
                        else if ((pathObject == null) || (length <= startIndex))
                        {
                            this._pathInfo = null;
                        }
                        else
                        {
                            string virtualPath = pathObject.VirtualPathString.Substring(startIndex, length - startIndex);
                            this._pathInfo = VirtualPath.CreateAbsolute(virtualPath);
                        }
                    }
                }
                return this._pathInfo;
            }
        }

        internal VirtualPath PathObject
        {
            get
            {
                if (this._path == null)
                {
                    this._path = VirtualPath.Create(this._wr.GetUriPath(), VirtualPathOptions.AllowAbsolutePath);
                }
                return this._path;
            }
        }

        internal string PathWithQueryString
        {
            get
            {
                string queryStringText = this.QueryStringText;
                if (string.IsNullOrEmpty(queryStringText))
                {
                    return this.Path;
                }
                return (this.Path + "?" + queryStringText);
            }
        }

        public string PhysicalApplicationPath
        {
            get
            {
                InternalSecurityPermissions.AppPathDiscovery.Demand();
                if (this._wr != null)
                {
                    return this._wr.GetAppPathTranslated();
                }
                return null;
            }
        }

        public string PhysicalPath
        {
            get
            {
                string physicalPathInternal = this.PhysicalPathInternal;
                InternalSecurityPermissions.PathDiscovery(physicalPathInternal).Demand();
                return physicalPathInternal;
            }
        }

        internal string PhysicalPathInternal
        {
            get
            {
                if (this._pathTranslated == null)
                {
                    if (!this._computePathInfo)
                    {
                        this._pathTranslated = this._wr.GetFilePathTranslated();
                    }
                    if ((this._pathTranslated == null) && (this._wr != null))
                    {
                        this._pathTranslated = HostingEnvironment.MapPathInternal(this.FilePath);
                    }
                }
                return this._pathTranslated;
            }
        }

        public NameValueCollection QueryString
        {
            get
            {
                if (this._queryString == null)
                {
                    this._queryString = new HttpValueCollection();
                    if (this._wr != null)
                    {
                        this.FillInQueryStringCollection();
                    }
                    this._queryString.MakeReadOnly();
                }
                if (this._flags[1])
                {
                    this._flags.Clear(1);
                    ValidateNameValueCollection(this._queryString, "Request.QueryString");
                }
                return this._queryString;
            }
        }

        internal byte[] QueryStringBytes
        {
            get
            {
                if (this._queryStringOverriden)
                {
                    return null;
                }
                if ((this._queryStringBytes == null) && (this._wr != null))
                {
                    this._queryStringBytes = this._wr.GetQueryStringRawBytes();
                }
                return this._queryStringBytes;
            }
        }

        internal Encoding QueryStringEncoding
        {
            get
            {
                Encoding contentEncoding = this.ContentEncoding;
                if (!contentEncoding.Equals(Encoding.Unicode))
                {
                    return contentEncoding;
                }
                return Encoding.UTF8;
            }
        }

        internal string QueryStringText
        {
            get
            {
                if (this._queryStringText == null)
                {
                    if (this._wr != null)
                    {
                        byte[] queryStringBytes = this.QueryStringBytes;
                        if (queryStringBytes != null)
                        {
                            if (queryStringBytes.Length > 0)
                            {
                                this._queryStringText = this.QueryStringEncoding.GetString(queryStringBytes);
                            }
                            else
                            {
                                this._queryStringText = string.Empty;
                            }
                        }
                        else
                        {
                            this._queryStringText = this._wr.GetQueryString();
                        }
                    }
                    if (this._queryStringText == null)
                    {
                        this._queryStringText = string.Empty;
                    }
                }
                return this._queryStringText;
            }
            set
            {
                this._queryStringText = value;
                this._queryStringOverriden = true;
                if (this._queryString != null)
                {
                    this._params = null;
                    this._queryString.MakeReadWrite();
                    this._queryString.Reset();
                    this.FillInQueryStringCollection();
                    this._queryString.MakeReadOnly();
                }
            }
        }

        public string RawUrl
        {
            get
            {
                string rawUrl;
                if (this._wr != null)
                {
                    rawUrl = this._wr.GetRawUrl();
                }
                else
                {
                    string path = this.Path;
                    string queryStringText = this.QueryStringText;
                    if (!string.IsNullOrEmpty(queryStringText))
                    {
                        rawUrl = path + "?" + queryStringText;
                    }
                    else
                    {
                        rawUrl = path;
                    }
                }
                if (this._flags[0x80])
                {
                    this._flags.Clear(0x80);
                    ValidateString(rawUrl, null, "Request.RawUrl");
                }
                return rawUrl;
            }
        }

        public string RequestType
        {
            get
            {
                if (this._requestType == null)
                {
                    return this.HttpMethod;
                }
                return this._requestType;
            }
            set
            {
                this._requestType = value;
            }
        }

        internal HttpResponse Response =>
            this._context?.Response;

        internal string RewrittenUrl =>
            this._rewrittenUrl;

        public NameValueCollection ServerVariables
        {
            get
            {
                if (HttpRuntime.HasAspNetHostingPermission(AspNetHostingPermissionLevel.Low))
                {
                    return this.GetServerVars();
                }
                return this.GetServerVarsWithDemand();
            }
        }

        public int TotalBytes
        {
            get
            {
                Stream inputStream = this.InputStream;
                if (inputStream == null)
                {
                    return 0;
                }
                return (int) inputStream.Length;
            }
        }

        public Uri Url
        {
            get
            {
                if ((this._url == null) && (this._wr != null))
                {
                    string queryStringText = this.QueryStringText;
                    if (!string.IsNullOrEmpty(queryStringText))
                    {
                        queryStringText = "?" + HttpUtility.CollapsePercentUFromStringInternal(queryStringText, this.QueryStringEncoding);
                    }
                    if (AppSettings.UseHostHeaderForRequestUrl)
                    {
                        string knownRequestHeader = this._wr.GetKnownRequestHeader(0x1c);
                        try
                        {
                            if (!string.IsNullOrEmpty(knownRequestHeader))
                            {
                                this._url = new Uri(this._wr.GetProtocol() + "://" + knownRequestHeader + this.Path + queryStringText);
                            }
                        }
                        catch (UriFormatException)
                        {
                        }
                    }
                    if (this._url == null)
                    {
                        string serverName = this._wr.GetServerName();
                        if ((serverName.IndexOf(':') >= 0) && (serverName[0] != '['))
                        {
                            serverName = "[" + serverName + "]";
                        }
                        this._url = new Uri(this._wr.GetProtocol() + "://" + serverName + ":" + this._wr.GetLocalPortAsString() + this.Path + queryStringText);
                    }
                }
                return this._url;
            }
        }

        internal string UrlInternal
        {
            get
            {
                string queryStringText = this.QueryStringText;
                if (!string.IsNullOrEmpty(queryStringText))
                {
                    queryStringText = "?" + HttpUtility.CollapsePercentUFromStringInternal(queryStringText, this.QueryStringEncoding);
                }
                if (AppSettings.UseHostHeaderForRequestUrl)
                {
                    string knownRequestHeader = this._wr.GetKnownRequestHeader(0x1c);
                    try
                    {
                        if (!string.IsNullOrEmpty(knownRequestHeader))
                        {
                            string uriString = this._wr.GetProtocol() + "://" + knownRequestHeader + this.Path + queryStringText;
                            this._url = new Uri(uriString);
                            return uriString;
                        }
                    }
                    catch (UriFormatException)
                    {
                    }
                }
                string serverName = this._wr.GetServerName();
                if ((serverName.IndexOf(':') >= 0) && (serverName[0] != '['))
                {
                    serverName = "[" + serverName + "]";
                }
                if (this._wr.GetLocalPortAsString() == "80")
                {
                    return (this._wr.GetProtocol() + "://" + serverName + this.Path + queryStringText);
                }
                return (this._wr.GetProtocol() + "://" + serverName + ":" + this._wr.GetLocalPortAsString() + this.Path + queryStringText);
            }
        }

        public Uri UrlReferrer
        {
            get
            {
                if ((this._referrer == null) && (this._wr != null))
                {
                    string knownRequestHeader = this._wr.GetKnownRequestHeader(0x24);
                    if (!string.IsNullOrEmpty(knownRequestHeader))
                    {
                        try
                        {
                            if (knownRequestHeader.IndexOf("://", StringComparison.Ordinal) >= 0)
                            {
                                this._referrer = new Uri(knownRequestHeader);
                            }
                            else
                            {
                                this._referrer = new Uri(this.Url, knownRequestHeader);
                            }
                        }
                        catch (HttpException)
                        {
                            this._referrer = null;
                        }
                    }
                }
                return this._referrer;
            }
        }

        public string UserAgent
        {
            get
            {
                if (this._wr != null)
                {
                    return this._wr.GetKnownRequestHeader(0x27);
                }
                return null;
            }
        }

        public string UserHostAddress
        {
            get
            {
                if (this._wr != null)
                {
                    return this._wr.GetRemoteAddress();
                }
                return null;
            }
        }

        public string UserHostName
        {
            get
            {
                string remoteName = this._wr?.GetRemoteName();
                if (string.IsNullOrEmpty(remoteName))
                {
                    remoteName = this.UserHostAddress;
                }
                return remoteName;
            }
        }

        public string[] UserLanguages
        {
            get
            {
                if ((this._userLanguages == null) && (this._wr != null))
                {
                    this._userLanguages = ParseMultivalueHeader(this._wr.GetKnownRequestHeader(0x17));
                }
                return this._userLanguages;
            }
        }
    }
}

