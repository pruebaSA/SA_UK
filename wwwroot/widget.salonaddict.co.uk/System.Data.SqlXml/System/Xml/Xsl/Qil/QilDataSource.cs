namespace System.Xml.Xsl.Qil
{
    using System;

    internal class QilDataSource : QilBinary
    {
        public QilDataSource(QilNodeType nodeType, QilNode name, QilNode baseUri) : base(nodeType, name, baseUri)
        {
        }

        public QilNode BaseUri
        {
            get => 
                base.Right;
            set
            {
                base.Right = value;
            }
        }

        public QilNode Name
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

