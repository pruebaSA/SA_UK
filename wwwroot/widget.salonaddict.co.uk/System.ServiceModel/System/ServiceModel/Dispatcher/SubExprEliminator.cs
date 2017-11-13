namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Xml.XPath;

    internal class SubExprEliminator
    {
        private List<SubExpr> exprList = new List<SubExpr>();
        private int nextVar;
        private Dictionary<XPathMessageFilter, List<SubExpr>> removalMapping = new Dictionary<XPathMessageFilter, List<SubExpr>>();

        internal SubExprEliminator()
        {
            Opcode ops = new XPathMessageFunctionCallOpcode(XPathMessageContext.HeaderFun, 0);
            SubExprHeader item = new SubExprHeader(ops, 0);
            this.exprList.Add(item);
            this.nextVar = 1;
        }

        internal Opcode Add(XPathMessageFilter filter, Opcode ops)
        {
            List<SubExpr> list = new List<SubExpr>();
            this.removalMapping.Add(filter, list);
            while (ops.Next != null)
            {
                ops = ops.Next;
            }
            Opcode opcode = ops;
            while (ops != null)
            {
                if (IsExprStarter(ops))
                {
                    SubExprOpcode opcode4;
                    Opcode op = ops;
                    Opcode prev = ops.Prev;
                    ops.DetachFromParent();
                    ops = ops.Next;
                    while (ops.ID == OpcodeID.Select)
                    {
                        ops = ops.Next;
                    }
                    ops.DetachFromParent();
                    SubExpr item = null;
                    for (int i = 0; i < this.exprList.Count; i++)
                    {
                        if (this.exprList[i].FirstOp.Equals(op))
                        {
                            item = this.exprList[i];
                            break;
                        }
                    }
                    if (item == null)
                    {
                        item = new SubExpr(null, op, this.NewVarID());
                        this.exprList.Add(item);
                        opcode4 = new SubExprOpcode(item);
                    }
                    else
                    {
                        opcode4 = item.Add(op, this);
                    }
                    opcode4.Expr.IncRef();
                    list.Add(opcode4.Expr);
                    opcode4.Attach(ops);
                    ops = opcode4;
                    if (prev != null)
                    {
                        prev.Attach(ops);
                    }
                }
                opcode = ops;
                ops = ops.Prev;
            }
            return opcode;
        }

        internal static bool IsExprStarter(Opcode op)
        {
            if (op.ID == OpcodeID.SelectRoot)
            {
                return true;
            }
            if (op.ID == OpcodeID.XsltInternalFunction)
            {
                XPathMessageFunctionCallOpcode opcode = (XPathMessageFunctionCallOpcode) op;
                if ((opcode.ReturnType == XPathResultType.NodeSet) && (opcode.ArgCount == 0))
                {
                    return true;
                }
            }
            return false;
        }

        internal int NewVarID() => 
            this.nextVar++;

        internal void Remove(XPathMessageFilter filter)
        {
            List<SubExpr> list;
            if (this.removalMapping.TryGetValue(filter, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].DecRef(this);
                }
                this.removalMapping.Remove(filter);
                this.Renumber();
            }
        }

        private void Renumber()
        {
            this.nextVar = 0;
            for (int i = 0; i < this.exprList.Count; i++)
            {
                this.exprList[i].Renumber(this);
            }
        }

        internal void Trim()
        {
            this.exprList.Capacity = this.exprList.Count;
            for (int i = 0; i < this.exprList.Count; i++)
            {
                this.exprList[i].Trim();
            }
        }

        internal List<SubExpr> Exprs =>
            this.exprList;

        internal int VariableCount =>
            this.nextVar;
    }
}

