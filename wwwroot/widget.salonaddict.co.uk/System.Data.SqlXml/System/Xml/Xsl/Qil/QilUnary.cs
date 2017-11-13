namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Reflection;

    internal class QilUnary : QilNode
    {
        private QilNode child;

        public QilUnary(QilNodeType nodeType, QilNode child) : base(nodeType)
        {
            this.child = child;
        }

        public QilNode Child
        {
            get => 
                this.child;
            set
            {
                this.child = value;
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
                return this.child;
            }
            set
            {
                if (index != 0)
                {
                    throw new IndexOutOfRangeException();
                }
                this.child = value;
            }
        }
    }
}

