namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Reflection;

    internal class QilTernary : QilNode
    {
        private QilNode center;
        private QilNode left;
        private QilNode right;

        public QilTernary(QilNodeType nodeType, QilNode left, QilNode center, QilNode right) : base(nodeType)
        {
            this.left = left;
            this.center = center;
            this.right = right;
        }

        public QilNode Center
        {
            get => 
                this.center;
            set
            {
                this.center = value;
            }
        }

        public override int Count =>
            3;

        public override QilNode this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.left;

                    case 1:
                        return this.center;

                    case 2:
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
                        this.center = value;
                        return;

                    case 2:
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

