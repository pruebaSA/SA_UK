namespace System.Xml.Xsl.IlGen
{
    using System;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;

    internal class XmlILStateAnalyzer
    {
        protected QilFactory fac;
        protected XmlILConstructInfo parentInfo;
        protected bool withinElem;
        protected PossibleXmlStates xstates;

        public XmlILStateAnalyzer(QilFactory fac)
        {
            this.fac = fac;
        }

        public virtual QilNode Analyze(QilNode ndConstr, QilNode ndContent)
        {
            if (ndConstr == null)
            {
                this.parentInfo = null;
                this.xstates = PossibleXmlStates.WithinSequence;
                this.withinElem = false;
                ndContent = this.AnalyzeContent(ndContent);
                return ndContent;
            }
            this.parentInfo = XmlILConstructInfo.Write(ndConstr);
            if (ndConstr.NodeType == QilNodeType.Function)
            {
                this.parentInfo.ConstructMethod = XmlILConstructMethod.Writer;
                PossibleXmlStates none = PossibleXmlStates.None;
                foreach (XmlILConstructInfo info in this.parentInfo.CallersInfo)
                {
                    if (none == PossibleXmlStates.None)
                    {
                        none = info.InitialStates;
                    }
                    else if (none != info.InitialStates)
                    {
                        none = PossibleXmlStates.Any;
                    }
                    info.PushToWriterFirst = true;
                }
                this.parentInfo.InitialStates = none;
            }
            else
            {
                if (ndConstr.NodeType != QilNodeType.Choice)
                {
                    this.parentInfo.InitialStates = this.parentInfo.FinalStates = PossibleXmlStates.WithinSequence;
                }
                if (ndConstr.NodeType != QilNodeType.RtfCtor)
                {
                    this.parentInfo.ConstructMethod = XmlILConstructMethod.WriterThenIterator;
                }
            }
            this.withinElem = ndConstr.NodeType == QilNodeType.ElementCtor;
            switch (ndConstr.NodeType)
            {
                case QilNodeType.Choice:
                    this.xstates = PossibleXmlStates.Any;
                    break;

                case QilNodeType.Function:
                    this.xstates = this.parentInfo.InitialStates;
                    break;

                case QilNodeType.ElementCtor:
                    this.xstates = PossibleXmlStates.EnumAttrs;
                    break;

                case QilNodeType.AttributeCtor:
                    this.xstates = PossibleXmlStates.WithinAttr;
                    break;

                case QilNodeType.CommentCtor:
                    this.xstates = PossibleXmlStates.WithinComment;
                    break;

                case QilNodeType.PICtor:
                    this.xstates = PossibleXmlStates.WithinPI;
                    break;

                case QilNodeType.DocumentCtor:
                    this.xstates = PossibleXmlStates.WithinContent;
                    break;

                case QilNodeType.RtfCtor:
                    this.xstates = PossibleXmlStates.WithinContent;
                    break;

                case QilNodeType.XsltCopy:
                    this.xstates = PossibleXmlStates.Any;
                    break;
            }
            if (ndContent != null)
            {
                ndContent = this.AnalyzeContent(ndContent);
            }
            if (ndConstr.NodeType == QilNodeType.Choice)
            {
                this.AnalyzeChoice(ndConstr as QilChoice, this.parentInfo);
            }
            if (ndConstr.NodeType == QilNodeType.Function)
            {
                this.parentInfo.FinalStates = this.xstates;
            }
            return ndContent;
        }

        protected virtual void AnalyzeChoice(QilChoice ndChoice, XmlILConstructInfo info)
        {
            int num = ndChoice.Branches.Count - 1;
            ndChoice.Branches[num] = this.AnalyzeContent(ndChoice.Branches[num]);
            PossibleXmlStates xstates = this.xstates;
            while (--num >= 0)
            {
                this.xstates = info.InitialStates;
                ndChoice.Branches[num] = this.AnalyzeContent(ndChoice.Branches[num]);
                if (xstates != this.xstates)
                {
                    xstates = PossibleXmlStates.Any;
                }
            }
            this.xstates = xstates;
        }

        protected virtual void AnalyzeConditional(QilTernary ndCond, XmlILConstructInfo info)
        {
            info.ConstructMethod = XmlILConstructMethod.Writer;
            ndCond.Center = this.AnalyzeContent(ndCond.Center);
            PossibleXmlStates xstates = this.xstates;
            this.xstates = info.InitialStates;
            ndCond.Right = this.AnalyzeContent(ndCond.Right);
            if (xstates != this.xstates)
            {
                this.xstates = PossibleXmlStates.Any;
            }
        }

        protected virtual QilNode AnalyzeContent(QilNode nd)
        {
            switch (nd.NodeType)
            {
                case QilNodeType.For:
                case QilNodeType.Let:
                case QilNodeType.Parameter:
                    nd = this.fac.Nop(nd);
                    break;
            }
            XmlILConstructInfo info = XmlILConstructInfo.Write(nd);
            info.ParentInfo = this.parentInfo;
            info.PushToWriterLast = true;
            info.InitialStates = this.xstates;
            QilNodeType nodeType = nd.NodeType;
            switch (nodeType)
            {
                case QilNodeType.Nop:
                {
                    QilNode child = (nd as QilUnary).Child;
                    switch (child.NodeType)
                    {
                        case QilNodeType.For:
                        case QilNodeType.Let:
                        case QilNodeType.Parameter:
                            this.AnalyzeCopy(nd, info);
                            goto Label_0126;
                    }
                    info.ConstructMethod = XmlILConstructMethod.Writer;
                    this.AnalyzeContent(child);
                    goto Label_0126;
                }
                case QilNodeType.Error:
                case QilNodeType.Warning:
                    info.ConstructMethod = XmlILConstructMethod.Writer;
                    goto Label_0126;

                case QilNodeType.Conditional:
                    this.AnalyzeConditional(nd as QilTernary, info);
                    goto Label_0126;

                case QilNodeType.Choice:
                    this.AnalyzeChoice(nd as QilChoice, info);
                    goto Label_0126;

                case QilNodeType.Length:
                    break;

                case QilNodeType.Sequence:
                    this.AnalyzeSequence(nd as QilList, info);
                    goto Label_0126;

                default:
                    if (nodeType != QilNodeType.Loop)
                    {
                        break;
                    }
                    this.AnalyzeLoop(nd as QilLoop, info);
                    goto Label_0126;
            }
            this.AnalyzeCopy(nd, info);
        Label_0126:
            info.FinalStates = this.xstates;
            return nd;
        }

        protected virtual void AnalyzeCopy(QilNode ndCopy, XmlILConstructInfo info)
        {
            XmlQueryType xmlType = ndCopy.XmlType;
            if (!xmlType.IsSingleton)
            {
                this.StartLoop(xmlType, info);
            }
            if (this.MaybeContent(xmlType))
            {
                if (this.MaybeAttrNmsp(xmlType))
                {
                    if (this.xstates == PossibleXmlStates.EnumAttrs)
                    {
                        this.xstates = PossibleXmlStates.Any;
                    }
                }
                else if ((this.xstates == PossibleXmlStates.EnumAttrs) || this.withinElem)
                {
                    this.xstates = PossibleXmlStates.WithinContent;
                }
            }
            if (!xmlType.IsSingleton)
            {
                this.EndLoop(xmlType, info);
            }
        }

        protected virtual void AnalyzeLoop(QilLoop ndLoop, XmlILConstructInfo info)
        {
            XmlQueryType xmlType = ndLoop.XmlType;
            info.ConstructMethod = XmlILConstructMethod.Writer;
            if (!xmlType.IsSingleton)
            {
                this.StartLoop(xmlType, info);
            }
            ndLoop.Body = this.AnalyzeContent(ndLoop.Body);
            if (!xmlType.IsSingleton)
            {
                this.EndLoop(xmlType, info);
            }
        }

        protected virtual void AnalyzeSequence(QilList ndSeq, XmlILConstructInfo info)
        {
            info.ConstructMethod = XmlILConstructMethod.Writer;
            for (int i = 0; i < ndSeq.Count; i++)
            {
                ndSeq[i] = this.AnalyzeContent(ndSeq[i]);
            }
        }

        private void EndLoop(XmlQueryType typ, XmlILConstructInfo info)
        {
            info.EndLoopStates = this.xstates;
            if (typ.MaybeEmpty && (info.InitialStates != this.xstates))
            {
                this.xstates = PossibleXmlStates.Any;
            }
        }

        private bool MaybeAttrNmsp(XmlQueryType typ) => 
            ((typ.NodeKinds & (XmlNodeKindFlags.Namespace | XmlNodeKindFlags.Attribute)) != XmlNodeKindFlags.None);

        private bool MaybeContent(XmlQueryType typ)
        {
            if (typ.IsNode)
            {
                return ((typ.NodeKinds & ~(XmlNodeKindFlags.Namespace | XmlNodeKindFlags.Attribute)) != XmlNodeKindFlags.None);
            }
            return true;
        }

        private void StartLoop(XmlQueryType typ, XmlILConstructInfo info)
        {
            info.BeginLoopStates = this.xstates;
            if ((typ.MaybeMany && (this.xstates == PossibleXmlStates.EnumAttrs)) && this.MaybeContent(typ))
            {
                info.BeginLoopStates = this.xstates = PossibleXmlStates.Any;
            }
        }
    }
}

