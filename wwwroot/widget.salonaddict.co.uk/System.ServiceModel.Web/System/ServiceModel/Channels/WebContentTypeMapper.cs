namespace System.ServiceModel.Channels
{
    using System;

    public abstract class WebContentTypeMapper
    {
        protected WebContentTypeMapper()
        {
        }

        public abstract WebContentFormat GetMessageFormatForContentType(string contentType);
    }
}

