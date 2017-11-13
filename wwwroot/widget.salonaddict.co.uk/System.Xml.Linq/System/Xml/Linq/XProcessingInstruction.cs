namespace System.Xml.Linq
{
    using System;
    using System.Xml;

    public class XProcessingInstruction : XNode
    {
        internal string data;
        internal string target;

        public XProcessingInstruction(XProcessingInstruction other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            this.target = other.target;
            this.data = other.data;
        }

        internal XProcessingInstruction(XmlReader r)
        {
            this.target = r.Name;
            this.data = r.Value;
            r.Read();
        }

        public XProcessingInstruction(string target, string data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            ValidateName(target);
            this.target = target;
            this.data = data;
        }

        internal override XNode CloneNode() => 
            new XProcessingInstruction(this);

        internal override bool DeepEquals(XNode node)
        {
            XProcessingInstruction instruction = node as XProcessingInstruction;
            return (((instruction != null) && (this.target == instruction.target)) && (this.data == instruction.data));
        }

        internal override int GetDeepHashCode() => 
            (this.target.GetHashCode() ^ this.data.GetHashCode());

        private static void ValidateName(string name)
        {
            XmlConvert.VerifyNCName(name);
            if (string.Compare(name, "xml", StringComparison.OrdinalIgnoreCase) == 0)
            {
                throw new ArgumentException(System.Xml.Linq.Res.GetString("Argument_InvalidPIName", new object[] { name }));
            }
        }

        public override void WriteTo(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            writer.WriteProcessingInstruction(this.target, this.data);
        }

        public string Data
        {
            get => 
                this.data;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
                this.data = value;
                if (flag)
                {
                    base.NotifyChanged(this, XObjectChangeEventArgs.Value);
                }
            }
        }

        public override XmlNodeType NodeType =>
            XmlNodeType.ProcessingInstruction;

        public string Target
        {
            get => 
                this.target;
            set
            {
                ValidateName(value);
                bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Name);
                this.target = value;
                if (flag)
                {
                    base.NotifyChanged(this, XObjectChangeEventArgs.Name);
                }
            }
        }
    }
}

