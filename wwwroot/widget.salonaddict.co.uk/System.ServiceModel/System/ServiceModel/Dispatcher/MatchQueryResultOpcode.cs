﻿namespace System.ServiceModel.Dispatcher
{
    using System;

    internal class MatchQueryResultOpcode : QueryResultOpcode
    {
        internal MatchQueryResultOpcode() : this(OpcodeID.MatchQueryResult)
        {
        }

        protected MatchQueryResultOpcode(OpcodeID id) : base(id)
        {
            base.flags |= OpcodeFlags.Result;
        }

        internal override Opcode Eval(ProcessingContext context)
        {
            context.Processor.Result = this.IsSuccess(context);
            context.PopFrame();
            return base.next;
        }

        protected bool IsSuccess(ProcessingContext context)
        {
            StackFrame topArg = context.TopArg;
            if (1 == topArg.Count)
            {
                return context.Values[topArg.basePtr].ToBoolean();
            }
            context.Processor.Result = false;
            for (int i = topArg.basePtr; i <= topArg.endPtr; i++)
            {
                if (context.Values[i].ToBoolean())
                {
                    return true;
                }
            }
            return false;
        }
    }
}

