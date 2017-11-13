﻿namespace System.Web.Configuration
{
    using System;
    using System.Text.RegularExpressions;

    internal class CheckPair
    {
        private string _header;
        private string _match;
        private bool _nonMatch;

        internal CheckPair(string header, string match)
        {
            this._header = header;
            this._match = match;
            this._nonMatch = false;
            new Regex(match);
        }

        internal CheckPair(string header, string match, bool nonMatch)
        {
            this._header = header;
            this._match = match;
            this._nonMatch = nonMatch;
            new Regex(match);
        }

        public string Header =>
            this._header;

        public string MatchString =>
            this._match;

        public bool NonMatch =>
            this._nonMatch;
    }
}

