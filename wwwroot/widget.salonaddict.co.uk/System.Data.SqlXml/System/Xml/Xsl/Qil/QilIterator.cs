namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Reflection;

    internal class QilIterator : QilReference
    {
        private QilNode binding;

        public QilIterator(QilNodeType nodeType, QilNode binding) : base(nodeType)
        {
            this.Binding = binding;
        }

        public QilNode Binding
        {
            get => 
                this.binding;
            set
            {
                this.binding = value;
            }
        }

        public override int Count =>
            1;

        public override QilNode this[int index]
        {
            get
            {
                if (index != 0)
                {
                    throw new IndexOutOfRangeException();
                }
                return this.binding;
            }
            set
            {
                if (index != 0)
                {
                    throw new IndexOutOfRangeException();
                }
                this.binding = value;
            }
        }
    }
}

