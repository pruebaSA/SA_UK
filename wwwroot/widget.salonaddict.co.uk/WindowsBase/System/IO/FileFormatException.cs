namespace System.IO
{
    using MS.Internal.WindowsBase;
    using System;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;

    [Serializable]
    public class FileFormatException : FormatException, ISerializable
    {
        private Uri _sourceUri;

        public FileFormatException() : base(SR.Get("FileFormatException"))
        {
        }

        public FileFormatException(string message) : base(message)
        {
        }

        public FileFormatException(Uri sourceUri) : base((sourceUri == null) ? SR.Get("FileFormatException") : SR.Get("FileFormatExceptionWithFileName", new object[] { sourceUri }))
        {
            this._sourceUri = sourceUri;
        }

        protected FileFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            string uriString = info.GetString("SourceUri");
            if (uriString != null)
            {
                this._sourceUri = new Uri(uriString, UriKind.RelativeOrAbsolute);
            }
        }

        public FileFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public FileFormatException(Uri sourceUri, Exception innerException) : base((sourceUri == null) ? SR.Get("FileFormatException") : SR.Get("FileFormatExceptionWithFileName", new object[] { sourceUri }), innerException)
        {
            this._sourceUri = sourceUri;
        }

        public FileFormatException(Uri sourceUri, string message) : base(message)
        {
            this._sourceUri = sourceUri;
        }

        public FileFormatException(Uri sourceUri, string message, Exception innerException) : base(message, innerException)
        {
            this._sourceUri = sourceUri;
        }

        [SecurityCritical, SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            base.GetObjectData(info, context);
            Uri sourceUri = this.SourceUri;
            info.AddValue("SourceUri", sourceUri?.GetComponents(UriComponents.SerializationInfoString, UriFormat.SafeUnescaped), typeof(string));
        }

        public Uri SourceUri
        {
            get
            {
                if (((this._sourceUri != null) && this._sourceUri.IsAbsoluteUri) && this._sourceUri.IsFile)
                {
                    SecurityHelper.DemandPathDiscovery(this._sourceUri.LocalPath);
                }
                return this._sourceUri;
            }
        }
    }
}

