namespace System.Xml.Xsl
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("{uriString} [{startLine},{startPos} -- {endLine},{endPos}]")]
    internal class SourceLineInfo : ISourceLineInfo
    {
        private int endLine;
        private int endPos;
        public static SourceLineInfo NoSource = new SourceLineInfo(string.Empty, 0xfeefee, 0, 0xfeefee, 0);
        private const int NoSourceMagicNumber = 0xfeefee;
        private int startLine;
        private int startPos;
        private string uriString;

        public SourceLineInfo(string uriString, int startLine, int startPos, int endLine, int endPos)
        {
            this.uriString = uriString;
            this.startLine = startLine;
            this.startPos = startPos;
            this.endLine = endLine;
            this.endPos = endPos;
        }

        public static string GetFileName(string uriString)
        {
            System.Uri uri;
            if (((uriString.Length != 0) && System.Uri.TryCreate(uriString, UriKind.Absolute, out uri)) && uri.IsFile)
            {
                return uri.LocalPath;
            }
            return uriString;
        }

        internal void SetEndLinePos(int endLine, int endPos)
        {
            this.endLine = endLine;
            this.endPos = endPos;
        }

        [Conditional("DEBUG")]
        public static void Validate(ISourceLineInfo lineInfo)
        {
            if (((lineInfo.StartLine != 0) && (lineInfo.StartLine != 0xfeefee)) && (lineInfo.StartLine == lineInfo.EndLine))
            {
            }
        }

        public int EndLine =>
            this.endLine;

        public int EndPos =>
            this.endPos;

        public bool IsNoSource =>
            (this.startLine == 0xfeefee);

        public int StartLine =>
            this.startLine;

        public int StartPos =>
            this.startPos;

        public string Uri =>
            this.uriString;
    }
}

