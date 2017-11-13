namespace System.ServiceModel.Channels
{
    using System;

    internal class RawContentTypeMapper : WebContentTypeMapper
    {
        private static readonly RawContentTypeMapper instance = new RawContentTypeMapper();

        public override WebContentFormat GetMessageFormatForContentType(string contentType) => 
            WebContentFormat.Raw;

        public static RawContentTypeMapper Instance =>
            instance;
    }
}

