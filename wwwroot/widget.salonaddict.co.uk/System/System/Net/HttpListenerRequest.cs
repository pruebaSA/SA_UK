namespace System.Net
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Authentication.ExtendedProtection;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Principal;
    using System.Text;

    public sealed class HttpListenerRequest
    {
        internal const uint CertBoblSize = 0x5dc;
        private BoundaryType m_BoundaryType;
        private X509Certificate2 m_ClientCertificate;
        private int m_ClientCertificateError;
        private ListenerClientCertState m_ClientCertState;
        internal ulong m_ConnectionId;
        private long m_ContentLength;
        private string m_CookedUrl;
        private CookieCollection m_Cookies;
        private System.Net.HttpListenerContext m_HttpContext;
        private string m_HttpMethod;
        private bool m_IsDisposed;
        private TriState m_KeepAlive;
        private IPEndPoint m_LocalEndPoint;
        private RequestContextBase m_MemoryBlob;
        private string m_RawUrl;
        private IPEndPoint m_RemoteEndPoint;
        private ulong m_RequestId;
        private Stream m_RequestStream;
        private Uri m_RequestUri;
        private string m_ServiceName;
        private SslStatus m_SslStatus;
        private Version m_Version;
        private WebHeaderCollection m_WebHeaders;

        internal unsafe HttpListenerRequest(System.Net.HttpListenerContext httpContext, RequestContextBase memoryBlob)
        {
            if (Logging.On)
            {
                Logging.PrintInfo(Logging.HttpListener, this, ".ctor", "httpContext#" + ValidationHelper.HashString(httpContext) + " memoryBlob# " + ValidationHelper.HashString((IntPtr) memoryBlob.RequestBlob));
            }
            if (Logging.On)
            {
                Logging.Associate(Logging.HttpListener, this, httpContext);
            }
            this.m_HttpContext = httpContext;
            this.m_MemoryBlob = memoryBlob;
            this.m_BoundaryType = BoundaryType.None;
            this.m_RequestId = memoryBlob.RequestBlob.RequestId;
            this.m_ConnectionId = memoryBlob.RequestBlob.ConnectionId;
            this.m_SslStatus = (memoryBlob.RequestBlob.pSslInfo == null) ? SslStatus.Insecure : ((memoryBlob.RequestBlob.pSslInfo.SslClientCertNegotiated == 0) ? SslStatus.NoClientCert : SslStatus.ClientCert);
            if ((memoryBlob.RequestBlob.pRawUrl != null) && (memoryBlob.RequestBlob.RawUrlLength > 0))
            {
                this.m_RawUrl = Marshal.PtrToStringAnsi((IntPtr) memoryBlob.RequestBlob.pRawUrl, memoryBlob.RequestBlob.RawUrlLength);
            }
            if ((memoryBlob.RequestBlob.CookedUrl.pFullUrl != null) && (memoryBlob.RequestBlob.CookedUrl.FullUrlLength > 0))
            {
                this.m_CookedUrl = Marshal.PtrToStringUni((IntPtr) memoryBlob.RequestBlob.CookedUrl.pFullUrl, memoryBlob.RequestBlob.CookedUrl.FullUrlLength / 2);
            }
            this.m_Version = new Version(memoryBlob.RequestBlob.Version.MajorVersion, memoryBlob.RequestBlob.Version.MinorVersion);
            this.m_ClientCertState = ListenerClientCertState.NotInitialized;
            if (Logging.On)
            {
                Logging.PrintInfo(Logging.HttpListener, this, ".ctor", "httpContext#" + ValidationHelper.HashString(httpContext) + " RequestUri:" + ValidationHelper.ToString(this.RequestUri) + " Content-Length:" + ValidationHelper.ToString(this.ContentLength64) + " HTTP Method:" + ValidationHelper.ToString(this.HttpMethod));
            }
            if (Logging.On)
            {
                StringBuilder builder = new StringBuilder("HttpListenerRequest Headers:\n");
                for (int i = 0; i < this.Headers.Count; i++)
                {
                    builder.Append("\t");
                    builder.Append(this.Headers.GetKey(i));
                    builder.Append(" : ");
                    builder.Append(this.Headers.Get(i));
                    builder.Append("\n");
                }
                Logging.PrintInfo(Logging.HttpListener, this, ".ctor", builder.ToString());
            }
        }

        private unsafe ListenerClientCertAsyncResult AsyncProcessClientCertificate(AsyncCallback requestCallback, object state)
        {
            if (this.m_ClientCertState == ListenerClientCertState.InProgress)
            {
                throw new InvalidOperationException(SR.GetString("net_listener_callinprogress", new object[] { "GetClientCertificate()/BeginGetClientCertificate()" }));
            }
            this.m_ClientCertState = ListenerClientCertState.InProgress;
            this.HttpListenerContext.EnsureBoundHandle();
            ListenerClientCertAsyncResult result = null;
            if (this.m_SslStatus != SslStatus.Insecure)
            {
                uint size = 0x5dc;
                result = new ListenerClientCertAsyncResult(this, state, requestCallback, size);
                try
                {
                    uint num2;
                Label_0058:
                    num2 = 0;
                    uint num3 = UnsafeNclNativeMethods.HttpApi.HttpReceiveClientCertificate(this.HttpListenerContext.RequestQueueHandle, this.m_ConnectionId, 0, result.RequestBlob, size, &num2, result.NativeOverlapped);
                    if (num3 == 0xea)
                    {
                        UnsafeNclNativeMethods.HttpApi.HTTP_SSL_CLIENT_CERT_INFO* requestBlob = result.RequestBlob;
                        size = num2 + requestBlob->CertEncodedSize;
                        result.Reset(size);
                        goto Label_0058;
                    }
                    if ((num3 != 0) && (num3 != 0x3e5))
                    {
                        throw new HttpListenerException((int) num3);
                    }
                    return result;
                }
                catch
                {
                    if (result != null)
                    {
                        result.InternalCleanup();
                    }
                    throw;
                }
            }
            result = new ListenerClientCertAsyncResult(this, state, requestCallback, 0);
            result.InvokeCallback();
            return result;
        }

        public IAsyncResult BeginGetClientCertificate(AsyncCallback requestCallback, object state)
        {
            if (Logging.On)
            {
                Logging.PrintInfo(Logging.HttpListener, this, "BeginGetClientCertificate", "");
            }
            return this.AsyncProcessClientCertificate(requestCallback, state);
        }

        internal void CheckDisposed()
        {
            if (this.m_IsDisposed)
            {
                throw new ObjectDisposedException(base.GetType().FullName);
            }
        }

        internal void Close()
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.HttpListener, this, "Close", "");
            }
            RequestContextBase memoryBlob = this.m_MemoryBlob;
            if (memoryBlob != null)
            {
                memoryBlob.Close();
                this.m_MemoryBlob = null;
            }
            this.m_IsDisposed = true;
            if (Logging.On)
            {
                Logging.Exit(Logging.HttpListener, this, "Close", "");
            }
        }

        internal void DetachBlob(RequestContextBase memoryBlob)
        {
            if ((memoryBlob != null) && (memoryBlob == this.m_MemoryBlob))
            {
                this.m_MemoryBlob = null;
            }
        }

        public X509Certificate2 EndGetClientCertificate(IAsyncResult asyncResult)
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.HttpListener, this, "EndGetClientCertificate", "");
            }
            X509Certificate2 objectValue = null;
            try
            {
                if (asyncResult == null)
                {
                    throw new ArgumentNullException("asyncResult");
                }
                ListenerClientCertAsyncResult result = asyncResult as ListenerClientCertAsyncResult;
                if ((result == null) || (result.AsyncObject != this))
                {
                    throw new ArgumentException(SR.GetString("net_io_invalidasyncresult"), "asyncResult");
                }
                if (result.EndCalled)
                {
                    throw new InvalidOperationException(SR.GetString("net_io_invalidendcall", new object[] { "EndGetClientCertificate" }));
                }
                result.EndCalled = true;
                objectValue = result.InternalWaitForCompletion() as X509Certificate2;
            }
            finally
            {
                if (Logging.On)
                {
                    Logging.Exit(Logging.HttpListener, this, "EndGetClientCertificate", ValidationHelper.HashString(objectValue));
                }
            }
            return objectValue;
        }

        internal ChannelBinding GetChannelBinding() => 
            this.HttpListenerContext.Listener.GetChannelBindingFromTls(this.m_ConnectionId);

        public X509Certificate2 GetClientCertificate()
        {
            if (Logging.On)
            {
                Logging.Enter(Logging.HttpListener, this, "GetClientCertificate", "");
            }
            try
            {
                this.ProcessClientCertificate();
            }
            finally
            {
                if (Logging.On)
                {
                    Logging.Exit(Logging.HttpListener, this, "GetClientCertificate", ValidationHelper.ToString(this.m_ClientCertificate));
                }
            }
            return this.m_ClientCertificate;
        }

        private string GetKnownHeader(HttpRequestHeader header) => 
            UnsafeNclNativeMethods.HttpApi.GetKnownHeader(this.RequestBuffer, this.OriginalBlobAddress, (int) header);

        private CookieCollection ParseCookies(Uri uri, string setCookieHeader)
        {
            Cookie cookie;
            CookieCollection cookies = new CookieCollection();
            CookieParser parser = new CookieParser(setCookieHeader);
        Label_000D:
            cookie = parser.GetServer();
            if (cookie != null)
            {
                if (cookie.Name.Length != 0)
                {
                    cookies.InternalAdd(cookie, true);
                }
                goto Label_000D;
            }
            return cookies;
        }

        private unsafe void ProcessClientCertificate()
        {
            byte[] buffer;
            if (this.m_ClientCertState == ListenerClientCertState.InProgress)
            {
                throw new InvalidOperationException(SR.GetString("net_listener_callinprogress", new object[] { "GetClientCertificate()/BeginGetClientCertificate()" }));
            }
            this.m_ClientCertState = ListenerClientCertState.InProgress;
            if (this.m_SslStatus == SslStatus.Insecure)
            {
                goto Label_0100;
            }
            uint sslClientCertInfoSize = 0x5dc;
        Label_0044:
            buffer = new byte[sslClientCertInfoSize];
            try
            {
                fixed (byte* numRef = buffer)
                {
                    UnsafeNclNativeMethods.HttpApi.HTTP_SSL_CLIENT_CERT_INFO* pSslClientCertInfo = (UnsafeNclNativeMethods.HttpApi.HTTP_SSL_CLIENT_CERT_INFO*) numRef;
                    uint pBytesReceived = 0;
                    uint num3 = UnsafeNclNativeMethods.HttpApi.HttpReceiveClientCertificate(this.HttpListenerContext.RequestQueueHandle, this.m_ConnectionId, 0, pSslClientCertInfo, sslClientCertInfoSize, &pBytesReceived, null);
                    if (num3 == 0xea)
                    {
                        sslClientCertInfoSize = pBytesReceived + pSslClientCertInfo->CertEncodedSize;
                        goto Label_0044;
                    }
                    if ((num3 == 0) && (pSslClientCertInfo != null))
                    {
                        if (pSslClientCertInfo->pCertEncoded != null)
                        {
                            try
                            {
                                byte[] destination = new byte[pSslClientCertInfo->CertEncodedSize];
                                Marshal.Copy((IntPtr) pSslClientCertInfo->pCertEncoded, destination, 0, destination.Length);
                                this.m_ClientCertificate = new X509Certificate2(destination);
                            }
                            catch (CryptographicException)
                            {
                            }
                            catch (SecurityException)
                            {
                            }
                        }
                        this.m_ClientCertificateError = (int) pSslClientCertInfo->CertFlags;
                    }
                }
            }
            finally
            {
                numRef = null;
            }
        Label_0100:
            this.m_ClientCertState = ListenerClientCertState.Completed;
        }

        internal void ReleasePins()
        {
            this.m_MemoryBlob.ReleasePins();
        }

        internal void SetClientCertificateError(int clientCertificateError)
        {
            this.m_ClientCertificateError = clientCertificateError;
        }

        public string[] AcceptTypes =>
            Helpers.ParseMultivalueHeader(this.GetKnownHeader(HttpRequestHeader.Accept));

        internal X509Certificate2 ClientCertificate
        {
            set
            {
                this.m_ClientCertificate = value;
            }
        }

        public int ClientCertificateError
        {
            get
            {
                if (this.m_ClientCertState == ListenerClientCertState.NotInitialized)
                {
                    throw new InvalidOperationException(SR.GetString("net_listener_mustcall", new object[] { "GetClientCertificate()/BeginGetClientCertificate()" }));
                }
                if (this.m_ClientCertState == ListenerClientCertState.InProgress)
                {
                    throw new InvalidOperationException(SR.GetString("net_listener_mustcompletecall", new object[] { "GetClientCertificate()/BeginGetClientCertificate()" }));
                }
                return this.m_ClientCertificateError;
            }
        }

        internal ListenerClientCertState ClientCertState
        {
            set
            {
                this.m_ClientCertState = value;
            }
        }

        public Encoding ContentEncoding
        {
            get
            {
                if ((this.UserAgent != null) && CultureInfo.InvariantCulture.CompareInfo.IsPrefix(this.UserAgent, "UP"))
                {
                    string name = this.Headers["x-up-devcap-post-charset"];
                    if ((name != null) && (name.Length > 0))
                    {
                        try
                        {
                            return Encoding.GetEncoding(name);
                        }
                        catch (ArgumentException)
                        {
                        }
                    }
                }
                if (this.HasEntityBody && (this.ContentType != null))
                {
                    string attributeFromHeader = Helpers.GetAttributeFromHeader(this.ContentType, "charset");
                    if (attributeFromHeader != null)
                    {
                        try
                        {
                            return Encoding.GetEncoding(attributeFromHeader);
                        }
                        catch (ArgumentException)
                        {
                        }
                    }
                }
                return Encoding.Default;
            }
        }

        public long ContentLength64
        {
            get
            {
                if (this.m_BoundaryType == BoundaryType.None)
                {
                    if (this.GetKnownHeader(HttpRequestHeader.TransferEncoding) == "chunked")
                    {
                        this.m_BoundaryType = BoundaryType.Chunked;
                        this.m_ContentLength = -1L;
                    }
                    else
                    {
                        this.m_ContentLength = 0L;
                        this.m_BoundaryType = BoundaryType.ContentLength;
                        string knownHeader = this.GetKnownHeader(HttpRequestHeader.ContentLength);
                        if ((knownHeader != null) && !long.TryParse(knownHeader, NumberStyles.None, CultureInfo.InvariantCulture.NumberFormat, out this.m_ContentLength))
                        {
                            this.m_ContentLength = 0L;
                            this.m_BoundaryType = BoundaryType.Invalid;
                        }
                    }
                }
                return this.m_ContentLength;
            }
        }

        public string ContentType =>
            this.GetKnownHeader(HttpRequestHeader.ContentType);

        public CookieCollection Cookies
        {
            get
            {
                if (this.m_Cookies == null)
                {
                    string knownHeader = this.GetKnownHeader(HttpRequestHeader.Cookie);
                    if ((knownHeader != null) && (knownHeader.Length > 0))
                    {
                        this.m_Cookies = this.ParseCookies(this.RequestUri, knownHeader);
                    }
                    if (this.m_Cookies == null)
                    {
                        this.m_Cookies = new CookieCollection();
                    }
                    if (this.HttpListenerContext.PromoteCookiesToRfc2965)
                    {
                        for (int i = 0; i < this.m_Cookies.Count; i++)
                        {
                            if (this.m_Cookies[i].Variant == CookieVariant.Rfc2109)
                            {
                                this.m_Cookies[i].Variant = CookieVariant.Rfc2965;
                            }
                        }
                    }
                }
                return this.m_Cookies;
            }
        }

        public bool HasEntityBody
        {
            get
            {
                if (((this.ContentLength64 <= 0L) || (this.m_BoundaryType != BoundaryType.ContentLength)) && (this.m_BoundaryType != BoundaryType.Chunked))
                {
                    return (this.m_BoundaryType == BoundaryType.Multipart);
                }
                return true;
            }
        }

        public NameValueCollection Headers
        {
            get
            {
                if (this.m_WebHeaders == null)
                {
                    this.m_WebHeaders = UnsafeNclNativeMethods.HttpApi.GetHeaders(this.RequestBuffer, this.OriginalBlobAddress);
                }
                return this.m_WebHeaders;
            }
        }

        internal System.Net.HttpListenerContext HttpListenerContext =>
            this.m_HttpContext;

        public string HttpMethod
        {
            get
            {
                if (this.m_HttpMethod == null)
                {
                    this.m_HttpMethod = UnsafeNclNativeMethods.HttpApi.GetVerb(this.RequestBuffer, this.OriginalBlobAddress);
                }
                return this.m_HttpMethod;
            }
        }

        public Stream InputStream
        {
            get
            {
                if (Logging.On)
                {
                    Logging.Enter(Logging.HttpListener, this, "InputStream_get", "");
                }
                if (this.m_RequestStream == null)
                {
                    this.m_RequestStream = this.HasEntityBody ? new HttpRequestStream(this.HttpListenerContext) : Stream.Null;
                }
                if (Logging.On)
                {
                    Logging.Exit(Logging.HttpListener, this, "InputStream_get", "");
                }
                return this.m_RequestStream;
            }
        }

        internal bool InternalIsLocal =>
            this.LocalEndPoint.Address.Equals(this.RemoteEndPoint.Address);

        public bool IsAuthenticated
        {
            get
            {
                IPrincipal user = this.HttpListenerContext.User;
                return (((user != null) && (user.Identity != null)) && user.Identity.IsAuthenticated);
            }
        }

        public bool IsLocal =>
            (this.LocalEndPoint.Address == this.RemoteEndPoint.Address);

        public bool IsSecureConnection =>
            (this.m_SslStatus != SslStatus.Insecure);

        public bool KeepAlive
        {
            get
            {
                if (this.m_KeepAlive == TriState.Unspecified)
                {
                    string knownHeader = this.Headers["Proxy-Connection"];
                    if (string.IsNullOrEmpty(knownHeader))
                    {
                        knownHeader = this.GetKnownHeader(HttpRequestHeader.Connection);
                    }
                    if (string.IsNullOrEmpty(knownHeader))
                    {
                        if (this.ProtocolVersion >= HttpVersion.Version11)
                        {
                            this.m_KeepAlive = TriState.True;
                        }
                        else
                        {
                            knownHeader = this.GetKnownHeader(HttpRequestHeader.KeepAlive);
                            this.m_KeepAlive = string.IsNullOrEmpty(knownHeader) ? TriState.False : TriState.True;
                        }
                    }
                    else
                    {
                        this.m_KeepAlive = ((knownHeader.ToLower(CultureInfo.InvariantCulture).IndexOf("close") < 0) || (knownHeader.ToLower(CultureInfo.InvariantCulture).IndexOf("keep-alive") >= 0)) ? TriState.True : TriState.False;
                    }
                }
                return (this.m_KeepAlive == TriState.True);
            }
        }

        public IPEndPoint LocalEndPoint
        {
            get
            {
                if (this.m_LocalEndPoint == null)
                {
                    this.m_LocalEndPoint = UnsafeNclNativeMethods.HttpApi.GetLocalEndPoint(this.RequestBuffer, this.OriginalBlobAddress);
                }
                return this.m_LocalEndPoint;
            }
        }

        internal IntPtr OriginalBlobAddress
        {
            get
            {
                this.CheckDisposed();
                return this.m_MemoryBlob.OriginalBlobAddress;
            }
        }

        public Version ProtocolVersion =>
            this.m_Version;

        public NameValueCollection QueryString
        {
            get
            {
                NameValueCollection nvc = new NameValueCollection();
                Helpers.FillFromString(nvc, this.Url.Query, true, this.ContentEncoding);
                return nvc;
            }
        }

        public string RawUrl =>
            this.m_RawUrl;

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                if (this.m_RemoteEndPoint == null)
                {
                    this.m_RemoteEndPoint = UnsafeNclNativeMethods.HttpApi.GetRemoteEndPoint(this.RequestBuffer, this.OriginalBlobAddress);
                }
                return this.m_RemoteEndPoint;
            }
        }

        internal byte[] RequestBuffer
        {
            get
            {
                this.CheckDisposed();
                return this.m_MemoryBlob.RequestBuffer;
            }
        }

        internal ulong RequestId =>
            this.m_RequestId;

        private string RequestScheme
        {
            get
            {
                if (!this.IsSecureConnection)
                {
                    return "http://";
                }
                return "https://";
            }
        }

        public Guid RequestTraceIdentifier
        {
            get
            {
                Guid guid = new Guid();
                ((IntPtr) 8)[(int) ((IntPtr) &guid)] = (IntPtr) this.RequestId;
                return guid;
            }
        }

        private Uri RequestUri
        {
            get
            {
                if (this.m_RequestUri == null)
                {
                    bool flag = false;
                    if (!string.IsNullOrEmpty(this.m_CookedUrl))
                    {
                        flag = Uri.TryCreate(this.m_CookedUrl, UriKind.Absolute, out this.m_RequestUri);
                    }
                    if ((!flag && (this.RawUrl != null)) && ((this.RawUrl.Length > 0) && (this.RawUrl[0] != '/')))
                    {
                        flag = Uri.TryCreate(this.RawUrl, UriKind.Absolute, out this.m_RequestUri);
                    }
                    if (!flag)
                    {
                        string uriString = this.RequestScheme + this.UserHostName;
                        if ((this.RawUrl != null) && (this.RawUrl.Length > 0))
                        {
                            if (this.RawUrl[0] != '/')
                            {
                                uriString = uriString + "/" + this.RawUrl;
                            }
                            else
                            {
                                uriString = uriString + this.RawUrl;
                            }
                        }
                        else
                        {
                            uriString = uriString + "/";
                        }
                        flag = Uri.TryCreate(uriString, UriKind.Absolute, out this.m_RequestUri);
                    }
                }
                return this.m_RequestUri;
            }
        }

        public string ServiceName
        {
            get => 
                this.m_ServiceName;
            internal set
            {
                this.m_ServiceName = value;
            }
        }

        public System.Net.TransportContext TransportContext =>
            new HttpListenerRequestContext(this);

        public Uri Url =>
            this.RequestUri;

        public Uri UrlReferrer
        {
            get
            {
                Uri uri;
                string knownHeader = this.GetKnownHeader(HttpRequestHeader.Referer);
                if (knownHeader == null)
                {
                    return null;
                }
                if (!Uri.TryCreate(knownHeader, UriKind.RelativeOrAbsolute, out uri))
                {
                    return null;
                }
                return uri;
            }
        }

        public string UserAgent =>
            this.GetKnownHeader(HttpRequestHeader.UserAgent);

        public string UserHostAddress =>
            this.LocalEndPoint.ToString();

        public string UserHostName =>
            this.GetKnownHeader(HttpRequestHeader.Host);

        public string[] UserLanguages =>
            Helpers.ParseMultivalueHeader(this.GetKnownHeader(HttpRequestHeader.AcceptLanguage));

        private static class Helpers
        {
            internal static void FillFromString(NameValueCollection nvc, string s, bool urlencoded, Encoding encoding)
            {
                int num = (s != null) ? s.Length : 0;
                for (int i = ((s.Length > 0) && (s[0] == '?')) ? 1 : 0; i < num; i++)
                {
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
                        nvc.Add((str == null) ? null : UrlDecodeStringFromStringInternal(str, encoding), UrlDecodeStringFromStringInternal(str2, encoding));
                    }
                    else
                    {
                        nvc.Add(str, str2);
                    }
                    if ((i == (num - 1)) && (s[i] == '&'))
                    {
                        nvc.Add(null, "");
                    }
                }
            }

            internal static string GetAttributeFromHeader(string headerValue, string attrName)
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

            private static int HexToInt(char h)
            {
                if ((h >= '0') && (h <= '9'))
                {
                    return (h - '0');
                }
                if ((h >= 'a') && (h <= 'f'))
                {
                    return ((h - 'a') + 10);
                }
                if ((h >= 'A') && (h <= 'F'))
                {
                    return ((h - 'A') + 10);
                }
                return -1;
            }

            internal static string[] ParseMultivalueHeader(string s)
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

            private static string UrlDecodeStringFromStringInternal(string s, Encoding e)
            {
                int length = s.Length;
                UrlDecoder decoder = new UrlDecoder(length, e);
                for (int i = 0; i < length; i++)
                {
                    char ch = s[i];
                    if (ch == '+')
                    {
                        ch = ' ';
                    }
                    else if ((ch == '%') && (i < (length - 2)))
                    {
                        if ((s[i + 1] == 'u') && (i < (length - 5)))
                        {
                            int num3 = HexToInt(s[i + 2]);
                            int num4 = HexToInt(s[i + 3]);
                            int num5 = HexToInt(s[i + 4]);
                            int num6 = HexToInt(s[i + 5]);
                            if (((num3 < 0) || (num4 < 0)) || ((num5 < 0) || (num6 < 0)))
                            {
                                goto Label_0106;
                            }
                            ch = (char) ((((num3 << 12) | (num4 << 8)) | (num5 << 4)) | num6);
                            i += 5;
                            decoder.AddChar(ch);
                            continue;
                        }
                        int num7 = HexToInt(s[i + 1]);
                        int num8 = HexToInt(s[i + 2]);
                        if ((num7 >= 0) && (num8 >= 0))
                        {
                            byte b = (byte) ((num7 << 4) | num8);
                            i += 2;
                            decoder.AddByte(b);
                            continue;
                        }
                    }
                Label_0106:
                    if ((ch & 0xff80) == 0)
                    {
                        decoder.AddByte((byte) ch);
                    }
                    else
                    {
                        decoder.AddChar(ch);
                    }
                }
                return decoder.GetString();
            }

            private class UrlDecoder
            {
                private int _bufferSize;
                private byte[] _byteBuffer;
                private char[] _charBuffer;
                private Encoding _encoding;
                private int _numBytes;
                private int _numChars;

                internal UrlDecoder(int bufferSize, Encoding encoding)
                {
                    this._bufferSize = bufferSize;
                    this._encoding = encoding;
                    this._charBuffer = new char[bufferSize];
                }

                internal void AddByte(byte b)
                {
                    if (this._byteBuffer == null)
                    {
                        this._byteBuffer = new byte[this._bufferSize];
                    }
                    this._byteBuffer[this._numBytes++] = b;
                }

                internal void AddChar(char ch)
                {
                    if (this._numBytes > 0)
                    {
                        this.FlushBytes();
                    }
                    this._charBuffer[this._numChars++] = ch;
                }

                private void FlushBytes()
                {
                    if (this._numBytes > 0)
                    {
                        this._numChars += this._encoding.GetChars(this._byteBuffer, 0, this._numBytes, this._charBuffer, this._numChars);
                        this._numBytes = 0;
                    }
                }

                internal string GetString()
                {
                    if (this._numBytes > 0)
                    {
                        this.FlushBytes();
                    }
                    if (this._numChars > 0)
                    {
                        return new string(this._charBuffer, 0, this._numChars);
                    }
                    return string.Empty;
                }
            }
        }

        private enum SslStatus : byte
        {
            ClientCert = 2,
            Insecure = 0,
            NoClientCert = 1
        }
    }
}

