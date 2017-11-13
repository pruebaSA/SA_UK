namespace System.Web
{
    using System;
    using System.IO;

    internal abstract class HttpResponseInternalBase : HttpResponseBase
    {
        protected HttpResponseInternalBase()
        {
        }

        public virtual TextWriter SwitchWriter(TextWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}

