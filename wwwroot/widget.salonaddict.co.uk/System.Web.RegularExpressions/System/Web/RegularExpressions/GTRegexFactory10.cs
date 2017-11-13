namespace System.Web.RegularExpressions
{
    using System.Text.RegularExpressions;

    internal class GTRegexFactory10 : RegexRunnerFactory
    {
        public override RegexRunner CreateInstance() => 
            new GTRegexRunner10();
    }
}

