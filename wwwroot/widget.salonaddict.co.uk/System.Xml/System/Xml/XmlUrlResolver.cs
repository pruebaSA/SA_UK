namespace System.Xml
{
    using System;
    using System.IO;
    using System.Net;
    using System.Security.Permissions;
    using System.Threading;

    public class XmlUrlResolver : XmlResolver
    {
        private ICredentials _credentials;
        private static object s_DownloadManager;

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            if ((ofObjectToReturn != null) && (ofObjectToReturn != typeof(Stream)))
            {
                throw new XmlException("Xml_UnsupportedClass", string.Empty);
            }
            return DownloadManager.GetStream(absoluteUri, this._credentials);
        }

        [PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
        public override Uri ResolveUri(Uri baseUri, string relativeUri) => 
            base.ResolveUri(baseUri, relativeUri);

        public override ICredentials Credentials
        {
            set
            {
                this._credentials = value;
            }
        }

        private static XmlDownloadManager DownloadManager
        {
            get
            {
                if (s_DownloadManager == null)
                {
                    object obj2 = new XmlDownloadManager();
                    Interlocked.CompareExchange(ref s_DownloadManager, obj2, null);
                }
                return (XmlDownloadManager) s_DownloadManager;
            }
        }
    }
}

