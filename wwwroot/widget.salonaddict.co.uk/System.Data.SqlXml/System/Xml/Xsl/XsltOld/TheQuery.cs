namespace System.Xml.Xsl.XsltOld
{
    using MS.Internal.Xml.XPath;
    using System;

    internal sealed class TheQuery
    {
        private CompiledXpathExpr _CompiledQuery;
        internal InputScopeManager _ScopeManager;

        internal TheQuery(CompiledXpathExpr compiledQuery, InputScopeManager manager)
        {
            this._CompiledQuery = compiledQuery;
            this._ScopeManager = manager.Clone();
        }

        internal CompiledXpathExpr CompiledQuery =>
            this._CompiledQuery;
    }
}

