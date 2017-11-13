namespace System.Xml
{
    using System;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Threading;

    [Serializable]
    public class XmlException : SystemException
    {
        private string[] args;
        private int lineNumber;
        private int linePosition;
        private string message;
        private string res;
        [OptionalField]
        private string sourceUri;

        public XmlException() : this(null)
        {
        }

        public XmlException(string message) : this(message, (Exception) null, 0, 0)
        {
        }

        protected XmlException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.res = (string) info.GetValue("res", typeof(string));
            this.args = (string[]) info.GetValue("args", typeof(string[]));
            this.lineNumber = (int) info.GetValue("lineNumber", typeof(int));
            this.linePosition = (int) info.GetValue("linePosition", typeof(int));
            this.sourceUri = string.Empty;
            string str = null;
            SerializationInfoEnumerator enumerator = info.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SerializationEntry current = enumerator.Current;
                string name = current.Name;
                if (name != null)
                {
                    if (name == "sourceUri")
                    {
                        this.sourceUri = (string) current.Value;
                    }
                    else if (name == "version")
                    {
                        goto Label_00E0;
                    }
                }
                continue;
            Label_00E0:
                str = (string) current.Value;
            }
            if (str == null)
            {
                this.message = CreateMessage(this.res, this.args, this.lineNumber, this.linePosition);
            }
            else
            {
                this.message = null;
            }
        }

        public XmlException(string message, Exception innerException) : this(message, innerException, 0, 0)
        {
        }

        internal XmlException(string res, string[] args) : this(res, args, null, 0, 0, null)
        {
        }

        internal XmlException(string res, string arg) : this(res, new string[] { arg }, null, 0, 0, null)
        {
        }

        internal XmlException(string res, int lineNumber, int linePosition) : this(res, null, null, lineNumber, linePosition)
        {
        }

        internal XmlException(string res, string[] args, string sourceUri) : this(res, args, null, 0, 0, sourceUri)
        {
        }

        internal XmlException(string res, string arg, string sourceUri) : this(res, new string[] { arg }, null, 0, 0, sourceUri)
        {
        }

        internal XmlException(string res, string arg, IXmlLineInfo lineInfo) : this(res, new string[] { arg }, lineInfo, (string) null)
        {
        }

        internal XmlException(string res, string[] args, IXmlLineInfo lineInfo) : this(res, args, lineInfo, (string) null)
        {
        }

        public XmlException(string message, Exception innerException, int lineNumber, int linePosition) : this(message, innerException, lineNumber, linePosition, null)
        {
        }

        internal XmlException(string res, string arg, Exception innerException, IXmlLineInfo lineInfo) : this(res, new string[] { arg }, innerException, (lineInfo == null) ? 0 : lineInfo.LineNumber, (lineInfo == null) ? 0 : lineInfo.LinePosition, null)
        {
        }

        internal XmlException(string res, string arg, int lineNumber, int linePosition) : this(res, new string[] { arg }, null, lineNumber, linePosition, null)
        {
        }

        internal XmlException(string res, string[] args, int lineNumber, int linePosition) : this(res, args, null, lineNumber, linePosition, null)
        {
        }

        internal XmlException(string res, string arg, IXmlLineInfo lineInfo, string sourceUri) : this(res, new string[] { arg }, lineInfo, sourceUri)
        {
        }

        internal XmlException(string res, string[] args, IXmlLineInfo lineInfo, string sourceUri) : this(res, args, null, (lineInfo == null) ? 0 : lineInfo.LineNumber, (lineInfo == null) ? 0 : lineInfo.LinePosition, sourceUri)
        {
        }

        internal XmlException(string res, string[] args, Exception innerException, int lineNumber, int linePosition) : this(res, args, innerException, lineNumber, linePosition, null)
        {
        }

        internal XmlException(string message, Exception innerException, int lineNumber, int linePosition, string sourceUri) : this((message == null) ? "Xml_DefaultException" : "Xml_UserException", new string[] { message }, innerException, lineNumber, linePosition, sourceUri)
        {
        }

        internal XmlException(string res, string arg, int lineNumber, int linePosition, string sourceUri) : this(res, new string[] { arg }, null, lineNumber, linePosition, sourceUri)
        {
        }

        internal XmlException(string res, string[] args, int lineNumber, int linePosition, string sourceUri) : this(res, args, null, lineNumber, linePosition, sourceUri)
        {
        }

        internal XmlException(string res, string[] args, Exception innerException, int lineNumber, int linePosition, string sourceUri) : base(CreateMessage(res, args, lineNumber, linePosition), innerException)
        {
            base.HResult = -2146232000;
            this.res = res;
            this.args = args;
            this.sourceUri = sourceUri;
            this.lineNumber = lineNumber;
            this.linePosition = linePosition;
        }

        internal static string[] BuildCharExceptionStr(char ch)
        {
            string[] strArray = new string[2];
            if (ch == '\0')
            {
                strArray[0] = ".";
            }
            else
            {
                strArray[0] = ch.ToString(CultureInfo.InvariantCulture);
            }
            strArray[1] = "0x" + ((int) ch).ToString("X2", CultureInfo.InvariantCulture);
            return strArray;
        }

        private static string CreateMessage(string res, string[] args, int lineNumber, int linePosition)
        {
            try
            {
                string str = Res.GetString(res, args);
                if (lineNumber != 0)
                {
                    string[] strArray = new string[] { lineNumber.ToString(CultureInfo.InvariantCulture), linePosition.ToString(CultureInfo.InvariantCulture) };
                    str = str + " " + Res.GetString("Xml_ErrorPosition", strArray);
                }
                return str;
            }
            catch (MissingManifestResourceException)
            {
                return ("UNKNOWN(" + res + ")");
            }
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("res", this.res);
            info.AddValue("args", this.args);
            info.AddValue("lineNumber", this.lineNumber);
            info.AddValue("linePosition", this.linePosition);
            info.AddValue("sourceUri", this.sourceUri);
            info.AddValue("version", "2.0");
        }

        internal static bool IsCatchableException(Exception e) => 
            ((((!(e is StackOverflowException) && !(e is OutOfMemoryException)) && (!(e is ThreadAbortException) && !(e is ThreadInterruptedException))) && !(e is NullReferenceException)) && !(e is AccessViolationException));

        public int LineNumber =>
            this.lineNumber;

        public int LinePosition =>
            this.linePosition;

        public override string Message
        {
            get
            {
                if (this.message != null)
                {
                    return this.message;
                }
                return base.Message;
            }
        }

        internal string ResString =>
            this.res;

        public string SourceUri =>
            this.sourceUri;
    }
}

