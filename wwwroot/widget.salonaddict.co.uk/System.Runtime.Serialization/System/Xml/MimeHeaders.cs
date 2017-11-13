﻿namespace System.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    internal class MimeHeaders
    {
        private Dictionary<string, MimeHeader> headers = new Dictionary<string, MimeHeader>();

        public void Add(MimeHeader header)
        {
            MimeHeader header2;
            if (header == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("header");
            }
            if (this.headers.TryGetValue(header.Name, out header2))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(System.Runtime.Serialization.SR.GetString("MimeReaderHeaderAlreadyExists", new object[] { header.Name })));
            }
            this.headers.Add(header.Name, header);
        }

        public void Add(string name, string value, ref int remaining)
        {
            if (name == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("name");
            }
            if (value == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
            }
            switch (name)
            {
                case "content-type":
                    this.Add(new ContentTypeHeader(value));
                    break;

                case "content-id":
                    this.Add(new ContentIDHeader(name, value));
                    break;

                case "content-transfer-encoding":
                    this.Add(new ContentTransferEncodingHeader(value));
                    break;

                case "mime-version":
                    this.Add(new MimeVersionHeader(value));
                    break;

                default:
                    remaining += value.Length * 2;
                    break;
            }
            remaining += name.Length * 2;
        }

        public void Release(ref int remaining)
        {
            foreach (MimeHeader header in this.headers.Values)
            {
                remaining += header.Value.Length * 2;
            }
        }

        public ContentIDHeader ContentID
        {
            get
            {
                MimeHeader header;
                if (this.headers.TryGetValue("content-id", out header))
                {
                    return (header as ContentIDHeader);
                }
                return null;
            }
        }

        public ContentTransferEncodingHeader ContentTransferEncoding
        {
            get
            {
                MimeHeader header;
                if (this.headers.TryGetValue("content-transfer-encoding", out header))
                {
                    return (header as ContentTransferEncodingHeader);
                }
                return null;
            }
        }

        public ContentTypeHeader ContentType
        {
            get
            {
                MimeHeader header;
                if (this.headers.TryGetValue("content-type", out header))
                {
                    return (header as ContentTypeHeader);
                }
                return null;
            }
        }

        public MimeVersionHeader MimeVersion
        {
            get
            {
                MimeHeader header;
                if (this.headers.TryGetValue("mime-version", out header))
                {
                    return (header as MimeVersionHeader);
                }
                return null;
            }
        }

        private static class Constants
        {
            public const string ContentID = "content-id";
            public const string ContentTransferEncoding = "content-transfer-encoding";
            public const string ContentType = "content-type";
            public const string MimeVersion = "mime-version";
        }
    }
}

