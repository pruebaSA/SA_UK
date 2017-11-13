namespace System.Web.RegularExpressions
{
    using System.Text.RegularExpressions;

    internal class AspCodeRegexFactory4 : RegexRunnerFactory
    {
        public override RegexRunner CreateInstance() => 
            new AspCodeRegexRunner4();
    }
}

