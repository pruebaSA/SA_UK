namespace System.Web.Services.Protocols
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    public abstract class MimeParameterWriter : MimeFormatter
    {
        protected MimeParameterWriter()
        {
        }

        public virtual string GetRequestUrl(string url, object[] parameters) => 
            url;

        public virtual void InitializeRequest(WebRequest request, object[] values)
        {
        }

        public virtual void WriteRequest(Stream requestStream, object[] values)
        {
        }

        public virtual Encoding RequestEncoding
        {
            get => 
                null;
            set
            {
            }
        }

        public virtual bool UsesWriteRequest =>
            false;
    }
}

