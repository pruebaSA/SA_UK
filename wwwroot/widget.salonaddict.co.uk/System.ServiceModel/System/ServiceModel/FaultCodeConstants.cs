﻿namespace System.ServiceModel
{
    using System;

    internal class FaultCodeConstants
    {
        internal class Actions
        {
            public const string NetDispatcher = "http://schemas.microsoft.com/net/2005/12/windowscommunicationfoundation/dispatcher/fault";
            public const string Transactions = "http://schemas.microsoft.com/net/2005/12/windowscommunicationfoundation/transactions/fault";
        }

        internal class Codes
        {
            public const string DeserializationFailed = "DeserializationFailed";
            public const string InternalServiceFault = "InternalServiceFault";
            public const string IssuedTokenFlowNotAllowed = "IssuedTokenFlowNotAllowed";
            public const string SessionTerminated = "SessionTerminated";
            public const string TransactionAborted = "TransactionAborted";
            public const string TransactionHeaderMalformed = "TransactionHeaderMalformed";
            public const string TransactionHeaderMissing = "TransactionHeaderMissing";
            public const string TransactionIsolationLevelMismatch = "TransactionIsolationLevelMismatch";
            public const string TransactionUnmarshalingFailed = "TransactionUnmarshalingFailed";
        }

        internal class Namespaces
        {
            public const string NetDispatch = "http://schemas.microsoft.com/net/2005/12/windowscommunicationfoundation/dispatcher";
            public const string Transactions = "http://schemas.microsoft.com/net/2005/12/windowscommunicationfoundation/transactions";
        }
    }
}

