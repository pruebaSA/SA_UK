namespace System.Web.RegularExpressions
{
    using System.Text.RegularExpressions;

    internal class LTRegexFactory11 : RegexRunnerFactory
    {
        public override RegexRunner CreateInstance() => 
            new LTRegexRunner11();
    }
}

