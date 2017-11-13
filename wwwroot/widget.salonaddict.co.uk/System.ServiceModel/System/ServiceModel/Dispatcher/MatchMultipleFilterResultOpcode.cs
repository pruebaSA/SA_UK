namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;

    internal class MatchMultipleFilterResultOpcode : MatchQueryResultOpcode
    {
        internal QueryBuffer<XPathMessageFilter> results;

        internal MatchMultipleFilterResultOpcode() : base(OpcodeID.MatchMultipleFilterResult)
        {
            this.results = new QueryBuffer<XPathMessageFilter>(0);
        }

        internal MatchMultipleFilterResultOpcode(XPathMessageFilter filter) : this()
        {
            this.results = new QueryBuffer<XPathMessageFilter>(1);
            this.results.Add(filter);
        }

        internal override void Add(Opcode op)
        {
            MatchMultipleFilterResultOpcode opcode = op as MatchMultipleFilterResultOpcode;
            if (opcode != null)
            {
                this.results.Add(ref opcode.results);
                this.results.TrimToCount();
            }
            else
            {
                base.Add(op);
            }
        }

        internal override void CollectXPathFilters(ICollection<MessageFilter> filters)
        {
            for (int i = 0; i < this.results.Count; i++)
            {
                filters.Add(this.results[i]);
            }
        }

        internal override bool Equals(Opcode op) => 
            (base.Equals(op) && object.ReferenceEquals(this, op));

        internal override Opcode Eval(ProcessingContext context)
        {
            if (base.IsSuccess(context))
            {
                ICollection<MessageFilter> resultSet = context.Processor.ResultSet;
                int num = 0;
                int count = this.results.Count;
                while (num < count)
                {
                    resultSet.Add(this.results[num]);
                    num++;
                }
            }
            context.PopFrame();
            return base.next;
        }

        internal override void Remove()
        {
            if (this.results.Count == 0)
            {
                base.Remove();
            }
        }

        internal void Remove(XPathMessageFilter resultFilter)
        {
            this.results.Remove(resultFilter);
            this.Remove();
        }

        internal override void Trim()
        {
            this.results.TrimToCount();
        }
    }
}

