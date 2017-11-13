namespace System.Xml.Xsl.Qil
{
    using System;

    internal class QilInvoke : QilBinary
    {
        public QilInvoke(QilNodeType nodeType, QilNode function, QilNode arguments) : base(nodeType, function, arguments)
        {
        }

        public QilList Arguments
        {
            get => 
                ((QilList) base.Right);
            set
            {
                base.Right = value;
            }
        }

        public QilFunction Function
        {
            get => 
                ((QilFunction) base.Left);
            set
            {
                base.Left = value;
            }
        }
    }
}

