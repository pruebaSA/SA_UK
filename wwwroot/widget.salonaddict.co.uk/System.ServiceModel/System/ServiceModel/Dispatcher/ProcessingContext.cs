namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Reflection;

    internal class ProcessingContext
    {
        internal ProcessingContext next;
        private int nodeCount;
        private QueryProcessor processor;
        private EvalStack sequenceStack;
        private EvalStack valueStack;

        internal ProcessingContext() : this(null)
        {
        }

        internal ProcessingContext(QueryProcessor processor)
        {
            this.processor = processor;
            this.valueStack = new EvalStack(2, 4);
            this.sequenceStack = new EvalStack(1, 2);
            this.nodeCount = -1;
        }

        internal void ClearContext()
        {
            this.sequenceStack.Clear();
            this.valueStack.Clear();
            this.nodeCount = -1;
        }

        internal ProcessingContext Clone() => 
            this.processor.CloneContext(this);

        internal void CopyFrom(ProcessingContext context)
        {
            this.processor = context.processor;
            if (context.sequenceStack.frames.Count > 0)
            {
                this.sequenceStack.CopyFrom(ref context.sequenceStack);
            }
            else
            {
                this.sequenceStack.Clear();
            }
            if (context.valueStack.frames.Count > 0)
            {
                this.valueStack.CopyFrom(ref context.valueStack);
            }
            else
            {
                this.valueStack.Clear();
            }
            this.nodeCount = context.nodeCount;
        }

        internal NodeSequence CreateSequence()
        {
            NodeSequence sequence = this.processor.PopSequence();
            if (sequence == null)
            {
                sequence = new NodeSequence();
            }
            sequence.OwnerContext = this;
            sequence.refCount++;
            return sequence;
        }

        internal void EvalCodeBlock(Opcode block)
        {
            this.processor.Eval(block, this);
        }

        internal bool LoadVariable(int var) => 
            this.Processor.LoadVariable(this, var);

        internal bool PeekBoolean(int index) => 
            this.valueStack.PeekBoolean(index);

        internal double PeekDouble(int index) => 
            this.valueStack.PeekDouble(index);

        internal NodeSequence PeekSequence(int index) => 
            this.valueStack.PeekSequence(index);

        internal string PeekString(int index) => 
            this.valueStack.PeekString(index);

        internal void PopContextSequenceFrame()
        {
            this.PopSequenceFrame();
            if (!this.sequenceStack.InUse)
            {
                this.sequenceStack.contextOnTopOfStack = false;
            }
        }

        internal void PopFrame()
        {
            this.valueStack.PopFrame(this);
        }

        internal void PopSequenceFrame()
        {
            this.sequenceStack.PopFrame(this);
            this.nodeCount = -1;
        }

        internal void PopSequenceFrameToValueStack()
        {
            this.sequenceStack.PopSequenceFrameTo(ref this.valueStack);
            this.nodeCount = -1;
        }

        internal void Push(bool boolVal)
        {
            this.valueStack.Push(boolVal);
        }

        internal void Push(NodeSequence sequence)
        {
            this.valueStack.Push(sequence);
        }

        internal void Push(string stringVal)
        {
            this.valueStack.Push(stringVal);
        }

        internal void Push(bool boolVal, int addCount)
        {
            this.valueStack.Push(boolVal, addCount);
        }

        internal void Push(double doubleVal, int addCount)
        {
            this.valueStack.Push(doubleVal, addCount);
        }

        internal void Push(NodeSequence sequence, int addCount)
        {
            this.valueStack.Push(sequence, addCount);
        }

        internal void Push(string stringVal, int addCount)
        {
            this.valueStack.Push(stringVal, addCount);
        }

        internal void PushContextSequenceFrame()
        {
            if (!this.sequenceStack.InUse)
            {
                this.sequenceStack.contextOnTopOfStack = true;
            }
            this.PushSequenceFrame();
        }

        internal void PushFrame()
        {
            this.valueStack.PushFrame();
        }

        internal void PushSequence(NodeSequence seq)
        {
            this.sequenceStack.Push(seq);
            this.nodeCount = -1;
        }

        internal void PushSequenceFrame()
        {
            this.sequenceStack.PushFrame();
            this.nodeCount = -1;
        }

        internal void PushSequenceFrameFromValueStack()
        {
            this.valueStack.PopSequenceFrameTo(ref this.sequenceStack);
            this.nodeCount = -1;
        }

        internal void Release()
        {
            this.processor.ReleaseContext(this);
        }

        internal void ReleaseSequence(NodeSequence sequence)
        {
            if (this == sequence.OwnerContext)
            {
                sequence.refCount--;
                if (sequence.refCount == 0)
                {
                    this.processor.ReleaseSequenceToPool(sequence);
                }
            }
        }

        internal void ReplaceSequenceAt(int index, NodeSequence sequence)
        {
            this.sequenceStack.ReplaceAt(index, sequence);
            this.nodeCount = -1;
        }

        internal void SaveVariable(int var, int count)
        {
            this.Processor.SaveVariable(this, var, count);
        }

        internal void SetValue(ProcessingContext context, int index, bool val)
        {
            this.valueStack.SetValue(this, index, val);
        }

        internal void SetValue(ProcessingContext context, int index, double val)
        {
            this.valueStack.SetValue(this, index, val);
        }

        internal void SetValue(ProcessingContext context, int index, NodeSequence val)
        {
            this.valueStack.SetValue(this, index, val);
        }

        internal void SetValue(ProcessingContext context, int index, string val)
        {
            this.valueStack.SetValue(this, index, val);
        }

        internal void TransferSequencePositions()
        {
            this.sequenceStack.TransferPositionsTo(ref this.valueStack);
        }

        internal void TransferSequenceSize()
        {
            this.sequenceStack.TransferSequenceSizeTo(ref this.valueStack);
        }

        internal StackFrame this[int frameIndex] =>
            this.valueStack[frameIndex];

        internal int IterationCount
        {
            get
            {
                if (-1 == this.nodeCount)
                {
                    this.nodeCount = this.sequenceStack.CalculateNodecount();
                    if ((this.nodeCount == 0) && !this.sequenceStack.InUse)
                    {
                        this.nodeCount = 1;
                    }
                }
                return this.nodeCount;
            }
        }

        internal ProcessingContext Next
        {
            get => 
                this.next;
            set
            {
                this.next = value;
            }
        }

        internal int NodeCount
        {
            get => 
                this.nodeCount;
            set
            {
                this.nodeCount = value;
            }
        }

        internal QueryProcessor Processor
        {
            get => 
                this.processor;
            set
            {
                this.processor = value;
            }
        }

        internal StackFrame SecondArg =>
            this.valueStack.SecondArg;

        internal Value[] Sequences =>
            this.sequenceStack.Buffer;

        internal bool SequenceStackInUse =>
            this.sequenceStack.InUse;

        internal bool StacksInUse
        {
            get
            {
                if (this.valueStack.frames.Count <= 0)
                {
                    return (this.sequenceStack.frames.Count > 0);
                }
                return true;
            }
        }

        internal StackFrame TopArg =>
            this.valueStack.TopArg;

        internal StackFrame TopSequenceArg =>
            this.sequenceStack.TopArg;

        internal Value[] Values =>
            this.valueStack.Buffer;
    }
}

