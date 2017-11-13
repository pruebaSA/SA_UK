namespace System.Web.RegularExpressions
{
    using System.Text.RegularExpressions;

    internal class DirectiveRegexFactory2 : RegexRunnerFactory
    {
        public override RegexRunner CreateInstance() => 
            new DirectiveRegexRunner2();
    }
}

