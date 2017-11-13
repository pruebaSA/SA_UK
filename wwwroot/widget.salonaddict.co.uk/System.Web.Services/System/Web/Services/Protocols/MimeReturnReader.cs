namespace System.Web.Services.Protocols
{
    using System;
    using System.IO;
    using System.Net;

    public abstract class MimeReturnReader : MimeFormatter
    {
        protected MimeReturnReader()
        {
        }

        public abstract object Read(WebResponse response, Stream responseStream);
    }
}

