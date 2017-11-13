﻿namespace System.Web.RegularExpressions
{
    using System.Text.RegularExpressions;

    internal class ExpressionBuilderRegexFactory16 : RegexRunnerFactory
    {
        public override RegexRunner CreateInstance() => 
            new ExpressionBuilderRegexRunner16();
    }
}

