namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Xml.Xsl;

    internal class QilTargetType : QilBinary
    {
        public QilTargetType(QilNodeType nodeType, QilNode expr, QilNode targetType) : base(nodeType, expr, targetType)
        {
        }

        public QilNode Source
        {
            get => 
                base.Left;
            set
            {
                base.Left = value;
            }
        }

        public XmlQueryType TargetType
        {
            get => 
                ((XmlQueryType) ((QilLiteral) base.Right).Value);
            set
            {
                ((QilLiteral) base.Right).Value = value;
            }
        }
    }
}

