﻿namespace System.Web.RegularExpressions
{
    using System;
    using System.Collections;
    using System.Text.RegularExpressions;

    public class DatabindExprRegex : Regex
    {
        public DatabindExprRegex()
        {
            base.pattern = @"\G<%#(?<code>.*?)?%>";
            base.roptions = RegexOptions.Singleline | RegexOptions.Multiline;
            base.factory = new DatabindExprRegexFactory6();
            base.capnames = new Hashtable();
            base.capnames.Add("code", 1);
            base.capnames.Add("0", 0);
            base.capslist = new string[] { "0", "code" };
            base.capsize = 2;
            base.InitializeReferences();
        }
    }
}

