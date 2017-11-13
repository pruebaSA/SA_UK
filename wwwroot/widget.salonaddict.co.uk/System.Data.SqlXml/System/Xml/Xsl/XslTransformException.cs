namespace System.Xml.Xsl
{
    using System;
    using System.Resources;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Xml.Utils;

    [Serializable]
    internal class XslTransformException : XsltException
    {
        public XslTransformException(string message) : base(CreateMessage(message, null), null)
        {
        }

        protected XslTransformException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        internal XslTransformException(string res, params string[] args) : this(null, res, args)
        {
        }

        public XslTransformException(Exception inner, string res, params string[] args) : base(CreateMessage(res, args), inner)
        {
        }

        internal static string CreateMessage(string res, params string[] args)
        {
            string str = null;
            try
            {
                str = Res.GetString(res, args);
            }
            catch (MissingManifestResourceException)
            {
            }
            if (str != null)
            {
                return str;
            }
            StringBuilder builder = new StringBuilder(res);
            if ((args != null) && (args.Length > 0))
            {
                builder.Append('(');
                builder.Append(args[0]);
                for (int i = 1; i < args.Length; i++)
                {
                    builder.Append(", ");
                    builder.Append(args[i]);
                }
                builder.Append(')');
            }
            return builder.ToString();
        }

        internal virtual string FormatDetailedMessage() => 
            this.Message;

        public override string ToString()
        {
            string fullName = base.GetType().FullName;
            string str2 = this.FormatDetailedMessage();
            if ((str2 != null) && (str2.Length > 0))
            {
                fullName = fullName + ": " + str2;
            }
            if (base.InnerException != null)
            {
                string str3 = fullName;
                fullName = str3 + " ---> " + base.InnerException.ToString() + Environment.NewLine + "   " + CreateMessage("Xml_EndOfInnerExceptionStack", new string[0]);
            }
            if (this.StackTrace != null)
            {
                fullName = fullName + Environment.NewLine + this.StackTrace;
            }
            return fullName;
        }
    }
}

