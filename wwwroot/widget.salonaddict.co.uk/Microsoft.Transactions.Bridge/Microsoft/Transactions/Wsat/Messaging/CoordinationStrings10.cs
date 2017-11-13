namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;

    internal class CoordinationStrings10 : CoordinationStrings
    {
        private static CoordinationStrings instance = new CoordinationStrings10();

        public override string CreateCoordinationContextAction =>
            "http://schemas.xmlsoap.org/ws/2004/10/wscoor/CreateCoordinationContext";

        public override string CreateCoordinationContextResponseAction =>
            "http://schemas.xmlsoap.org/ws/2004/10/wscoor/CreateCoordinationContextResponse";

        public override string FaultAction =>
            "http://schemas.xmlsoap.org/ws/2004/10/wscoor/fault";

        public static CoordinationStrings Instance =>
            instance;

        public override string Namespace =>
            "http://schemas.xmlsoap.org/ws/2004/10/wscoor";

        public override string RegisterAction =>
            "http://schemas.xmlsoap.org/ws/2004/10/wscoor/Register";

        public override string RegisterResponseAction =>
            "http://schemas.xmlsoap.org/ws/2004/10/wscoor/RegisterResponse";
    }
}

