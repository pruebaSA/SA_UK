﻿namespace System.Web.RegularExpressions
{
    using System.Text.RegularExpressions;

    internal class ServerTagsRegexFactory12 : RegexRunnerFactory
    {
        public override RegexRunner CreateInstance() => 
            new ServerTagsRegexRunner12();
    }
}

