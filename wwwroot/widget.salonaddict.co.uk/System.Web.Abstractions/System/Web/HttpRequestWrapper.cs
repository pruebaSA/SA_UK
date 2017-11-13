namespace System.Web
{
    using System;
    using System.Collections.Specialized;
    using System.IO;
    using System.Reflection;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Text;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HttpRequestWrapper : HttpRequestBase
    {
        private HttpRequest _httpRequest;

        public HttpRequestWrapper(HttpRequest httpRequest)
        {
            if (httpRequest == null)
            {
                throw new ArgumentNullException("httpRequest");
            }
            this._httpRequest = httpRequest;
        }

        public override byte[] BinaryRead(int count) => 
            this._httpRequest.BinaryRead(count);

        public override int[] MapImageCoordinates(string imageFieldName) => 
            this._httpRequest.MapImageCoordinates(imageFieldName);

        public override string MapPath(string virtualPath) => 
            this._httpRequest.MapPath(virtualPath);

        public override string MapPath(string virtualPath, string baseVirtualDir, bool allowCrossAppMapping) => 
            this._httpRequest.MapPath(virtualPath, baseVirtualDir, allowCrossAppMapping);

        public override void SaveAs(string filename, bool includeHeaders)
        {
            this._httpRequest.SaveAs(filename, includeHeaders);
        }

        public override void ValidateInput()
        {
            this._httpRequest.ValidateInput();
        }

        public override string[] AcceptTypes =>
            this._httpRequest.AcceptTypes;

        public override string AnonymousID =>
            this._httpRequest.AnonymousID;

        public override string ApplicationPath =>
            this._httpRequest.ApplicationPath;

        public override string AppRelativeCurrentExecutionFilePath =>
            this._httpRequest.AppRelativeCurrentExecutionFilePath;

        public override HttpBrowserCapabilitiesBase Browser =>
            new HttpBrowserCapabilitiesWrapper(this._httpRequest.Browser);

        public override HttpClientCertificate ClientCertificate =>
            this._httpRequest.ClientCertificate;

        public override Encoding ContentEncoding
        {
            get => 
                this._httpRequest.ContentEncoding;
            set
            {
                this._httpRequest.ContentEncoding = value;
            }
        }

        public override int ContentLength =>
            this._httpRequest.ContentLength;

        public override string ContentType
        {
            get => 
                this._httpRequest.ContentType;
            set
            {
                this._httpRequest.ContentType = value;
            }
        }

        public override HttpCookieCollection Cookies =>
            this._httpRequest.Cookies;

        public override string CurrentExecutionFilePath =>
            this._httpRequest.CurrentExecutionFilePath;

        public override string FilePath =>
            this._httpRequest.FilePath;

        public override HttpFileCollectionBase Files =>
            new HttpFileCollectionWrapper(this._httpRequest.Files);

        public override Stream Filter
        {
            get => 
                this._httpRequest.Filter;
            set
            {
                this._httpRequest.Filter = value;
            }
        }

        public override NameValueCollection Form =>
            this._httpRequest.Form;

        public override NameValueCollection Headers =>
            this._httpRequest.Headers;

        public override string HttpMethod =>
            this._httpRequest.HttpMethod;

        public override Stream InputStream =>
            this._httpRequest.InputStream;

        public override bool IsAuthenticated =>
            this._httpRequest.IsAuthenticated;

        public override bool IsLocal =>
            this._httpRequest.IsLocal;

        public override bool IsSecureConnection =>
            this._httpRequest.IsSecureConnection;

        public override string this[string key] =>
            this._httpRequest[key];

        public override WindowsIdentity LogonUserIdentity =>
            this._httpRequest.LogonUserIdentity;

        public override NameValueCollection Params =>
            this._httpRequest.Params;

        public override string Path =>
            this._httpRequest.Path;

        public override string PathInfo =>
            this._httpRequest.PathInfo;

        public override string PhysicalApplicationPath =>
            this._httpRequest.PhysicalApplicationPath;

        public override string PhysicalPath =>
            this._httpRequest.PhysicalPath;

        public override NameValueCollection QueryString =>
            this._httpRequest.QueryString;

        public override string RawUrl =>
            this._httpRequest.RawUrl;

        public override string RequestType
        {
            get => 
                this._httpRequest.RequestType;
            set
            {
                this._httpRequest.RequestType = value;
            }
        }

        public override NameValueCollection ServerVariables =>
            this._httpRequest.ServerVariables;

        public override int TotalBytes =>
            this._httpRequest.TotalBytes;

        public override Uri Url =>
            this._httpRequest.Url;

        public override Uri UrlReferrer =>
            this._httpRequest.UrlReferrer;

        public override string UserAgent =>
            this._httpRequest.UserAgent;

        public override string UserHostAddress =>
            this._httpRequest.UserHostAddress;

        public override string UserHostName =>
            this._httpRequest.UserHostName;

        public override string[] UserLanguages =>
            this._httpRequest.UserLanguages;
    }
}

