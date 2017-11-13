﻿namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    [DataContract]
    public class MatchAllMessageFilter : MessageFilter
    {
        public override bool Match(Message message)
        {
            if (message == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
            }
            return true;
        }

        public override bool Match(MessageBuffer messageBuffer)
        {
            if (messageBuffer == null)
            {
                throw System.ServiceModel.DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("messageBuffer");
            }
            return true;
        }
    }
}

