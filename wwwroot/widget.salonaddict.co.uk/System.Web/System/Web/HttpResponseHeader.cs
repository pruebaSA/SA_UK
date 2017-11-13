namespace System.Web
{
    using System;
    using System.Text;

    internal class HttpResponseHeader
    {
        private int _knownHeaderIndex;
        private string _unknownHeader;
        private string _value;
        private static readonly string[] EncodingTable = new string[] { 
            "%00", "%01", "%02", "%03", "%04", "%05", "%06", "%07", "%08", "%09", "%0a", "%0b", "%0c", "%0d", "%0e", "%0f",
            "%10", "%11", "%12", "%13", "%14", "%15", "%16", "%17", "%18", "%19", "%1a", "%1b", "%1c", "%1d", "%1e", "%1f"
        };

        internal HttpResponseHeader(int knownHeaderIndex, string value)
        {
            this._unknownHeader = null;
            this._knownHeaderIndex = knownHeaderIndex;
            if (HttpRuntime.EnableHeaderChecking)
            {
                this._value = MaybeEncodeHeader(value);
            }
            else
            {
                this._value = value;
            }
        }

        internal HttpResponseHeader(string unknownHeader, string value)
        {
            if (HttpRuntime.EnableHeaderChecking)
            {
                this._unknownHeader = MaybeEncodeHeader(unknownHeader);
                this._knownHeaderIndex = HttpWorkerRequest.GetKnownResponseHeaderIndex(this._unknownHeader);
                this._value = MaybeEncodeHeader(value);
            }
            else
            {
                this._unknownHeader = unknownHeader;
                this._knownHeaderIndex = HttpWorkerRequest.GetKnownResponseHeaderIndex(this._unknownHeader);
                this._value = value;
            }
        }

        internal static string MaybeEncodeHeader(string value)
        {
            string str = value;
            if (!NeedsEncoding(value))
            {
                return str;
            }
            StringBuilder builder = new StringBuilder();
            foreach (char ch in value)
            {
                if ((ch < ' ') && (ch != '\t'))
                {
                    builder.Append(EncodingTable[ch]);
                }
                else if (ch == '\x007f')
                {
                    builder.Append("%7f");
                }
                else
                {
                    builder.Append(ch);
                }
            }
            return builder.ToString();
        }

        internal static bool NeedsEncoding(string value)
        {
            foreach (char ch in value)
            {
                if (((ch < ' ') && (ch != '\t')) || (ch == '\x007f'))
                {
                    return true;
                }
            }
            return false;
        }

        internal void Send(HttpWorkerRequest wr)
        {
            if (this._knownHeaderIndex >= 0)
            {
                wr.SendKnownResponseHeader(this._knownHeaderIndex, this._value);
            }
            else
            {
                wr.SendUnknownResponseHeader(this._unknownHeader, this._value);
            }
        }

        internal virtual string Name
        {
            get
            {
                if (this._unknownHeader != null)
                {
                    return this._unknownHeader;
                }
                return HttpWorkerRequest.GetKnownResponseHeaderName(this._knownHeaderIndex);
            }
        }

        internal string Value =>
            this._value;
    }
}

