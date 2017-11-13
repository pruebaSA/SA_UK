namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Reflection;
    using System.Xml.Xsl;

    internal class QilParameter : QilIterator
    {
        private QilNode name;

        public QilParameter(QilNodeType nodeType, QilNode defaultValue, QilNode name, XmlQueryType xmlType) : base(nodeType, defaultValue)
        {
            this.name = name;
            base.xmlType = xmlType;
        }

        public override int Count =>
            2;

        public QilNode DefaultValue
        {
            get => 
                base.Binding;
            set
            {
                base.Binding = value;
            }
        }

        public override QilNode this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return base.Binding;

                    case 1:
                        return this.name;
                }
                throw new IndexOutOfRangeException();
            }
            set
            {
                switch (index)
                {
                    case 0:
                        base.Binding = value;
                        return;

                    case 1:
                        this.name = value;
                        return;
                }
                throw new IndexOutOfRangeException();
            }
        }

        public QilName Name
        {
            get => 
                ((QilName) this.name);
            set
            {
                this.name = value;
            }
        }
    }
}

