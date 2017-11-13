namespace System.Data.Common.EntitySql
{
    using System;
    using System.Data;
    using System.Data.Entity;

    internal sealed class ParserOptions
    {
        private bool _allowQuotedIdentifiers;
        private bool _bReadOnly;
        private CompilationMode _compilationMode;
        private string _defaultOrderByCollation;
        private CaseSensitiveness _identifierCaseSensitiveness = CaseSensitiveness.CaseInsensitive;

        private void CheckIfReadOnly()
        {
            if (this._bReadOnly)
            {
                throw EntityUtil.EntitySqlError(Strings.PropertyCannotBeChangedAtThisTime);
            }
        }

        internal ParserOptions MakeReadOnly() => 
            this;

        internal bool AllowQuotedIdentifiers =>
            this._allowQuotedIdentifiers;

        internal string DefaultOrderByCollation =>
            (this._defaultOrderByCollation ?? string.Empty);

        internal CaseSensitiveness IdentifierCaseSensitiveness =>
            this._identifierCaseSensitiveness;

        internal CompilationMode ParserCompilationMode
        {
            get => 
                this._compilationMode;
            set
            {
                this.CheckIfReadOnly();
                this._compilationMode = value;
            }
        }

        internal enum CaseSensitiveness
        {
            CaseSensitive,
            CaseInsensitive
        }

        internal enum CompilationMode
        {
            NormalMode,
            RestrictedViewGenerationMode,
            UserViewGenerationMode
        }
    }
}

