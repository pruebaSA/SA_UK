namespace System.Xml.Xsl.Qil
{
    using System;

    internal class QilInvokeLateBound : QilBinary
    {
        public QilInvokeLateBound(QilNodeType nodeType, QilNode name, QilNode arguments) : base(nodeType, name, arguments)
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

        public QilName Name
        {
            get => 
                ((QilName) base.Left);
            set
            {
                base.Left = value;
            }
        }
    }
}

