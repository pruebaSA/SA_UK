namespace System.Diagnostics.Eventing.Reader
{
    using System;
    using System.Collections.Generic;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public sealed class EventMetadata
    {
        private byte channelId;
        private string description;
        private long id;
        private long keywords;
        private byte level;
        private short opcode;
        private ProviderMetadata pmReference;
        private int task;
        private string template;
        private byte version;

        internal EventMetadata(uint id, byte version, byte channelId, byte level, byte opcode, short task, long keywords, string template, string description, ProviderMetadata pmReference)
        {
            this.id = id;
            this.version = version;
            this.channelId = channelId;
            this.level = level;
            this.opcode = opcode;
            this.task = task;
            this.keywords = keywords;
            this.template = template;
            this.description = description;
            this.pmReference = pmReference;
        }

        public string Description =>
            this.description;

        public long Id =>
            this.id;

        public IEnumerable<EventKeyword> Keywords
        {
            get
            {
                List<EventKeyword> list = new List<EventKeyword>();
                ulong keywords = (ulong) this.keywords;
                ulong num2 = 9223372036854775808L;
                for (int i = 0; i < 0x40; i++)
                {
                    if ((keywords & num2) > 0L)
                    {
                        list.Add(new EventKeyword((long) num2, this.pmReference));
                    }
                    num2 = num2 >> 1;
                }
                return list;
            }
        }

        public EventLevel Level =>
            new EventLevel(this.level, this.pmReference);

        public EventLogLink LogLink =>
            new EventLogLink(this.channelId, this.pmReference);

        public EventOpcode Opcode =>
            new EventOpcode(this.opcode, this.pmReference);

        public EventTask Task =>
            new EventTask(this.task, this.pmReference);

        public string Template =>
            this.template;

        public byte Version =>
            this.version;
    }
}

