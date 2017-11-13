namespace System.Xml.Xsl.IlGen
{
    using System;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Xsl.Qil;

    internal class XmlILAnnotation : ListBase<object>
    {
        private object annPrev;
        private int argPos;
        private XmlILConstructInfo constrInfo;
        private MethodInfo funcMethod;
        private IteratorDescriptor iterInfo;
        private OptimizerPatterns optPatt;

        private XmlILAnnotation(object annPrev)
        {
            this.annPrev = annPrev;
        }

        public static XmlILAnnotation Write(QilNode nd)
        {
            XmlILAnnotation annotation = nd.Annotation as XmlILAnnotation;
            if (annotation == null)
            {
                annotation = new XmlILAnnotation(nd.Annotation);
                nd.Annotation = annotation;
            }
            return annotation;
        }

        public int ArgumentPosition
        {
            get => 
                this.argPos;
            set
            {
                this.argPos = value;
            }
        }

        public IteratorDescriptor CachedIteratorDescriptor
        {
            get => 
                this.iterInfo;
            set
            {
                this.iterInfo = value;
            }
        }

        public XmlILConstructInfo ConstructInfo
        {
            get => 
                this.constrInfo;
            set
            {
                this.constrInfo = value;
            }
        }

        public override int Count
        {
            get
            {
                if (this.annPrev == null)
                {
                    return 2;
                }
                return 3;
            }
        }

        public MethodInfo FunctionBinding
        {
            get => 
                this.funcMethod;
            set
            {
                this.funcMethod = value;
            }
        }

        public override object this[int index]
        {
            get
            {
                if (this.annPrev != null)
                {
                    if (index == 0)
                    {
                        return this.annPrev;
                    }
                    index--;
                }
                switch (index)
                {
                    case 0:
                        return this.constrInfo;

                    case 1:
                        return this.optPatt;
                }
                throw new IndexOutOfRangeException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public OptimizerPatterns Patterns
        {
            get => 
                this.optPatt;
            set
            {
                this.optPatt = value;
            }
        }
    }
}

