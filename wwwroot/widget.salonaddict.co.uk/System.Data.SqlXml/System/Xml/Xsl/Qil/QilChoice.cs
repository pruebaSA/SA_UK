namespace System.Xml.Xsl.Qil
{
    using System;

    internal class QilChoice : QilBinary
    {
        public QilChoice(QilNodeType nodeType, QilNode expression, QilNode branches) : base(nodeType, expression, branches)
        {
        }

        public QilList Branches
        {
            get => 
                ((QilList) base.Right);
            set
            {
                base.Right = value;
            }
        }

        public QilNode Expression
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

