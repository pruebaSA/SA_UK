namespace System.Web.Compilation
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [Serializable, AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class LinePragmaCodeInfo
    {
        internal int _codeLength;
        internal bool _isCodeNugget;
        internal int _startColumn;
        internal int _startGeneratedColumn;
        internal int _startLine;

        public LinePragmaCodeInfo()
        {
        }

        public LinePragmaCodeInfo(int startLine, int startColumn, int startGeneratedColumn, int codeLength, bool isCodeNugget)
        {
            this._startLine = startLine;
            this._startColumn = startColumn;
            this._startGeneratedColumn = startGeneratedColumn;
            this._codeLength = codeLength;
            this._isCodeNugget = isCodeNugget;
        }

        public int CodeLength =>
            this._codeLength;

        public bool IsCodeNugget =>
            this._isCodeNugget;

        public int StartColumn =>
            this._startColumn;

        public int StartGeneratedColumn =>
            this._startGeneratedColumn;

        public int StartLine =>
            this._startLine;
    }
}

