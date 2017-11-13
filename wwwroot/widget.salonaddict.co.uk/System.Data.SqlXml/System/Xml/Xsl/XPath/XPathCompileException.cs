namespace System.Xml.Xsl.XPath
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Text;
    using System.Xml.Xsl;

    [Serializable]
    internal class XPathCompileException : XslLoadException
    {
        public int endChar;
        public string queryString;
        public int startChar;

        protected XPathCompileException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.queryString = (string) info.GetValue("QueryString", typeof(string));
            this.startChar = (int) info.GetValue("StartChar", typeof(int));
            this.endChar = (int) info.GetValue("EndChar", typeof(int));
        }

        internal XPathCompileException(string resId, params string[] args) : base(resId, args)
        {
        }

        internal XPathCompileException(string queryString, int startChar, int endChar, string resId, params string[] args) : base(resId, args)
        {
            this.queryString = queryString;
            this.startChar = startChar;
            this.endChar = endChar;
        }

        private static void AppendTrimmed(StringBuilder sb, string value, int startIndex, int count, TrimType trimType)
        {
            if (count <= 0x20)
            {
                sb.Append(value, startIndex, count);
            }
            else
            {
                switch (trimType)
                {
                    case TrimType.Left:
                        sb.Append("...");
                        sb.Append(value, (startIndex + count) - 0x20, 0x20);
                        return;

                    case TrimType.Right:
                        sb.Append(value, startIndex, 0x20);
                        sb.Append("...");
                        return;

                    case TrimType.Middle:
                        sb.Append(value, startIndex, 0x10);
                        sb.Append("...");
                        sb.Append(value, (startIndex + count) - 0x10, 0x10);
                        return;
                }
            }
        }

        internal override string FormatDetailedMessage()
        {
            string message = this.Message;
            string str2 = this.MarkOutError();
            if ((str2 == null) || (str2.Length <= 0))
            {
                return message;
            }
            if (message.Length > 0)
            {
                message = message + Environment.NewLine;
            }
            return (message + str2);
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("QueryString", this.queryString);
            info.AddValue("StartChar", this.startChar);
            info.AddValue("EndChar", this.endChar);
        }

        internal string MarkOutError()
        {
            if ((this.queryString == null) || (this.queryString.Trim(new char[] { ' ' }).Length == 0))
            {
                return null;
            }
            int count = this.endChar - this.startChar;
            StringBuilder sb = new StringBuilder();
            AppendTrimmed(sb, this.queryString, 0, this.startChar, TrimType.Left);
            if (count > 0)
            {
                sb.Append(" -->");
                AppendTrimmed(sb, this.queryString, this.startChar, count, TrimType.Middle);
            }
            sb.Append("<-- ");
            AppendTrimmed(sb, this.queryString, this.endChar, this.queryString.Length - this.endChar, TrimType.Right);
            return sb.ToString();
        }

        private enum TrimType
        {
            Left,
            Right,
            Middle
        }
    }
}

