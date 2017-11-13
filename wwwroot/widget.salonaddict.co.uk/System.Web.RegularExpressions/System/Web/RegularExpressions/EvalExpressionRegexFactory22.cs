namespace System.Web.RegularExpressions
{
    using System.Text.RegularExpressions;

    internal class EvalExpressionRegexFactory22 : RegexRunnerFactory
    {
        public override RegexRunner CreateInstance() => 
            new EvalExpressionRegexRunner22();
    }
}

