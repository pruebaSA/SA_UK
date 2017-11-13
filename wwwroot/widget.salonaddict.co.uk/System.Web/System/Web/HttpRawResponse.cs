namespace System.Web
{
    using System;
    using System.Collections;

    internal class HttpRawResponse
    {
        private ArrayList _buffers;
        private bool _hasSubstBlocks;
        private ArrayList _headers;
        private int _statusCode;
        private string _statusDescr;

        internal HttpRawResponse(int statusCode, string statusDescription, ArrayList headers, ArrayList buffers, bool hasSubstBlocks)
        {
            this._statusCode = statusCode;
            this._statusDescr = statusDescription;
            this._headers = headers;
            this._buffers = buffers;
            this._hasSubstBlocks = hasSubstBlocks;
        }

        internal ArrayList Buffers =>
            this._buffers;

        internal bool HasSubstBlocks =>
            this._hasSubstBlocks;

        internal ArrayList Headers =>
            this._headers;

        internal int StatusCode =>
            this._statusCode;

        internal string StatusDescription =>
            this._statusDescr;
    }
}

