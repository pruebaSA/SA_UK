namespace System.Web.RegularExpressions
{
    using System.Text.RegularExpressions;

    internal class IncludeRegexFactory8 : RegexRunnerFactory
    {
        public override RegexRunner CreateInstance() => 
            new IncludeRegexRunner8();
    }
}

