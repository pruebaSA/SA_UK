namespace System.ServiceModel.ComIntegration
{
    using System;
    using System.Diagnostics;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;

    internal static class ComPlusInstanceCreationTrace
    {
        public static void Trace(TraceEventType type, TraceCode code, string description, ServiceInfo info, Message message, Guid incomingTransactionID)
        {
            if (DiagnosticUtility.ShouldTrace(type))
            {
                WindowsIdentity messageIdentity = MessageUtil.GetMessageIdentity(message);
                Uri from = null;
                if (message.Headers.From != null)
                {
                    from = message.Headers.From.Uri;
                }
                ComPlusInstanceCreationRequestSchema trace = new ComPlusInstanceCreationRequestSchema(info.AppID, info.Clsid, from, incomingTransactionID, messageIdentity.Name);
                Guid activityId = TraceUtility.ExtractActivityId(message);
                DiagnosticUtility.DiagnosticTrace.TraceEvent(type, code, System.ServiceModel.SR.GetString(description), trace, null, activityId, null);
            }
        }

        public static void Trace(TraceEventType type, TraceCode code, string description, ServiceInfo info, InstanceContext instanceContext, int instanceID)
        {
            if (DiagnosticUtility.ShouldTrace(type))
            {
                ComPlusInstanceReleasedSchema trace = new ComPlusInstanceReleasedSchema(info.AppID, info.Clsid, instanceID);
                DiagnosticUtility.DiagnosticTrace.TraceEvent(type, code, System.ServiceModel.SR.GetString(description), trace);
            }
        }

        public static void Trace(TraceEventType type, TraceCode code, string description, ServiceInfo info, Message message, int instanceID, Guid incomingTransactionID)
        {
            if (DiagnosticUtility.ShouldTrace(type))
            {
                WindowsIdentity messageIdentity = MessageUtil.GetMessageIdentity(message);
                Uri from = null;
                if (message.Headers.From != null)
                {
                    from = message.Headers.From.Uri;
                }
                ComPlusInstanceCreationSuccessSchema trace = new ComPlusInstanceCreationSuccessSchema(info.AppID, info.Clsid, from, incomingTransactionID, messageIdentity.Name, instanceID);
                Guid activityId = TraceUtility.ExtractActivityId(message);
                DiagnosticUtility.DiagnosticTrace.TraceEvent(type, code, System.ServiceModel.SR.GetString(description), trace, null, activityId, null);
            }
        }
    }
}

