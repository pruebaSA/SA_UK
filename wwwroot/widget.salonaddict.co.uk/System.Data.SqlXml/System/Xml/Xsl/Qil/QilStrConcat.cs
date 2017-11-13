namespace System.Xml.Xsl.Qil
{
    using System;

    internal class QilStrConcat : QilBinary
    {
        public QilStrConcat(QilNodeType nodeType, QilNode delimiter, QilNode values) : base(nodeType, delimiter, values)
        {
        }

        public QilNode Delimiter
        {
            get => 
                base.Left;
            set
            {
                base.Left = value;
            }
        }

        public QilNode Values
        {
            get => 
                base.Right;
            set
            {
                base.Right = value;
            }
        }
    }
}

