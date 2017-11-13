namespace System.Xml.Xsl.Qil
{
    using System;

    internal class QilLoop : QilBinary
    {
        public QilLoop(QilNodeType nodeType, QilNode variable, QilNode body) : base(nodeType, variable, body)
        {
        }

        public QilNode Body
        {
            get => 
                base.Right;
            set
            {
                base.Right = value;
            }
        }

        public QilIterator Variable
        {
            get => 
                ((QilIterator) base.Left);
            set
            {
                base.Left = value;
            }
        }
    }
}

