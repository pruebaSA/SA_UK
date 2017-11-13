namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;

    internal class InverseQueryMatcher : QueryMatcher
    {
        private SubExprEliminator elim = new SubExprEliminator();
        private Dictionary<XPathMessageFilter, Opcode> lastLookup = new Dictionary<XPathMessageFilter, Opcode>();

        internal InverseQueryMatcher()
        {
        }

        internal void Add(XPathMessageFilter filter, bool forceExternal)
        {
            bool flag = false;
            OpcodeBlock newBlock = new OpcodeBlock();
            newBlock.Append(new NoOpOpcode(OpcodeID.QueryTree));
            if (!forceExternal)
            {
                try
                {
                    ValueDataType none = ValueDataType.None;
                    newBlock.Append(QueryMatcher.CompileForInternalEngine(filter, QueryCompilerFlags.InverseQuery, out none));
                    newBlock.Append(new MatchMultipleFilterResultOpcode(filter));
                    flag = true;
                    newBlock = new OpcodeBlock(this.elim.Add(filter, newBlock.First));
                    base.subExprVars = this.elim.VariableCount;
                }
                catch (QueryCompileException)
                {
                }
            }
            if (!flag)
            {
                newBlock.Append(QueryMatcher.CompileForExternalEngine(filter));
            }
            QueryTreeBuilder builder = new QueryTreeBuilder();
            base.query = builder.Build(base.query, newBlock);
            this.lastLookup[filter] = builder.LastOpcode;
        }

        internal void Clear()
        {
            foreach (XPathMessageFilter filter in this.lastLookup.Keys)
            {
                this.Remove(this.lastLookup[filter], filter);
                this.elim.Remove(filter);
            }
            base.subExprVars = this.elim.VariableCount;
            this.lastLookup.Clear();
        }

        internal void Remove(XPathMessageFilter filter)
        {
            this.Remove(this.lastLookup[filter], filter);
            this.lastLookup.Remove(filter);
            this.elim.Remove(filter);
            base.subExprVars = this.elim.VariableCount;
        }

        private void Remove(Opcode opcode, XPathMessageFilter filter)
        {
            if (opcode.ID != OpcodeID.MatchMultipleFilterResult)
            {
                opcode.Remove();
            }
            else
            {
                ((MatchMultipleFilterResultOpcode) opcode).Remove(filter);
            }
        }

        internal override void Trim()
        {
            base.Trim();
            this.elim.Trim();
        }
    }
}

