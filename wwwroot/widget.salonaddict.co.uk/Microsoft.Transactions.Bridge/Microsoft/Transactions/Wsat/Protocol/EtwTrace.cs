namespace Microsoft.Transactions.Wsat.Protocol
{
    using System;
    using System.Diagnostics;

    internal static class EtwTrace
    {
        private const int MaxSupportedStringSize = 0xffce;
        private static EtwTraceProvider provider;
        private static object syncRoot = new object();
        private static Guid WsatProviderGuid = new Guid("7f3fe630-462b-47c5-ab07-67ca84934abd");
        private static Guid WsatTraceGuid = new Guid("{eb6517d4-090c-48ab-825e-adad366406a2}");

        private static Guid GetActivityId()
        {
            object activityId = System.Diagnostics.Trace.CorrelationManager.ActivityId;
            if (activityId != null)
            {
                return (Guid) activityId;
            }
            return Guid.Empty;
        }

        internal static void Trace(string xml, TraceType type, int eventId)
        {
            TraceInternal(GetActivityId(), xml, type, eventId);
        }

        private static unsafe uint TraceInternal(Guid guid, string xml, TraceType type, int eventId)
        {
            uint maxValue = uint.MaxValue;
            if ((Provider != null) && Provider.ShouldTrace)
            {
                int num2 = (((xml.Length + 1) * 2) < 0xffce) ? ((xml.Length + 1) * 2) : 0xffce;
                Mof3Event event2 = new Mof3Event {
                    Header = { 
                        Guid = WsatTraceGuid,
                        Type = (byte) type,
                        ClientContext = 0,
                        Flags = 0x120000,
                        BufferSize = 0x60
                    },
                    Mof2 = { Length = (uint) num2 },
                    Mof1 = { 
                        Length = 0x10,
                        Data = (IntPtr) &guid
                    },
                    Mof3 = { 
                        Length = 4,
                        Data = (IntPtr) &eventId
                    }
                };
                fixed (char* str = ((char*) xml))
                {
                    char* chPtr = str;
                    event2.Mof2.Data = (IntPtr) chPtr;
                    if (Provider != null)
                    {
                        maxValue = provider.Trace((MofEvent*) &event2);
                    }
                }
            }
            return maxValue;
        }

        internal static uint TraceTransfer(Guid relatedId) => 
            TraceTransfer(GetActivityId(), relatedId);

        private static unsafe uint TraceTransfer(Guid activityId, Guid relatedId)
        {
            uint maxValue = uint.MaxValue;
            if ((Provider != null) && Provider.ShouldTrace)
            {
                Guid2Event event2 = new Guid2Event {
                    Header = { 
                        Guid = WsatTraceGuid,
                        Type = 5,
                        ClientContext = 0,
                        Flags = 0x20000,
                        BufferSize = 80
                    },
                    Guid1 = activityId,
                    Guid2 = relatedId
                };
                if (Provider != null)
                {
                    maxValue = provider.Trace((MofEvent*) &event2);
                }
            }
            return maxValue;
        }

        internal static EtwTraceProvider Provider
        {
            get
            {
                if (provider == null)
                {
                    lock (syncRoot)
                    {
                        if (provider == null)
                        {
                            provider = new EtwTraceProvider(WsatProviderGuid, WsatTraceGuid);
                        }
                    }
                }
                return provider;
            }
        }
    }
}

