namespace System.Xml.Xsl.Qil
{
    using System;

    internal class QilSortKey : QilBinary
    {
        public QilSortKey(QilNodeType nodeType, QilNode key, QilNode collation) : base(nodeType, key, collation)
        {
        }

        public QilNode Collation
        {
            get => 
                base.Right;
            set
            {
                base.Right = value;
            }
        }

        public QilNode Key
        {
            get => 
                base.Left;
            set
            {
                base.Left = value;
            }
        }
    }
}

