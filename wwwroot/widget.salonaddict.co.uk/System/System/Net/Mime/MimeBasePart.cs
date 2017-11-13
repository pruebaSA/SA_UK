namespace System.Net.Mime
{
    using System;
    using System.Collections.Specialized;
    using System.Net;
    using System.Net.Mail;
    using System.Text;

    internal class MimeBasePart
    {
        protected ContentDisposition contentDisposition;
        protected System.Net.Mime.ContentType contentType;
        internal const string defaultCharSet = "utf-8";
        private HeaderCollection headers;

        internal MimeBasePart()
        {
        }

        internal virtual IAsyncResult BeginSend(BaseWriter writer, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        internal static Encoding DecodeEncoding(string value)
        {
            if ((value == null) || (value.Length == 0))
            {
                return null;
            }
            string[] strArray = value.Split(new char[] { '?' });
            if (((strArray.Length != 5) || (strArray[0] != "=")) || (strArray[4] != "="))
            {
                return null;
            }
            string name = strArray[1];
            return Encoding.GetEncoding(name);
        }

        internal static string DecodeHeaderValue(string value)
        {
            int num;
            if ((value == null) || (value.Length == 0))
            {
                return string.Empty;
            }
            string[] strArray = value.Split(new char[] { '?' });
            if (((strArray.Length != 5) || (strArray[0] != "=")) || (strArray[4] != "="))
            {
                return value;
            }
            string name = strArray[1];
            bool flag = strArray[2] == "B";
            byte[] bytes = Encoding.ASCII.GetBytes(strArray[3]);
            if (flag)
            {
                num = new Base64Stream().DecodeBytes(bytes, 0, bytes.Length);
            }
            else
            {
                num = new QuotedPrintableStream().DecodeBytes(bytes, 0, bytes.Length);
            }
            return Encoding.GetEncoding(name).GetString(bytes, 0, num);
        }

        internal static string EncodeHeaderValue(string value, Encoding encoding, bool base64Encoding)
        {
            StringBuilder builder = new StringBuilder();
            if ((encoding == null) && IsAscii(value, false))
            {
                return value;
            }
            if (encoding == null)
            {
                encoding = Encoding.GetEncoding("utf-8");
            }
            string bodyName = encoding.BodyName;
            if (encoding == Encoding.BigEndianUnicode)
            {
                bodyName = "utf-16be";
            }
            builder.Append("=?");
            builder.Append(bodyName);
            builder.Append("?");
            builder.Append(base64Encoding ? "B" : "Q");
            builder.Append("?");
            byte[] bytes = encoding.GetBytes(value);
            if (base64Encoding)
            {
                Base64Stream stream = new Base64Stream(-1);
                stream.EncodeBytes(bytes, 0, bytes.Length, true);
                builder.Append(Encoding.ASCII.GetString(stream.WriteState.Buffer, 0, stream.WriteState.Length));
            }
            else
            {
                QuotedPrintableStream stream2 = new QuotedPrintableStream(-1);
                stream2.EncodeBytes(bytes, 0, bytes.Length);
                builder.Append(Encoding.ASCII.GetString(stream2.WriteState.Buffer, 0, stream2.WriteState.Length));
            }
            builder.Append("?=");
            return builder.ToString();
        }

        internal void EndSend(IAsyncResult asyncResult)
        {
            if (asyncResult == null)
            {
                throw new ArgumentNullException("asyncResult");
            }
            LazyAsyncResult result = asyncResult as MimePartAsyncResult;
            if ((result == null) || (result.AsyncObject != this))
            {
                throw new ArgumentException(SR.GetString("net_io_invalidasyncresult"), "asyncResult");
            }
            if (result.EndCalled)
            {
                throw new InvalidOperationException(SR.GetString("net_io_invalidendcall", new object[] { "EndSend" }));
            }
            result.InternalWaitForCompletion();
            result.EndCalled = true;
            if (result.Result is Exception)
            {
                throw ((Exception) result.Result);
            }
        }

        internal static bool IsAnsi(string value, bool permitCROrLF)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            foreach (char ch in value)
            {
                if (ch > '\x00ff')
                {
                    return false;
                }
                if (!permitCROrLF && ((ch == '\r') || (ch == '\n')))
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool IsAscii(string value, bool permitCROrLF)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            foreach (char ch in value)
            {
                if (ch > '\x007f')
                {
                    return false;
                }
                if (!permitCROrLF && ((ch == '\r') || (ch == '\n')))
                {
                    return false;
                }
            }
            return true;
        }

        internal virtual void Send(BaseWriter writer)
        {
            throw new NotImplementedException();
        }

        internal static bool ShouldUseBase64Encoding(Encoding encoding)
        {
            if (((encoding != Encoding.Unicode) && (encoding != Encoding.UTF8)) && ((encoding != Encoding.UTF32) && (encoding != Encoding.BigEndianUnicode)))
            {
                return false;
            }
            return true;
        }

        internal string ContentID
        {
            get => 
                this.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentID)];
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.ContentID));
                }
                else
                {
                    this.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentID)] = value;
                }
            }
        }

        internal string ContentLocation
        {
            get => 
                this.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentLocation)];
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.Headers.Remove(MailHeaderInfo.GetString(MailHeaderID.ContentLocation));
                }
                else
                {
                    this.Headers[MailHeaderInfo.GetString(MailHeaderID.ContentLocation)] = value;
                }
            }
        }

        internal System.Net.Mime.ContentType ContentType
        {
            get
            {
                if (this.contentType == null)
                {
                    this.contentType = new System.Net.Mime.ContentType();
                }
                return this.contentType;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.contentType = value;
                this.contentType.PersistIfNeeded((HeaderCollection) this.Headers, true);
            }
        }

        internal NameValueCollection Headers
        {
            get
            {
                if (this.headers == null)
                {
                    this.headers = new HeaderCollection();
                }
                if (this.contentType == null)
                {
                    this.contentType = new System.Net.Mime.ContentType();
                }
                this.contentType.PersistIfNeeded(this.headers, false);
                if (this.contentDisposition != null)
                {
                    this.contentDisposition.PersistIfNeeded(this.headers, false);
                }
                return this.headers;
            }
        }

        internal class MimePartAsyncResult : LazyAsyncResult
        {
            internal MimePartAsyncResult(MimeBasePart part, object state, AsyncCallback callback) : base(part, state, callback)
            {
            }
        }
    }
}

