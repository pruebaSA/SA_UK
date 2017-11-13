namespace System.Xml.Xsl.Xslt
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Xsl;
    using System.Xml.Xsl.Qil;

    internal class InvokeGenerator : QilCloneVisitor
    {
        private int curArg;
        private bool debug;
        private XsltQilFactory fac;
        private QilList formalArgs;
        private QilList invokeArgs;
        private Stack<QilIterator> iterStack;

        public InvokeGenerator(XsltQilFactory f, bool debug) : base(f.BaseFactory)
        {
            this.debug = debug;
            this.fac = f;
            this.iterStack = new Stack<QilIterator>();
        }

        private QilNode FindActualArg(QilParameter formalArg, IList<XslNode> actualArgs)
        {
            QilName name = formalArg.Name;
            foreach (XslNode node in actualArgs)
            {
                if (node.Name.Equals(name))
                {
                    return ((VarPar) node).Value;
                }
            }
            return null;
        }

        public QilNode GenerateInvoke(QilFunction func, IList<XslNode> actualArgs)
        {
            this.iterStack.Clear();
            this.formalArgs = func.Arguments;
            this.invokeArgs = this.fac.ActualParameterList();
            this.curArg = 0;
            while (this.curArg < this.formalArgs.Count)
            {
                QilParameter formalArg = (QilParameter) this.formalArgs[this.curArg];
                QilNode expr = this.FindActualArg(formalArg, actualArgs);
                if (expr == null)
                {
                    if (this.debug)
                    {
                        if (formalArg.Name.NamespaceUri == "urn:schemas-microsoft-com:xslt-debug")
                        {
                            expr = base.Clone(formalArg.DefaultValue);
                        }
                        else
                        {
                            expr = this.fac.DefaultValueMarker();
                        }
                    }
                    else
                    {
                        expr = base.Clone(formalArg.DefaultValue);
                    }
                }
                XmlQueryType xmlType = formalArg.XmlType;
                if (!expr.XmlType.IsSubtypeOf(xmlType))
                {
                    expr = this.fac.TypeAssert(expr, xmlType);
                }
                this.invokeArgs.Add(expr);
                this.curArg++;
            }
            QilNode body = this.fac.Invoke(func, this.invokeArgs);
            while (this.iterStack.Count != 0)
            {
                body = this.fac.Loop(this.iterStack.Pop(), body);
            }
            return body;
        }

        protected override QilNode VisitFunction(QilFunction n) => 
            n;

        protected override QilNode VisitReference(QilNode n)
        {
            QilNode node = base.FindClonedReference(n);
            if (node != null)
            {
                return node;
            }
            for (int i = 0; i < this.curArg; i++)
            {
                if (n == this.formalArgs[i])
                {
                    if (this.invokeArgs[i] is QilLiteral)
                    {
                        return this.invokeArgs[i].ShallowClone(this.fac.BaseFactory);
                    }
                    if (!(this.invokeArgs[i] is QilIterator))
                    {
                        QilIterator item = this.fac.BaseFactory.Let(this.invokeArgs[i]);
                        this.iterStack.Push(item);
                        this.invokeArgs[i] = item;
                    }
                    return this.invokeArgs[i];
                }
            }
            return n;
        }
    }
}

