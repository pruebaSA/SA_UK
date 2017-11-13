namespace System.ServiceModel
{
    using System;
    using System.ServiceModel.Channels;

    public class MessageHeader<T>
    {
        private string actor;
        private T content;
        private bool mustUnderstand;
        private bool relay;

        public MessageHeader()
        {
        }

        public MessageHeader(T content) : this(content, false, "", false)
        {
        }

        public MessageHeader(T content, bool mustUnderstand, string actor, bool relay)
        {
            this.content = content;
            this.mustUnderstand = mustUnderstand;
            this.actor = actor;
            this.relay = relay;
        }

        internal Type GetGenericArgument() => 
            typeof(T);

        public MessageHeader GetUntypedHeader(string name, string ns) => 
            MessageHeader.CreateHeader(name, ns, this.content, this.mustUnderstand, this.actor, this.relay);

        public string Actor
        {
            get => 
                this.actor;
            set
            {
                this.actor = value;
            }
        }

        public T Content
        {
            get => 
                this.content;
            set
            {
                this.content = value;
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
    }
}

