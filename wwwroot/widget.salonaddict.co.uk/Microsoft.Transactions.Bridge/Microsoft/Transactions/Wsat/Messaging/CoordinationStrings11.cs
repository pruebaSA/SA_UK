namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;

    internal class CoordinationStrings11 : CoordinationStrings
    {
        private static CoordinationStrings instance = new CoordinationStrings11();

        public override string CreateCoordinationContextAction =>
            "http://docs.oasis-open.org/ws-tx/wscoor/2006/06/CreateCoordinationContext";

        public override string CreateCoordinationContextResponseAction =>
            "http://docs.oasis-open.org/ws-tx/wscoor/2006/06/CreateCoordinationContextResponse";

        public override string FaultAction =>
            "http://docs.oasis-open.org/ws-tx/wscoor/2006/06/fault";

        public static CoordinationStrings Instance =>
            instance;

        public override string Namespace =>
            "http://docs.oasis-open.org/ws-tx/wscoor/2006/06";

        public override string RegisterAction =>
            "http://docs.oasis-open.org/ws-tx/wscoor/2006/06/Register";

        public override string RegisterResponseAction =>
            "http://docs.oasis-open.org/ws-tx/wscoor/2006/06/RegisterResponse";
    }
}

