namespace System.ServiceModel.Description
{
    using System;

    public class MessageBodyDescription
    {
        private MessagePartDescriptionCollection parts;
        private MessagePartDescription returnValue;
        private XmlName wrapperName;
        private string wrapperNs;

        public MessageBodyDescription()
        {
            this.parts = new MessagePartDescriptionCollection();
        }

        internal MessageBodyDescription(MessageBodyDescription other)
        {
            this.WrapperName = other.WrapperName;
            this.WrapperNamespace = other.WrapperNamespace;
            this.parts = new MessagePartDescriptionCollection();
            foreach (MessagePartDescription description in other.Parts)
            {
                this.Parts.Add(description.Clone());
            }
            if (other.ReturnValue != null)
            {
                this.ReturnValue = other.ReturnValue.Clone();
            }
        }

        internal MessageBodyDescription Clone() => 
            new MessageBodyDescription(this);

        public MessagePartDescriptionCollection Parts =>
            this.parts;

        public MessagePartDescription ReturnValue
        {
            get => 
                this.returnValue;
            set
            {
                this.returnValue = value;
            }
        }

        public string WrapperName
        {
            get
            {
                if (this.wrapperName != null)
                {
                    return this.wrapperName.EncodedName;
                }
                return null;
            }
            set
            {
                this.wrapperName = new XmlName(value, true);
            }
        }

        public string WrapperNamespace
        {
            get => 
                this.wrapperNs;
            set
            {
                this.wrapperNs = value;
            }
        }
    }
}

