namespace System.Web.RegularExpressions
{
    using System.Text.RegularExpressions;

    internal class TextRegexFactory9 : RegexRunnerFactory
    {
        public override RegexRunner CreateInstance() => 
            new TextRegexRunner9();
    }
}

