namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Xml.Xsl;

    internal class QilLiteral : QilNode
    {
        private object value;

        public QilLiteral(QilNodeType nodeType, object value) : base(nodeType)
        {
            this.Value = value;
        }

        public static implicit operator decimal(QilLiteral literal) => 
            ((decimal) literal.value);

        public static implicit operator double(QilLiteral literal) => 
            ((double) literal.value);

        public static implicit operator int(QilLiteral literal) => 
            ((int) literal.value);

        public static implicit operator long(QilLiteral literal) => 
            ((long) literal.value);

        public static implicit operator string(QilLiteral literal) => 
            ((string) literal.value);

        public static implicit operator XmlQueryType(QilLiteral literal) => 
            ((XmlQueryType) literal.value);

        public object Value
        {
            get => 
                this.value;
            set
            {
                this.value = value;
            }
        }
    }
}

