namespace System.ServiceModel.Description
{
    using System;
    using System.Xml;

    public class MessageHeaderDescription : MessagePartDescription
    {
        private string actor;
        private bool isUnknownHeader;
        private bool mustUnderstand;
        private bool relay;
        private bool typedHeader;

        internal MessageHeaderDescription(MessageHeaderDescription other) : base(other)
        {
            this.MustUnderstand = other.MustUnderstand;
            this.Relay = other.Relay;
            this.Actor = other.Actor;
            this.TypedHeader = other.TypedHeader;
            this.IsUnknownHeaderCollection = other.IsUnknownHeaderCollection;
        }

        public MessageHeaderDescription(string name, string ns) : base(name, ns)
        {
        }

        internal override MessagePartDescription Clone() => 
            new MessageHeaderDescription(this);

        public string Actor
        {
            get => 
                this.actor;
            set
            {
                this.actor = value;
            }
        }

        internal bool IsUnknownHeaderCollection
        {
            get => 
                (this.isUnknownHeader || (base.Multiple && (base.Type == typeof(XmlElement))));
            set
            {
                this.isUnknownHeader = value;
            }
        }

        public bool MustUnderstand
        {
            get => 
                this.mustUnderstand;
            set
            {
                this.mustUnderstand = value;
            }
        }

        public bool Relay
        {
            get => 
                this.relay;
            set
            {
                this.relay = value;
            }
        }

        public bool TypedHeader
        {
            get => 
                this.typedHeader;
            set
            {
                this.typedHeader = value;
            }
        }
    }
}

