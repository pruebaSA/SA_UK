namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    public class XPathMessageContext : XsltContext
    {
        internal const string ActionE = "Action";
        internal static readonly XPathMessageFunction ActionFun = new XPathMessageFunctionAction();
        internal static string Actor11A = EnvelopeVersion.Soap11.Actor;
        internal static string Actor12A = EnvelopeVersion.Soap12.Actor;
        internal static readonly XPathMessageFunction ActorFun = new XPathMessageFunctionActor();
        internal const string BodyE = "Body";
        internal static readonly XPathMessageFunction BodyFun = new XPathMessageFunctionBody();
        internal static readonly XPathMessageFunction DateFun = new XPathMessageFunctionDateStr();
        internal static readonly XPathMessageFunction DateNowFun = new XPathMessageFunctionDateNow();
        internal const string EnvelopeE = "Envelope";
        internal const string FaultToE = "FaultTo";
        internal static readonly XPathMessageFunction FaultToFun = new XPathMessageFunctionFaultTo();
        internal const string FromE = "From";
        internal static readonly XPathMessageFunction FromFun = new XPathMessageFunctionFrom();
        private static Function[] functions = new Function[] { 
            new Function("http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions", "header", HeaderFun), new Function("http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions", "body", BodyFun), new Function("http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions", "soap-uri", SoapUriFun), new Function("http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions", "headers-with-actor", HeadersWithActorFun), new Function("http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions", "actor", ActorFun), new Function("http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions", "is-mandatory", IsMandatoryFun), new Function("http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions", "is-actor-next", IsActorNextFun), new Function("http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions", "is-actor-ultimate-receiver", IsActorUltRecFun), new Function("http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions", "messageId", MessageIDFun), new Function("http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions", "relatesTo", RelatesToFun), new Function("http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions", "replyTo", ReplyToFun), new Function("http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions", "from", FromFun), new Function("http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions", "faultTo", FaultToFun), new Function("http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions", "to", ToFun), new Function("http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions", "action", ActionFun), new Function("http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions", "date-time", DateFun),
            new Function("http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions", "duration", SpanFun), new Function("http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions", "utc-now", DateNowFun)
        };
        internal const string HeaderE = "Header";
        internal static readonly XPathMessageFunction HeaderFun = new XPathMessageFunctionHeader();
        internal static readonly XPathMessageFunction HeadersWithActorFun = new XPathMessageFunctionHeadersWithActor();
        internal const string IndigoNS = "http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions";
        internal const string IndigoP = "sm";
        internal static readonly XPathMessageFunction IsActorNextFun = new XPathMessageFunctionIsActorNext();
        internal static readonly XPathMessageFunction IsActorUltRecFun = new XPathMessageFunctionIsActorUltimateReceiver();
        internal static readonly XPathMessageFunction IsMandatoryFun = new XPathMessageFunctionIsMandatory();
        internal const string MandatoryA = "mustUnderstand";
        internal const string MessageIDE = "MessageID";
        internal static readonly XPathMessageFunction MessageIDFun = new XPathMessageFunctionMessageID();
        internal const string RelatesToE = "RelatesTo";
        internal static readonly XPathMessageFunction RelatesToFun = new XPathMessageFunctionRelatesTo();
        internal const string ReplyToE = "ReplyTo";
        internal static readonly XPathMessageFunction ReplyToFun = new XPathMessageFunctionReplyTo();
        internal const string S11NS = "http://schemas.xmlsoap.org/soap/envelope/";
        internal const string S11P = "s11";
        internal const string S12NS = "http://www.w3.org/2003/05/soap-envelope";
        internal const string S12P = "s12";
        internal static readonly XPathMessageFunction SoapUriFun = new XPathMessageFunctionSoapUri();
        internal static readonly XPathMessageFunction SpanFun = new XPathMessageFunctionSpanStr();
        internal const string ToE = "To";
        internal static readonly XPathMessageFunction ToFun = new XPathMessageFunctionTo();
        internal const string Wsa10NS = "http://www.w3.org/2005/08/addressing";
        internal const string Wsa10P = "wsa10";
        internal const string Wsa200408NS = "http://schemas.xmlsoap.org/ws/2004/08/addressing";
        internal const string Wsa200408P = "wsaAugust2004";
        internal const string WsaNoneNS = "http://schemas.microsoft.com/ws/2005/05/addressing/none";

        public XPathMessageContext() : this(new NameTable())
        {
        }

        public XPathMessageContext(NameTable table) : base(ArgValidator(table))
        {
            this.AddNamespace("s11", "http://schemas.xmlsoap.org/soap/envelope/");
            this.AddNamespace("s12", "http://www.w3.org/2003/05/soap-envelope");
            this.AddNamespace("wsaAugust2004", "http://schemas.xmlsoap.org/ws/2004/08/addressing");
            this.AddNamespace("wsa10", "http://www.w3.org/2005/08/addressing");
            this.AddNamespace("sm", "http://schemas.microsoft.com/serviceModel/2004/05/xpathfunctions");
        }

        private static NameTable ArgValidator(NameTable table)
        {
            if (table == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("table");
            }
            return table;
        }

        public override int CompareDocument(string baseUri, string nextBaseUri) => 
            0;

        public override bool PreserveWhitespace(XPathNavigator node) => 
            false;

        public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
        {
            if (argTypes == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("argTypes");
            }
            string str = this.LookupNamespace(prefix);
            for (int i = 0; i < functions.Length; i++)
            {
                if ((functions[i].name == name) && (functions[i].ns == str))
                {
                    IXsltContextFunction function = functions[i].function;
                    if ((argTypes.Length <= function.Maxargs) && (argTypes.Length >= function.Minargs))
                    {
                        return function;
                    }
                }
            }
            return null;
        }

        public override IXsltContextVariable ResolveVariable(string prefix, string name) => 
            null;

        public override bool Whitespace =>
            false;

        [StructLayout(LayoutKind.Sequential)]
        internal struct Function
        {
            internal string ns;
            internal string name;
            internal IXsltContextFunction function;
            internal Function(string ns, string name, IXsltContextFunction function)
            {
                this.ns = ns;
                this.name = name;
                this.function = function;
            }
        }
    }
}

