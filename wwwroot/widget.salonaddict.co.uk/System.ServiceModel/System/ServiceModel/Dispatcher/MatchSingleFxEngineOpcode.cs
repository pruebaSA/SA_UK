namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Xml.XPath;

    internal class MatchSingleFxEngineOpcode : QueryResultOpcode
    {
        private XPathMessageFilter filter;
        private XPathExpression xpath;

        internal MatchSingleFxEngineOpcode() : base(OpcodeID.MatchSingleFx)
        {
            base.flags |= OpcodeFlags.FxMatch;
        }

        internal override void CollectXPathFilters(ICollection<MessageFilter> filters)
        {
            filters.Add(this.filter);
        }

        internal override bool Equals(Opcode op) => 
            false;

        internal override Opcode Eval(ProcessingContext context)
        {
            SeekableXPathNavigator contextNode = context.Processor.ContextNode;
            bool flag = this.Match(contextNode);
            context.Processor.Result = flag;
            if ((flag && (this.filter != null)) && (context.Processor.ResultSet != null))
            {
                context.Processor.ResultSet.Add(this.filter);
            }
            return base.next;
        }

        internal object Evaluate(XPathNavigator nav)
        {
            SeekableMessageNavigator navigator = nav as SeekableMessageNavigator;
            if (navigator != null)
            {
                navigator.Atomize();
            }
            if (XPathResultType.NodeSet == this.xpath.ReturnType)
            {
                return nav.Select(this.xpath);
            }
            return nav.Evaluate(this.xpath);
        }

        internal bool Match(XPathNavigator nav)
        {
            object obj2 = this.Evaluate(nav);
            switch (this.xpath.ReturnType)
            {
                case XPathResultType.Number:
                    return (((double) obj2) != 0.0);

                case XPathResultType.String:
                {
                    string str = (string) obj2;
                    return ((str != null) && (str.Length > 0));
                }
                case XPathResultType.Boolean:
                    return (bool) obj2;

                case XPathResultType.NodeSet:
                {
                    XPathNodeIterator iterator = (XPathNodeIterator) obj2;
                    return ((iterator != null) && (iterator.Count > 0));
                }
                case XPathResultType.Any:
                    return (null != obj2);
            }
            return false;
        }

        internal string Select(XPathNavigator nav)
        {
            string str = string.Empty;
            object obj2 = this.Evaluate(nav);
            switch (this.xpath.ReturnType)
            {
                case XPathResultType.Number:
                    return QueryValueModel.String((double) obj2);

                case XPathResultType.String:
                    return (string) obj2;

                case XPathResultType.Boolean:
                    return QueryValueModel.String((bool) obj2);

                case XPathResultType.NodeSet:
                {
                    XPathNodeIterator iterator = (XPathNodeIterator) obj2;
                    if (iterator.MoveNext())
                    {
                        str = iterator.Current.Value;
                    }
                    return str;
                }
            }
            return str;
        }

        internal XPathMessageFilter Filter
        {
            set
            {
                this.filter = value;
            }
        }

        internal XPathExpression XPath
        {
            set
            {
                this.xpath = value;
            }
        }
    }
}

