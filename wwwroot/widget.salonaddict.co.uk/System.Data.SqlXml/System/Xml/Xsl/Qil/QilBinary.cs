namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Reflection;

    internal class QilBinary : QilNode
    {
        private QilNode left;
        private QilNode right;

        public QilBinary(QilNodeType nodeType, QilNode left, QilNode right) : base(nodeType)
        {
            this.left = left;
            this.right = right;
        }

        public override int Count =>
            2;

        public override QilNode this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.left;

                    case 1:
                        return this.right;
                }
                throw new IndexOutOfRangeException();
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.left = value;
                        return;

                    case 1:
                        this.right = value;
                        return;
                }
                throw new IndexOutOfRangeException();
            }
        }

        public QilNode Left
        {
            get => 
                this.left;
            set
            {
                this.left = value;
            }
        }

        public QilNode Right
        {
            get => 
                this.right;
            set
            {
                this.right = value;
            }
        }
    }
}

