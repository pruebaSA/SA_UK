namespace System.Xml.Xsl
{
    using System;
    using System.CodeDom.Compiler;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Xml;

    [Serializable]
    internal class XslLoadException : XslTransformException
    {
        private ISourceLineInfo lineInfo;

        internal XslLoadException(CompilerError error) : base("Xml_UserException", new string[] { error.ErrorText })
        {
            this.SetSourceLineInfo(new SourceLineInfo(error.FileName, error.Line, error.Column, error.Line, error.Column));
        }

        internal XslLoadException(Exception inner, ISourceLineInfo lineInfo) : base(inner, "Xslt_CompileError2", null)
        {
            this.lineInfo = lineInfo;
        }

        protected XslLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if ((bool) info.GetValue("hasLineInfo", typeof(bool)))
            {
                string uriString = (string) info.GetValue("Uri", typeof(string));
                int startLine = (int) info.GetValue("StartLine", typeof(int));
                int startPos = (int) info.GetValue("StartPos", typeof(int));
                int endLine = (int) info.GetValue("EndLine", typeof(int));
                int endPos = (int) info.GetValue("EndPos", typeof(int));
                this.lineInfo = new SourceLineInfo(uriString, startLine, startPos, endLine, endPos);
            }
        }

        internal XslLoadException(string res, params string[] args) : base(null, res, args)
        {
        }

        private static string AppendLineInfoMessage(string message, ISourceLineInfo lineInfo)
        {
            if (lineInfo != null)
            {
                string fileName = SourceLineInfo.GetFileName(lineInfo.Uri);
                string str2 = XslTransformException.CreateMessage("Xml_ErrorFilePosition", new string[] { fileName, lineInfo.StartLine.ToString(CultureInfo.InvariantCulture), lineInfo.StartPos.ToString(CultureInfo.InvariantCulture) });
                if ((str2 == null) || (str2.Length <= 0))
                {
                    return message;
                }
                if ((message.Length > 0) && !XmlCharType.Instance.IsWhiteSpace(message[message.Length - 1]))
                {
                    message = message + " ";
                }
                message = message + str2;
            }
            return message;
        }

        internal static string CreateMessage(ISourceLineInfo lineInfo, string res, params string[] args) => 
            AppendLineInfoMessage(XslTransformException.CreateMessage(res, args), lineInfo);

        internal override string FormatDetailedMessage() => 
            AppendLineInfoMessage(this.Message, this.lineInfo);

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("hasLineInfo", this.lineInfo != null);
            if (this.lineInfo != null)
            {
                info.AddValue("Uri", this.lineInfo.Uri);
                info.AddValue("StartLine", this.lineInfo.StartLine);
                info.AddValue("StartPos", this.lineInfo.StartPos);
                info.AddValue("EndLine", this.lineInfo.EndLine);
                info.AddValue("EndPos", this.lineInfo.EndPos);
            }
        }

        internal void SetSourceLineInfo(ISourceLineInfo lineInfo)
        {
            this.lineInfo = lineInfo;
        }

        public override int LineNumber =>
            this.lineInfo?.StartLine;

        public override int LinePosition =>
            this.lineInfo?.StartPos;

        public override string SourceUri =>
            this.lineInfo?.Uri;
    }
}

