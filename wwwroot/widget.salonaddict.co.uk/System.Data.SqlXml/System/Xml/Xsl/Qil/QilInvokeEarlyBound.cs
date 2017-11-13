namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Reflection;
    using System.Xml.Xsl;

    internal class QilInvokeEarlyBound : QilTernary
    {
        public QilInvokeEarlyBound(QilNodeType nodeType, QilNode name, QilNode method, QilNode arguments, XmlQueryType resultType) : base(nodeType, name, method, arguments)
        {
            base.xmlType = resultType;
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

        public MethodInfo ClrMethod
        {
            get => 
                ((MethodInfo) ((QilLiteral) base.Center).Value);
            set
            {
                ((QilLiteral) base.Center).Value = value;
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

