namespace System.Web.RegularExpressions
{
    using System.Text.RegularExpressions;

    internal class WebResourceRegexFactory20 : RegexRunnerFactory
    {
        public override RegexRunner CreateInstance() => 
            new WebResourceRegexRunner20();
    }
}

