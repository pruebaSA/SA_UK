namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections;
    using System.Reflection;

    internal class XPathExprList
    {
        private ArrayList list = new ArrayList(2);

        internal XPathExprList()
        {
        }

        internal void Add(XPathExpr expr)
        {
            this.list.Add(expr);
        }

        internal int Count =>
            this.list.Count;

        internal XPathExpr this[int index] =>
            ((XPathExpr) this.list[index]);
    }
}

