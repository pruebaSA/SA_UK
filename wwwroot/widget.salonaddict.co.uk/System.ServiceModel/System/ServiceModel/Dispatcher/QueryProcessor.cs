namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;

    internal class QueryProcessor : ProcessingContext
    {
        private SeekableXPathNavigator contextNode;
        private ProcessingContext contextPool;
        private INodeCounter counter;
        private QueryProcessingFlags flags;
        private QueryMatcher matcher;
        private bool matchMessageBody;
        private Message message;
        private string messageAction;
        private string messageId;
        private string messageSoapUri;
        private string messageTo;
        private bool result;
        private Collection<MessageFilter> resultList;
        private QueryBranchResultSet resultPool;
        private ICollection<MessageFilter> resultSet;
        private NodeSequence sequencePool;
        private SubExprVariable[] subExprVars;

        internal QueryProcessor(QueryMatcher matcher)
        {
            base.Processor = this;
            this.matcher = matcher;
            this.flags = QueryProcessingFlags.Match;
            this.messageAction = null;
            this.messageId = null;
            this.messageSoapUri = null;
            this.messageTo = null;
            if (matcher.SubExprVarCount > 0)
            {
                this.subExprVars = new SubExprVariable[matcher.SubExprVarCount];
            }
        }

        internal void ClearProcessor()
        {
            base.ClearContext();
            this.flags = QueryProcessingFlags.Match;
            this.messageAction = null;
            this.messageId = null;
            this.messageSoapUri = null;
            this.messageTo = null;
            int subExprVarCount = this.matcher.SubExprVarCount;
            if (subExprVarCount == 0)
            {
                this.subExprVars = null;
            }
            else
            {
                SubExprVariable[] subExprVars = this.subExprVars;
                if (subExprVars == null)
                {
                    this.subExprVars = new SubExprVariable[subExprVarCount];
                }
                else
                {
                    int length = subExprVars.Length;
                    if (length != subExprVarCount)
                    {
                        this.subExprVars = new SubExprVariable[subExprVarCount];
                    }
                    else if (length == 1)
                    {
                        NodeSequence seq = subExprVars[0].seq;
                        if (seq != null)
                        {
                            this.ReleaseSequenceToPool(seq);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < length; i++)
                        {
                            NodeSequence sequence = subExprVars[i].seq;
                            if ((sequence != null) && (sequence.refCount > 0))
                            {
                                this.ReleaseSequenceToPool(sequence);
                            }
                        }
                        Array.Clear(subExprVars, 0, subExprVars.Length);
                    }
                }
            }
        }

        internal ProcessingContext CloneContext(ProcessingContext srcContext)
        {
            ProcessingContext context = this.PopContext();
            if (context == null)
            {
                context = new ProcessingContext();
            }
            context.CopyFrom(srcContext);
            return context;
        }

        internal QueryBranchResultSet CreateResultSet()
        {
            QueryBranchResultSet set = this.PopResultSet();
            if (set == null)
            {
                return new QueryBranchResultSet();
            }
            set.Clear();
            return set;
        }

        internal int ElapsedCount(int marker) => 
            this.counter.ElapsedCount(marker);

        internal void EnsureFilterCollection()
        {
            if (this.resultSet == null)
            {
                if (this.resultList == null)
                {
                    this.resultList = new Collection<MessageFilter>();
                }
                else
                {
                    this.resultList.Clear();
                }
                this.resultSet = this.resultList;
            }
        }

        internal void Eval(Opcode block)
        {
            Opcode op = block;
            try
            {
                while (op != null)
                {
                    op = op.Eval(this);
                }
            }
            catch (XPathNavigatorException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception.Process(op));
            }
            catch (NavigatorInvalidBodyAccessException exception2)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception2.Process(op));
            }
        }

        internal void Eval(Opcode block, ProcessingContext context)
        {
            Opcode op = block;
            try
            {
                while (op != null)
                {
                    op = op.Eval(context);
                }
            }
            catch (XPathNavigatorException exception)
            {
                throw TraceUtility.ThrowHelperError(exception.Process(op), this.message);
            }
            catch (NavigatorInvalidBodyAccessException exception2)
            {
                throw TraceUtility.ThrowHelperError(exception2.Process(op), this.message);
            }
        }

        internal void Eval(Opcode block, SeekableXPathNavigator navigator)
        {
            this.result = false;
            this.ContextNode = navigator;
            this.ContextMessage = null;
            this.Eval(block);
        }

        internal void Eval(Opcode block, Message message, bool matchBody)
        {
            this.result = false;
            this.ContextNode = null;
            this.ContextMessage = message;
            this.MatchBody = matchBody;
            this.Eval(block);
            this.message = null;
            this.contextNode = null;
        }

        internal bool LoadVariable(ProcessingContext context, int var)
        {
            if (this.subExprVars[var].seq == null)
            {
                return false;
            }
            int iterationCount = context.IterationCount;
            this.counter.IncreaseBy(iterationCount * this.subExprVars[var].count);
            NodeSequence seq = this.subExprVars[var].seq;
            context.PushSequenceFrame();
            for (int i = 0; i < iterationCount; i++)
            {
                seq.refCount++;
                context.PushSequence(seq);
            }
            return true;
        }

        internal ProcessingContext PopContext()
        {
            ProcessingContext contextPool = this.contextPool;
            if (contextPool != null)
            {
                this.contextPool = contextPool.Next;
                contextPool.Next = null;
            }
            return contextPool;
        }

        internal QueryBranchResultSet PopResultSet()
        {
            QueryBranchResultSet resultPool = this.resultPool;
            if (resultPool != null)
            {
                this.resultPool = resultPool.Next;
                resultPool.Next = null;
            }
            return resultPool;
        }

        internal NodeSequence PopSequence()
        {
            NodeSequence sequencePool = this.sequencePool;
            if (sequencePool != null)
            {
                this.sequencePool = sequencePool.Next;
                sequencePool.Next = null;
            }
            return sequencePool;
        }

        internal void PushContext(ProcessingContext context)
        {
            context.Next = this.contextPool;
            this.contextPool = context;
        }

        internal void PushResultSet(QueryBranchResultSet resultSet)
        {
            resultSet.Next = this.resultPool;
            this.resultPool = resultSet;
        }

        internal void ReleaseContext(ProcessingContext context)
        {
            this.PushContext(context);
        }

        internal void ReleaseResults(QueryBranchResultSet resultSet)
        {
            this.PushResultSet(resultSet);
        }

        internal void ReleaseSequenceToPool(NodeSequence sequence)
        {
            if (NodeSequence.Empty != sequence)
            {
                sequence.Reset(this.sequencePool);
                this.sequencePool = sequence;
            }
        }

        internal void SaveVariable(ProcessingContext context, int var, int count)
        {
            NodeSequence sequence = context.Sequences[context.TopSequenceArg.basePtr].Sequence;
            if (sequence == null)
            {
                sequence = base.CreateSequence();
            }
            sequence.OwnerContext = null;
            this.subExprVars[var].seq = sequence;
            this.subExprVars[var].count = count;
        }

        internal string Action
        {
            get => 
                this.messageAction;
            set
            {
                this.messageAction = value;
            }
        }

        internal Message ContextMessage
        {
            get => 
                this.message;
            set
            {
                this.message = value;
                if (value != null)
                {
                    this.flags = (QueryProcessingFlags) ((byte) (this.flags | QueryProcessingFlags.Message));
                }
                else
                {
                    this.flags = (QueryProcessingFlags) ((byte) (this.flags & ((QueryProcessingFlags) 0xfd)));
                }
            }
        }

        internal SeekableXPathNavigator ContextNode
        {
            get
            {
                if (this.contextNode == null)
                {
                    if (this.message == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperCritical(new QueryProcessingException(QueryProcessingError.Unexpected));
                    }
                    this.contextNode = this.matcher.CreateMessageNavigator(this.message, this.matchMessageBody);
                    this.counter = this.contextNode as INodeCounter;
                    if (this.counter == null)
                    {
                        this.counter = DummyNodeCounter.Dummy;
                    }
                }
                return this.contextNode;
            }
            set
            {
                this.contextNode = value;
                this.counter = value as INodeCounter;
            }
        }

        internal int CounterMarker
        {
            get => 
                this.counter?.CounterMarker;
            set
            {
                this.counter.CounterMarker = value;
            }
        }

        internal bool MatchBody
        {
            set
            {
                this.matchMessageBody = value;
            }
        }

        internal string MessageId
        {
            get => 
                this.messageId;
            set
            {
                this.messageId = value;
            }
        }

        internal bool Result
        {
            get => 
                this.result;
            set
            {
                this.result = value;
            }
        }

        internal Collection<MessageFilter> ResultList =>
            this.resultList;

        internal ICollection<MessageFilter> ResultSet
        {
            get => 
                this.resultSet;
            set
            {
                this.resultSet = value;
            }
        }

        internal string SoapUri
        {
            get => 
                this.messageSoapUri;
            set
            {
                this.messageSoapUri = value;
            }
        }

        internal string ToHeader
        {
            get => 
                this.messageTo;
            set
            {
                this.messageTo = value;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SubExprVariable
        {
            internal NodeSequence seq;
            internal int count;
        }
    }
}

