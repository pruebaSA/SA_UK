namespace System.ServiceModel.Dispatcher
{
    using System;

    internal abstract class QueryResultOpcode : Opcode
    {
        internal QueryResultOpcode(OpcodeID id) : base(id)
        {
            base.flags |= OpcodeFlags.Result;
        }
    }
}

