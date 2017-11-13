namespace System.Xml.Xsl.Qil
{
    using System;
    using System.Reflection;
    using System.Xml.Xsl;

    internal class QilFunction : QilReference
    {
        private QilNode arguments;
        private QilNode definition;
        private QilNode sideEffects;

        public QilFunction(QilNodeType nodeType, QilNode arguments, QilNode definition, QilNode sideEffects, XmlQueryType resultType) : base(nodeType)
        {
            this.arguments = arguments;
            this.definition = definition;
            this.sideEffects = sideEffects;
            base.xmlType = resultType;
        }

        public QilList Arguments
        {
            get => 
                ((QilList) this.arguments);
            set
            {
                this.arguments = value;
            }
        }

        public override int Count =>
            3;

        public QilNode Definition
        {
            get => 
                this.definition;
            set
            {
                this.definition = value;
            }
        }

        public override QilNode this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return this.arguments;

                    case 1:
                        return this.definition;

                    case 2:
                        return this.sideEffects;
                }
                throw new IndexOutOfRangeException();
            }
            set
            {
                switch (index)
                {
                    case 0:
                        this.arguments = value;
                        return;

                    case 1:
                        this.definition = value;
                        return;

                    case 2:
                        this.sideEffects = value;
                        return;
                }
                throw new IndexOutOfRangeException();
            }
        }

        public bool MaybeSideEffects
        {
            get => 
                (this.sideEffects.NodeType == QilNodeType.True);
            set
            {
                this.sideEffects.NodeType = value ? QilNodeType.True : QilNodeType.False;
            }
        }
    }
}

