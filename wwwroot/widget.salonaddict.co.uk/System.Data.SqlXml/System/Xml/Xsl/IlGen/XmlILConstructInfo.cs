namespace System.Xml.Xsl.IlGen
{
    using System;
    using System.Collections;
    using System.Xml.Xsl.Qil;

    internal class XmlILConstructInfo : IQilAnnotation
    {
        private ArrayList callersInfo;
        private XmlILConstructMethod constrMeth;
        private static XmlILConstructInfo Default;
        private bool isNmspInScope;
        private bool isReadOnly;
        private bool mightHaveAttrs;
        private bool mightHaveDupAttrs;
        private bool mightHaveNmsp;
        private bool mightHaveNmspAfterAttrs;
        private QilNodeType nodeType;
        private XmlILConstructInfo parentInfo;
        private PossibleXmlStates xstatesBeginLoop;
        private PossibleXmlStates xstatesEndLoop;
        private PossibleXmlStates xstatesFinal;
        private PossibleXmlStates xstatesInitial;

        private XmlILConstructInfo(QilNodeType nodeType)
        {
            this.nodeType = nodeType;
            this.xstatesInitial = this.xstatesFinal = PossibleXmlStates.Any;
            this.xstatesBeginLoop = this.xstatesEndLoop = PossibleXmlStates.None;
            this.isNmspInScope = false;
            this.mightHaveNmsp = true;
            this.mightHaveAttrs = true;
            this.mightHaveDupAttrs = true;
            this.mightHaveNmspAfterAttrs = true;
            this.constrMeth = XmlILConstructMethod.Iterator;
            this.parentInfo = null;
        }

        public static XmlILConstructInfo Read(QilNode nd)
        {
            XmlILAnnotation annotation = nd.Annotation as XmlILAnnotation;
            XmlILConstructInfo constructInfo = annotation?.ConstructInfo;
            if (constructInfo != null)
            {
                return constructInfo;
            }
            if (Default == null)
            {
                constructInfo = new XmlILConstructInfo(QilNodeType.Unknown) {
                    isReadOnly = true
                };
                Default = constructInfo;
                return constructInfo;
            }
            return Default;
        }

        public override string ToString()
        {
            string str = "";
            if (this.constrMeth != XmlILConstructMethod.Iterator)
            {
                str = (str + this.constrMeth.ToString()) + ", " + this.xstatesInitial;
                if (this.xstatesBeginLoop != PossibleXmlStates.None)
                {
                    string str2 = str;
                    str = str2 + " => " + this.xstatesBeginLoop.ToString() + " => " + this.xstatesEndLoop.ToString();
                }
                str = str + " => " + this.xstatesFinal;
                if (!this.MightHaveAttributes)
                {
                    str = str + ", NoAttrs";
                }
                if (!this.MightHaveDuplicateAttributes)
                {
                    str = str + ", NoDupAttrs";
                }
                if (!this.MightHaveNamespaces)
                {
                    str = str + ", NoNmsp";
                }
                if (!this.MightHaveNamespacesAfterAttributes)
                {
                    str = str + ", NoNmspAfterAttrs";
                }
            }
            return str;
        }

        public static XmlILConstructInfo Write(QilNode nd)
        {
            XmlILAnnotation annotation = XmlILAnnotation.Write(nd);
            XmlILConstructInfo constructInfo = annotation.ConstructInfo;
            if ((constructInfo == null) || constructInfo.isReadOnly)
            {
                constructInfo = new XmlILConstructInfo(nd.NodeType);
                annotation.ConstructInfo = constructInfo;
            }
            return constructInfo;
        }

        public PossibleXmlStates BeginLoopStates
        {
            set
            {
                this.xstatesBeginLoop = value;
            }
        }

        public ArrayList CallersInfo
        {
            get
            {
                if (this.callersInfo == null)
                {
                    this.callersInfo = new ArrayList();
                }
                return this.callersInfo;
            }
        }

        public XmlILConstructMethod ConstructMethod
        {
            get => 
                this.constrMeth;
            set
            {
                this.constrMeth = value;
            }
        }

        public PossibleXmlStates EndLoopStates
        {
            set
            {
                this.xstatesEndLoop = value;
            }
        }

        public PossibleXmlStates FinalStates
        {
            get => 
                this.xstatesFinal;
            set
            {
                this.xstatesFinal = value;
            }
        }

        public PossibleXmlStates InitialStates
        {
            get => 
                this.xstatesInitial;
            set
            {
                this.xstatesInitial = value;
            }
        }

        public bool IsNamespaceInScope
        {
            get => 
                this.isNmspInScope;
            set
            {
                this.isNmspInScope = value;
            }
        }

        public bool MightHaveAttributes
        {
            get => 
                this.mightHaveAttrs;
            set
            {
                this.mightHaveAttrs = value;
            }
        }

        public bool MightHaveDuplicateAttributes
        {
            get => 
                this.mightHaveDupAttrs;
            set
            {
                this.mightHaveDupAttrs = value;
            }
        }

        public bool MightHaveNamespaces
        {
            get => 
                this.mightHaveNmsp;
            set
            {
                this.mightHaveNmsp = value;
            }
        }

        public bool MightHaveNamespacesAfterAttributes
        {
            get => 
                this.mightHaveNmspAfterAttrs;
            set
            {
                this.mightHaveNmspAfterAttrs = value;
            }
        }

        public virtual string Name =>
            "ConstructInfo";

        public XmlILConstructInfo ParentElementInfo
        {
            get
            {
                if ((this.parentInfo != null) && (this.parentInfo.nodeType == QilNodeType.ElementCtor))
                {
                    return this.parentInfo;
                }
                return null;
            }
        }

        public XmlILConstructInfo ParentInfo
        {
            set
            {
                this.parentInfo = value;
            }
        }

        public bool PullFromIteratorFirst
        {
            get
            {
                if (this.constrMeth != XmlILConstructMethod.IteratorThenWriter)
                {
                    return (this.constrMeth == XmlILConstructMethod.Iterator);
                }
                return true;
            }
            set
            {
                switch (this.constrMeth)
                {
                    case XmlILConstructMethod.Writer:
                        this.constrMeth = XmlILConstructMethod.IteratorThenWriter;
                        return;

                    case XmlILConstructMethod.WriterThenIterator:
                        this.constrMeth = XmlILConstructMethod.Iterator;
                        return;
                }
            }
        }

        public bool PushToWriterFirst
        {
            get
            {
                if (this.constrMeth != XmlILConstructMethod.Writer)
                {
                    return (this.constrMeth == XmlILConstructMethod.WriterThenIterator);
                }
                return true;
            }
            set
            {
                XmlILConstructMethod constrMeth = this.constrMeth;
                if (constrMeth != XmlILConstructMethod.Iterator)
                {
                    if (constrMeth != XmlILConstructMethod.IteratorThenWriter)
                    {
                        return;
                    }
                }
                else
                {
                    this.constrMeth = XmlILConstructMethod.WriterThenIterator;
                    return;
                }
                this.constrMeth = XmlILConstructMethod.Writer;
            }
        }

        public bool PushToWriterLast
        {
            get
            {
                if (this.constrMeth != XmlILConstructMethod.Writer)
                {
                    return (this.constrMeth == XmlILConstructMethod.IteratorThenWriter);
                }
                return true;
            }
            set
            {
                switch (this.constrMeth)
                {
                    case XmlILConstructMethod.Iterator:
                        this.constrMeth = XmlILConstructMethod.IteratorThenWriter;
                        return;

                    case XmlILConstructMethod.Writer:
                        break;

                    case XmlILConstructMethod.WriterThenIterator:
                        this.constrMeth = XmlILConstructMethod.Writer;
                        break;

                    default:
                        return;
                }
            }
        }
    }
}

